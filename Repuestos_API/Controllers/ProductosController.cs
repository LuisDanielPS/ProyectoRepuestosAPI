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
    public class ProductosController : ApiController
    {
        [HttpPost]
        [Route("api/RegistrarProducto")]
        public string RegistrarProducto(ProductoEN producto)
        {
            using (var bd = new ProyectoEntities())
            {
                try
                {
                    if (producto.categoria_id == 0 || producto.estado_id == 0)
                    {
                        return "Por favor, indique la categoría y el estado del producto";
                    }

                    Productos tabla = new Productos();
                    tabla.categoria_id = producto.categoria_id;
                    tabla.estado_id = producto.estado_id;
                    tabla.producto_descripcion = producto.producto_descripcion;
                    tabla.producto_existencias = producto.producto_existencias;
                    tabla.producto_precio = producto.producto_precio;
                    bd.Productos.Add(tabla);
                    bd.SaveChanges();

                    return "Producto registrado con éxito";
                }
                catch (Exception ex)
                {
                    return "Error al ejecutar la consulta: " + ex;
                }
            }
        }

        [HttpGet]
        [Route("api/ConsultarProductos")]
        public List<ProductoEN> ConsultarProductos()
        {
            using (var bd = new ProyectoEntities())
            {
                var datos = (from x in bd.Productos
                             join y in bd.Categorias on x.categoria_id equals y.categoria_id
                             join u in bd.Estados on x.estado_id equals u.estado_id
                             select new
                             {
                                 x.producto_id,
                                 x.categoria_id,
                                 x.estado_id,
                                 x.producto_descripcion,
                                 x.producto_existencias,
                                 x.producto_precio,
                                 y.categoria_descripcion ,
                                 u.estado_descripcion
                             }).ToList();

                if (datos.Count > 0)
                {
                    List<ProductoEN> res = new List<ProductoEN>();
                    foreach (var item in datos)
                    {
                        res.Add(new ProductoEN
                        {
                            producto_id = item.producto_id,
                            categoria_id = item.categoria_id,
                            categoria_descripcion = item.categoria_descripcion,
                            estado_id = item.estado_id,
                            producto_descripcion = item.producto_descripcion,
                            producto_existencias = item.producto_existencias,
                            producto_precio = item.producto_precio,
                            estado_descripcion = item.estado_descripcion                 
                        });
                    }

                    return res;
                }

                return new List<ProductoEN>();
            }
        }


        [HttpPost]
        [Route("api/EliminarProducto")]
        public string EliminarProducto(int producto_id)
        {
            using (var bd = new ProyectoEntities())
            {
                try
                {
                    var producto = bd.Productos.FirstOrDefault(p => p.producto_id == producto_id);
                    if (producto != null)
                    {
                        bd.Productos.Remove(producto);
                        bd.SaveChanges();
                        return "Producto eliminado con éxito";
                    }

                    return "El producto indicado no existe, por favor verifique";
                }
                catch (Exception ex)
                {
                    return "Error al ejecutar la consulta: " + ex;
                }
            }
        }

        [HttpPost]
        [Route("api/EditarProducto")]
        public string EditarProducto(ProductoEN nuevoProducto)
        {
            using (var bd = new ProyectoEntities())
            {
                try
                {
                    var producto = bd.Productos.FirstOrDefault(p => p.producto_id == nuevoProducto.producto_id);
                    if (producto != null)
                    {
                        producto.categoria_id = nuevoProducto.categoria_id;
                        producto.estado_id = nuevoProducto.estado_id;
                        producto.producto_descripcion = nuevoProducto.producto_descripcion;
                        producto.producto_existencias = nuevoProducto.producto_existencias;
                        producto.producto_precio = nuevoProducto.producto_precio;
                        bd.SaveChanges();
                        return "Producto modificado con éxito";
                    }

                    return "El producto indicado no existe, por favor verifique";
                }
                catch (Exception ex)
                {
                    return "Error al ejecutar la consulta: " + ex;
                }
            }
        }
    }
}
