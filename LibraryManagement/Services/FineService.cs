using LibraryManagement.Data;
using LibraryManagement.DTOs;
using LibraryManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Services
{
    public interface IFineService
    {
        Task<List<FineDto>> GetUserFines(int userId);
        Task<List<FineDto>> GetAllFines();
        Task<FineDto?> PayFine(PayFineDto payFineDto);
    }

    public class FineService : IFineService
    {
        private readonly LibraryDbContext _context;

        public FineService(LibraryDbContext context)
        {
            _context = context;
        }

        public async Task<List<FineDto>> GetUserFines(int userId)
        {
            return await _context.Fines
                .Include(f => f.User)
                .Include(f => f.BookIssue)
                .ThenInclude(bi => bi.Book)
                .Where(f => f.UserId == userId)
                .OrderByDescending(f => f.CreatedDate)
                .Select(f => new FineDto
                {
                    FineId = f.FineId,
                    IssueId = f.IssueId,
                    UserId = f.UserId,
                    UserName = f.User.Name,
                    BookTitle = f.BookIssue.Book.Title,
                    Amount = f.Amount,
                    Status = f.Status.ToString(),
                    CreatedDate = f.CreatedDate,
                    PaidDate = f.PaidDate,
                    Reason = f.Reason
                })
                .ToListAsync();
        }

        public async Task<List<FineDto>> GetAllFines()
        {
            return await _context.Fines
                .Include(f => f.User)
                .Include(f => f.BookIssue)
                .ThenInclude(bi => bi.Book)
                .OrderByDescending(f => f.CreatedDate)
                .Select(f => new FineDto
                {
                    FineId = f.FineId,
                    IssueId = f.IssueId,
                    UserId = f.UserId,
                    UserName = f.User.Name,
                    BookTitle = f.BookIssue.Book.Title,
                    Amount = f.Amount,
                    Status = f.Status.ToString(),
                    CreatedDate = f.CreatedDate,
                    PaidDate = f.PaidDate,
                    Reason = f.Reason
                })
                .ToListAsync();
        }

        public async Task<FineDto?> PayFine(PayFineDto payFineDto)
        {
            var fine = await _context.Fines
                .Include(f => f.User)
                .Include(f => f.BookIssue)
                .ThenInclude(bi => bi.Book)
                .FirstOrDefaultAsync(f => f.FineId == payFineDto.FineId);

            if (fine == null || fine.Status == FineStatus.Paid)
                return null;

            fine.Status = FineStatus.Paid;
            fine.PaidDate = DateTime.Now;
            await _context.SaveChangesAsync();

            return new FineDto
            {
                FineId = fine.FineId,
                IssueId = fine.IssueId,
                UserId = fine.UserId,
                UserName = fine.User.Name,
                BookTitle = fine.BookIssue.Book.Title,
                Amount = fine.Amount,
                Status = fine.Status.ToString(),
                CreatedDate = fine.CreatedDate,
                PaidDate = fine.PaidDate,
                Reason = fine.Reason
            };
        }
    }
}
