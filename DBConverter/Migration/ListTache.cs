using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DBConverter.Migration;

[Table("ListTache")]
public class ListTache
{
    [Key]
    public int idTache { get; set; }

    public string? nameTache { get; set; }
}
