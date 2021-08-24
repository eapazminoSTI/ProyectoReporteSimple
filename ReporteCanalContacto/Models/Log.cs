using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReporteCanalContacto.Models
{
    public class Log
    {
        public int id { get; set; }
        public string cedula { get; set; }
        public string consulta { get; set; }
        public string tipo { get; set; }
        public DateTime fecha { get; set; }

    }
}