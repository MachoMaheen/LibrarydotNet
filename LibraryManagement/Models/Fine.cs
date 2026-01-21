using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagement.Models
{
    public enum FineStatus
    {
        Pending,
        Paid
    }

    public class Fine
    {
        [Key]
        public int FineId { get; set; }

        [Required]
        public int IssueId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Amount { get; set; }

        [Required]
        public FineStatus Status { get; set; } = FineStatus.Pending;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? PaidDate { get; set; }

        [StringLength(200)]
        public string? Reason { get; set; }

        // Navigation properties
        [ForeignKey("IssueId")]
        public virtual BookIssue BookIssue { get; set; } = null!;

        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;
    }
}
