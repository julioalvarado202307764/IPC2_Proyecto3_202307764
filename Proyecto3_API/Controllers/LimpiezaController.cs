using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;
using Proyecto3_API.Services;

namespace Proyecto3_API.Controllers
{
    [ApiController]
    [Route("")]
    public class LimpiezaController : ControllerBase
    {
        private readonly DataStoreService _dataStore;

        public LimpiezaController(DataStoreService dataStore)
        {
            _dataStore = dataStore;
        }

        [HttpPost("limpiarDatos")]
        public IActionResult LimpiarDatos()
        {
            // Ejecutamos la limpieza de todas las listas (Clientes, Bancos, Facturas, Pagos)
            _dataStore.ResetearDatos();

            // Aunque el PDF no exige un formato de respuesta específico para este endpoint, 
            // es buena práctica devolver un XML confirmando la acción.
            XDocument xmlRespuesta = new XDocument(
                new XDeclaration("1.0", "utf-8", null),
                new XElement("respuesta",
                    new XElement("mensaje", "Datos eliminados exitosamente"),
                    new XElement("estado", "Sistema en estado inicial")
                )
            );

            return Content(xmlRespuesta.ToString(), "application/xml");
        }
    }
}