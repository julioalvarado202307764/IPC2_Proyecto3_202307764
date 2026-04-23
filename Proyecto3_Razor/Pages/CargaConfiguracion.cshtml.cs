using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto3_Razor.Pages
{
    public class CargaConfiguracionModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CargaConfiguracionModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // Propiedad para capturar el archivo del formulario
        [BindProperty]
        public IFormFile ArchivoXml { get; set; }

        // Propiedad para mostrar el resultado en la pantalla
        public string MensajeRespuesta { get; set; }

        public void OnGet()
        {
            // Se ejecuta al cargar la página por primera vez
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ArchivoXml == null || ArchivoXml.Length == 0)
            {
                MensajeRespuesta = "Error: Por favor seleccione un archivo válido.";
                return Page();
            }

            // 1. Leer el contenido del archivo subido
            using var reader = new System.IO.StreamReader(ArchivoXml.OpenReadStream());
            string contenidoXml = await reader.ReadToEndAsync();

            // 2. Preparar el paquete para la API (con el Content-Type correcto)
            var content = new StringContent(contenidoXml, Encoding.UTF8, "application/xml");

            // 3. Crear el cliente y apuntar a nuestro endpoint del Backend
            var client = _httpClientFactory.CreateClient("API_Backend");

            try
            {
                // 4. Disparar el POST a la API
                var response = await client.PostAsync("/grabarConfiguracion", content);
                
                // 5. Leer el XML de respuesta que nos devolvió el servidor
                string respuestaApi = await response.Content.ReadAsStringAsync();
                
                if (response.IsSuccessStatusCode)
                {
                    MensajeRespuesta = respuestaApi; // Mostrará el XML con los creados/actualizados
                }
                else
                {
                    MensajeRespuesta = $"Error de la API: {response.StatusCode}\n{respuestaApi}";
                }
            }
            catch (System.Exception ex)
            {
                MensajeRespuesta = $"Error de conexión con el backend: Asegúrate de que la API esté corriendo. Detalles: {ex.Message}";
            }

            return Page();
        }
    }
}