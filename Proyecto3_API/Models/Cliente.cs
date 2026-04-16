namespace Proyecto3_API.Models
{
    // Modelo extraído de config.xml
    public class Cliente
    {
        public string NIT { get; set; }
        public string Nombre { get; set; }
    }

    // Modelo extraído de config.xml
    public class Banco
    {
        public int Codigo { get; set; }
        public string Nombre { get; set; }
    }

    // Modelo extraído de transac.xml
    public class Factura
    {
        public string NumeroFactura { get; set; }

        public string NITCliente { get; set; }

        public string Fecha { get; set; }

        public decimal Valor { get; set; }
    }

    // Modelo extraído de transac.xml
    public class Pago
    {
        public int CodigoBanco { get; set; }

        public string Fecha { get; set; }

        public string NITCliente { get; set; }

        public decimal Valor { get; set; }
    }
}