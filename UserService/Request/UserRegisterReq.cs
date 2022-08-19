namespace UserService.Request
{
    public class UserRegisterReq
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public string VerificationCode { get; set; }
    }
}
