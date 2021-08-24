using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ReporteCanalContacto.Models
{
    public class FiltroBusqueda
    {
        [Display(Name = "Filtrar por Fecha: ")]
        public DateTime fechaDesde { get; set; }
        public DateTime fechaHasta { get; set; }
        public string tipoCanal { get; set; }
        public string tipoConsulta { get; set; }

    }
}