using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UPC.Common.Seguridad;

namespace UPC.Common
{
    public class EncriptacionFactory : ICriptografiaFactory
    {

        public ICriptografia Crear()
        {
            return new Encriptacion();
        }
    }
}
