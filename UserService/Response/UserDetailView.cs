namespace UserService.Response
{
    public class UserDetailView
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Nick { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int Sex { get; set; }
        public DateTime Birthday { get; set; }
        public DateTime RegisterTime { get; set; }
        public string Remark { get; set; }
        public decimal Score { get; set; }
        public string Photo { get; set; }
        public int Status { get; set; }
    }
}
