using System.Threading.Tasks;

namespace AriaPM.Services
{
    public interface IAuthenticaiton
    {
        Task<int?> IsAuthenticated(string userId, string password);
    }
}
