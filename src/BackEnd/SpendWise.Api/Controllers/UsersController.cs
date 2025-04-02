using Microsoft.AspNetCore.Mvc;
using SpendWise.Domain.Models.UserModels;
using SpendWise.Domain.Services.Contracts;


namespace SpendWise.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    
    public class UsersController : ControllerBase
    {
        private readonly IUserManagementService  _userManagementService;

        public UsersController(IUserManagementService userManagementService)
        {
            _userManagementService = userManagementService;
        }

        [HttpGet]

        public async Task<IActionResult> GetUsers()
        {
            List<User> users = await _userManagementService.GetAllUsers();
            return Ok(users);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetFriends(Guid userId)
        {
            List<User> friends = await _userManagementService.GetFriends(userId);
            return Ok(friends);
        }
    }
}


