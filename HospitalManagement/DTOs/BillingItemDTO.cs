namespace HospitalManagement.DTOs
{
    public class BillingItemDTO
    {
        public int Id { get; set; }

        public string ServiceDescription { get; set; }
        public decimal ServiceCost { get; set; }
        public int Quantity { get; set; }
        public decimal Subtotal { get; set; }
    }
}
