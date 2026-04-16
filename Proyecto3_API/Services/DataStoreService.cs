using System.Collections.Generic;
using System.Linq;
using Proyecto3_API.Models;

namespace Proyecto3_API.Services
{
    public class DataStoreService
    {
        // Nuestras "tablas" en memoria
        public List<Cliente> Clientes { get; private set; } = new List<Cliente>();
        public List<Banco> Bancos { get; private set; } = new List<Banco>();
        public List<Factura> Facturas { get; private set; } = new List<Factura>();
        public List<Pago> Pagos { get; private set; } = new List<Pago>();

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

            return (creados, actualizados);
        }

        // Método para procesar transacciones (Ejemplo parcial con Facturas)
        public (int nuevas, int duplicadas) ProcesarFacturas(List<Factura> nuevasFacturas)
        {
            int nuevas = 0;
            int duplicadas = 0;

            foreach (var nueva in nuevasFacturas)
            {
                // Asumiendo que el "NumeroFactura" es el identificador único
                if (Facturas.Any(f => f.NumeroFactura == nueva.NumeroFactura))
                {
                    // Regla del PDF: En transacciones no hay actualizaciones, son duplicados
                    duplicadas++;
                }
                else
                {
                    Facturas.Add(nueva);
                    nuevas++;
                }
            }

            return (nuevas, duplicadas);
        }

        // Método para el endpoint /limpiarDatos
        public void ResetearDatos()
        {
            Clientes.Clear();
            Bancos.Clear();
            Facturas.Clear();
            Pagos.Clear();
        }
    }
}