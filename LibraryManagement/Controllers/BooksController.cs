using LibraryManagement.DTOs;
using LibraryManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BooksController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBooks()
        {
            var books = await _bookService.GetAllBooks();
            return Ok(books);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchBooks([FromQuery] string? searchTerm, [FromQuery] string? category)
        {
            var books = await _bookService.SearchBooks(searchTerm, category);
            return Ok(books);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookById(int id)
        {
            var book = await _bookService.GetBookById(id);
            if (book == null)
                return NotFound(new { message = "Book not found" });

            return Ok(book);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateBook([FromBody] CreateBookDto createBookDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var book = await _bookService.CreateBook(createBookDto);
            if (book == null)
                return BadRequest(new { message = "Book with this ISBN already exists" });

            return CreatedAtAction(nameof(GetBookById), new { id = book.BookId }, book);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(int id, [FromBody] UpdateBookDto updateBookDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var book = await _bookService.UpdateBook(id, updateBookDto);
            if (book == null)
                return NotFound(new { message = "Book not found" });

            return Ok(book);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var result = await _bookService.DeleteBook(id);
            if (!result)
                return NotFound(new { message = "Book not found" });

            return Ok(new { message = "Book deleted successfully" });
        }
    }
}
