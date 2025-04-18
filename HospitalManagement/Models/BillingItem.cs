namespace HospitalManagement.Models
{
    public class BillingItem
    {
        public int Id { get; set; }
        public int BillingId { get; set; }
        public Billing Billing { get; set; }
        public string ServiceDescription { get; set; }
        public decimal ServiceCost { get; set; }
        public int Quantity { get; set; }
        public decimal Subtotal { get; set; }
    }
}
