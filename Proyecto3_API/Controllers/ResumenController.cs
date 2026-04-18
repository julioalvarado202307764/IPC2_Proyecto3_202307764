using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;
using Proyecto3_API.Services;
using System.Linq;

namespace Proyecto3_API.Controllers
{
    [ApiController]
    [Route("")]
    public class ResumenController : ControllerBase
    {
        private readonly DataStoreService _dataStore;

        public ResumenController(DataStoreService dataStore)
        {
            _dataStore = dataStore;
        }

        // Endpoint: GET /devolverResumenPagos?mes=3&anio=2024
        [HttpGet("devolverResumenPagos")]
        public IActionResult DevolverResumenPagos([FromQuery] int mes, [FromQuery] int anio)
        {
            if (mes < 1 || mes > 12 || anio < 2000)
                return BadRequest("Mes o año inválido.");

            var (etiquetas, datos) = _dataStore.ObtenerResumenPagos(mes, anio);

            XDocument xmlRespuesta = new XDocument(
                new XDeclaration("1.0", "utf-8", null),
                new XElement("respuesta",
                    new XElement("rango_fechas",
                        new XElement("mes_1", etiquetas[0]),
                        new XElement("mes_2", etiquetas[1]),
                        new XElement("mes_3", etiquetas[2])
                    ),
                    new XElement("bancos",
                        datos.Select(b => new XElement("banco",
                            new XElement("nombre", b.NombreBanco),
                            new XElement("totales",
                                new XElement("mes_1", b.TotalMesAntiguo.ToString("F2")),
                                new XElement("mes_2", b.TotalMesMedio.ToString("F2")),
                                new XElement("mes_3", b.TotalMesActual.ToString("F2"))
                            )
                        ))
                    )
                )
            );

            return Content(xmlRespuesta.ToString(), "application/xml");
        }
    }
}