using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace JardinSecretoPrueba1.Models;

public partial class Categoria
{
    public int CategoriaId { get; set; }

    [Required]
    [StringLength(50)]
    public string Nombre { get; set; } = null!;

    [StringLength(255)]
    public string? Descripcion { get; set; }

    [Required]
    public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
}
