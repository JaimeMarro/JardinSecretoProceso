using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema; // Required for [ForeignKey]

namespace JardinSecretoPrueba1.Models
{
    public partial class Extra
    {
        public int ExtraId { get; set; }
        public string Nombre { get; set; } = null!;
        public decimal PrecioExtra { get; set; }
        public int IdProducto { get; set; }

        // --- THIS IS THE MISSING LINE ---
        // This "navigation property" tells C# how to get to the related Product.
        [ForeignKey("IdProducto")]
        public virtual Producto? IdProductoNavigation { get; set; }
    }
}