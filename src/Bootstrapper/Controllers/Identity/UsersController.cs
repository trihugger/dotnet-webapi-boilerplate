using DN.WebApi.Application.Abstractions.Services.Identity;
using DN.WebApi.Shared.DTOs.Identity.Requests;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DN.WebApi.Bootstrapper.Controllers.Identity
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IActiveDirectoryService _activeDirectoryService;

        public UsersController(IUserService userService, IActiveDirectoryService activeDirectoryService)
        {
            _userService = userService;
            _activeDirectoryService = activeDirectoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(string id)
        {
            var user = await _userService.GetAsync(id);
            return Ok(user);
        }

        [HttpGet("{id}/roles")]
        public async Task<IActionResult> GetRolesAsync(string id)
        {
            var userRoles = await _userService.GetRolesAsync(id);
            return Ok(userRoles);
        }

        [HttpGet("{id}/permissions")]
        public async Task<IActionResult> GetPermissionsAsync(string id)
        {
            var userPermissions = await _userService.GetPermissionsAsync(id);
            return Ok(userPermissions);
        }

        [HttpPost("{id}/roles")]
        public async Task<IActionResult> AssignRolesAsync(string id, UserRolesRequest request)
        {
            var result = await _userService.AssignRolesAsync(id, request);
            return Ok(result);
        }

        [HttpGet("import/")]
        public async Task<IActionResult> ImportAdUsersAsync()
        {
            var result = await _activeDirectoryService.ImportAdUsersAsync();
            if (result.Succeeded) return Ok();

            return UnprocessableEntity(result);
        }
    }
}