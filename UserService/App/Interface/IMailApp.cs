namespace UserService.App.Interface
{
    public interface IMailApp
    {
        Task<string> SendEmailCode(string email);
    }
}
