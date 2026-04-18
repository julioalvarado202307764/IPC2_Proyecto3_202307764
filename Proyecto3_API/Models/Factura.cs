namespace Proyecto3_API.Models
{ // Modelo extraído de transac.xml
    public class Factura
    {
        public string NumeroFactura { get; set; }

        public string NITCliente { get; set; }

        public string Fecha { get; set; }

        public decimal Valor { get; set; }
    }
}