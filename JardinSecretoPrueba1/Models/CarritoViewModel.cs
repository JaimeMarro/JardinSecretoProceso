using System.Collections.Generic;

namespace JardinSecretoPrueba1.Models
{
    public class CarritoViewModel
    {
        public int ProductoId { get; set; }
        public int SaborId { get; set; }
        public List<int> ExtrasIds { get; set; } = new List<int>();
    }
}