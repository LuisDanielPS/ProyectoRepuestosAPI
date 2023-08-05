using Newtonsoft.Json;
using Repuestos_API.Entities;
using Repuestos_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Repuestos_API.Controllers
{
    public class FacturasController : ApiController
    {
        ProductosController productosController = new ProductosController();

        [HttpGet]
        [Route("api/ConsultarFacturas")]
        public List<FacturaEncabezadoEN> ConsultarFacturas()
        {
            try
            {
                using (var bd = new ProyectoEntities())
                {
                    var facturas = (from x in bd.Facturas
                                    orderby x.factura_id descending
                                     select x).ToList();

                    if (facturas.Count > 0)
                    {
                        List<FacturaEncabezadoEN> res = new List<FacturaEncabezadoEN>();
                        foreach (var dato in facturas)
                        {
                            res.Add(new FacturaEncabezadoEN
                            {
                                factura_id = dato.factura_id,
                                cliente_id = dato.cliente_id,
                                factura_tipo = dato.factura_tipo,
                                factura_fecha = dato.factura_fecha,
                                factura_descripcion = dato.factura_descripcion,
                                factura_total = dato.factura_total
                            });
                        }
                        return res;
                    }
                    return new List<FacturaEncabezadoEN>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al generar la consulta: " + ex.Message);
                return null;
            }
        }

        [HttpPost]
        [Route("api/RegistrarFactura")]
        public string RegistrarFactura([FromBody] FacturaEncabezadoEN factura)
        {

            using (var bd = new ProyectoEntities())
            {
                try
                {
                    DateTime fechaActual = DateTime.Now;
                    decimal totalFactura = 0.00M;
                    foreach (var item in factura.factura_detalle)
                    {
                        ProductoEN producto = productosController.ConsultarProductoId(item.producto_id);
                        totalFactura = totalFactura + Math.Round(producto.producto_precio * item.facturaD_cantidad - item.facturaD_descuento, 2);
                    }

                    Facturas tabla = new Facturas();
                    tabla.cliente_id = factura.cliente_id;
                    tabla.factura_tipo = factura.factura_tipo;
                    tabla.factura_fecha = fechaActual;
                    tabla.factura_descripcion = factura.factura_descripcion;
                    tabla.factura_total = totalFactura;
                    bd.Facturas.Add(tabla);
                    bd.SaveChanges();

                    List<FacturaEncabezadoEN> facturas = ConsultarFacturas();
                    int facturaID = facturas.FirstOrDefault().factura_id;

                    if (facturaID == null)
                    {
                        facturaID = 0;
                    }

                    foreach (var item in factura.factura_detalle)
                    {
                        ProductoEN producto = productosController.ConsultarProductoId(item.producto_id);

                        if (producto == null)
                        {
                            return "No existe un producto con el id indicado, por favor verifique";
                        }

                        facturasDetalle tabla2 = new facturasDetalle();
                        tabla2.factura_id = facturaID;
                        tabla2.producto_id = item.producto_id;
                        tabla2.facturaD_cantidad = item.facturaD_cantidad;
                        tabla2.facturaD_precio = Math.Round(producto.producto_precio, 2);
                        tabla2.facturaD_descuento = Math.Round(item.facturaD_descuento, 2);
                        decimal totalDetalle = Math.Round(producto.producto_precio * item.facturaD_cantidad - item.facturaD_descuento, 2);
                        tabla2.facturaD_total = totalDetalle;
                        bd.facturasDetalle.Add(tabla2);
                        bd.SaveChanges();
                    }

                    return "Factura registrada con éxito";
                }
                catch (Exception ex)
                {
                    return "Error al ejecutar la consulta: " + ex;
                }

            }
        }

        [HttpDelete]
        [Route("api/EliminarFactura")]
        public string EliminarFactura(int factura_id)
        {
            using (var bd = new ProyectoEntities())
            {
                try
                {
                    var factura = bd.Facturas.FirstOrDefault(p => p.factura_id == factura_id);
                    if (factura != null)
                    {
                        var detalles = bd.facturasDetalle.Where(d => d.factura_id == factura_id);
                        foreach (var detalle in detalles)
                        {
                            bd.facturasDetalle.Remove(detalle);
                        }
                        bd.Facturas.Remove(factura);
                        bd.SaveChanges();
                        return "Factura eliminada con éxito";
                    }

                    return "La factura indicada no existe, por favor verifique";
                }
                catch (Exception ex)
                {
                    return "Error al ejecutar la consulta: " + ex;
                }
            }
        }
    }
}
