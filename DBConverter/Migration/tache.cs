using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DBConverter.Migration;

[Table("tache")]
public class tache
{
    [Key]
    public int empId { get; set; }

    [Column("tache")]
    public string? JobName { get; set; }

    public string? dateDebut { get; set; }

    public string? dateFin { get; set; }

    public long? dateD { get; set; }

    public long? dateF { get; set; }

    public int? mois { get; set; }

    public int? annee { get; set; }
}
