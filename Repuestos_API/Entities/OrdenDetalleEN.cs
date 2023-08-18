using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Repuestos_API.Entities
{
    public class OrdenDetalleEN
    {
        public int ordenD_id { get; set; }
        public int orden_id { get; set; }
        public int producto_id { get; set; }
        public int producto_cantidad { get; set; }
    }
}