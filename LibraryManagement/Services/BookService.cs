using LibraryManagement.Data;
using LibraryManagement.DTOs;
using LibraryManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Services
{
    public interface IBookService
    {
        Task<List<BookDto>> GetAllBooks();
        Task<List<BookDto>> SearchBooks(string? searchTerm, string? category);
        Task<BookDto?> GetBookById(int bookId);
        Task<BookDto?> CreateBook(CreateBookDto createBookDto);
        Task<BookDto?> UpdateBook(int bookId, UpdateBookDto updateBookDto);
        Task<bool> DeleteBook(int bookId);
    }

    public class BookService : IBookService
    {
        private readonly LibraryDbContext _context;

        public BookService(LibraryDbContext context)
        {
            _context = context;
        }

        public async Task<List<BookDto>> GetAllBooks()
        {
            return await _context.Books
                .Where(b => b.IsActive)
                .Select(b => new BookDto
                {
                    BookId = b.BookId,
                    ISBN = b.ISBN,
                    Title = b.Title,
                    Author = b.Author,
                    Publisher = b.Publisher,
                    PublishedYear = b.PublishedYear,
                    Category = b.Category,
                    TotalCopies = b.TotalCopies,
                    AvailableCopies = b.AvailableCopies,
                    Description = b.Description
                })
                .ToListAsync();
        }

        public async Task<List<BookDto>> SearchBooks(string? searchTerm, string? category)
        {
            var query = _context.Books.Where(b => b.IsActive);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                query = query.Where(b =>
                    b.Title.ToLower().Contains(searchTerm) ||
                    b.Author.ToLower().Contains(searchTerm) ||
                    b.ISBN.Contains(searchTerm)
                );
            }

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(b => b.Category == category);
            }

            return await query
                .Select(b => new BookDto
                {
                    BookId = b.BookId,
                    ISBN = b.ISBN,
                    Title = b.Title,
                    Author = b.Author,
                    Publisher = b.Publisher,
                    PublishedYear = b.PublishedYear,
                    Category = b.Category,
                    TotalCopies = b.TotalCopies,
                    AvailableCopies = b.AvailableCopies,
                    Description = b.Description
                })
                .ToListAsync();
        }

        public async Task<BookDto?> GetBookById(int bookId)
        {
            var book = await _context.Books.FindAsync(bookId);
            if (book == null || !book.IsActive)
                return null;

            return new BookDto
            {
                BookId = book.BookId,
                ISBN = book.ISBN,
                Title = book.Title,
                Author = book.Author,
                Publisher = book.Publisher,
                PublishedYear = book.PublishedYear,
                Category = book.Category,
                TotalCopies = book.TotalCopies,
                AvailableCopies = book.AvailableCopies,
                Description = book.Description
            };
        }

        public async Task<BookDto?> CreateBook(CreateBookDto createBookDto)
        {
            // Check if ISBN already exists
            if (await _context.Books.AnyAsync(b => b.ISBN == createBookDto.ISBN))
                return null;

            var book = new Book
            {
                ISBN = createBookDto.ISBN,
                Title = createBookDto.Title,
                Author = createBookDto.Author,
                Publisher = createBookDto.Publisher,
                PublishedYear = createBookDto.PublishedYear,
                Category = createBookDto.Category,
                TotalCopies = createBookDto.TotalCopies,
                AvailableCopies = createBookDto.TotalCopies,
                Description = createBookDto.Description,
                AddedDate = DateTime.Now,
                IsActive = true
            };

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return new BookDto
            {
                BookId = book.BookId,
                ISBN = book.ISBN,
                Title = book.Title,
                Author = book.Author,
                Publisher = book.Publisher,
                PublishedYear = book.PublishedYear,
                Category = book.Category,
                TotalCopies = book.TotalCopies,
                AvailableCopies = book.AvailableCopies,
                Description = book.Description
            };
        }

        public async Task<BookDto?> UpdateBook(int bookId, UpdateBookDto updateBookDto)
        {
            var book = await _context.Books.FindAsync(bookId);
            if (book == null || !book.IsActive)
                return null;

            if (!string.IsNullOrEmpty(updateBookDto.Title))
                book.Title = updateBookDto.Title;
            if (!string.IsNullOrEmpty(updateBookDto.Author))
                book.Author = updateBookDto.Author;
            if (updateBookDto.Publisher != null)
                book.Publisher = updateBookDto.Publisher;
            if (updateBookDto.PublishedYear.HasValue)
                book.PublishedYear = updateBookDto.PublishedYear;
            if (updateBookDto.Category != null)
                book.Category = updateBookDto.Category;
            if (updateBookDto.TotalCopies.HasValue)
            {
                var diff = updateBookDto.TotalCopies.Value - book.TotalCopies;
                book.TotalCopies = updateBookDto.TotalCopies.Value;
                book.AvailableCopies += diff;
            }
            if (updateBookDto.Description != null)
                book.Description = updateBookDto.Description;

            await _context.SaveChangesAsync();

            return new BookDto
            {
                BookId = book.BookId,
                ISBN = book.ISBN,
                Title = book.Title,
                Author = book.Author,
                Publisher = book.Publisher,
                PublishedYear = book.PublishedYear,
                Category = book.Category,
                TotalCopies = book.TotalCopies,
                AvailableCopies = book.AvailableCopies,
                Description = book.Description
            };
        }

        public async Task<bool> DeleteBook(int bookId)
        {
            var book = await _context.Books.FindAsync(bookId);
            if (book == null)
                return false;

            // Soft delete
            book.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
