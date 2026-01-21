using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.DTOs
{
    public class IssueBookDto
    {
        [Required]
        public int BookId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int DueDays { get; set; } = 14; // Default 14 days
    }

    public class ReturnBookDto
    {
        [Required]
        public int IssueId { get; set; }
    }

    public class BookIssueDto
    {
        public int IssueId { get; set; }
        public int BookId { get; set; }
        public string BookTitle { get; set; } = string.Empty;
        public string BookAuthor { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public DateTime IssueDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal? FineAmount { get; set; }
    }
}
