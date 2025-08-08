using System.ComponentModel.DataAnnotations;

namespace EmailApplicationIdentity.Models.IdentityModels;

public class UserEditViewModel
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string PasswordConfirm { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string City { get; set; }
    public string ImageUrl { get; set; }
    [MinLength(10, ErrorMessage = "Phone number should be 10 character.")]
    [MaxLength(10, ErrorMessage = "Phone number should be 10 character.")]
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
}
