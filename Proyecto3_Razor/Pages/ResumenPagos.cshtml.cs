using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Xml.Linq;
using System.Net.Http;
using System.Linq;
using System.Threading.Tasks;
using System;
using Proyecto3_Razor.Models;

namespace Proyecto3_Razor.Pages
{
    public class ResumenPagosModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ResumenPagosModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // Estas propiedades capturan lo que el usuario escribe en la URL o el formulario
        [BindProperty(SupportsGet = true)]
        public int? Mes { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? Anio { get; set; }

        public ResumenPagosVM ResumenDatos { get; set; }
        public string MensajeError { get; set; }

        public async Task OnGetAsync()
        {
            // Solo consultamos la API si el usuario ya ingresó un mes y año
            if (Mes.HasValue && Anio.HasValue)
            {
                var client = _httpClientFactory.CreateClient("API_Backend");
                try
                {
                    var response = await client.GetAsync($"/devolverResumenPagos?mes={Mes.Value}&anio={Anio.Value}");
                    
                    if (response.IsSuccessStatusCode)
                    {
                        var contenido = await response.Content.ReadAsStringAsync();
                        XDocument doc = XDocument.Parse(contenido);

                        ResumenDatos = new ResumenPagosVM();
                        
                        var rango = doc.Descendants("rango_fechas").FirstOrDefault();
                        if (rango != null)
                        {
                            ResumenDatos.Fechas.Mes1 = rango.Element("mes_1")?.Value;
                            ResumenDatos.Fechas.Mes2 = rango.Element("mes_2")?.Value;
                            ResumenDatos.Fechas.Mes3 = rango.Element("mes_3")?.Value;
                        }

                        ResumenDatos.Bancos = doc.Descendants("banco").Select(b => new BancoResumenVM
                        {
                            Nombre = b.Element("nombre")?.Value,
                            TotalMes1 = decimal.Parse(b.Element("totales")?.Element("mes_1")?.Value ?? "0"),
                            TotalMes2 = decimal.Parse(b.Element("totales")?.Element("mes_2")?.Value ?? "0"),
                            TotalMes3 = decimal.Parse(b.Element("totales")?.Element("mes_3")?.Value ?? "0")
                        }).ToList();
                    }
                    else
                    {
                        MensajeError = "No se encontraron datos para esa fecha o la API devolvió un error.";
                    }
                }
                catch (Exception ex)
                {
                    MensajeError = $"Error de conexión con el backend: {ex.Message}";
                }
            }
        }
    }
}