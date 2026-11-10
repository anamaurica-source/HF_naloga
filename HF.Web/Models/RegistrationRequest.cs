
using System.ComponentModel.DataAnnotations;

namespace HF.Web.Models;

public class RegistrationRequest
{
    [Required(ErrorMessage = "Vnesi ime in priimek.")]
    [StringLength(150, MinimumLength = 2)]
    public string FullName { get; set; } = "";

    [Required(ErrorMessage = "Vnesi e-poštni naslov.")]
    [EmailAddress(ErrorMessage = "Neveljaven e-poštni naslov.")]
    public string Email { get; set; } = "";
}
