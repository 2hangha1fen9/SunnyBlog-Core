using UserService.Domain;
using UserService.Request;

namespace UserService.App.Interface
{
    public interface IScoreApp
    {
        Task<string> Increase(int id, string reason);
        Task<bool> CanIncrease(int id, string reason);
        Task<List<ScoreUnit>> ListUnit();
        Task<string> SetUnit(string key, decimal value);
    }
}
