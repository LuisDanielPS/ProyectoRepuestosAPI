using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Repuestos_API.Entities
{
    public class ProveedorEN
    {
        public int proveedor_id { get; set; }
        public string proveedor_cedula { get; set; }
        public string proveedor_nombre { get; set; }
        public string proveedor_apellido { get; set; }
        public string proveedor_correo { get; set; }
        public string proveedor_telefono { get; set; }
        public string proveedor_direccion { get; set; }
    }
}