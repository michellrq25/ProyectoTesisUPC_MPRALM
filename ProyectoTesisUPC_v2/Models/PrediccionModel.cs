using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProyectoTesisUPC_v2.Models
{
    public class PrediccionModel
    {
        [Required]
        [Display(Name = "Cargar archivo")]
        public HttpPostedFileBase ArchivoExcel { get; set; }
    }
}