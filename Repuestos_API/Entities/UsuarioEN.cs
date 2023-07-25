using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Repuestos_API.Entities
{
    public class UsuarioEN
    {
        public long usuario_id { get; set; }
        public string usu_correo { get; set; }
        public string usu_clave { get; set; }
        public string usu_identificacion { get; set; }
        public string usu_nombre { get; set; }
        public int rol_id { get; set; }
        public string rol_descripcion { get; set; }
        public string ConfirmarContrasenna { get; set; }
        public string ContrasennaNueva { get; set; }
    }
}