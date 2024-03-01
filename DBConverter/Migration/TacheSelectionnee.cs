using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace DBConverter.Migration;

[Keyless]
[Table("TacheSelectionnee")]
public class TacheSelectionnee
{
    public string? nameTache { get; set; }
}
