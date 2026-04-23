using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto3_Razor.Pages
{
    public class CargaTransaccionesModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CargaTransaccionesModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty]
        public IFormFile ArchivoXml { get; set; }

        public string MensajeRespuesta { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ArchivoXml == null || ArchivoXml.Length == 0)
            {
                MensajeRespuesta = "Error: Por favor seleccione un archivo válido.";
                return Page();
            }

            using var reader = new System.IO.StreamReader(ArchivoXml.OpenReadStream());
            string contenidoXml = await reader.ReadToEndAsync();

            var content = new StringContent(contenidoXml, Encoding.UTF8, "application/xml");
            var client = _httpClientFactory.CreateClient("API_Backend");

            try
            {
                var response = await client.PostAsync("/grabarTransaccion", content);
                
                string respuestaApi = await response.Content.ReadAsStringAsync();
                
                if (response.IsSuccessStatusCode)
                {
                    MensajeRespuesta = respuestaApi; 
                }
                else
                {
                    MensajeRespuesta = $"Error de la API: {response.StatusCode}\n{respuestaApi}";
                }
            }
            catch (System.Exception ex)
            {
                MensajeRespuesta = $"Error de conexión con el backend: {ex.Message}";
            }

            return Page();
        }
    }
}