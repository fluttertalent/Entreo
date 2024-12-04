using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Entreo.Models;

[Table(nameof(PostalCode))]
[Keyless]
public class PostalCode
{
    [MaxLength(10)]
    public string PoststalCode { get; set; } = string.Empty;
    [MaxLength(64)]
    public string City { get; set; } = string.Empty;
    [MaxLength(64)] public string Township { get; set; } = string.Empty;
    public float Latitude { get; set; }
    public float Longitude { get; set; }
}
