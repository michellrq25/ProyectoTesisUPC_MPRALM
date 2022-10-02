using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UPC.BusinessEntities
{
    public class RolBE
    {
        public virtual string CodigoRol { get; set; }
        [Display(Name = "Rol")]
        public virtual string DescripcionRol { get; set; }
    }
}
