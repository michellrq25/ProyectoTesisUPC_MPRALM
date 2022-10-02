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
    public class RepositoryPrediccion : IRepositoryPrediccion
    {
        readonly string cadena = ConfigurationManager.AppSettings["USU.Cnx_DB"].ToString();

        public int ObtenerCodPredMax()
        {
            int codigo = 0;
            using (SqlConnection conexion = new SqlConnection(cadena))
            {

                using (SqlCommand cmd = new SqlCommand("SP_OBTENER_COD_PRED_MAX", conexion))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conexion.Open();
                    cmd.ExecuteNonQuery();
                    SqlDataReader filas = cmd.ExecuteReader();
                    while (filas.Read())
                    {
                        codigo = Convert.ToInt32(filas.GetInt32(0));
                    }
                    conexion.Close();
                    cmd.Dispose();
                }

                return codigo;
            }
        }

        public int PostResultadoPredAgregar(List<PrediccionBE> listarPredicciones)
        {
            int codPrediccion = ObtenerCodPredMax();

            var ID_ACT = 0;
            Random rnd = new Random();
            try
            {
                foreach (PrediccionBE predicion in listarPredicciones)
                {
                    predicion.codPrediccion = codPrediccion;
                    predicion.Resultado = rnd.Next(2);
                    InsertarPrediccion(predicion);
                }
                ID_ACT = 1;
            }
            catch (Exception)
            {
                return ID_ACT;
            }
            return ID_ACT;
        }

        public void InsertarPrediccion(PrediccionBE prediccion)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(cadena))
                {
                    SqlCommand cmd = new SqlCommand("SP_Registrar_Resultado_Prediccion", conexion);
                    cmd.Parameters.Add("@codPrediccion", SqlDbType.Int).Value = prediccion.codPrediccion;
                    cmd.Parameters.Add("@DNI", SqlDbType.VarChar).Value = prediccion.DNI;
                    cmd.Parameters.Add("@ApellidosNombres", SqlDbType.VarChar).Value = prediccion.ApellidosNombres;
                    cmd.Parameters.Add("@Sexo", SqlDbType.VarChar).Value = prediccion.Sexo;
                    cmd.Parameters.Add("@PersonalSocial", SqlDbType.VarChar).Value = prediccion.Curso.PersonalSocial;
                    cmd.Parameters.Add("@EducacionReligiosa", SqlDbType.VarChar).Value = prediccion.Curso.EducacionReligiosa;
                    cmd.Parameters.Add("@EducacionFisica", SqlDbType.VarChar).Value = prediccion.Curso.EducacionFisica;
                    cmd.Parameters.Add("@Comunicacion", SqlDbType.VarChar).Value = prediccion.Curso.Comunicacion;
                    cmd.Parameters.Add("@ArteYCultura", SqlDbType.VarChar).Value = prediccion.Curso.ArteYCultura;
                    cmd.Parameters.Add("@Ingles", SqlDbType.VarChar).Value = prediccion.Curso.Ingles;
                    cmd.Parameters.Add("@Matematica", SqlDbType.VarChar).Value = prediccion.Curso.Matematica;
                    cmd.Parameters.Add("@CienciaYTecnologia", SqlDbType.VarChar).Value = prediccion.Curso.CienciaYTecnologia;
                    cmd.Parameters.Add("@Inasistencias", SqlDbType.Int).Value = prediccion.Inasistencias;
                    cmd.Parameters.Add("@HorasDedicacion", SqlDbType.Int).Value = prediccion.HorasDedicacion;
                    cmd.Parameters.Add("@HorasDormir", SqlDbType.Int).Value = prediccion.HorasDormir;
                    cmd.Parameters.Add("@GradoDeInstruccion", SqlDbType.VarChar).Value = prediccion.GradoDeInstruccion;
                    cmd.Parameters.Add("@DistritoDeResidencia", SqlDbType.VarChar).Value = prediccion.DistritoDeResidencia;
                    cmd.Parameters.Add("@resultado", SqlDbType.Int).Value = prediccion.Resultado;

                    cmd.CommandType = CommandType.StoredProcedure;
                    conexion.Open();
                    cmd.ExecuteNonQuery();
                    conexion.Close();
                    cmd.Dispose();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<EstadoResultadoBE> ListaEstadosResultados()
        {
            List<EstadoResultadoBE> listResult = new List<EstadoResultadoBE>();
            try
            {
                using (SqlConnection conexion = new SqlConnection(cadena))
                {

                    using (SqlCommand cmd = new SqlCommand("SP_listadoEstadosPrediccion", conexion))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        conexion.Open();
                        cmd.ExecuteNonQuery();
                        SqlDataReader filas = cmd.ExecuteReader();
                        while (filas.Read())
                        {
                            listResult.Add(new EstadoResultadoBE
                            {
                                idEstado = Convert.ToString(filas["idEstado"] is DBNull ? "0" : filas["idEstado"]),
                                desEstado = Convert.ToString(filas["desEstado"] is DBNull ? "" : filas["desEstado"])
                            });
                        }
                        conexion.Close();
                        cmd.Dispose();
                    }

                    return listResult;
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }

        }
        public List<SeccionBE> ListaSecciones()
        {
            List<SeccionBE> listResult = new List<SeccionBE>();
            try
            {
                using (SqlConnection conexion = new SqlConnection(cadena))
                {

                    using (SqlCommand cmd = new SqlCommand("SP_listarSecciones", conexion))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        conexion.Open();
                        cmd.ExecuteNonQuery();
                        SqlDataReader filas = cmd.ExecuteReader();
                        while (filas.Read())
                        {
                            listResult.Add(new SeccionBE
                            {
                                idSeccion = Convert.ToString(filas["cod_seccion"] is DBNull ? "0" : filas["cod_seccion"]),
                                desSeccion = Convert.ToString(filas["descrip_seccion"] is DBNull ? "" : filas["descrip_seccion"])
                            });
                        }
                        conexion.Close();
                        cmd.Dispose();
                    }

                    return listResult;
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
        public List<GradoBE> ListaGrados()
        {
            List<GradoBE> listResult = new List<GradoBE>();
            try
            {
                using (SqlConnection conexion = new SqlConnection(cadena))
                {

                    using (SqlCommand cmd = new SqlCommand("SP_listarGrados", conexion))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        conexion.Open();
                        cmd.ExecuteNonQuery();
                        SqlDataReader filas = cmd.ExecuteReader();
                        while (filas.Read())
                        {
                            listResult.Add(new GradoBE
                            {
                                idGrado = Convert.ToString(filas["cod_grado"] is DBNull ? "0" : filas["cod_grado"]),
                                desGrado = Convert.ToString(filas["descrip_grado"] is DBNull ? "" : filas["descrip_grado"])
                            });
                        }
                        conexion.Close();
                        cmd.Dispose();
                    }
                    return listResult;
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
    }
}
