using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema; // Necesario para [ForeignKey]

namespace JardinSecretoPrueba1.Models
{
    public partial class Sabor
    {
        public int SaborId { get; set; }
        public string Nombre { get; set; } = null!;
        public decimal PrecioSabor { get; set; }
        public int IdProducto { get; set; }

        // Esta "propiedad de navegación" le enseña a C# cómo llegar al Producto relacionado.
        [ForeignKey("IdProducto")]
        public virtual Producto? Producto { get; set; }
    }
}