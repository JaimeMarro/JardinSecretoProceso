using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace JardinSecretoPrueba1.Models;

public partial class Extra
{
    public int ExtraId { get; set; }

    [StringLength(75)]
    public string? Extra1 { get; set; }

    [StringLength(75)]
    public string? Extra2 { get; set; }

    [StringLength(75)]
    public string? Extra3 { get; set; }

    [StringLength(75)]
    public string? Extra4 { get; set; }

    public decimal? Precio_Extra { get; set; }
    public int IdProducto { get; set; }

    public virtual Producto? IdProductoNavigation { get; set; }

}