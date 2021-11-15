using DN.WebApi.Application.Wrapper;
using System.Threading.Tasks;

namespace DN.WebApi.Application.Abstractions.Services.Identity
{
    public interface IActiveDirectoryService : ITransientService
    {
        Task<bool> AuthenticateAsync(string userName, string password);
        Task<IResult> UpdateUserAsync(string userName, string password, bool rolesFromDeparment = true, bool rolesFromGroups = true);
        Task<IResult> ImportAdUsersAsync();
    }
}
