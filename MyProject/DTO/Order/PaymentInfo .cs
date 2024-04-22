namespace MyProject.DTO.Order
{
	public class PaymentInfo
	{
		public int Id { get; set; }

		public string? OrderId { get; set; }
		public int? CustomerId { get; set; }
		public string? Name { get; set; }
		public string? Address { get; set; }
		public string? Address2 { get; set; }
		public string? Email { get; set; }
		public string? Phone { get; set; }
		public string? Payment { get; set; }
		public decimal? TotalAmount { get; set; }
	}
}
