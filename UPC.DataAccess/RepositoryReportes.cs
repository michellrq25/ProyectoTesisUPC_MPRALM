using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UPC.BusinessEntities;

namespace UPC.DataAccess
{
    public class RepositoryReportes
    {
        readonly string cadena = ConfigurationManager.AppSettings["USU.Cnx_DB"].ToString();

        public List<ResultadoPrediccionBE> ListarResultadosPorFiltro(string dniAlumno, string seccion, string grado, string estado)
        {
            try
            {
                List<ResultadoPrediccionBE> lstResultados = new List<ResultadoPrediccionBE>();
                using (SqlConnection conexion = new SqlConnection(cadena))
                {
                    SqlCommand cmd = new SqlCommand("SP_ListarResultadoPred", conexion);
                    cmd.Parameters.Add("@dniAlumno", SqlDbType.VarChar).Value = dniAlumno;
                    cmd.Parameters.Add("@seccion", SqlDbType.VarChar).Value = seccion;
                    cmd.Parameters.Add("@grado", SqlDbType.VarChar).Value = grado;
                    cmd.Parameters.Add("@estado", SqlDbType.VarChar).Value = estado;
                    cmd.CommandType = CommandType.StoredProcedure;

                    conexion.Open();
                    SqlDataReader filas = cmd.ExecuteReader();

                    while (filas.Read())
                    {
                        lstResultados.Add(
                                 new ResultadoPrediccionBE
                                 {
                                     CodPrediccion = Convert.ToInt32(filas["codPrediccion"]),
                                     DNI = Convert.ToString(filas["DNI"]),
                                     ApellidosNombres = Convert.ToString(filas["ApellidosNombres"]),
                                     Sexo = Convert.ToString(filas["Sexo"]),
                                     Curso = new CursoBE()
                                     {
                                         PersonalSocial = Convert.ToString(filas["PersonalSocial"]),
                                         EducacionReligiosa = Convert.ToString(filas["EducacionReligiosa"]),
                                         EducacionFisica = Convert.ToString(filas["EducacionFisica"]),
                                         Comunicacion = Convert.ToString(filas["Comunicacion"]),
                                         ArteYCultura = Convert.ToString(filas["ArteYCultura"]),
                                         Ingles = Convert.ToString(filas["Ingles"]),
                                         Matematica = Convert.ToString(filas["Matematica"]),
                                         CienciaYTecnologia = Convert.ToString(filas["CienciaYTecnologia"])
                                     },
                                     Inasistencias = Convert.ToInt32(filas["Inasistencias"]),
                                     HorasDedicacion = Convert.ToInt32(filas["HorasDedicacion"]),
                                     HorasDormir = Convert.ToInt32(filas["HorasDormir"]),
                                     GradoDeInstruccion = Convert.ToString(filas["GradoDeInstruccion"]),
                                     DistritoDeResidencia = Convert.ToString(filas["DistritoDeResidencia"]),
                                     Resultado = Convert.ToInt32(filas["Resultado"])
                                 }
                             );
                    }
                    conexion.Close();
                    cmd.Dispose();
                }
                return lstResultados;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public RepVerAlumnoResultadoBE F_ConsultaAlumnoResultado(string DNI, string CodPrediccion)
        {
            RepVerAlumnoResultadoBE repVerAlumno = new RepVerAlumnoResultadoBE();
            try
            {
                using (SqlConnection conexion = new SqlConnection(cadena))
                {
                    using (SqlCommand cmd = new SqlCommand("SW_SP_VerAlumnoResultado", conexion))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@dniAlumno", SqlDbType.VarChar).Value = DNI;
                        cmd.Parameters.Add("@CodPrediccion", SqlDbType.Int).Value = CodPrediccion;
                        conexion.Open();
                        SqlDataReader filas = cmd.ExecuteReader();
                        while (filas.Read())
                        {
                            repVerAlumno.CodPrediccion = Convert.ToInt32(filas["codPrediccion"] is DBNull ? 0 : filas["codPrediccion"]);
                            repVerAlumno.DNI = Convert.ToString(filas["DNI"] is DBNull ? 0 : filas["DNI"]);
                            repVerAlumno.ApellidosNombres = Convert.ToString(filas["ApellidosNombres"] is DBNull ? "" : filas["ApellidosNombres"]);
                            repVerAlumno.Sexo = Convert.ToString(filas["Sexo"] is DBNull ? "" : filas["Sexo"]);
                            repVerAlumno.Curso = new CursoBE()
                            {
                                PersonalSocial = Convert.ToString(filas["PersonalSocial"] is DBNull ? "" : filas["PersonalSocial"]),
                                EducacionReligiosa = Convert.ToString(filas["EducacionReligiosa"] is DBNull ? "" : filas["EducacionReligiosa"]),
                                EducacionFisica = Convert.ToString(filas["EducacionFisica"] is DBNull ? "" : filas["EducacionFisica"]),
                                Comunicacion = Convert.ToString(filas["Comunicacion"] is DBNull ? "" : filas["Comunicacion"]),
                                ArteYCultura = Convert.ToString(filas["ArteYCultura"] is DBNull ? "" : filas["ArteYCultura"]),
                                Ingles = Convert.ToString(filas["Ingles"] is DBNull ? "" : filas["Ingles"]),
                                Matematica = Convert.ToString(filas["Matematica"] is DBNull ? "" : filas["Matematica"]),
                                CienciaYTecnologia = Convert.ToString(filas["CienciaYTecnologia"] is DBNull ? "" : filas["CienciaYTecnologia"]),
                            };
                            repVerAlumno.Inasistencias = Convert.ToInt32(filas["Inasistencias"] is DBNull ? 0 : filas["Inasistencias"]);
                            repVerAlumno.HorasDedicacion = Convert.ToInt32(filas["HorasDedicacion"] is DBNull ? 0 : filas["HorasDedicacion"]);
                            repVerAlumno.HorasDormir = Convert.ToInt32(filas["HorasDormir"] is DBNull ? 0 : filas["HorasDormir"]);
                            repVerAlumno.GradoDeInstruccion = Convert.ToString(filas["GradoDeInstruccion"] is DBNull ? "" : filas["GradoDeInstruccion"]);
                            repVerAlumno.DistritoDeResidencia = Convert.ToString(filas["DistritoDeResidencia"] is DBNull ? "" : filas["DistritoDeResidencia"]);
                            repVerAlumno.Resultado = Convert.ToInt32(filas["Resultado"] is DBNull ? 0 : filas["Resultado"]);
                        }
                        conexion.Close();
                        cmd.Dispose();
                    }
                }
                return repVerAlumno;
            }
            catch (System.Exception ex)
            {
                throw new System.Exception(ex.Message, ex);
            }
        }

        public List<SugerenciaCursoBE> ListarSugerenciaCursos()
        {
            List<SugerenciaCursoBE> lst = new List<SugerenciaCursoBE>();
            using (SqlConnection conexion = new SqlConnection(cadena))
            {
                SqlCommand cmd = new SqlCommand("SP_ListarSugerenciaCursos", conexion);
                cmd.CommandType = CommandType.StoredProcedure;

                conexion.Open();
                SqlDataReader filas = cmd.ExecuteReader();

                while (filas.Read())
                {

                    lst.Add(
                             new SugerenciaCursoBE
                             {
                                 Nombre = Convert.ToString(filas["cabecera"]),
                                 Descripcion = Convert.ToString(filas["comentarios"])
                             }
                         );
                }
                conexion.Close();
                cmd.Dispose();
            }
            return lst;
        }

        public List<SugerenciasBE> ListarSugerenciaHabitos()
        {
            List<SugerenciasBE> lst = new List<SugerenciasBE>();
            using (SqlConnection conexion = new SqlConnection(cadena))
            {
                SqlCommand cmd = new SqlCommand("SP_ListarSugerenciaHabitos", conexion);
                cmd.CommandType = CommandType.StoredProcedure;

                conexion.Open();
                SqlDataReader filas = cmd.ExecuteReader();

                while (filas.Read())
                {

                    lst.Add(
                             new SugerenciasBE
                             {
                                 Nombre = Convert.ToString(filas["cabecera"]),
                                 Descripcion = Convert.ToString(filas["comentarios"])
                             }
                         );
                }
                conexion.Close();
                cmd.Dispose();
            }
            return lst;
        }
    }
}
