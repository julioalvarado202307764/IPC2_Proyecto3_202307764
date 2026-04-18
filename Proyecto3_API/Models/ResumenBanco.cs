namespace Proyecto3_API.Models
{
    public class ResumenBanco
    {
        public string NombreBanco { get; set; }
        public decimal TotalMesAntiguo { get; set; } // Hace 2 meses
        public decimal TotalMesMedio { get; set; }   // Hace 1 mes
        public decimal TotalMesActual { get; set; }  // El mes solicitado
    }
}