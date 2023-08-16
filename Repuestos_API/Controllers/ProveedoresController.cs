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
    public class ProveedoresController : ApiController
    {
        [HttpPost]
        [Route("api/RegistrarProveedor")]
        public string RegistrarProveedor(ProveedorEN proveedor)
        {
            using (var bd = new ProyectoEntities())
            {
                try
                {
                    Proveedores tabla = new Proveedores();
                    tabla.proveedor_cedula = proveedor.proveedor_cedula;
                    tabla.proveedor_nombre = proveedor.proveedor_nombre;
                    tabla.proveedor_apellido = proveedor.proveedor_apellido;
                    tabla.proveedor_correo = proveedor.proveedor_correo;
                    tabla.proveedor_telefono = proveedor.proveedor_telefono;
                    tabla.proveedor_direccion = proveedor.proveedor_direccion;
                    bd.Proveedores.Add(tabla);
                    bd.SaveChanges();

                    return "Proveedor registrado con éxito";
                }
                catch (Exception ex)
                {
                    return "Error al ejecutar la consulta: " + ex;
                }
            }
        }

        [HttpGet]
        [Route("api/ConsultarProveedores")]
        public List<ProveedorEN> ConsultarProveedores()
        {
            try
            {
                using (var bd = new ProyectoEntities())
                {
                    var datos = (from x in bd.Proveedores
                                 select x).ToList();

                    if (datos.Count > 0)
                    {
                        List<ProveedorEN> res = new List<ProveedorEN>();
                        foreach (var item in datos)
                        {
                            res.Add(new ProveedorEN
                            {
                                proveedor_id = item.proveedor_id,
                                proveedor_cedula = item.proveedor_cedula,
                                proveedor_nombre = item.proveedor_nombre,
                                proveedor_apellido = item.proveedor_apellido,
                                proveedor_correo = item.proveedor_correo,
                                proveedor_telefono = item.proveedor_telefono,
                                proveedor_direccion = item.proveedor_direccion
                            });
                        }

                        return res;
                    }

                    return new List<ProveedorEN>();
                }
            }catch(Exception ex)
            {
                return new List<ProveedorEN>();
            }
        }

        [HttpDelete]
        [Route("api/EliminarProveedor")]
        public string EliminarProveedor(int proveedor_id)
        {
            using (var bd = new ProyectoEntities())
            {
                try
                {
                    var proveedor = bd.Proveedores.FirstOrDefault(p => p.proveedor_id == proveedor_id);
                    if (proveedor != null)
                    {
                        bd.Proveedores.Remove(proveedor);
                        bd.SaveChanges();
                        return "Proveedor eliminado con éxito";
                    }

                    return "El proveedor indicado no existe, por favor verifique";
                }
                catch (Exception ex)
                {
                    return "Error al ejecutar la consulta: " + ex;
                }
            }
        }

        [HttpPut]
        [Route("api/EditaProveedor")]
        public int EditarProveedor(ProveedorEN entidad)
        {
            try
            {
                using (var bd = new ProyectoEntities())
                {
                    var datos = (from x in bd.Proveedores
                                 where x.proveedor_id == entidad.proveedor_id
                                 select x).FirstOrDefault();

                    if (datos != null)
                    {
                        datos.proveedor_id = entidad.proveedor_id;
                        datos.proveedor_nombre = entidad.proveedor_nombre;
                        datos.proveedor_apellido = entidad.proveedor_apellido;
                        datos.proveedor_cedula = entidad.proveedor_cedula;
                        datos.proveedor_correo = entidad.proveedor_correo;
                        datos.proveedor_direccion = entidad.proveedor_direccion;
                        datos.proveedor_telefono = entidad.proveedor_telefono;
                        return bd.SaveChanges();
                    }

                    return 0;
                }
            }catch(Exception ex)
            {
                return 0 ;
            }
        }

    }
}
