using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.DTOs
{
    public class BookDto
    {
        public int BookId { get; set; }
        public string ISBN { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string? Publisher { get; set; }
        public int? PublishedYear { get; set; }
        public string? Category { get; set; }
        public int TotalCopies { get; set; }
        public int AvailableCopies { get; set; }
        public string? Description { get; set; }
    }

    public class CreateBookDto
    {
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
        [Range(1, 1000)]
        public int TotalCopies { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }
    }

    public class UpdateBookDto
    {
        [StringLength(200)]
        public string? Title { get; set; }

        [StringLength(100)]
        public string? Author { get; set; }

        [StringLength(100)]
        public string? Publisher { get; set; }

        public int? PublishedYear { get; set; }

        [StringLength(50)]
        public string? Category { get; set; }

        [Range(1, 1000)]
        public int? TotalCopies { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }
    }
}
