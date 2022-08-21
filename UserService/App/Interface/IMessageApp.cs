namespace UserService.App.Interface
{
    public interface IMessageApp
    {
        Task<string> SendMessageCode(string phone);
    }
}
