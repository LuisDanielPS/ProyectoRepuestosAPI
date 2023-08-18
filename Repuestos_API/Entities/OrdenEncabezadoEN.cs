using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Repuestos_API.Entities
{
    public class OrdenEncabezadoEN
    {
        public int orden_id { get; set; }
        public int proveedor_id { get; set; }
        public int estado_id { get; set; }
        public string orden_tipo { get; set; }
        public DateTime orden_fecha { get; set; }
        public string orden_facturarA { get; set; }
        public string orden_entregarA { get; set; }
        public string orden_descripcion { get; set; }
        public List<OrdenDetalleEN> orden_detalle { get; set; }
    }
}