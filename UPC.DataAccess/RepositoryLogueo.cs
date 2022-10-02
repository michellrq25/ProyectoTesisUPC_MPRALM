using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UPC.BusinessEntities;
using UPC.Repositories;

namespace UPC.DataAccess
{
    public class RepositoryLogueo : IRepositoryLogueo
    {
        string cadena = ConfigurationManager.AppSettings["USU.Cnx_DB"].ToString();

        public UsuarioBE f_login(string strLogin, string StrPassword, out String strMensaje)
        {
            try
            {
                UsuarioBE obj = new UsuarioBE();
                strMensaje = string.Empty;
                using (SqlConnection conexion = new SqlConnection(cadena))
                {
                    SqlCommand cmd = new SqlCommand("ACC_USU_VAL", conexion);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@Login", SqlDbType.VarChar, 15).Value = strLogin;
                    cmd.Parameters.Add("@Password", SqlDbType.VarChar, 25).Value = StrPassword;
                    
                    conexion.Open();
                    SqlDataReader filas = cmd.ExecuteReader();
                    while (filas.Read())
                    {
                        obj.id_usu = filas.GetInt32(0);
                        obj.DNIUsuario = filas.GetString(1);
                        obj.ApeUsuario = filas.GetString(2);
                        obj.NomUsuario = filas.GetString(3);
                        obj.cod_rol = filas.GetString(4);
                        strMensaje = filas.GetString(5);
                    }
                    conexion.Close();
                    cmd.Dispose();
                }
                return obj;
            }
            catch (System.Exception ex) { throw ex; }
        }
    }
}
