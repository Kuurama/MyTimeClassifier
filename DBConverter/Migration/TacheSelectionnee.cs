﻿using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DBConverter.Migration;

[Keyless]
[Table("TacheSelectionnee")]
public class TacheSelectionnee
{
    public string? nameTache { get; set; }
}