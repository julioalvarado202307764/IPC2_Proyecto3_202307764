using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Proyecto3_API.Models;

namespace Proyecto3_API.Services
{
    public class XmlProcessorService
    {
        // Expresiones Regulares
        // Captura secuencias de números, letras y guiones (ignora espacios o caracteres raros extra)
        private readonly Regex _regexNit = new Regex(@"[A-Za-z0-9\-]+");

        // Captura estrictamente el formato dd/mm/yyyy, ignorando texto alrededor (como nombres de lugares)
        private readonly Regex _regexFecha = new Regex(@"\d{2}/\d{2}/\d{4}");

        public List<Cliente> ExtraerClientes(string xmlContent)
        {
            var clientes = new List<Cliente>();
            XDocument doc = XDocument.Parse(xmlContent);

            foreach (var nodo in doc.Descendants("cliente"))
            {
                string nitCrudo = nodo.Element("NIT")?.Value ?? string.Empty;
                string nombre = nodo.Element("nombre")?.Value ?? string.Empty;

                //Aplicamos la expresión regular solo al NIT
                string nitLimpio = _regexNit.Match(nitCrudo).Value;

                if (!string.IsNullOrEmpty(nitLimpio))
                {
                    clientes.Add(new Cliente
                    {
                        NIT = nitLimpio,
                        Nombre = nombre.Trim()
                    });
                }
            }

            return clientes;
        }

        public List<Factura> ExtraerFacturas(string xmlContent)
        {
            var facturas = new List<Factura>();
            XDocument doc = XDocument.Parse(xmlContent);

            foreach (var nodo in doc.Descendants("factura"))
            {
                string numCrudo = nodo.Element("numeroFactura")?.Value ?? string.Empty;
                string nitCrudo = nodo.Element("NITcliente")?.Value ?? string.Empty;
                string fechaCruda = nodo.Element("fecha")?.Value ?? string.Empty;
                string valorCrudo = nodo.Element("valor")?.Value ?? "0";

                //Aplicamos Regex al NIT y a la Fecha
                string nitLimpio = _regexNit.Match(nitCrudo).Value;
                string fechaLimpia = _regexFecha.Match(fechaCruda).Value;

                // Convertimos el valor a decimal
                decimal.TryParse(valorCrudo, out decimal valorLimpio);

                facturas.Add(new Factura
                {
                    NumeroFactura = numCrudo.Trim(),
                    NITCliente = nitLimpio,
                    Fecha = fechaLimpia,
                    Valor = valorLimpio
                });
            }

            return facturas;
        }

        public List<Banco> ExtraerBancos(string xmlContent)
        {
            var bancos = new List<Banco>();
            XDocument doc = XDocument.Parse(xmlContent);

            foreach (var nodo in doc.Descendants("banco"))
            {
                string codigoCrudo = nodo.Element("codigo")?.Value ?? "0";
                string nombre = nodo.Element("nombre")?.Value ?? string.Empty;

                if (int.TryParse(codigoCrudo, out int codigo))
                {
                    bancos.Add(new Banco
                    {
                        Codigo = codigo,
                        Nombre = nombre.Trim()
                    });
                }
            }
            return bancos;
        }

        public List<Pago> ExtraerPagos(string xmlContent)
{
    var pagos = new List<Pago>();
    XDocument doc = XDocument.Parse(xmlContent);

    foreach (var nodo in doc.Descendants("pago"))
    {
        string codigoCrudo = nodo.Element("codigoBanco")?.Value ?? "0";
        string fechaCruda = nodo.Element("fecha")?.Value ?? string.Empty;
        string nitCrudo = nodo.Element("NITcliente")?.Value ?? string.Empty;
        string valorCrudo = nodo.Element("valor")?.Value ?? "0";

        //Aplicamos Regex al NIT y a la Fecha
        string nitLimpio = _regexNit.Match(nitCrudo).Value;
        string fechaLimpia = _regexFecha.Match(fechaCruda).Value;
        
        int.TryParse(codigoCrudo, out int codigoBanco);
        decimal.TryParse(valorCrudo, out decimal valorLimpio);

        pagos.Add(new Pago
        {
            CodigoBanco = codigoBanco,
            Fecha = fechaLimpia,
            NITCliente = nitLimpio,
            Valor = valorLimpio
        });
    }

    return pagos;
}
    }
}