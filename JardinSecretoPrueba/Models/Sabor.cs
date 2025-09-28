using System;
using System.Collections.Generic;

namespace JardinSecretoPrueba1.Models;

public partial class Sabor
{
    public int SaborId { get; set; }

    public int IdProducto { get; set; }

    public string Nombre { get; set; } = null!;

    public decimal PrecioSabor { get; set; }

    public virtual Producto? IdProductoNavigation { get; set; }
}
