using Microsoft.AspNetCore.Mvc;
using WebApi.MinimalApi.Domain;
using WebApi.MinimalApi.Models;
using AutoMapper;

namespace WebApi.MinimalApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : Controller
{
    // Чтобы ASP.NET положил что-то в userRepository требуется конфигурация
    private IUserRepository userRepository;
    private IMapper mapper;
    
    public UsersController(IUserRepository userRepository, IMapper mapper)
    {
        this.userRepository = userRepository;
        this.mapper = mapper;
    }

    [HttpGet("{userId:guid}")]
    [Produces("application/json", "application/xml")]
    public ActionResult<UserDto> GetUserById([FromRoute] Guid userId)
    {
        var src = userRepository.FindById(userId);
        if (src == null)
            return NotFound();
        
        var userDto = mapper.Map<UserDto>(src);
        return Ok(userDto);
    }

    [HttpPost]
    public IActionResult CreateUser([FromBody] object user)
    {
        throw new NotImplementedException();
    }
}