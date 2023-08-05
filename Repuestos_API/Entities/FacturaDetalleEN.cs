using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Repuestos_API.Entities
{
    public class FacturaDetalleEN
    {
        public int facturaD_id { get; set; }
        public int factura_id { get; set; }
        public int producto_id { get; set; }
        public int facturaD_cantidad { get; set; }
        public decimal facturaD_precio { get; set; }
        public decimal facturaD_descuento { get; set; }
        public decimal facturaD_total { get; set; }
    }
}