namespace HospitalManagement.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
        public string NotificationType { get; set; } 
        public string RelatedEntityId { get; set; } 
        public string Content { get; internal set; }
        public string Type { get; internal set; }
        public string ReferenceId { get; internal set; }
        public DateTime CreatedDate { get; internal set; }
    }
}
