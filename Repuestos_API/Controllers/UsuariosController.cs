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
    public class UsuariosController : ApiController
    {
        UtilitariosModel utilModel = new UtilitariosModel();


        [HttpPost]
        [Route("api/IniciarSesion")]
        public UsuarioEN IniciarSesion(UsuarioEN entidad)
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
                    tabla.usu_correo = usuario.usu_correo;
                    tabla.usu_clave = usuario.usu_clave;
                    tabla.usu_identificacion = usuario.usu_identificacion;
                    tabla.usu_nombre = usuario.usu_nombre;
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


        [HttpPost]
        [Route("api/RecuperarContrasenia")]
        public bool RecuperarContrasenia(UsuarioEN entidad)
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
    }
}
