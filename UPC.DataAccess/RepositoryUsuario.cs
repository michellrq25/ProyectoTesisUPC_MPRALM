using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UPC.BusinessEntities;

namespace UPC.DataAccess
{
    public class RepositoryUsuario
    {
        string cadena = ConfigurationManager.AppSettings["USU.Cnx_DB"].ToString();

        public List<RolBE> ListarRoles()
        {
            List<RolBE> lstRoles = new List<RolBE>();
            using (SqlConnection conexion = new SqlConnection(cadena))
            {
                SqlCommand cmd = new SqlCommand("SP_ListarRoles", conexion);
                cmd.CommandType = CommandType.StoredProcedure;

                conexion.Open();
                SqlDataReader filas = cmd.ExecuteReader();

                while (filas.Read())
                {

                    lstRoles.Add(
                             new RolBE
                             {
                                 CodigoRol = Convert.ToString(filas["cod_rol"]),
                                 DescripcionRol = filas.GetString(1)
                             }
                         );
                }
                conexion.Close();
                cmd.Dispose();
            }
            return lstRoles;
        }

        public string ObtenerRol(string cod_rol)
        {
            string rol = "";

            using (SqlConnection conexion = new SqlConnection(cadena))
            {

                SqlCommand cmd = new SqlCommand("SP_ObtieneRol", conexion);
                cmd.Parameters.Add("@cod_rol", SqlDbType.VarChar).Value = cod_rol;
                cmd.CommandType = CommandType.StoredProcedure;
                conexion.Open();
                cmd.ExecuteNonQuery();
                SqlDataReader filas = cmd.ExecuteReader();
                while (filas.Read())
                {
                    rol = Convert.ToString(filas["descrip_rol"]);
                }
                conexion.Close();
                cmd.Dispose();

                return rol;

            }
        }

        public bool RecuperarPassword(string email, out string mensaje, out string clave, out string usuario)
        {
            bool estado = false;
            try
            {
                usuario = string.Empty;
                mensaje = string.Empty;
                clave = string.Empty;
                using (SqlConnection conexion = new SqlConnection(cadena))
                {
                    SqlCommand cmd = new SqlCommand("usp_RecuperarContrasena", conexion);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@email", SqlDbType.VarChar).Value = email;
                    cmd.CommandType = CommandType.StoredProcedure;
                    conexion.Open();
                    SqlDataReader filas = cmd.ExecuteReader();
                    while (filas.Read())
                    {
                        usuario = Convert.ToString(filas["login"]);
                        clave = filas.GetString(1);
                        mensaje = filas.GetString(2);
                    }
                    estado = true;
                    conexion.Close();
                    cmd.Dispose();
                }
            }
            catch (System.Exception)
            {
                throw;
            }
            return estado;
        }

        public List<UsuarioBE> ListarUsuariosPorFiltro(string login, string dniUsuario, string rol, string estado)
        {
            try
            {
                List<UsuarioBE> lstRoles = new List<UsuarioBE>();
                using (SqlConnection conexion = new SqlConnection(cadena))
                {
                    SqlCommand cmd = new SqlCommand("SP_ListarUsuarios", conexion);
                    cmd.Parameters.Add("@login", SqlDbType.VarChar).Value = login;
                    cmd.Parameters.Add("@dniUsuario", SqlDbType.VarChar).Value = dniUsuario;
                    cmd.Parameters.Add("@rol", SqlDbType.VarChar).Value = rol;
                    cmd.Parameters.Add("@estado", SqlDbType.VarChar).Value = estado;
                    cmd.CommandType = CommandType.StoredProcedure;

                    conexion.Open();
                    SqlDataReader filas = cmd.ExecuteReader();

                    while (filas.Read())
                    {

                        lstRoles.Add(
                                 new UsuarioBE
                                 {
                                     login = Convert.ToString(filas["login"]),
                                     DNIUsuario = Convert.ToString(filas["DNIUsuario"]),
                                     ApeUsuario = Convert.ToString(filas["ApeUsuario"]),
                                     NomUsuario = Convert.ToString(filas["NomUsuario"]),
                                     cod_sexo = Convert.ToString(filas["cod_sexo"]),
                                     SexoBE = new SexoBE()
                                     {
                                         DescripcionSexo = Convert.ToString(filas["DescripcionSexo"])
                                     },
                                     cod_rol = Convert.ToString(filas["cod_rol"]),
                                     RolBE = new RolBE()
                                     {
                                         DescripcionRol = Convert.ToString(filas["descrip_rol"])
                                     },
                                     Estado = Convert.ToString(filas["flag_activo"]),
                                     id_usu = Convert.ToInt32(filas["id_usu"]),
                                     e_mail = Convert.ToString(filas["email"]),
                                     fec_mod = Convert.ToDateTime(filas["fec_mod"])
                                 }
                             );
                    }
                    conexion.Close();
                    cmd.Dispose();
                }
                return lstRoles;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public UsuarioBE CargaUsuarios_ID(int intUsuario)
        {
            try
            {
                UsuarioBE lstUsuarios = new UsuarioBE();
                using (SqlConnection conexion = new SqlConnection(cadena))
                {
                    SqlCommand cmd = new SqlCommand("SW_SP_CargaUsuario", conexion);
                    cmd.Parameters.Add("@idUsuario", SqlDbType.VarChar).Value = intUsuario;
                    cmd.CommandType = CommandType.StoredProcedure;

                    conexion.Open();
                    SqlDataReader filas = cmd.ExecuteReader();

                    while (filas.Read())
                    {
                        RolBE objRol = new RolBE();
                        objRol.CodigoRol = Convert.ToString(filas["cod_rol"]);
                        lstUsuarios.cod_rol = Convert.ToString(filas["cod_rol"]);
                        lstUsuarios.id_usu = Convert.ToInt32(filas["id_usu"]);
                        lstUsuarios.login = Convert.ToString(filas["login"]);
                        lstUsuarios.fec_mod = Convert.ToDateTime(filas["fec_mod"]);
                        lstUsuarios.DNIUsuario = Convert.ToString(filas["DNIUsuario"]);
                        lstUsuarios.ApeUsuario = Convert.ToString(filas["ApeUsuario"]);
                        lstUsuarios.NomUsuario = Convert.ToString(filas["NomUsuario"]);
                        lstUsuarios.cod_sexo = Convert.ToString(filas["cod_sexo"]);
                        lstUsuarios.RolBE = objRol;
                        lstUsuarios.Estado = Convert.ToString(filas["flag_activo"]);
                        lstUsuarios.e_mail = Convert.ToString(filas["email"]);
                    }
                    conexion.Close();
                    cmd.Dispose();
                }
                return lstUsuarios;
            }
            catch (System.Exception ex)
            {
                throw new System.Exception(ex.Message, ex);
            }
        }

        public int RegistraUsuario(string codRol, string Login, string Password, string Numdoc, string Apellidos, string Nombres, string codSexo, int Estado,
        String email, ref string MensajeError)
        {
            try
            {
                int id_usu = 0;
                using (SqlConnection conexion = new SqlConnection(cadena))
                {
                    SqlCommand cmd = new SqlCommand("SW_SP_RegistraUsuario", conexion);
                    cmd.Parameters.Add("@cod_rol", SqlDbType.VarChar).Value = codRol;
                    cmd.Parameters.Add("@login", SqlDbType.VarChar).Value = Login;
                    cmd.Parameters.Add("@password", SqlDbType.VarChar).Value = Password;
                    cmd.Parameters.Add("@DNIUsuario", SqlDbType.VarChar).Value = Numdoc;
                    cmd.Parameters.Add("@ApeUsuario", SqlDbType.VarChar).Value = Apellidos;
                    cmd.Parameters.Add("@NomUsuario", SqlDbType.VarChar).Value = Nombres;
                    cmd.Parameters.Add("@cod_sexo", SqlDbType.VarChar).Value = codSexo;
                    cmd.Parameters.Add("@flag_activo", SqlDbType.Int).Value = Estado;
                    cmd.Parameters.Add("@email", SqlDbType.VarChar).Value = email;

                    SqlParameter pUsuario = new SqlParameter("@idUsuario", SqlDbType.Int);
                    pUsuario.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(pUsuario);
                    cmd.CommandType = CommandType.StoredProcedure;
                    conexion.Open();
                    cmd.ExecuteNonQuery();
                    conexion.Close();
                    cmd.Dispose();

                    id_usu = Convert.ToInt32(pUsuario.Value);

                }
                return id_usu;
            }
            catch (System.Exception ex)
            {
                MensajeError = ex.Message.ToString();
                return 0;
            }
        }

        public bool SW_SP_ModificaUsuario(string codRol, string login, string NumDoc, string apellidos,
            string nombres, string codSexo, Int32 estado, Int32 idUsuario, String email,
            ref string MensajeError)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(cadena))
                {
                    SqlCommand cmd = new SqlCommand("SW_SP_ModificaUsuario", conexion);
                    cmd.Parameters.Add("@cod_rol", SqlDbType.VarChar).Value = codRol;
                    cmd.Parameters.Add("@login", SqlDbType.VarChar).Value = login;
                    cmd.Parameters.Add("@DNIUsuario", SqlDbType.VarChar).Value = NumDoc;
                    cmd.Parameters.Add("@ApeUsuario", SqlDbType.VarChar).Value = apellidos;
                    cmd.Parameters.Add("@NomUsuario", SqlDbType.VarChar).Value = nombres;
                    cmd.Parameters.Add("@cod_sexo", SqlDbType.VarChar).Value = codSexo;
                    cmd.Parameters.Add("@flag_activo", SqlDbType.Int).Value = estado;
                    cmd.Parameters.Add("@idUsuario", SqlDbType.Int).Value = idUsuario;
                    cmd.Parameters.Add("@email", SqlDbType.VarChar).Value = email;

                    cmd.CommandType = CommandType.StoredProcedure;
                    conexion.Open();
                    cmd.ExecuteNonQuery();

                    conexion.Close();
                    cmd.Dispose();

                    return true;

                }
            }
            catch (System.Exception ex)
            {

                MensajeError = ex.Message.ToString();
                return true;
            }
        }

        public String HabilitarInhabilitarEliminar(int tipo, int idUsuario)
        {
            try
            {
                String login = "";
                using (SqlConnection conexion = new SqlConnection(cadena))
                {
                    SqlCommand cmd = new SqlCommand("SEL_ADM_USU_EDI", conexion);
                    cmd.Parameters.Add("@tipo", SqlDbType.Int).Value = tipo;
                    cmd.Parameters.Add("@id_usu", SqlDbType.Int).Value = idUsuario;
                    cmd.CommandType = CommandType.StoredProcedure;
                    conexion.Open();
                    SqlDataReader filas = cmd.ExecuteReader();

                    while (filas.Read())
                    {
                        login = filas.GetString(0);
                    }
                    conexion.Close();
                    cmd.Dispose();
                }
                return login;
            }
            catch (System.Exception ex) { throw new Exception(ex.Message, ex); }
        }

        public bool ExisteLogin(string strLogin)
        {

            using (SqlConnection conexion = new SqlConnection(cadena))
            {

                SqlCommand cmd = new SqlCommand("SW_SP_ExisteLogin", conexion);
                cmd.Parameters.Add("@Login", SqlDbType.VarChar).Value = strLogin;
                cmd.CommandType = CommandType.StoredProcedure;
                conexion.Open();
                cmd.ExecuteNonQuery();
                SqlDataReader filas = cmd.ExecuteReader();
                bool Estado = false;
                while (filas.Read())
                {
                    if (filas.GetString(0) == "NO EXISTE")
                    {
                        Estado = false;
                    }
                    else { Estado = true; }
                }
                conexion.Close();
                cmd.Dispose();

                return Estado;

            }
        }

        public bool ExisteDNI(string dni)
        {
            int resultado;
            using (SqlConnection conexion = new SqlConnection(cadena))
            {

                SqlCommand cmd = new SqlCommand("SW_SP_ValidarDNI", conexion);
                cmd.Parameters.Add("@DNI", SqlDbType.VarChar).Value = dni;
                cmd.Parameters.Add("@Resultado", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.CommandType = CommandType.StoredProcedure;
                conexion.Open();
                cmd.ExecuteNonQuery();
                bool Estado = false;

                resultado = Convert.ToInt32(cmd.Parameters["@Resultado"].Value);

                if (resultado > 0)
                {
                    Estado = false;
                }
                else
                {
                    Estado = true;
                }

                conexion.Close();
                cmd.Dispose();


                return Estado;

            }

        }

        public string CorreoValido(int idusu, string email)
        {

            using (SqlConnection conexion = new SqlConnection(cadena))
            {

                SqlCommand cmd = new SqlCommand("SW_SP_ValidarCorreo", conexion);
                cmd.Parameters.Add("@p_email", SqlDbType.VarChar).Value = email;
                cmd.Parameters.Add("@p_idUsu", SqlDbType.Int).Value = idusu;
                cmd.CommandType = CommandType.StoredProcedure;
                conexion.Open();
                cmd.ExecuteNonQuery();
                SqlDataReader filas = cmd.ExecuteReader();
                string mensaje = "";
                while (filas.Read())
                {
                    mensaje = filas.GetString(0);
                }
                conexion.Close();
                cmd.Dispose();

                return mensaje;

            }

        }
    }
}
