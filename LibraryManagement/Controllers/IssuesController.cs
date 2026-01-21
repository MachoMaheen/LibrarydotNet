using LibraryManagement.DTOs;
using LibraryManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LibraryManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class IssuesController : ControllerBase
    {
        private readonly IIssueService _issueService;

        public IssuesController(IIssueService issueService)
        {
            _issueService = issueService;
        }

        [Authorize(Roles = "Librarian,Admin")]
        [HttpPost("issue")]
        public async Task<IActionResult> IssueBook([FromBody] IssueBookDto issueBookDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _issueService.IssueBook(issueBookDto);
            if (result == null)
                return BadRequest(new { message = "Unable to issue book. Check if book is available and user has no pending fines." });

            return Ok(result);
        }

        [Authorize(Roles = "Librarian,Admin")]
        [HttpPost("return")]
        public async Task<IActionResult> ReturnBook([FromBody] ReturnBookDto returnBookDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _issueService.ReturnBook(returnBookDto);
            if (result == null)
                return BadRequest(new { message = "Unable to return book. Issue not found or already returned." });

            return Ok(result);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserIssues(int userId)
        {
            // Members can only view their own issues
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userRole == "Member" && currentUserId != userId.ToString())
                return Forbid();

            var issues = await _issueService.GetUserIssues(userId);
            return Ok(issues);
        }

        [Authorize(Roles = "Librarian,Admin")]
        [HttpGet("active")]
        public async Task<IActionResult> GetAllActiveIssues()
        {
            var issues = await _issueService.GetAllActiveIssues();
            return Ok(issues);
        }
    }
}
