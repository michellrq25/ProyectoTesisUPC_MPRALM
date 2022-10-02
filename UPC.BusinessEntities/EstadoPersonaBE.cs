using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UPC.BusinessEntities
{
    public class EstadoPersonaBE
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

        public virtual ICollection<UsuarioBE> USUARIOEBE
        {
            get;
            set;
        }
        
        public EstadoPersonaBE()
        {
        }

    }
}
