using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UPC.BusinessEntities;

namespace UPC.Repositories
{
    public interface IRepositoryLogueo
    {
        UsuarioBE f_login(String strLogin, String StrPassword, out String strMensaje);       
    }
}
