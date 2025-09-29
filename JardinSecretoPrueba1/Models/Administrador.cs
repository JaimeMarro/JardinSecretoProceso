using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace JardinSecretoPrueba1.Models;

public partial class Administrador
{
    public int AdminId { get; set; }

    [Required]
    [StringLength(50)]
    public string Usuario { get; set; } = null!;

    [Required]
    [StringLength(255)]
    public string ContraseñaHash { get; set; } = null!;

    public DateTime? FechaCreacion { get; set; } = DateTime.Now;
}
