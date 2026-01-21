using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Models
{
    public class Book
    {
        [Key]
        public int BookId { get; set; }

        [Required]
        [StringLength(13)]
        public string ISBN { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Author { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Publisher { get; set; }

        public int? PublishedYear { get; set; }

        [StringLength(50)]
        public string? Category { get; set; }

        [Required]
        public int TotalCopies { get; set; }

        [Required]
        public int AvailableCopies { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        public DateTime AddedDate { get; set; } = DateTime.Now;

        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual ICollection<BookIssue> BookIssues { get; set; } = new List<BookIssue>();
    }
}
