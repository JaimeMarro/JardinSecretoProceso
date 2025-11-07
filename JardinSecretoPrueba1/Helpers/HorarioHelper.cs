using System;

namespace JardinSecretoPrueba1.Helpers
{
    public static class HorarioHelper
    {
        // Esta función ahora es la ÚNICA fuente de verdad.
        // Devuelve el texto, la clase CSS, y un bool 'EstaAbierto'
        public static (string Estado, string ColorClass, bool EstaAbierto) ObtenerEstadoRestaurante()
        {
            try
            {
                // Usamos la zona horaria
                var timeZoneId = "Central America Standard Time";
                var elSalvadorTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
                var nowElSalvador = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, elSalvadorTimeZone);

                var day = nowElSalvador.DayOfWeek;
                var hour = nowElSalvador.Hour; // 0-23

                // Cerrado todo el miércoles
                if (day == DayOfWeek.Wednesday)
                {
                    return ("Cerrado", "text-danger", false);
                }

                // Horario: 4 PM (16:00) a 2 AM (01:59)
                bool isOpen = (hour >= 16 || hour < 2);

                if (isOpen)
                {
                    return ("Abierto", "text-success", true);
                }
                else
                {
                    return ("Cerrado", "text-danger", false);
                }
            }
            catch (Exception ex)
            {
                // Si algo falla (como no encontrar la zona horaria),
                // es más seguro decir que está cerrado.
                Console.Error.WriteLine($"Error al obtener estado del restaurante: {ex.Message}");
                return ("Cerrado", "text-danger", false);
            }
        }
    }
}