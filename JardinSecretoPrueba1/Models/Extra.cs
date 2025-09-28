using System;
using System.Collections.Generic;

namespace JardinSecretoPrueba1.Models;

public partial class Extra
{
    public int ExtraId { get; set; }


    public int IdProducto { get; set; }

    public string Nombre { get; set; } = null!;

    public decimal PrecioExtra { get; set; }

    public virtual Producto? IdProductoNavigation { get; set; }
}
