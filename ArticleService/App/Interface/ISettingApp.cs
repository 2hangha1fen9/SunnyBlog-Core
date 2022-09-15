using ArticleService.Domain;

namespace ArticleService.App.Interface
{
    public interface ISettingApp
    {
        Task<List<GlobalSetting>> ListSetting();
        Task<string> SetValue(string key, int value);
    }
}
