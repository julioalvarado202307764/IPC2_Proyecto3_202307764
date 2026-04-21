using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using Proyecto3_API.Services;

namespace Proyecto3_API.Controllers
{
    [ApiController]
    [Route("")]
    public class ConfiguracionController : ControllerBase
    {
        private readonly XmlProcessorService _xmlProcessor;
        private readonly DataStoreService _dataStore;

        // Inyectamos los servicios a través del constructor
        public ConfiguracionController(XmlProcessorService xmlProcessor, DataStoreService dataStore)
        {
            _xmlProcessor = xmlProcessor;
            _dataStore = dataStore;
        }

        [HttpPost("grabarConfiguracion")]
        public async Task<IActionResult> GrabarConfiguracion()
        {
            //Leemos el cuerpo de la petición (el XML crudo)
            using StreamReader reader = new StreamReader(Request.Body);
            string xmlContent = await reader.ReadToEndAsync();

            if (string.IsNullOrWhiteSpace(xmlContent))
                return BadRequest("El archivo XML está vacío.");

            //Usamos el procesador para extraer los datos
            var nuevosClientes = _xmlProcessor.ExtraerClientes(xmlContent);
            var nuevosBancos = _xmlProcessor.ExtraerBancos(xmlContent);
            //Guardamos en memoria y obtenemos los contadores
            var (clientesCreados, clientesActualizados) = _dataStore.ProcesarClientes(nuevosClientes);
            var (bancosCreados, bancosActualizados) = _dataStore.ProcesarBancos(nuevosBancos); 

            //Construimos el XML de respuesta exacto que pide el PDF
            XDocument xmlRespuesta = new XDocument(
                new XDeclaration("1.0", "utf-8", null),
                new XElement("respuesta",
                    new XElement("clientes",
                        new XElement("creados", clientesCreados),
                        new XElement("actualizados", clientesActualizados)
                    ),
                    new XElement("bancos",
                        new XElement("creados", bancosCreados),
                        new XElement("actualizados", bancosActualizados)
                    )
                )
            );

            // Devolvemos el XML con el Content-Type correcto
            return Content(xmlRespuesta.ToString(), "application/xml");
        }
    }
}