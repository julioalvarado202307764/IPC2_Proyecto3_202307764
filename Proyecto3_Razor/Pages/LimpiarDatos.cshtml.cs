using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Threading.Tasks;
using System;

namespace Proyecto3_Razor.Pages
{
    public class LimpiarDatosModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public LimpiarDatosModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public string MensajeRespuesta { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var client = _httpClientFactory.CreateClient("API_Backend");

            try
            {
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
                MensajeRespuesta = $"Error de conexión con el backend: {ex.Message}";
            }

            return Page();
        }
    }
}