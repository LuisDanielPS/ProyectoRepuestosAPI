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
    public class EstadosController : ApiController
    {
        [HttpPost]
        [Route("api/RegistrarEstado")]
        public string RegistrarEstado(EstadoEN estado)
        {
            using (var bd = new ProyectoEntities())
            {
                try
                {
                    Estados tabla = new Estados();
                    tabla.estado_descripcion = estado.estado_descripcion;
                    bd.Estados.Add(tabla);
                    bd.SaveChanges();

                    return "Estado registrado con éxito";
                }
                catch (Exception ex)
                {
                    return "Error al ejecutar la consulta: " + ex;
                }
            }
        }

        [HttpGet]
        [Route("api/ConsultarEstados")]
        public List<EstadoEN> ConsultarEstados()
        {
            try
            {
                using (var bd = new ProyectoEntities())
                {
                    var datos = (from x in bd.Estados
                                 select x).ToList();

                    if (datos.Count > 0)
                    {
                        List<EstadoEN> res = new List<EstadoEN>();
                        foreach (var item in datos)
                        {
                            res.Add(new EstadoEN
                            {
                                estado_id = item.estado_id,
                                estado_descripcion = item.estado_descripcion
                            });
                        }

                        return res;
                    }

                    return new List<EstadoEN>();
                }
            }catch (Exception ex)
            {
                return new List<EstadoEN>();

            }
        }

        [HttpPost]
        [Route("api/EliminarEstado")]
        public string EliminarEstado(int estado_id)
        {
            using (var bd = new ProyectoEntities())
            {
                try
                {
                    var estado = bd.Estados.FirstOrDefault(e => e.estado_id == estado_id);
                    if (estado != null)
                    {
                        bd.Estados.Remove(estado);
                        bd.SaveChanges();
                        return "Estado eliminado con éxito";
                    }

                    return "El estado indicado no existe, por favor verifique";
                }
                catch (Exception ex)
                {
                    return "Error al ejecutar la consulta: " + ex;
                }
            }
        }

        [HttpPost]
        [Route("api/EditarEstado")]
        public string EditarEstado(EstadoEN nuevoEstado)
        {
            using (var bd = new ProyectoEntities())
            {
                try
                {
                    var estado = bd.Estados.FirstOrDefault(e => e.estado_id == nuevoEstado.estado_id);
                    if (estado != null)
                    {
                        estado.estado_descripcion = nuevoEstado.estado_descripcion;
                        bd.SaveChanges();
                        return "Estado modificado con éxito";
                    }

                    return "El estado indicado no existe, por favor verifique";
                }
                catch (Exception ex)
                {
                    return "Error al ejecutar la consulta: " + ex;
                }
            }
        }
    }
}
