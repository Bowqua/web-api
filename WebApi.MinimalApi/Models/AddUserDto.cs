    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    namespace WebApi.MinimalApi.Models;

    public record AddUserDto(
        [Required] string Login,
        [property: DefaultValue("John")] string FirstName = "John",
        [property: DefaultValue("Doe")] string LastName = "Doe"
    );  