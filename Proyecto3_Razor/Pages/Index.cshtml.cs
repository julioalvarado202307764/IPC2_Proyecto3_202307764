using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Threading.Tasks;
using System;

namespace Proyecto3_Razor.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public string MensajeRespuesta { get; set; }

        public void OnGet()
        {
            // Método que carga la vista normal
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var client = _httpClientFactory.CreateClient("API_Backend");

            try
            {
                // Disparamos el POST hacia el endpoint de limpieza. 
                // Mandamos 'null' porque este endpoint no requiere ningún Body.
                var response = await client.PostAsync("/limpiarDatos", null);

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
            catch (Exception ex)
            {
                MensajeRespuesta = $"Error de conexión con el backend: Asegúrate de que la API esté corriendo. Detalles: {ex.Message}";
            }

            return Page();
        }
    }
}