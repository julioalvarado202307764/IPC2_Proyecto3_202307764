namespace Proyecto3_API.Models
{
    // Representa una línea en el estado de cuenta
    public class TransaccionHistorial
    {
        public DateTime FechaOrdenamiento { get; set; } // Oculta, solo sirve para ordenar
        public string FechaStr { get; set; }
        public decimal? Cargo { get; set; } // Valor de la Factura (null si es pago)
        public string DetalleCargo { get; set; }
        public decimal? Abono { get; set; } // Valor del Pago (null si es factura)
        public string DetalleAbono { get; set; }
    }
}