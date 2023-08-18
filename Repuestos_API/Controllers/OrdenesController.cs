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
    public class OrdenesController : ApiController
    {
        ProductosController productosController = new ProductosController();

        [HttpGet]
        [Route("api/ConsultarOrdenes")]
        public List<OrdenEncabezadoEN> ConsultarOrdenes()
        {
            try
            {
                using (var bd = new ProyectoEntities())
                {
                    var ordens = (from x in bd.ordenesCompra
                                    orderby x.orden_id descending
                                    select x).ToList();

                    if (ordens.Count > 0)
                    {
                        List<OrdenEncabezadoEN> res = new List<OrdenEncabezadoEN>();
                        foreach (var dato in ordens)
                        {
                            res.Add(new OrdenEncabezadoEN
                            {
                                orden_id = dato.orden_id,
                                proveedor_id = dato.proveedor_id,
                                estado_id = dato.estado_id,
                                orden_tipo = dato.orden_tipo,
                                orden_fecha = dato.orden_fecha,
                                orden_facturarA = dato.orden_facturarA,
                                orden_entregarA = dato.orden_entregarA,
                                orden_descripcion = dato.orden_descripcion,
                            });
                        }
                        return res;
                    }
                    return new List<OrdenEncabezadoEN>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al generar la consulta: " + ex.Message);
                return null;
            }
        }

        [HttpPost]
        [Route("api/RegistrarOrden")]
        public string RegistrarOrden([FromBody] OrdenEncabezadoEN orden)
        {

            using (var bd = new ProyectoEntities())
            {
                try
                {
                    DateTime fechaActual = DateTime.Now;

                    ordenesCompra tabla = new ordenesCompra();
                    tabla.proveedor_id = orden.proveedor_id;
                    tabla.estado_id = orden.estado_id;
                    tabla.orden_tipo = orden.orden_tipo;
                    tabla.orden_fecha = fechaActual;
                    tabla.orden_facturarA = orden.orden_facturarA;
                    tabla.orden_entregarA = orden.orden_entregarA;
                    tabla.orden_descripcion = orden.orden_descripcion;
                    bd.ordenesCompra.Add(tabla);
                    bd.SaveChanges();

                    List<OrdenEncabezadoEN> ordenes = ConsultarOrdenes();
                    int ordenID = ordenes.FirstOrDefault().orden_id;

                    if (ordenID == null)
                    {
                        ordenID = 0;
                    }

                    foreach (var item in orden.orden_detalle)
                    {
                        ProductoEN producto = productosController.ConsultarProductoId(item.producto_id);

                        if (producto == null)
                        {
                            return "No existe un producto con el id indicado, por favor verifique";
                        }

                        ordenDetalle tabla2 = new ordenDetalle();
                        tabla2.orden_id = ordenID;
                        tabla2.producto_id = item.producto_id;
                        tabla2.producto_cantidad = item.producto_cantidad;
                        bd.ordenDetalle.Add(tabla2);
                        bd.SaveChanges();
                        if (orden.estado_id == 3)
                        {
                            productosController.EditarProductoExistencia(item.producto_id, item.producto_cantidad, "orden");
                        }
                    }

                    return "Orden de compra registrada con éxito";
                }
                catch (Exception ex)
                {
                    return "Error al ejecutar la consulta: " + ex;
                }

            }
        }

        [HttpDelete]
        [Route("api/EliminarOrden")]
        public string EliminarOrden(int orden_id)
        {
            using (var bd = new ProyectoEntities())
            {
                try
                {
                    var orden = bd.ordenesCompra.FirstOrDefault(p => p.orden_id == orden_id);
                    if (orden != null)
                    {
                        var detalles = bd.ordenDetalle.Where(d => d.orden_id == orden_id);
                        foreach (var detalle in detalles)
                        {
                            bd.ordenDetalle.Remove(detalle);
                        }
                        bd.ordenesCompra.Remove(orden);
                        bd.SaveChanges();
                        return "Orden de compra eliminada con éxito";
                    }

                    return "La orden de compra indicada no existe, por favor verifique";
                }
                catch (Exception ex)
                {
                    return "Error al ejecutar la consulta: " + ex;
                }
            }
        }

        [HttpDelete]
        [Route("api/EliminarOrdenDetalle")]
        public string EliminarOrdenDetalle(int orden_id)
        {
            using (var bd = new ProyectoEntities())
            {
                try
                {
                    var orden = bd.ordenesCompra.FirstOrDefault(p => p.orden_id == orden_id);
                    if (orden != null)
                    {
                        var detalles = (from x in bd.ordenDetalle
                                        where x.orden_id == orden_id
                                        orderby x.ordenD_id descending
                                        select x).ToList();

                        foreach (var detalle in detalles)
                        {
                            bd.ordenDetalle.Remove(detalle);
                            bd.SaveChanges();
                        }

                        return "Línea eliminada con éxito";
                    }

                    return "La orden de compra indicada no existe, por favor verifique";
                }
                catch (Exception ex)
                {
                    return "Error al ejecutar la consulta: " + ex;
                }
            }
        }

        [HttpGet]
        [Route("api/ConsultarOrden")]
        public OrdenEncabezadoEN ConsultarOrden(int q)
        {
            using (var bd = new ProyectoEntities())
            {
                try
                {
                    var encabezado = (from x in bd.ordenesCompra
                                      where x.orden_id == q
                                      select x).FirstOrDefault();

                    if (encabezado != null)
                    {
                        OrdenEncabezadoEN res = new OrdenEncabezadoEN();
                        res.orden_id = encabezado.orden_id;
                        res.proveedor_id = encabezado.proveedor_id;
                        res.estado_id = encabezado.estado_id;
                        res.orden_tipo = encabezado.orden_tipo;
                        res.orden_fecha = encabezado.orden_fecha;
                        res.orden_facturarA = encabezado.orden_facturarA;
                        res.orden_entregarA = encabezado.orden_entregarA;
                        res.orden_descripcion = encabezado.orden_descripcion;
                        var detalle = (from x in bd.ordenDetalle
                                       where x.orden_id == q
                                       select new OrdenDetalleEN
                                       {
                                           ordenD_id = x.ordenD_id,
                                           orden_id = x.orden_id,
                                           producto_id = x.producto_id,
                                           producto_cantidad = x.producto_cantidad
                                       }).ToList();

                        res.orden_detalle = detalle;
                        return res;
                    }

                    return new OrdenEncabezadoEN();
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        [HttpPut]
        [Route("api/EditarOrden")]
        public string EditarOrden([FromBody] OrdenEncabezadoEN entidad)
        {
            try
            {
                using (var bd = new ProyectoEntities())
                {
                    var datos = (from x in bd.ordenesCompra
                                 where x.orden_id == entidad.orden_id
                                 select x).FirstOrDefault();

                    if (datos != null)
                    {

                        datos.proveedor_id = entidad.proveedor_id;
                        datos.estado_id = entidad.estado_id;
                        datos.orden_tipo = entidad.orden_tipo;
                        datos.orden_facturarA = entidad.orden_facturarA;
                        datos.orden_entregarA = entidad.orden_entregarA;
                        datos.orden_descripcion = entidad.orden_descripcion;
                        bd.SaveChanges();

                        EliminarOrdenDetalle(entidad.orden_id);

                        foreach (var item in entidad.orden_detalle)
                        {
                            ProductoEN producto = productosController.ConsultarProductoId(item.producto_id);

                            ordenDetalle tabla2 = new ordenDetalle();
                            tabla2.orden_id = entidad.orden_id;
                            tabla2.producto_id = item.producto_id;
                            tabla2.producto_cantidad = item.producto_cantidad;
                            bd.ordenDetalle.Add(tabla2);
                            bd.SaveChanges();
                            if (entidad.estado_id == 3)
                            {
                                productosController.EditarProductoExistencia(item.producto_id, item.producto_cantidad, "orden");
                            }
                        }
                    }

                    return "Orden de compra modificada con éxito";
                }
            }
            catch (Exception ex)
            {
                return ("Error" + ex);
            }
        }

    }
}
