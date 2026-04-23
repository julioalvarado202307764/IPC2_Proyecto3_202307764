using System.Collections.Generic;

namespace Proyecto3_Razor.Models
{
    public class ClienteVM
    {
        public string NIT { get; set; }
        public string Nombre { get; set; }
        public decimal SaldoActual { get; set; }
        public List<TransaccionVM> Transacciones { get; set; } = new List<TransaccionVM>();
    }
}