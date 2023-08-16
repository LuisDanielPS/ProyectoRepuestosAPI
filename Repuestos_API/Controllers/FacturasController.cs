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
                        productosController.EditarProductoExistencia(item.producto_id, item.facturaD_cantidad, "factura");
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

        [HttpDelete]
        [Route("api/EliminarFacturaDetalle")]
        public string EliminarFacturaDetalle(int factura_id)
        {
            using (var bd = new ProyectoEntities())
            {
                try
                {
                    var factura = bd.Facturas.FirstOrDefault(p => p.factura_id == factura_id);
                    if (factura != null)
                    {
                        var detalles = (from x in bd.facturasDetalle
                                        where x.factura_id == factura_id
                                        orderby x.factura_id descending
                                        select x).ToList();

                        foreach (var detalle in detalles)
                        {
                            bd.facturasDetalle.Remove(detalle);
                            bd.SaveChanges();
                        }

                        return "Factura modificada con éxito";
                    }

                    return "La factura indicada no existe, por favor verifique";
                }
                catch (Exception ex)
                {
                    return "Error al ejecutar la consulta: " + ex;
                }
            }
        }

        [HttpGet]
        [Route("api/ConsultarFactura")]
        public FacturaEncabezadoEN ConsultarFactura(int q)
        {
            using (var bd = new ProyectoEntities())
            {
                try
                {
                    var encabezado = (from x in bd.Facturas
                                 where x.factura_id == q
                                 select x).FirstOrDefault();

                    if (encabezado != null)
                    {
                        FacturaEncabezadoEN res = new FacturaEncabezadoEN();
                        res.factura_id = encabezado.factura_id;
                        res.cliente_id = encabezado.cliente_id;
                        res.factura_tipo = encabezado.factura_tipo;
                        res.factura_fecha = encabezado.factura_fecha;
                        res.factura_descripcion = encabezado.factura_descripcion;
                        res.factura_total = encabezado.factura_total;
                        var detalle = (from x in bd.facturasDetalle
                                       where x.factura_id == q
                                       select new FacturaDetalleEN
                                       {
                                           facturaD_id = x.facturaD_id,
                                           factura_id = x.factura_id,
                                           producto_id = x.producto_id,
                                           facturaD_cantidad = x.facturaD_cantidad,
                                           facturaD_precio = x.facturaD_precio,
                                           facturaD_descuento = x.facturaD_descuento,
                                           facturaD_total = x.facturaD_total
                                       }).ToList();

                        res.factura_detalle = detalle;
                        return res;
                    }

                    return new FacturaEncabezadoEN();
                } catch (Exception ex)
                {
                    return null;
                }
            }
        }

        [HttpPut]
        [Route("api/EditarFactura")]
        public string EditarFactura([FromBody] FacturaEncabezadoEN entidad)
        {
            try
            {
                using (var bd = new ProyectoEntities())
                {
                    var datos = (from x in bd.Facturas
                                 where x.factura_id == entidad.factura_id
                                 select x).FirstOrDefault();

                    if (datos != null)
                    {
                        decimal totalFactura = 0.00M;
                        foreach (var item in entidad.factura_detalle)
                        {
                            ProductoEN producto = productosController.ConsultarProductoId(item.producto_id);
                            totalFactura = totalFactura + Math.Round(producto.producto_precio * item.facturaD_cantidad - item.facturaD_descuento, 2);
                        }

                        datos.cliente_id = entidad.cliente_id;
                        datos.factura_tipo = entidad.factura_tipo;
                        datos.factura_descripcion = entidad.factura_descripcion;
                        datos.factura_total = totalFactura;
                        bd.SaveChanges();

                        EliminarFacturaDetalle(entidad.factura_id);

                        foreach (var item in entidad.factura_detalle)
                        {
                            ProductoEN producto = productosController.ConsultarProductoId(item.producto_id);

                            facturasDetalle tabla2 = new facturasDetalle();
                            tabla2.factura_id = entidad.factura_id;
                            tabla2.producto_id = item.producto_id;
                            tabla2.facturaD_cantidad = item.facturaD_cantidad;
                            tabla2.facturaD_precio = Math.Round(producto.producto_precio, 2);
                            tabla2.facturaD_descuento = Math.Round(item.facturaD_descuento, 2);
                            decimal totalDetalle = Math.Round(producto.producto_precio * item.facturaD_cantidad - item.facturaD_descuento, 2);
                            tabla2.facturaD_total = totalDetalle;
                            bd.facturasDetalle.Add(tabla2);
                            bd.SaveChanges();
                            productosController.EditarProductoExistencia(item.producto_id, item.facturaD_cantidad, "factura");
                        }
                    }

                    return "Factura modificada con éxito";
                }
            }catch(Exception ex)
            {
                return ("Error" + ex);
            }
        }

    }
}
