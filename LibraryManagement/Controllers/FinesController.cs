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
    public class FinesController : ControllerBase
    {
        private readonly IFineService _fineService;

        public FinesController(IFineService fineService)
        {
            _fineService = fineService;
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserFines(int userId)
        {
            // Members can only view their own fines
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userRole == "Member" && currentUserId != userId.ToString())
                return Forbid();

            var fines = await _fineService.GetUserFines(userId);
            return Ok(fines);
        }

        [Authorize(Roles = "Librarian,Admin")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllFines()
        {
            var fines = await _fineService.GetAllFines();
            return Ok(fines);
        }

        [HttpPost("pay")]
        public async Task<IActionResult> PayFine([FromBody] PayFineDto payFineDto)
        {
            var result = await _fineService.PayFine(payFineDto);
            if (result == null)
                return BadRequest(new { message = "Fine not found or already paid" });

            return Ok(result);
        }
    }
}
