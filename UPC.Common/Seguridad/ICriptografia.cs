using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UPC.Common.Seguridad
{
    public interface ICriptografia
    {
        string Encrypt(string strText, string strEncrKey);

        string Decrypt(string strText, string sDecrKey);
    }
}
