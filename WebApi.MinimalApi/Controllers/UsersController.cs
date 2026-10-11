using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.JsonPatch;
using WebApi.MinimalApi.Domain;
using WebApi.MinimalApi.Models;
using AutoMapper;

namespace WebApi.MinimalApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : Controller
{
    private readonly IUserRepository userRepository;
    private readonly IMapper mapper;
    
    public UsersController(IUserRepository userRepository, IMapper mapper)
    {
        this.userRepository = userRepository;
        this.mapper = mapper;
    }

    [HttpGet("{userId}", Name = nameof(GetUserById))]
    [Produces("application/json", "application/xml")]
    public ActionResult<UserDto> GetUserById([FromRoute] Guid userId)
    {
        var userEntity = userRepository.FindById(userId);
        if (userEntity == null)
            return NotFound();
        
        var userDto = mapper.Map<UserDto>(userEntity);
        return Ok(userDto);
    }

    [HttpPost]
    public IActionResult CreateUser([FromBody] AddUserDto? user)
    {
        if (user == null)
        {
            return BadRequest();
        }
        
        if (!string.IsNullOrEmpty(user.Login) && !user.Login.All(char.IsLetterOrDigit))
        {
            ModelState.AddModelError("Login", "Login should contain only letters and digits");
        }

        if (!ModelState.IsValid)
        {
            return UnprocessableEntity(ModelState);
        }
        
        var userEntity = mapper.Map<UserEntity>(user);
        var createdUserEntity = userRepository.Insert(userEntity);
        
        var userDto = mapper.Map<UserDto>(createdUserEntity);

        return CreatedAtRoute(
            nameof(GetUserById),
            new { userId = createdUserEntity.Id },
            userDto);
    }
    
    [HttpPut("{userId}")]
    public IActionResult UpdateUser([FromRoute] string userId, [FromBody] UpdateUserDto? user)
    {
        if (user == null)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return UnprocessableEntity(ModelState);
        }

        if (!Guid.TryParse(userId, out var userGuid))
        {
            return BadRequest();
        }
        
        var userEntity = mapper.Map(user, new UserEntity(userGuid));

        userRepository.UpdateOrInsert(userEntity, out var isInserted);

        if (isInserted)
        {
            var userDto = mapper.Map<UserDto>(userEntity);

            return CreatedAtRoute(
                nameof(GetUserById),
                new { userId = userEntity.Id },
                userDto);
        }

        return NoContent();
    }

    [HttpPatch("{userId}")]
    public IActionResult PartiallyUpdateUser([FromRoute] string userId, [FromBody] JsonPatchDocument<UpdateUserDto>? patchDoc)
    {
        if (patchDoc == null)
            return BadRequest();
        
        if (!Guid.TryParse(userId, out var userGuid))
            return NotFound();
        
        var userEntity = userRepository.FindById(userGuid);
        if (userEntity == null)
            return NotFound();

        var updateDto = new UpdateUserDto
        {
            Login = userEntity.Login,
            FirstName = userEntity.FirstName,
            LastName = userEntity.LastName,
        };
        
        patchDoc.ApplyTo(updateDto, ModelState);
        if (ModelState.IsValid)
            TryValidateModel(updateDto);
        
        if (!ModelState.IsValid)
        {
            var errors = ModelState.ToDictionary(
                entry => char.ToLowerInvariant(entry.Key[0]) + entry.Key[1..],
                entry => entry.Value.Errors.Select(error => error.ErrorMessage).ToArray()
            );

            return UnprocessableEntity(errors);
        }
        
        mapper.Map(updateDto, userEntity);
        userRepository.Update(userEntity);
        
        return NoContent();
    }

    [HttpDelete("{userId}")]
    public IActionResult DeleteUser([FromRoute] string userId)
    {
        if (!Guid.TryParse(userId, out var userGuid))
            return NotFound();
        
        var userEntity = userRepository.FindById(userGuid);
        if (userEntity == null)
            return NotFound();
        
        userRepository.Delete(userGuid);
        return NoContent();
    }

    [HttpHead("{userId}")]
    public IActionResult HeadUserById([FromRoute] string userId)
    {
        if (!Guid.TryParse(userId, out var userGuid))
            return NotFound();
        
        var userEntity = userRepository.FindById(userGuid);
        if (userEntity == null)
            return NotFound();
        
        Response.ContentType = "application/json; charset=utf-8";
        return Ok();
    }
}