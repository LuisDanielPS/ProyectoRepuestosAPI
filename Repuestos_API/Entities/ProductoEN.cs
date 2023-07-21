using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Repuestos_API.Entities
{
    public class ProductoEN
    {
        public int producto_id { get; set; }
        public int categoria_id { get; set; }
        public string  categoria_descripcion { get; set; }
        public int estado_id { get; set; }
        public string producto_descripcion { get; set; }
        public int producto_existencias { get; set; }
        public decimal producto_precio { get; set; }
    }
}