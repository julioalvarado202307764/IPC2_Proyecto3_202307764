using System.Collections.Generic;
using System.Linq;
using Proyecto3_API.Models;
using System;

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
        public (int nuevas, int duplicadas, int conError) ProcesarFacturas(List<Factura> nuevasFacturas)
        {
            int nuevas = 0, duplicadas = 0, conError = 0;

            foreach (var nueva in nuevasFacturas)
            {
                // Validación de error: Si el Regex dejó el NIT o Fecha vacíos, es inválido
                if (string.IsNullOrEmpty(nueva.NITCliente) || string.IsNullOrEmpty(nueva.Fecha))
                {
                    conError++;
                    continue;
                }

                if (Facturas.Any(f => f.NumeroFactura == nueva.NumeroFactura))
                {
                    duplicadas++; // No hay actualizaciones, solo duplicados 
                }
                else
                {
                    Facturas.Add(nueva);
                    nuevas++;
                }
            }
            return (nuevas, duplicadas, conError);
        }

        public (int nuevos, int duplicados, int conError) ProcesarPagos(List<Pago> nuevosPagos)
        {
            int nuevos = 0, duplicados = 0, conError = 0;

            foreach (var nuevo in nuevosPagos)
            {
                if (string.IsNullOrEmpty(nuevo.NITCliente) || string.IsNullOrEmpty(nuevo.Fecha))
                {
                    conError++;
                    continue;
                }

                // Asumimos duplicado si todos los campos críticos son idénticos
                if (Pagos.Any(p => p.CodigoBanco == nuevo.CodigoBanco &&
                                   p.NITCliente == nuevo.NITCliente &&
                                   p.Fecha == nuevo.Fecha &&
                                   p.Valor == nuevo.Valor))
                {
                    duplicados++;
                }
                else
                {
                    Pagos.Add(nuevo);
                    nuevos++;
                }
            }
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

            return (creados, actualizados);
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