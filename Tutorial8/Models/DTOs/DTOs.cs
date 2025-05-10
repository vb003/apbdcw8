using System.ComponentModel.DataAnnotations;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace Tutorial8.Models.DTOs;

public class TripDTO
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(120)]
    public string Name { get; set; }
    public List<CountryDTO> Countries { get; set; }
    
    [StringLength(220)]
    public string? Description { get; set; }
    
    [Required]
    public DateTime DateFrom { get; set; }
    
    [Required]
    public DateTime DateTo { get; set; }
    public int maxPeople { get; set; }
}

public class CountryDTO
{
    [Required]
    [StringLength(120)]
    public string Name { get; set; }
}

public class ClientDTO
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(120, MinimumLength = 1)]
    public string FirstName { get; set; }
    
    [Required]
    [StringLength(120, MinimumLength = 1)]
    public string LastName { get; set; }
    
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    
    [Required]
    [StringLength(120)]
    public string Telephone { get; set; }
    
    [Required]
    [RegularExpression(@"^\d{11}$")]
    public string Pesel { get; set; }
}