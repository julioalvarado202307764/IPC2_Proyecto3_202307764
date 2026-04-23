using System.Collections.Generic;

namespace Proyecto3_Razor.Models
{
    public class RangoFechasVM
    {
        public string Mes1 { get; set; }
        public string Mes2 { get; set; }
        public string Mes3 { get; set; }
    }

    public class BancoResumenVM
    {
        public string Nombre { get; set; }
        public decimal TotalMes1 { get; set; }
        public decimal TotalMes2 { get; set; }
        public decimal TotalMes3 { get; set; }
    }

    public class ResumenPagosVM
    {
        public RangoFechasVM Fechas { get; set; } = new RangoFechasVM();
        public List<BancoResumenVM> Bancos { get; set; } = new List<BancoResumenVM>();
    }
}