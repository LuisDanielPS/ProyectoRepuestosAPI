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
    public class CategoriasController : ApiController
    {
        [HttpPost]
        [Route("api/RegistrarCategoria")]
        public string RegistrarCategoria(CategoriaEN categoria)
        {
            using (var bd = new ProyectoEntities())
            {
                try 
                {
                    Categorias tabla = new Categorias();
                    tabla.categoria_descripcion = categoria.categoria_descripcion;
                    bd.Categorias.Add(tabla);
                    bd.SaveChanges();

                    return "Categoría registrada con éxito";
                }
                catch (Exception ex)
                {
                    return "Error al ejecutar la consulta: " + ex;
                }
            }
        }

        [HttpGet]
        [Route("api/ConsultarCategorias")]
        public List<CategoriaEN> ConsultarCategorias()
        {
            try
            {
                using (var bd = new ProyectoEntities())
                {
                    var datos = (from x in bd.Categorias
                                 select x).ToList();

                    if (datos.Count > 0)
                    {
                        List<CategoriaEN> res = new List<CategoriaEN>();
                        foreach (var item in datos)
                        {
                            res.Add(new CategoriaEN
                            {
                                categoria_id = item.categoria_id,
                                categoria_descripcion = item.categoria_descripcion
                            });
                        }

                        return res;
                    }

                    return new List<CategoriaEN>();
                }
            }
            catch (Exception ex)
            {
                return new List<CategoriaEN>() ;
            }
        }
        [HttpPost]
        [Route("api/EliminarCategoria")]
        public string EliminarCategoria(int categoria_id)
        {
            using (var bd = new ProyectoEntities())
            {
                try
                {
                    var categoria = bd.Categorias.FirstOrDefault(c => c.categoria_id == categoria_id);
                    if (categoria != null)
                    {
                        bd.Categorias.Remove(categoria);
                        bd.SaveChanges();
                        return "Categoría eliminada con éxito";
                    }

                    return "La categoría indicada no existe, por favor verifique";
                }
                catch (Exception ex)
                {
                    return "Error al ejecutar la consulta: " + ex;
                }
            }
        }

        [HttpPost]
        [Route("api/EditarCategoria")]
        public string EditarCategoria(CategoriaEN nuevaCategoria)
        {
            using (var bd = new ProyectoEntities())
            {
                try
                {
                    var categoria = bd.Categorias.FirstOrDefault(c => c.categoria_id == nuevaCategoria.categoria_id);
                    if (categoria != null)
                    {
                        categoria.categoria_descripcion = nuevaCategoria.categoria_descripcion;
                        bd.SaveChanges();
                        return "Categoría modificada con éxito";
                    }

                    return "La categoría indicada no existe, por favor verifique";
                }
                catch (Exception ex)
                {
                    return "Error al ejecutar la consulta: " + ex;
                }
            }
        }
    }
}
