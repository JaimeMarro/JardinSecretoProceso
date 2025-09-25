using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace JardinSecretoPrueba1.Models;

public partial class Sabor
{
    public int SaborId { get; set; }
    [Required]
    [StringLength(75)]
    public string Sabor1 { get; set; } = null!;

    [StringLength(75)]
    public string? Sabor2 { get; set; }

    [StringLength(75)]
    public string? Sabor3 { get; set; }

    [StringLength(75)]
    public string? Sabor4 { get; set; }

    [StringLength(75)]
    public string? Sabor5 { get; set; }

    public decimal? Precio_Sabor { get; set; }

    [Required]
    public int IdProducto { get; set; }

    public virtual Producto? IdProductoNavigation { get; set; }
}
