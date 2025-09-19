using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace JardinSecretoPrueba1.Models;

public partial class Producto
{
    public int ProductoId { get; set; }

    [Required]
    [StringLength(100)]
    public string Nombre { get; set; } = null!;

    [StringLength(255)]
    public string? Descripcion { get; set; }

    [Range(0.1, double.MaxValue, ErrorMessage = "El precio debe ser mayor a '0'")]
    public decimal Precio { get; set; }

    public bool Disponible { get; set; }

    //[Required(ErrorMessage = "Debes colocar una imagen, obligatoriamente")]
    //[Url]
    [StringLength(500)]
    public string? ImagenUrl { get; set; }

    public int? CategoriaId { get; set; }

    
    public virtual Categoria? Categoria { get; set; }
}
