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
    public class ClientesController : ApiController
    {
        [HttpPost]
        [Route("api/RegistrarCliente")]
        public string RegistrarCliente(ClienteEN cliente)
        {
            using (var bd = new ProyectoEntities())
            {
                try
                {
                    
                    Clientes tabla = new Clientes();
                    tabla.cliente_cedula = cliente.cliente_cedula;
                    tabla.cliente_nombre = cliente.cliente_nombre;
                    tabla.cliente_apellido = cliente.cliente_apellido;
                    tabla.cliente_correo = cliente.cliente_correo;
                    tabla.cliente_telefono = cliente.cliente_telefono;
                    tabla.cliente_direccion = cliente.cliente_direccion;
                    bd.Clientes.Add(tabla);
                    bd.SaveChanges();

                    return "Cliente registrado con éxito";
                }
                catch (Exception ex)
                {
                    return "Error al ejecutar la consulta: " + ex;
                }
            }
        }


        [HttpGet]
        [Route("api/ConsultarClientes")]
        public List<ClienteEN> ConsultarClientes()
        {
            try
            {
                using (var bd = new ProyectoEntities())
                {
                    var datos = (from x in bd.Clientes
                                 select x).ToList();

                    if (datos.Count > 0)
                    {
                        List<ClienteEN> res = new List<ClienteEN>();
                        foreach (var item in datos)
                        {
                            res.Add(new ClienteEN
                            {
                                cliente_id = item.cliente_id,
                                cliente_cedula = item.cliente_cedula,
                                cliente_nombre = item.cliente_nombre,
                                cliente_apellido = item.cliente_apellido,
                                cliente_correo = item.cliente_correo,
                                cliente_telefono = item.cliente_telefono,
                                cliente_direccion = item.cliente_direccion
                            });
                        }

                        return res;
                    }

                    return new List<ClienteEN>();
                }
            }catch (Exception ex)
            {
                return new List<ClienteEN>();
            }
            }

        [HttpDelete]
        [Route("api/EliminaCliente")]
        public string EliminarCliente(int cliente_id)
        {
            using (var bd = new ProyectoEntities())
            {
                try
                {
                    var cliente = bd.Clientes.FirstOrDefault(p => p.cliente_id == cliente_id);
                    if (cliente != null)
                    {
                        bd.Clientes.Remove(cliente);
                        bd.SaveChanges();
                        return "Cliente eliminado con éxito";
                    }

                    return "El cliente indicado no existe, por favor verifique";
                }
                catch (Exception ex)
                {
                    return "Error al ejecutar la consulta: " + ex;
                }
            }
        }

        [HttpPut]
        [Route("api/EditarCliente")]
        public int EditarCliente(ClienteEN entidad)
        {
            try
            {
                using (var bd = new ProyectoEntities())
                {

                    var datos = (from x in bd.Clientes
                                 where x.cliente_id == entidad.cliente_id
                                 select x).FirstOrDefault();

                    if (datos != null)
                    {
                        datos.cliente_id = entidad.cliente_id;
                        datos.cliente_cedula = entidad.cliente_cedula;
                        datos.cliente_nombre = entidad.cliente_nombre;
                        datos.cliente_apellido = entidad.cliente_apellido;
                        datos.cliente_correo = entidad.cliente_correo;
                        datos.cliente_telefono = entidad.cliente_telefono;
                        datos.cliente_direccion = entidad.cliente_direccion;
                        return bd.SaveChanges();
                    }

                    return 0;
                }
            } catch (Exception ex) {
                return 0 ;
            }
        }
    }
}
