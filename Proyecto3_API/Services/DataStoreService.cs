using System.Collections.Generic;
using System.Linq;
using Proyecto3_API.Models;
using System;
using System.IO;
using System.Xml.Linq;

namespace Proyecto3_API.Services
{
    public class DataStoreService
    {
        private readonly string _folderPath = "Data";
        private readonly string _configFileName = "DB_Configuracion.xml";
        private readonly string _transacFileName = "DB_Transacciones.xml";
        // Nuestras "tablas" en memoria
        public List<Cliente> Clientes { get; private set; } = new List<Cliente>();
        public List<Banco> Bancos { get; private set; } = new List<Banco>();
        public List<Factura> Facturas { get; private set; } = new List<Factura>();
        public List<Pago> Pagos { get; private set; } = new List<Pago>();

        //correr una vez al inicar la API
        public DataStoreService()
        {
            CargarDatosDesdeDisco();
        }
        // Método para procesar los clientes que vienen del XML
        public (int creados, int actualizados) ProcesarClientes(List<Cliente> nuevosClientes)
        {
            int creados = 0;
            int actualizados = 0;

            foreach (var nuevo in nuevosClientes)
            {
                var existente = Clientes.FirstOrDefault(c => c.NIT == nuevo.NIT);

                if (existente != null)
                {
                    //si el NIT se repite, se actualizan los datos
                    existente.Nombre = nuevo.Nombre;
                    actualizados++;
                }
                else
                {
                    // Si no existe, lo agregamos
                    Clientes.Add(nuevo);
                    creados++;
                }
            }
            GuardarDatosEnDisco();
            return (creados, actualizados);
        }

        // Método para procesar transacciones (Ejemplo parcial con Facturas)
        public (int nuevas, int duplicadas, int conError) ProcesarFacturas(List<Factura> nuevasFacturas)
        {
            int nuevas = 0;
            int duplicadas = 0;
            int conError = 0;

            foreach (var factura in nuevasFacturas)
            {
                // Validar que los datos críticos no vengan vacíos
                if (string.IsNullOrWhiteSpace(factura.NumeroFactura) || string.IsNullOrWhiteSpace(factura.NITCliente))
                {
                    conError++;
                    continue;
                }

                // validaciones que el cliente exista en la memoria
                bool clienteExiste = Clientes.Any(c => c.NIT == factura.NITCliente);
                if (!clienteExiste)
                {
                    conError++;
                    continue; // Cliente no existe, la factura tiene error
                }

                // Validar si la factura ya está registrada (duplicada)
                if (Facturas.Any(f => f.NumeroFactura == factura.NumeroFactura))
                {
                    duplicadas++;
                }
                else
                {
                    // Todo correcto, se agrega
                    Facturas.Add(factura);
                    nuevas++;
                }
            }

            if (nuevas > 0) GuardarDatosEnDisco();

            return (nuevas, duplicadas, conError);
        }

        public (int nuevos, int duplicados, int conError) ProcesarPagos(List<Pago> nuevosPagos)
        {
            int nuevos = 0;
            int duplicados = 0;
            int conError = 0;

            foreach (var pago in nuevosPagos)
            {
                // Validar que el NIT no venga vacío
                if (string.IsNullOrWhiteSpace(pago.NITCliente))
                {
                    conError++;
                    continue;
                }

                // validaciones que el cliente Y el banco existan en memoria
                bool clienteExiste = Clientes.Any(c => c.NIT == pago.NITCliente);
                bool bancoExiste = Bancos.Any(b => b.Codigo == pago.CodigoBanco);

                if (!clienteExiste || !bancoExiste)
                {
                    conError++;
                    continue; // Banco o Cliente falso, el pago tiene error
                }

                // Validar duplicado (Asumiendo que un pago es duplicado si coinciden todos sus datos)
                bool esDuplicado = Pagos.Any(p =>
                    p.CodigoBanco == pago.CodigoBanco &&
                    p.NITCliente == pago.NITCliente &&
                    p.Fecha == pago.Fecha &&
                    p.Valor == pago.Valor);

                if (esDuplicado)
                {
                    duplicados++;
                }
                else
                {
                    Pagos.Add(pago);
                    nuevos++;
                }
            }

            if (nuevos > 0) GuardarDatosEnDisco();

            return (nuevos, duplicados, conError);
        }

        public (int creados, int actualizados) ProcesarBancos(List<Banco> nuevosBancos)
        {
            int creados = 0;
            int actualizados = 0;

            foreach (var nuevo in nuevosBancos)
            {
                var existente = Bancos.FirstOrDefault(b => b.Codigo == nuevo.Codigo);

                if (existente != null)
                {
                    existente.Nombre = nuevo.Nombre;
                    actualizados++;
                }
                else
                {
                    Bancos.Add(nuevo);
                    creados++;
                }
            }
            GuardarDatosEnDisco();
            return (creados, actualizados);
        }

        // Método para el endpoint /limpiarDatos
        public void ResetearDatos()
        {
            Clientes.Clear();
            Bancos.Clear();
            Facturas.Clear();
            Pagos.Clear();
            GuardarDatosEnDisco();
        }

public List<EstadoCuenta> ObtenerEstadosDeCuenta(string nitRequerido = null)
        {
            var resultados = new List<EstadoCuenta>();

            // Si mandan NIT, filtramos a ese cliente. Si no, los traemos todos ordenados por NIT
            var clientesAProcesar = string.IsNullOrEmpty(nitRequerido)
                ? Clientes.OrderBy(c => c.NIT).ToList()
                : Clientes.Where(c => c.NIT == nitRequerido).ToList();

            foreach (var cliente in clientesAProcesar)
            {
                var historial = new List<TransaccionHistorial>();
                decimal saldoActual = 0;

                // 1. Procesar Facturas (Cargos)
                var facturasCliente = Facturas.Where(f => f.NITCliente == cliente.NIT).ToList();
                foreach (var fac in facturasCliente)
                {
                    // 🛠️ CORRECCIÓN 1: Las facturas (cargos) SUMAN a la deuda del cliente
                    saldoActual += fac.Valor; 

                    // Convertir dd/mm/yyyy a DateTime real para ordenar
                    DateTime.TryParseExact(fac.Fecha, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime fechaParsed);

                    historial.Add(new TransaccionHistorial
                    {
                        FechaOrdenamiento = fechaParsed,
                        FechaStr = fac.Fecha,
                        Cargo = fac.Valor,
                        DetalleCargo = $"Fact. # {fac.NumeroFactura}"
                    });
                }

                // 2. Procesar Pagos (Abonos)
                var pagosCliente = Pagos.Where(p => p.NITCliente == cliente.NIT).ToList();
                foreach (var pago in pagosCliente)
                {
                    // 🛠️ CORRECCIÓN 2: Los pagos (abonos) RESTAN a la deuda del cliente
                    saldoActual -= pago.Valor; 

                    // Buscar el nombre del banco para el detalle
                    var banco = Bancos.FirstOrDefault(b => b.Codigo == pago.CodigoBanco);
                    string nombreBanco = banco != null ? banco.Nombre : pago.CodigoBanco.ToString();

                    DateTime.TryParseExact(pago.Fecha, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime fechaParsed);

                    historial.Add(new TransaccionHistorial
                    {
                        FechaOrdenamiento = fechaParsed,
                        FechaStr = pago.Fecha,
                        Abono = pago.Valor,
                        DetalleAbono = nombreBanco
                    });
                }
                
                // 🛠️ CORRECCIÓN 3: OrderBy (Ascendente) para que la fecha más vieja salga primero
                historial = historial.OrderBy(t => t.FechaOrdenamiento).ToList();

                resultados.Add(new EstadoCuenta
                {
                    NIT = cliente.NIT,
                    NombreCliente = cliente.Nombre,
                    Saldo = saldoActual,
                    Transacciones = historial
                });
            }

            return resultados;
        }

        public (List<string> EtiquetasMeses, List<ResumenBanco> DatosBancos) ObtenerResumenPagos(int mes, int anio)
        {
            // 1. Calcular los 3 meses objetivo
            DateTime mesActual = new DateTime(anio, mes, 1);
            DateTime mesMedio = mesActual.AddMonths(-1);
            DateTime mesAntiguo = mesActual.AddMonths(-2);

            // Nombres para las etiquetas del XML (ej. "01/2024", "02/2024", "03/2024")
            var etiquetas = new List<string> {
        mesAntiguo.ToString("MM/yyyy"),
        mesMedio.ToString("MM/yyyy"),
        mesActual.ToString("MM/yyyy")
    };

            var resumen = new List<ResumenBanco>();

            // 2. Agrupar y sumar por cada banco existente
            foreach (var banco in Bancos)
            {
                decimal sumaAntiguo = 0, sumaMedio = 0, sumaActual = 0;

                // Filtramos solo los pagos que pertenecen a este banco
                var pagosBanco = Pagos.Where(p => p.CodigoBanco == banco.Codigo).ToList();

                foreach (var pago in pagosBanco)
                {
                    // Convertimos la fecha del pago para evaluar su mes y año
                    if (DateTime.TryParseExact(pago.Fecha, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime fechaPago))
                    {
                        if (fechaPago.Year == mesAntiguo.Year && fechaPago.Month == mesAntiguo.Month)
                            sumaAntiguo += pago.Valor;
                        else if (fechaPago.Year == mesMedio.Year && fechaPago.Month == mesMedio.Month)
                            sumaMedio += pago.Valor;
                        else if (fechaPago.Year == mesActual.Year && fechaPago.Month == mesActual.Month)
                            sumaActual += pago.Valor;
                    }
                }

                resumen.Add(new ResumenBanco
                {
                    NombreBanco = banco.Nombre,
                    TotalMesAntiguo = sumaAntiguo,
                    TotalMesMedio = sumaMedio,
                    TotalMesActual = sumaActual
                });
            }

            return (etiquetas, resumen);
        }

        public void GuardarDatosEnDisco()
        {
            if (!Directory.Exists(_folderPath))
            {
                Directory.CreateDirectory(_folderPath);
            }

            // Guardar Configuración
            XDocument dbConfig = new XDocument(
                new XElement("configuracion_db",
                    new XElement("clientes", Clientes.Select(c => new XElement("cliente",
                    new XElement("NIT", c.NIT),
                    new XElement("nombre", c.Nombre)))),
                    new XElement("bancos", Bancos.Select(b => new XElement("banco",
                    new XElement("codigo", b.Codigo),
                    new XElement("nombre", b.Nombre))))
                )
            );
            dbConfig.Save(Path.Combine(_folderPath, _configFileName));

            // Guardar Transacciones
            XDocument dbTransac = new XDocument(
                new XElement("transacciones_db",
                    new XElement("facturas", Facturas.Select(f =>
                    new XElement("factura", new XElement("numeroFactura", f.NumeroFactura),
                    new XElement("NITcliente", f.NITCliente),
                    new XElement("fecha", f.Fecha),
                    new XElement("valor", f.Valor)))),
                    new XElement("pagos", Pagos.Select(p =>
                    new XElement("pago", new XElement("codigoBanco", p.CodigoBanco),
                    new XElement("fecha", p.Fecha), new XElement("NITcliente", p.NITCliente),
                    new XElement("valor", p.Valor))))
                )
            );
            dbTransac.Save(Path.Combine(_folderPath, _transacFileName));
        }

        //cargar los datos al iniciar la API
        private void CargarDatosDesdeDisco()
        {
            string configPath = Path.Combine(_folderPath, _configFileName);
            string transacPath = Path.Combine(_folderPath, _transacFileName);

            // 1. Cargar Clientes y Bancos
            if (File.Exists(configPath))
            {
                XDocument doc = XDocument.Load(configPath);

                Clientes = doc.Descendants("cliente").Select(c => new Cliente
                {
                    NIT = c.Element("NIT")?.Value,
                    Nombre = c.Element("nombre")?.Value
                }).ToList();

                Bancos = doc.Descendants("banco").Select(b => new Banco
                {
                    Codigo = int.Parse(b.Element("codigo")?.Value ?? "0"),
                    Nombre = b.Element("nombre")?.Value
                }).ToList();
            }

            // 2. Cargar Facturas y Pagos
            if (File.Exists(transacPath))
            {
                XDocument doc = XDocument.Load(transacPath);

                Facturas = doc.Descendants("factura").Select(f => new Factura
                {
                    NumeroFactura = f.Element("numeroFactura")?.Value,
                    NITCliente = f.Element("NITcliente")?.Value,
                    Fecha = f.Element("fecha")?.Value,
                    Valor = decimal.Parse(f.Element("valor")?.Value ?? "0")
                }).ToList();

                Pagos = doc.Descendants("pago").Select(p => new Pago
                {
                    CodigoBanco = int.Parse(p.Element("codigoBanco")?.Value ?? "0"),
                    Fecha = p.Element("fecha")?.Value,
                    NITCliente = p.Element("NITcliente")?.Value,
                    Valor = decimal.Parse(p.Element("valor")?.Value ?? "0")
                }).ToList();
            }
        }
    }

}