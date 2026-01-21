using LibraryManagement.Data;
using LibraryManagement.DTOs;
using LibraryManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Services
{
    public interface IIssueService
    {
        Task<BookIssueDto?> IssueBook(IssueBookDto issueBookDto);
        Task<BookIssueDto?> ReturnBook(ReturnBookDto returnBookDto);
        Task<List<BookIssueDto>> GetUserIssues(int userId);
        Task<List<BookIssueDto>> GetAllActiveIssues();
    }

    public class IssueService : IIssueService
    {
        private readonly LibraryDbContext _context;
        private readonly decimal _finePerDay = 5.0m; // $5 per day fine

        public IssueService(LibraryDbContext context)
        {
            _context = context;
        }

        public async Task<BookIssueDto?> IssueBook(IssueBookDto issueBookDto)
        {
            // Check if book exists and is available
            var book = await _context.Books.FindAsync(issueBookDto.BookId);
            if (book == null || !book.IsActive || book.AvailableCopies <= 0)
                return null;

            // Check if user exists
            var user = await _context.Users.FindAsync(issueBookDto.UserId);
            if (user == null || !user.IsActive)
                return null;

            // Check if user has any pending fines
            var hasPendingFines = await _context.Fines
                .AnyAsync(f => f.UserId == issueBookDto.UserId && f.Status == FineStatus.Pending);
            if (hasPendingFines)
                return null; // User must pay fines before borrowing

            var bookIssue = new BookIssue
            {
                BookId = issueBookDto.BookId,
                UserId = issueBookDto.UserId,
                IssueDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(issueBookDto.DueDays),
                Status = IssueStatus.Issued
            };

            _context.BookIssues.Add(bookIssue);
            book.AvailableCopies--;
            await _context.SaveChangesAsync();

            return new BookIssueDto
            {
                IssueId = bookIssue.IssueId,
                BookId = book.BookId,
                BookTitle = book.Title,
                BookAuthor = book.Author,
                UserId = user.UserId,
                UserName = user.Name,
                IssueDate = bookIssue.IssueDate,
                DueDate = bookIssue.DueDate,
                ReturnDate = bookIssue.ReturnDate,
                Status = bookIssue.Status.ToString()
            };
        }

        public async Task<BookIssueDto?> ReturnBook(ReturnBookDto returnBookDto)
        {
            var bookIssue = await _context.BookIssues
                .Include(bi => bi.Book)
                .Include(bi => bi.User)
                .FirstOrDefaultAsync(bi => bi.IssueId == returnBookDto.IssueId);

            if (bookIssue == null || bookIssue.Status == IssueStatus.Returned)
                return null;

            bookIssue.ReturnDate = DateTime.Now;
            bookIssue.Status = IssueStatus.Returned;

            // Update book availability
            var book = bookIssue.Book;
            book.AvailableCopies++;

            // Calculate fine if overdue
            decimal? fineAmount = null;
            if (bookIssue.ReturnDate > bookIssue.DueDate)
            {
                var daysOverdue = (bookIssue.ReturnDate.Value - bookIssue.DueDate).Days;
                fineAmount = daysOverdue * _finePerDay;

                var fine = new Fine
                {
                    IssueId = bookIssue.IssueId,
                    UserId = bookIssue.UserId,
                    Amount = fineAmount.Value,
                    Status = FineStatus.Pending,
                    CreatedDate = DateTime.Now,
                    Reason = $"Book returned {daysOverdue} day(s) late"
                };

                _context.Fines.Add(fine);
                bookIssue.Status = IssueStatus.Overdue;
            }

            await _context.SaveChangesAsync();

            return new BookIssueDto
            {
                IssueId = bookIssue.IssueId,
                BookId = bookIssue.BookId,
                BookTitle = book.Title,
                BookAuthor = book.Author,
                UserId = bookIssue.UserId,
                UserName = bookIssue.User.Name,
                IssueDate = bookIssue.IssueDate,
                DueDate = bookIssue.DueDate,
                ReturnDate = bookIssue.ReturnDate,
                Status = bookIssue.Status.ToString(),
                FineAmount = fineAmount
            };
        }

        public async Task<List<BookIssueDto>> GetUserIssues(int userId)
        {
            return await _context.BookIssues
                .Include(bi => bi.Book)
                .Include(bi => bi.User)
                .Include(bi => bi.Fine)
                .Where(bi => bi.UserId == userId)
                .OrderByDescending(bi => bi.IssueDate)
                .Select(bi => new BookIssueDto
                {
                    IssueId = bi.IssueId,
                    BookId = bi.BookId,
                    BookTitle = bi.Book.Title,
                    BookAuthor = bi.Book.Author,
                    UserId = bi.UserId,
                    UserName = bi.User.Name,
                    IssueDate = bi.IssueDate,
                    DueDate = bi.DueDate,
                    ReturnDate = bi.ReturnDate,
                    Status = bi.Status.ToString(),
                    FineAmount = bi.Fine != null ? bi.Fine.Amount : null
                })
                .ToListAsync();
        }

        public async Task<List<BookIssueDto>> GetAllActiveIssues()
        {
            return await _context.BookIssues
                .Include(bi => bi.Book)
                .Include(bi => bi.User)
                .Include(bi => bi.Fine)
                .Where(bi => bi.Status == IssueStatus.Issued)
                .OrderByDescending(bi => bi.IssueDate)
                .Select(bi => new BookIssueDto
                {
                    IssueId = bi.IssueId,
                    BookId = bi.BookId,
                    BookTitle = bi.Book.Title,
                    BookAuthor = bi.Book.Author,
                    UserId = bi.UserId,
                    UserName = bi.User.Name,
                    IssueDate = bi.IssueDate,
                    DueDate = bi.DueDate,
                    ReturnDate = bi.ReturnDate,
                    Status = bi.Status.ToString(),
                    FineAmount = bi.Fine != null ? bi.Fine.Amount : null
                })
                .ToListAsync();
        }
    }
}
