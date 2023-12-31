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
    public class UsuariosController : ApiController
    {
        UtilitariosModel utilModel = new UtilitariosModel();


        [HttpPost]
        [Route("api/IniciarSesion")]
        public UsuarioEN IniciarSesion(UsuarioEN entidad)
        {
            try
            {
                using (var bd = new ProyectoEntities())
                {
                    var datos = (from x in bd.Usuarios
                                 join y in bd.Roles on x.rol_id equals y.rol_id
                                 where x.usu_correo == entidad.usu_correo
                                    && x.usu_clave == entidad.usu_clave
                                 select new
                                 {
                                     x.usu_claveTemporal,
                                     x.usu_caducidad,
                                     x.usu_correo,
                                     x.usu_identificacion,
                                     x.usu_nombre,
                                     y.rol_id,
                                     x.usuario_id,
                                     y.rol_descripcion
                                 }).FirstOrDefault();

                    if (datos != null)
                    {
                        if (datos.usu_caducidad < DateTime.Now)
                        {
                            return null;
                        }

                        UsuarioEN res = new UsuarioEN();
                        res.usu_correo = datos.usu_correo;
                        res.usu_identificacion = datos.usu_identificacion;
                        res.usu_nombre = datos.usu_nombre;
                        res.rol_id = datos.rol_id;
                        res.usuario_id = datos.usuario_id;
                        res.rol_descripcion = datos.rol_descripcion;
                        return res;
                    }

                    return null;
                }
            }catch(Exception ex)
            {
                return new UsuarioEN();
            }
        }
        [HttpPost]
        [Route("api/RegistrarUsuario")]
        public string RegistrarUsuario(UsuarioEN usuario)
        {
            using (var bd = new ProyectoEntities())
            {
                try
                {
                    

                    Usuarios tabla = new Usuarios();
                    tabla.usu_nombre = usuario.usu_nombre;
                    tabla.usu_correo = usuario.usu_correo;
                    tabla.usu_identificacion = usuario.usu_identificacion;
                    tabla.rol_id = usuario.rol_id;
                    tabla.usu_caducidad = DateTime.Now.AddYears(1);
                    bd.Usuarios.Add(tabla);
                    bd.SaveChanges();

                    return "Usuario registrado con éxito";
                }
                catch (Exception ex)
                {
                    return "Error al ejecutar la consulta: " + ex;
                }
            }
        }



        [HttpGet]
        [Route("api/ConsultarUsuarios")]
        public List<UsuarioEN> ConsultarUsuarios()
        {
            try
            {
                using (var bd = new ProyectoEntities())
                {
                    var datos = (from x in bd.Usuarios
                                 join y in bd.Roles on x.rol_id equals y.rol_id
                                 select new
                                 {
                                     x.rol_id,
                                     x.usu_identificacion,
                                     x.usu_nombre,
                                     x.usu_correo,
                                     x.usuario_id,
                                     y.rol_descripcion,
                                 }).ToList();

                    if (datos.Count > 0)
                    {
                        List<UsuarioEN> res = new List<UsuarioEN>();
                        foreach (var item in datos)
                        {
                            res.Add(new UsuarioEN
                            {
                                usuario_id = item.usuario_id,
                                usu_correo = item.usu_correo,
                                usu_nombre = item.usu_nombre,
                                usu_identificacion = item.usu_identificacion,
                                rol_descripcion = item.rol_descripcion

                            });
                        }

                        return res;
                    }

                    return new List<UsuarioEN>();
                }
            }catch(Exception ex)
            {
                return new List<UsuarioEN>();

            }
        }

        [HttpPut]
        [Route("api/EditarUsuario")]
        public int EditarUsuario(UsuarioEN entidad)
        {
            try
            {
                using (var bd = new ProyectoEntities())
                {
                    var datos = (from x in bd.Usuarios
                                 where x.usuario_id == entidad.usuario_id
                                 select x).FirstOrDefault();

                    if (datos != null)
                    {
                        datos.usu_nombre = entidad.usu_nombre;
                        datos.usu_identificacion = entidad.usu_identificacion;
                        datos.usu_correo = entidad.usu_correo;
                        datos.rol_id = entidad.rol_id;
                        return bd.SaveChanges();
                    }

                    return 0;
                }
            }catch(Exception ex)
            {
                return 0;
            }
        }

        [HttpDelete]
        [Route("api/EliminarUsuario")]
        public string EliminarUsuario(int usuario_id)
        {
            using (var bd = new ProyectoEntities())
            {
                try
                {
                    var usuario = bd.Usuarios.FirstOrDefault(p => p.usuario_id == usuario_id);
                    if (usuario != null)
                    {
                        bd.Usuarios.Remove(usuario);
                        bd.SaveChanges();
                        return "Usuario eliminado con éxito";
                    }

                    return "El usuario indicado no existe, por favor verifique";
                }
                catch (Exception ex)
                {
                    return "Error al ejecutar la consulta: " + ex;
                }
            }
        }

        [HttpPost]
        [Route("api/RecuperarContrasenia")]
        public bool RecuperarContrasenia(UsuarioEN entidad)
        {
            try
            {
                using (var bd = new ProyectoEntities())
                {
                    var datos = (from x in bd.Usuarios
                                 where x.usu_correo == entidad.usu_correo
                                 select x).FirstOrDefault();

                    if (datos != null)
                    {
                        string claveTemp = utilModel.GenerarContraseniaTemp();
                        datos.usu_clave = utilModel.Encriptar(claveTemp);
                        datos.usu_claveTemporal = true;
                        datos.usu_caducidad = DateTime.Now.AddMinutes(60);
                        bd.SaveChanges();

                        string mensaje = "Estimado(a) " + datos.usu_nombre + ". Se ha generado la siguiente contraseña temporal: " + claveTemp;
                        utilModel.EnviarCorreo(datos.usu_correo, "Recuperación de contraseña", mensaje);
                        return true;
                    }

                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        [HttpPut]
        [Route("api/CambiarContrasenna")]
        public int CambiarContrasenna(UsuarioEN entidad)
        {
            try
            {
                using (var bd = new ProyectoEntities())
                {
                    var datos = (from x in bd.Usuarios
                                 where x.usuario_id == entidad.usuario_id
                                 select x).FirstOrDefault();

                    if (datos != null)
                    {
                        datos.usu_clave = entidad.ContrasennaNueva;
                        datos.usu_claveTemporal = false;
                        datos.usu_caducidad = DateTime.Now.AddYears(1);
                        return bd.SaveChanges();
                    }

                    return 0;
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

    }
}
