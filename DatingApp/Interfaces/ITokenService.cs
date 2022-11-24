using DatingApp.Models;
using System.Threading.Tasks;

namespace DatingApp.Interfaces
{
    public interface ITokenService
    {
        Task<string> CreateToken(User user);
    }
}
