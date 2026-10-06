using Microsoft.AspNetCore.Mvc;
using WebApi.MinimalApi.Domain;
using WebApi.MinimalApi.Models;

namespace WebApi.MinimalApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : Controller
{
    // Чтобы ASP.NET положил что-то в userRepository требуется конфигурация
    private IUserRepository userRepository;
    public UsersController(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    [HttpGet("{userId}")]
    public ActionResult<UserDto> GetUserById([FromRoute] Guid userId)
    {
        var src = userRepository.FindById(userId);
        var userDto = new UserDto() { Id = src.Id, Login = src.Login, FullName = src.FirstName + " " + src.LastName, GamesPlayed = src.GamesPlayed, CurrentGameId = src.CurrentGameId };
        return Ok(userDto);
    }

    [HttpPost]
    public IActionResult CreateUser([FromBody] object user)
    {
        throw new NotImplementedException();
    }
}