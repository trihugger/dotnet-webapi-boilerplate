using DN.WebApi.Application.Wrapper;
using System.Threading.Tasks;

namespace DN.WebApi.Application.Abstractions.Services.Identity
{
    public interface IActiveDirectoryService : ITransientService
    {
        bool AuthenticateAsync(string domain, string userName, string password);
        Task<IResult> UpdateUserAsync(string userName, string password);
        Task<IResult> ImportAdUsersAsync();
    }
}
