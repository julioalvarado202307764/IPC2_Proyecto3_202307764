namespace Proyecto3_API.Models
{// Representa el estado de cuenta completo de un cliente
    public class EstadoCuenta
    {
        public string NIT { get; set; }
        public string NombreCliente { get; set; }
        public decimal Saldo { get; set; }
        public List<TransaccionHistorial> Transacciones { get; set; } = new List<TransaccionHistorial>();
    }
}