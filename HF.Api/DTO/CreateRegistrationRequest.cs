
using System.ComponentModel.DataAnnotations;

namespace HF.Api.DTOs;

public class CreateRegistrationRequest
{
    [Required(ErrorMessage = "Ime in priimek sta obvezna.")]
    [StringLength(150, MinimumLength = 2)]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "E-poštni naslov je obvezen.")]
    [EmailAddress(ErrorMessage = "Neveljaven e-poštni naslov.")]
    [StringLength(254)]
    public string Email { get; set; } = string.Empty;
}
