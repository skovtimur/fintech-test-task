using System.ComponentModel.DataAnnotations;

namespace FintechTestTask.WebAPI.Queries;

public class LoginQuery
{
    [Required, StringLength(maximumLength: 24, MinimumLength = 1)] public string Name { get; set; }

    [Required, StringLength(maximumLength: 32, MinimumLength = 6)]
    public string Password { get; set; }

}