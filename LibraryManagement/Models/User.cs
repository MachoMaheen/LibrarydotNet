using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Models
{
    public enum UserRole
    {
        Admin,
        Librarian,
        Member
    }

    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public UserRole Role { get; set; }

        [StringLength(15)]
        public string? Phone { get; set; }

        [StringLength(200)]
        public string? Address { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual ICollection<BookIssue> BookIssues { get; set; } = new List<BookIssue>();
        public virtual ICollection<Fine> Fines { get; set; } = new List<Fine>();
    }
}
