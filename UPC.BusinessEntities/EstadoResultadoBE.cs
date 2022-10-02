using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace UPC.BusinessEntities
{
    public class EstadoResultadoBE
    {
        public virtual string idEstado
        {
            get;
            set;
        }

        [Display(Name = "Estado")]
        public virtual string desEstado
        {
            get;
            set;
        }


        public EstadoResultadoBE()
        {
        }
    }
}
