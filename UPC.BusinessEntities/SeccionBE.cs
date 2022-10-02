using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace UPC.BusinessEntities
{
    public class SeccionBE
    {
        public virtual string idSeccion
        {
            get;
            set;
        }

        public virtual string desSeccion
        {
            get;
            set;
        }
    }
}
