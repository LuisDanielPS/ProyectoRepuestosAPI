//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Repuestos_API.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Usuarios
    {
        public long usuario_id { get; set; }
        public string usu_correo { get; set; }
        public string usu_clave { get; set; }
        public string usu_nombre { get; set; }
        public int rol_id { get; set; }
        public string usu_identificacion { get; set; }
        public Nullable<bool> usu_claveTemporal { get; set; }
        public Nullable<System.DateTime> usu_caducidad { get; set; }
    
        public virtual Roles Roles { get; set; }
    }
}
