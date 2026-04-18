using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;
using Proyecto3_API.Services;
using System.Linq;

namespace Proyecto3_API.Controllers
{
    [ApiController]
    [Route("")]
    public class EstadoCuentaController : ControllerBase
    {
        private readonly DataStoreService _dataStore;

        public EstadoCuentaController(DataStoreService dataStore)
        {
            _dataStore = dataStore;
        }

        // Endpoint: GET /devolverEstadoCuenta?nit=12345
        [HttpGet("devolverEstadoCuenta")]
        public IActionResult DevolverEstadoCuenta([FromQuery] string nit = null)
        {
            // 1. Obtener los datos calculados desde el servicio
            var estados = _dataStore.ObtenerEstadosDeCuenta(nit);

            // 2. Construir la estructura XML de respuesta
            XDocument xmlRespuesta = new XDocument(
                new XDeclaration("1.0", "utf-8", null),
                new XElement("respuesta",
                    new XElement("clientes",
                        estados.Select(e => new XElement("cliente",
                            new XElement("NIT", e.NIT),
                            new XElement("nombre", e.NombreCliente),
                            new XElement("saldo_actual", e.Saldo.ToString("F2")),
                            new XElement("transacciones",
                                e.Transacciones.Select(t => new XElement("transaccion",
                                    new XElement("fecha", t.FechaStr),
                                    new XElement("cargo", t.Cargo.HasValue ? $"Q {t.Cargo.Value:F2} ({t.DetalleCargo})" : ""),
                                    new XElement("abono", t.Abono.HasValue ? $"Q {t.Abono.Value:F2} ({t.DetalleAbono})" : "")
                                ))
                            )
                        ))
                    )
                )
            );
            return Content(xmlRespuesta.ToString(), "application/xml");
        }
    }
}