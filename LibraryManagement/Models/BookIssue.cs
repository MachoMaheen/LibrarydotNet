using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagement.Models
{
    public enum IssueStatus
    {
        Issued,
        Returned,
        Overdue
    }

    public class BookIssue
    {
        [Key]
        public int IssueId { get; set; }

        [Required]
        public int BookId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public DateTime IssueDate { get; set; } = DateTime.Now;

        [Required]
        public DateTime DueDate { get; set; }

        public DateTime? ReturnDate { get; set; }

        [Required]
        public IssueStatus Status { get; set; } = IssueStatus.Issued;

        public int? IssuedByLibrarianId { get; set; }

        // Navigation properties
        [ForeignKey("BookId")]
        public virtual Book Book { get; set; } = null!;

        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;

        public virtual Fine? Fine { get; set; }
    }
}
