using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using Proyecto3_API.Services;

namespace Proyecto3_API.Controllers
{
    [ApiController]
    [Route("")]
    public class TransaccionController : ControllerBase
    {
        private readonly XmlProcessorService _xmlProcessor;
        private readonly DataStoreService _dataStore;

        public TransaccionController(XmlProcessorService xmlProcessor, DataStoreService dataStore)
        {
            _xmlProcessor = xmlProcessor;
            _dataStore = dataStore;
        }

        [HttpPost("grabarTransaccion")]
        public async Task<IActionResult> GrabarTransaccion()
        {
            using StreamReader reader = new StreamReader(Request.Body);
            string xmlContent = await reader.ReadToEndAsync();

            if (string.IsNullOrWhiteSpace(xmlContent))
                return BadRequest("El archivo XML está vacío.");

            // 1. Extraer datos
            var nuevasFacturas = _xmlProcessor.ExtraerFacturas(xmlContent);
            var nuevosPagos = _xmlProcessor.ExtraerPagos(xmlContent);

            // 2. Procesar y obtener contadores
            var (facturasNuevas, facturasDuplicadas, facturasConError) = _dataStore.ProcesarFacturas(nuevasFacturas);
            var (pagosNuevos, pagosDuplicados, pagosConError) = _dataStore.ProcesarPagos(nuevosPagos);

            // 3. Generar XML de respuesta exacto al PDF
            XDocument xmlRespuesta = new XDocument(
                new XDeclaration("1.0", "utf-8", null),
                new XElement("transacciones",
                    new XElement("facturas",
                        new XElement("nuevasFacturas", facturasNuevas),
                        new XElement("facturasDuplicadas", facturasDuplicadas),
                        new XElement("facturasConError", facturasConError)
                    ),
                    new XElement("pagos",
                        new XElement("nuevosPagos", pagosNuevos),
                        new XElement("pagosDuplicados", pagosDuplicados),
                        new XElement("pagosConError", pagosConError)
                    )
                )
            );

            return Content(xmlRespuesta.ToString(), "application/xml");
        }
    }
}