﻿using Repuestos_API.Entities;
using Repuestos_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Repuestos_API.Controllers
{
    public class RolesController : ApiController
    {
        [HttpPost]
        [Route("api/RegistrarRol")]
        public string RegistrarRol(RolEN rol)
        {
            using (var bd = new ProyectoEntities())
            {
                try
                {
                    Roles tabla = new Roles();
                    tabla.rol_descripcion = rol.rol_descripcion;
                    bd.Roles.Add(tabla);
                    bd.SaveChanges();

                    return "Rol registrado con éxito";
                }
                catch (Exception ex)
                {
                    return "Error al ejecutar la consulta: " + ex;
                }
            }
        }

        [HttpGet]
        [Route("api/ConsultarRoles")]
        public List<RolEN> ConsultarRoles()
        {
            using (var bd = new ProyectoEntities())
            {
                var datos = (from x in bd.Roles
                             select x).ToList();

                if (datos.Count > 0)
                {
                    List<RolEN> res = new List<RolEN>();
                    foreach (var item in datos)
                    {
                        res.Add(new RolEN
                        {
                            rol_id = item.rol_id,
                            rol_descripcion = item.rol_descripcion
                        });
                    }

                    return res;
                }

                return new List<RolEN>();
            }
        }

        [HttpPost]
        [Route("api/EliminarRol")]
        public string EliminarRol(int rol_id)
        {
            using (var bd = new ProyectoEntities())
            {
                try
                {
                    var rol = bd.Roles.FirstOrDefault(r => r.rol_id == rol_id);
                    if (rol != null)
                    {
                        bd.Roles.Remove(rol);
                        bd.SaveChanges();
                        return "Rol eliminado con éxito";
                    }

                    return "El rol indicado no existe, por favor verifique";
                }
                catch (Exception ex)
                {
                    return "Error al ejecutar la consulta: " + ex;
                }
            }
        }

        [HttpPost]
        [Route("api/EditarRol")]
        public string EditarRol(RolEN nuevoRol)
        {
            using (var bd = new ProyectoEntities())
            {
                try
                {
                    var rol = bd.Roles.FirstOrDefault(r => r.rol_id == nuevoRol.rol_id);
                    if (rol != null)
                    {
                        rol.rol_descripcion = nuevoRol.rol_descripcion;
                        bd.SaveChanges();
                        return "Rol modificado con éxito";
                    }

                    return "El rol indicado no existe, por favor verifique";
                }
                catch (Exception ex)
                {
                    return "Error al ejecutar la consulta: " + ex;
                }
            }
        }
    }
}
