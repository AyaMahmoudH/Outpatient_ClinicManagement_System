namespace HospitalManagement.Models
{
    public class LoginResponseModel
    {
        public bool Succeeded { get; set; }
        public string Token { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }
        public DateTime Expiration { get; set; }
    }
}
