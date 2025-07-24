namespace HospitalManagement.Models
{
  
    public class Billing
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public Patient Patient { get; set; }
        public DateTime BillingDate { get; set; }
        public decimal TotalAmount { get; set; }
        public bool IsPaid { get; set; }
        public string PaymentMethod { get; set; }
        public string InvoiceNumber { get; set; }

        public ICollection<BillingItem> BillingItems { get; set; }
    }
}
