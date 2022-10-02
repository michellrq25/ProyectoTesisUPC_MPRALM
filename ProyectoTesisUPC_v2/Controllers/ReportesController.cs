using ProyectoTesisUPC_v2.Models;
using ProyectoTesisUPC_v2.Permisos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UPC.BusinessEntities;
using UPC.Repositories;
using UPC.DataAccess;
using System.Globalization;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.IO.Packaging;
using System.Reflection;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Spreadsheet;
using Row = DocumentFormat.OpenXml.Spreadsheet.Row;
using OfficeOpenXml;
using System.IO;
using System.Data;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web.Helpers;
using System.Configuration;
using DocumentFormat.OpenXml.Office2010.Excel;
using OfficeOpenXml.Style;
using System.Web.Services.Description;
using RazorEngine.Templating;

namespace ProyectoTesisUPC_v2.Controllers
{
    [SessionAuthorize]
    public class ReportesController : Controller
    {
        readonly RepositoryPrediccion repositoryPrediccion = new RepositoryPrediccion();
        readonly RepositoryReportes repositoryReportes = new RepositoryReportes();
        private readonly string usuarioSMTP = ConfigurationManager.AppSettings["usuarioSMTP"].ToString();
        private readonly string claveSMTP = ConfigurationManager.AppSettings["claveSMTP"].ToString();
        static readonly string TemplateFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plantillas");

        private List<SelectListItem> ListaEstadosResultados()
        {
            try
            {
                List<EstadoResultadoBE> listaEstados = repositoryPrediccion.ListaEstadosResultados();
                listaEstados.Add(new EstadoResultadoBE { idEstado = "999", desEstado = "--Todos--" });
                return (from tab in listaEstados select new SelectListItem { Text = tab.desEstado, Value = tab.idEstado.ToString() }).ToList().OrderByDescending(o => o.Text).ToList();
            }
            catch (Exception ex) { throw ex; }
        }

        private List<SelectListItem> ListaSecciones()
        {
            try
            {
                List<SeccionBE> listaSecciones = repositoryPrediccion.ListaSecciones();
                return (from tab in listaSecciones select new SelectListItem { Text = tab.desSeccion, Value = tab.idSeccion.ToString() }).ToList().OrderBy(o => o.Text).ToList();
            }
            catch (Exception ex) { throw ex; }
        }

        private List<SelectListItem> ListaGrados()
        {
            try
            {
                List<GradoBE> listaGrados = repositoryPrediccion.ListaGrados();
                return (from tab in listaGrados select new SelectListItem { Text = tab.desGrado, Value = tab.idGrado.ToString() }).ToList().OrderBy(o => o.Value).ToList();
            }
            catch (Exception ex) { throw ex; }
        }

        public static string FirstCharToUpper(string input)
        {
            if (String.IsNullOrEmpty(input))
                throw new ArgumentException("Error  !");
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input.ToLower());
        }

        public void ExportarDataToExcel(List<RepVerAlumnoResultadoBE> lista)
        {
            if (lista is null)
            {
                throw new ArgumentNullException(nameof(lista));
            }
            //var response = Session["ActaNotas"] as dynamic;

            //if (response != null)
            //{
            //    var AlumnosResultados = new List<RepVerAlumnoResultadoBE>();
            //    var datos = response.data.datos_opcion;
            //    var alumnos = response.data.RepVerAlumnoResultadoBE;
            //    var cabecera = 0;
            //    var column = 2;
            //    var columnaRango = 0;

            //    foreach (var alumno in alumnos)
            //    {
            //        foreach (var columna in alumno.Columnas[0])
            //        {
            //            var objAlumno = new RepVerAlumnoResultadoBE
            //            {
            //                DNI = datos
            //                //Profesor = datos.Parofesor,
            //                //Asignatura = datos.Ramo + " " + datos["Nombre Ramo"],
            //                //Seccion = datos["Sección"],
            //                //Carrera = datos.Carrera + " " + datos["Nombre Carrera"],
            //                //Anio = datos["Año"],
            //                //Periodo = datos["Período"],
            //                //Jornada = datos.Jornada,
            //                //Columna = columna.Columna,
            //                //Valor = columna.Valor,
            //                //CodigoCliente = alumno.CodCli
            //            };

            //            AlumnosResultados.Add(objAlumno);
            //        }
            //    }
            //    var memoryStream = new MemoryStream();
            //    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            //    using (var excel = new ExcelPackage(memoryStream))
            //    {
            //        var worksheet = excel.Workbook.Worksheets.Add("Acta de Notas");
            //        var i = 11;
            //        var c = 1;

            //        worksheet.Cells[1, 16].Value = "Fecha Impresión:";
            //        worksheet.Cells[1, 16, 1, 18].Merge = true;
            //        worksheet.Cells[1, 16, 1, 18].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

            //        worksheet.Cells[1, 19].Value = DateTime.Now.ToString("dd/MM/yyyy");
            //        worksheet.Cells[1, 19, 1, 21].Merge = true;
            //        worksheet.Cells[1, 19, 1, 21].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

            //        worksheet.Cells[2, 1].Value = "ACTA DE NOTAS";
            //        worksheet.Cells[2, 1, 2, 24].Merge = true;
            //        worksheet.Cells[2, 1, 2, 24].Style.Font.Bold = true;
            //        worksheet.Cells[2, 1, 2, 24].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            //        worksheet.Cells[4, 1].Value = "Profesor:";
            //        worksheet.Cells[4, 1, 4, 2].Merge = true;
            //        worksheet.Cells[4, 1, 4, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            //        worksheet.Cells[4, 3].Value = AlumnosResultados[0].Profesor;
            //        worksheet.Cells[4, 3, 4, 12].Merge = true;
            //        worksheet.Cells[4, 3, 4, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            //        worksheet.Cells[5, 1].Value = "Asignatura:";
            //        worksheet.Cells[5, 1, 5, 2].Merge = true;
            //        worksheet.Cells[5, 1, 5, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            //        worksheet.Cells[5, 3].Value = AlumnosResultados[0].Asignatura;
            //        worksheet.Cells[5, 3, 5, 12].Merge = true;
            //        worksheet.Cells[5, 3, 5, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            //        worksheet.Cells[6, 1].Value = "Sección:";
            //        worksheet.Cells[6, 1, 6, 2].Merge = true;
            //        worksheet.Cells[6, 1, 6, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            //        worksheet.Cells[6, 3].Value = AlumnosResultados[0].Seccion;
            //        worksheet.Cells[6, 3, 6, 12].Merge = true;
            //        worksheet.Cells[6, 3, 6, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            //        worksheet.Cells[7, 1].Value = "Carrera:";
            //        worksheet.Cells[7, 1, 7, 2].Merge = true;
            //        worksheet.Cells[7, 1, 7, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            //        worksheet.Cells[7, 3].Value = AlumnosResultados[0].Carrera;
            //        worksheet.Cells[7, 3, 7, 12].Merge = true;
            //        worksheet.Cells[7, 3, 7, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

            //        worksheet.Cells[4, 16].Value = "Año:";
            //        worksheet.Cells[4, 18].Value = AlumnosResultados[0].Anio;
            //        worksheet.Cells[4, 18].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            //        worksheet.Cells[5, 16].Value = "Periódo:";
            //        worksheet.Cells[5, 18].Value = AlumnosResultados[0].Periodo;
            //        worksheet.Cells[5, 18].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            //        worksheet.Cells[6, 16].Value = "Jornada:";
            //        worksheet.Cells[6, 18].Value = AlumnosResultados[0].Jornada;
            //        worksheet.Cells[6, 18].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;


            //        worksheet.Cells[9, 1].Value = "Nomina de alumnos";
            //        worksheet.Cells[9, 1, 9, 2].Merge = true;
            //        worksheet.Cells[9, 1, 9, 2].Style.Font.Bold = true;
            //        worksheet.Cells[9, 1, 9, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            //        worksheet.Cells[10, 1].Value = "N°";
            //        worksheet.Column(1).Width = 3;
            //        worksheet.Cells[10, 1].Style.WrapText = true;
            //        worksheet.Cells[10, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            //        worksheet.Cells[10, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            //        foreach (var alumno in AlumnosResultados.GroupBy(x => new { x.DNI }))
            //        {
            //            worksheet.Cells[i, 1].Value = c;
            //            worksheet.Cells[i, 1].Style.WrapText = true;
            //            worksheet.Cells[i, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            //            worksheet.Cells[i, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            //            foreach (var col in alumno)
            //            {
            //                if (cabecera == 0)
            //                {
            //                    //if (!col.Columna.Contains("Concepto"))
            //                    //{
            //                    worksheet.Cells[10, column].Value = col.Columna;

            //                    if (col.Columna.Contains("Nombre"))
            //                    {
            //                        worksheet.Column(column).Width = 23;
            //                    }
            //                    else if (col.Columna.Contains("Estado") || col.Columna.ToLower().Contains("rut") || col.Columna.Contains("Concepto"))
            //                    {
            //                        worksheet.Column(column).Width = 10;
            //                    }
            //                    else
            //                    {
            //                        worksheet.Column(column).Width = 4;
            //                    }

            //                    worksheet.Cells[10, column].Style.WrapText = true;
            //                    worksheet.Cells[10, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            //                    worksheet.Cells[10, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            //                    worksheet.Row(10).Height = 38;
            //                    //}
            //                }

            //                //if (!col.Columna.Contains("Concepto"))
            //                //{
            //                if (column != 2)
            //                {
            //                    worksheet.Cells[i, column].Value = col.Valor;
            //                    worksheet.Cells[i, column].Style.WrapText = true;
            //                    worksheet.Cells[i, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            //                    worksheet.Cells[i, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            //                }
            //                else
            //                {
            //                    worksheet.Cells[i, column].Value = col.Valor;
            //                    worksheet.Cells[i, column].Style.WrapText = true;
            //                    worksheet.Cells[i, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            //                    worksheet.Cells[i, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            //                }

            //                worksheet.Row(i).Height = 25;

            //                column++;
            //                //}
            //            }

            //            i++;
            //            c++;

            //            if (cabecera == 0)
            //            {
            //                worksheet.Cells[9, 3].Value = "Notas";
            //                worksheet.Cells[9, 3, 9, column - 1].Merge = true;
            //                worksheet.Cells[9, 3, 9, column].Style.Font.Bold = true;
            //                worksheet.Cells[9, 3, 9, column - 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            //                columnaRango = column - 1;
            //            }

            //            cabecera = 1;
            //            column = 2;
            //        }

            //        using (var rango = worksheet.Cells[9, 1, i - 1, columnaRango])
            //        {
            //            rango.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            //            rango.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            //            rango.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            //            rango.Style.Border.Right.Style = ExcelBorderStyle.Thin;
            //        }

            //        if (columnaRango < 23)
            //        {
            //            for (int w = columnaRango + 1; w <= 22; w++)
            //            {
            //                worksheet.Column(w).Width = 4;
            //            }
            //        }

            //        using (var rango = worksheet.Cells[1, 1, i + 7, 24])
            //        {
            //            rango.Style.Font.Size = 7;
            //        }

            //        worksheet.Cells[i + 5, 2].Value = ActaNotas[0].Profesor;
            //        worksheet.Cells[i + 5, 2, i + 5, 4].Merge = true;
            //        worksheet.Cells[i + 5, 2, i + 5, 4].Style.WrapText = true;
            //        worksheet.Cells[i + 5, 2, i + 5, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            //        worksheet.Cells[i + 5, 2, i + 5, 4].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            //        worksheet.Cells[i + 6, 2].Value = "PROFESOR";
            //        worksheet.Cells[i + 6, 2, i + 6, 4].Merge = true;
            //        worksheet.Cells[i + 6, 2, i + 6, 4].Style.WrapText = true;
            //        worksheet.Cells[i + 6, 2, i + 6, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            //        worksheet.Cells[i + 6, 2, i + 6, 4].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            //        worksheet.Cells[i + 6, 8].Value = "COORDINADOR ACADÉMICO";
            //        worksheet.Cells[i + 6, 8, i + 6, 12].Merge = true;
            //        worksheet.Cells[i + 6, 8, i + 6, 12].Style.WrapText = true;
            //        worksheet.Cells[i + 6, 8, i + 6, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            //        worksheet.Cells[i + 6, 8, i + 6, 12].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            //        var data = excel.GetAsByteArray();
            //        var nombreArchivo = DateTime.Now.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("HHmmss");

            //        File(data, "application/octet-stream", $"ActaSemestralNotasParciales_{nombreArchivo}.xlsx");

            //        //return new EmptyResult();
            //    }
            //}
        }

        public ActionResult Reportes()
        {
            var listaEstados = ListaEstadosResultados();
            var listaSecciones = ListaSecciones();
            var listaGrados = ListaGrados();
            ViewBag.ddlEstado = listaEstados;
            ViewBag.ddlSecciones = listaSecciones;
            ViewBag.ddlGrados = listaGrados;
            return View();
        }

        [HttpGet]
        public ActionResult ListarResultadosPrincipal(string dniAlumno, string seccion, string grado, string estado)
        {
            try
            {
                int contador = 0;
                List<ResultadoPrediccionBE> objListResultadoGeneral;
                List<ResultadoPrediccionBE> objListaResultadoGeneralFinal = new List<ResultadoPrediccionBE>();
                objListResultadoGeneral = repositoryReportes.ListarResultadosPorFiltro(dniAlumno, seccion, grado, estado);
                foreach (ResultadoPrediccionBE entity in objListResultadoGeneral)
                {
                    if (objListaResultadoGeneralFinal.Where(f => f.DNI == entity.DNI).ToList().Count == 0)
                    {
                        if (contador == 400)
                        {
                            break;
                        }
                        else
                        {
                            contador++;
                        }
                        entity.ApellidosNombres = FirstCharToUpper(entity.ApellidosNombres);
                        objListaResultadoGeneralFinal.Add(entity);
                    }
                }
                ViewBag.CantidadRegistrosFiltrados = objListaResultadoGeneralFinal.Count;
                return PartialView("_ListarResultadosPrincipal", objListaResultadoGeneralFinal);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        [HttpGet]
        public ActionResult ConsultaResultadoUnico(string DNI, string CodPrediccion)
        {
            try
            {
                RepVerAlumnoResultadoBE repVerAlumnoResultadoBE;
                repVerAlumnoResultadoBE = repositoryReportes.F_ConsultaAlumnoResultado(DNI, CodPrediccion);
                repVerAlumnoResultadoBE.ApellidosNombres = FirstCharToUpper(repVerAlumnoResultadoBE.ApellidosNombres);
                repVerAlumnoResultadoBE.Seccion = "A";
                repVerAlumnoResultadoBE.GradoDeInstruccion = String.Concat(repVerAlumnoResultadoBE.GradoDeInstruccion, " de primaria");
                return PartialView("ConsultaAlumnoResultado", repVerAlumnoResultadoBE);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        [HttpGet]
        public ActionResult ExportarExcel(string dniAlumno, string seccion, string grado, string estado)
        {
            try
            {
                int contador = 0;
                List<ResultadoPrediccionBE> objListResultadoGeneral;
                List<ResultadoPrediccionBE> objListaResultadoGeneralFinal = new List<ResultadoPrediccionBE>();
                objListResultadoGeneral = repositoryReportes.ListarResultadosPorFiltro(dniAlumno, seccion, grado, estado);
                foreach (ResultadoPrediccionBE entity in objListResultadoGeneral)
                {
                    if (objListaResultadoGeneralFinal.Where(f => f.DNI == entity.DNI).ToList().Count == 0)
                    {
                        if (contador == 400)
                        {
                            break;
                        }
                        else
                        {
                            contador++;
                        }
                        entity.ApellidosNombres = FirstCharToUpper(entity.ApellidosNombres);
                        objListaResultadoGeneralFinal.Add(entity);
                    }
                }
                Session["ResultadoAlumnos"] = objListaResultadoGeneralFinal;
                var response = Session["ResultadoAlumnos"] as dynamic;

                if (response != null)
                {
                    var AlumnosResultados = new List<RepVerAlumnoResultadoBE>();
                    var datos = response.data.datos_opcion;
                    var alumnos = response.data.RepVerAlumnoResultadoBE;
                    var cabecera = 0;
                    var column = 2;
                    var columnaRango = 0;

                }
                return new JsonResult { Data = new { }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        [HttpGet]
        public ActionResult ObtenerCorreo(string DNI, string CodPrediccion)
        {
            RepVerAlumnoResultadoBE repVerAlumnoResultadoBE = repositoryReportes.F_ConsultaAlumnoResultado(DNI, CodPrediccion);
            repVerAlumnoResultadoBE.ApellidosNombres = FirstCharToUpper(repVerAlumnoResultadoBE.ApellidosNombres);
            repVerAlumnoResultadoBE.Seccion = "A";
            repVerAlumnoResultadoBE.GradoDeInstruccion = String.Concat(repVerAlumnoResultadoBE.GradoDeInstruccion, " de primaria");

            if (Session["CorreoResultado"] != null)
            {
                Session.Remove("CorreoResultado");
            }
            Session.Add("CorreoResultado", repVerAlumnoResultadoBE);

            return PartialView("_EnviarCorreoAlumno", repVerAlumnoResultadoBE);
        }

        [HttpGet]
        [Obsolete]
        public ActionResult EnviarCorreo(string p_correo)
        {
            RepVerAlumnoResultadoBE repVerAlumno = (RepVerAlumnoResultadoBE)Session["CorreoResultado"];

            if (repVerAlumno.Resultado == 0)
            {
                repVerAlumno.SugerenciaCursos = repositoryReportes.ListarSugerenciaCursos();
                repVerAlumno.Sugerencias = repositoryReportes.ListarSugerenciaHabitos();
            }
            string mensaje = "";
            try
            {
                if (p_correo == "")
                {
                    mensaje = "Debe ingresar un correo";
                }
                else if (!ValidarEmail(p_correo))
                {
                    p_correo = "El correo ingresado no tiene un formato válido";
                }

                if (mensaje == "")
                {
                    bool resultado = EnviaCorreo(p_correo, repVerAlumno);
                    mensaje = resultado ? "Se envió el correo exitosamente." : "Ocurrió un error al enviar el correo";
                }

                return new JsonResult { Data = new { message = mensaje }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
                return Json(new { IsValid = false, ErrorMessage = mensaje }, JsonRequestBehavior.AllowGet);
            }
        }

        [Obsolete]
        private static string GenerateHtml(RepVerAlumnoResultadoBE data)
        {
            try
            {
                if (data.Resultado == 0)
                {
                    var templateFilePath = Path.Combine(TemplateFolderPath, "EmailSugerencias_v1.0.chtml");
                    var templateService = new TemplateService();
                    var emailHtmlBody = templateService.Parse(System.IO.File.ReadAllText(templateFilePath), data, null, null);
                    return emailHtmlBody;
                }
                else
                {
                    var templateFilePath = Path.Combine(TemplateFolderPath, "EmailSugerencias_v2.0.chtml");
                    var templateService = new TemplateService();
                    var emailHtmlBody = templateService.Parse(System.IO.File.ReadAllText(templateFilePath), data, null, null);
                    return emailHtmlBody;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        [Obsolete]
        private bool EnviaCorreo(string EmailDestino, RepVerAlumnoResultadoBE obj)
        {
            bool blSalida = false;
            string EmailOrigen = usuarioSMTP;
            string Contraseña = claveSMTP;
            string sBody = GenerateHtml(obj);

            using (MailMessage oMailMessage = new MailMessage(EmailOrigen, EmailDestino, "MPRALM - Resultados del Alumno", sBody))
            {
                oMailMessage.IsBodyHtml = true;

                using (SmtpClient oSmtpClient = new SmtpClient("smtp.office365.com"))
                {
                    oSmtpClient.EnableSsl = true;
                    oSmtpClient.UseDefaultCredentials = false;
                    oSmtpClient.Port = 587;
                    oSmtpClient.Credentials = new System.Net.NetworkCredential(EmailOrigen, Contraseña);

                    try
                    {
                        oSmtpClient.Send(oMailMessage);
                        blSalida = true;
                    }
                    catch (System.Net.Mail.SmtpException)
                    {
                        throw;
                    }

                    oSmtpClient.Dispose();
                }
            }

            return blSalida;
        }

        bool ValidarEmail(string email)
        {
            bool bRespuesta = false;
            String expresion;
            expresion = @"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?";
            if (Regex.IsMatch(email, expresion))
            {
                bRespuesta = true;
            }

            return bRespuesta;
        }
    }
}