namespace LibraryManagement.DTOs
{
    public class FineDto
    {
        public int FineId { get; set; }
        public int IssueId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string BookTitle { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime? PaidDate { get; set; }
        public string? Reason { get; set; }
    }

    public class PayFineDto
    {
        public int FineId { get; set; }
    }
}
