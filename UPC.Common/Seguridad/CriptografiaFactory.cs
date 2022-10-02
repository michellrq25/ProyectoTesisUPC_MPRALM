using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UPC.Common.Seguridad
{
    public static class CriptografiaFactory
    {
        static ICriptografiaFactory _actualICriptografiaFactory = null;

        //public static void EstablecerActual(ICriptografiaFactory criptografiaFactory)
        //{
        //    _actualICriptografiaFactory = criptografiaFactory;
        //}

        public static ICriptografia CrearCriptografia()
        {
            return _actualICriptografiaFactory.Crear();
        }
    }
}
