using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Xml.Linq;
using System.Net.Http;
using Proyecto3_Razor.Models;
namespace Proyecto3_Razor.Pages
{
    public class EstadoCuentaModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public EstadoCuentaModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public List<ClienteVM> Clientes { get; set; } = new List<ClienteVM>();

        public async Task OnGetAsync(string nit)
        {
            var client = _httpClientFactory.CreateClient("API_Backend");
            
            // Construimos la URL. Si hay NIT, lo pasamos como parámetro.
            string url = "/devolverEstadoCuenta";
            if (!string.IsNullOrEmpty(nit)) url += $"?nit={nit}";

            try
            {
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var contenido = await response.Content.ReadAsStringAsync();
                    XDocument doc = XDocument.Parse(contenido);

                    // Mapeamos el XML a nuestra clase ViewModel
                    Clientes = doc.Descendants("cliente").Select(c => new ClienteVM
                    {
                        NIT = c.Element("NIT")?.Value,
                        Nombre = c.Element("nombre")?.Value,
                        SaldoActual = decimal.Parse(c.Element("saldo_actual")?.Value ?? "0"),
                        Transacciones = c.Descendants("transaccion").Select(t => new TransaccionVM
                        {
                            Fecha = t.Element("fecha")?.Value,
                            Cargo = t.Element("cargo")?.Value,
                            Abono = t.Element("abono")?.Value
                        }).ToList()
                    }).ToList();
                }
            }
            catch (Exception) { /* Manejar error de conexión */ }
        }
    }
}