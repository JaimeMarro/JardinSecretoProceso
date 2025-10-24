// Models/CarritoItem.cs (actualizar)
namespace JardinSecretoPrueba1.Models
{
    public class CarritoItem
    {
        //Identificador unico para item carrito
        public Guid CartItemId { get; set; } = Guid.NewGuid(); //Se genera automaticamente
        public int ProductoId { get; set; }
        public string Nombre { get; set; } = "";
        public decimal Precio { get; set; }
        public int Cantidad { get; set; }
        public string ImagenUrl { get; set; } = "";

        // Nuevos campos
        public int? SaborId { get; set; }
        public string? SaborNombre { get; set; }
        public decimal SaborPrecio { get; set; }

        // Ahora usamos una lista de nuestra nueva clase "ExtraItem"
        public List<ExtraItem> Extras { get; set; } = new();

        // Calculado
        public decimal Subtotal
        {
            get
            {
                decimal extrasTotal = Extras?.Sum(e => e.Precio) ?? 0m;
                return (Precio + SaborPrecio + extrasTotal) * Cantidad;
            }
        }
    }
}
