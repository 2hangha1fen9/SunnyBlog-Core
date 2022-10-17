namespace ArticleService.App.Interface
{
    public interface ISiteConfigApp
    {
        Task<string> SetConfig(string key,string value);
        Task<string> GetConfig(string key);
        Task<string> DelConfig(string key);
    }
}
