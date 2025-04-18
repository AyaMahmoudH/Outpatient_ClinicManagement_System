namespace HospitalManagement.DTOs
{
    public class BillingDTO
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; }
        public DateTime BillingDate { get; set; }
        public decimal TotalAmount { get; set; }
        public bool IsPaid { get; set; }
        public string PaymentMethod { get; set; }
        public string InvoiceNumber { get; set; }
        public List<BillingItemDTO> BillingItems { get; set; }
    }

}
