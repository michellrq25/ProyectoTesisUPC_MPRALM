using OfficeOpenXml;
using ProyectoTesisUPC_v2.Models;
using ProyectoTesisUPC_v2.Permisos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UPC.BusinessEntities;
using UPC.Repositories;
using UPC.DataAccess;
using System.Windows.Media.Animation;

namespace ProyectoTesisUPC_v2.Controllers
{
    [SessionAuthorize]
    public class PrediccionController : Controller
    {
        readonly RepositoryPrediccion repositoryPrediccion = new RepositoryPrediccion();

        // GET: Prediccion
        public ActionResult Prediccion()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Prediccion(PrediccionModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Prediccion", model);
            }

            byte[] fileData = null;
            var lista = new List<PrediccionBE>();

            using (var binary = new BinaryReader(Request.Files[0].InputStream))
            {
                fileData = binary.ReadBytes(Request.Files[0].ContentLength);
            }

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var ms = new MemoryStream(fileData))
            using (var package = new ExcelPackage(ms))
            {
                var ws = package.Workbook.Worksheets["Hoja1"];

                if (ws != null)
                {
                    if (ws.Dimension == null)
                    {
                        ViewBag.Message = "La Hoja1 no cuenta con datos.";
                        return View(model);
                    }

                    var contador = 0;

                    if (ws.Cells[1, 1].Text == "DNI") contador++;
                    if (ws.Cells[1, 2].Text == "APELLIDOS Y NOMBRES") contador++;
                    if (ws.Cells[1, 3].Text == "SEXO") contador++;
                    if (ws.Cells[1, 4].Text == "Personal Social") contador++;
                    if (ws.Cells[1, 5].Text == "Educación Religiosa") contador++;
                    if (ws.Cells[1, 6].Text == "Educación Física") contador++;
                    if (ws.Cells[1, 7].Text == "Comunicación") contador++;
                    if (ws.Cells[1, 8].Text == "Arte y Cultura") contador++;
                    if (ws.Cells[1, 9].Text == "Inglés") contador++;
                    if (ws.Cells[1, 10].Text == "Matemática") contador++;
                    if (ws.Cells[1, 11].Text == "Ciencia y Tecnología") contador++;
                    if (ws.Cells[1, 12].Text == "Inasistencias") contador++;
                    if (ws.Cells[1, 13].Text == "Horas de dedicación fuera del colegio (semanal)") contador++;
                    if (ws.Cells[1, 14].Text == "Horas de dormir diaria") contador++;
                    if (ws.Cells[1, 15].Text == "Grado de Instrucción") contador++;
                    if (ws.Cells[1, 16].Text == "Distrito de Residencia") contador++;

                    if (contador != 16)
                    {
                        ViewBag.Message = "La Hoja1 no cuenta con el formato correcto de columnas.";
                        return View(model);
                    }

                    var rowCount = ws.Dimension.End.Row;

                    contador = 0;
                    var listaColumnas = new List<string>();
                    var listaSumary = new List<string>();

                    for (int row = 2; row <= rowCount; row++)
                    {
                        if (ws.Cells[row, 1].Value == null) { contador++; listaColumnas.Add(ws.Cells[1, 1].Text); }
                        if (ws.Cells[row, 2].Value == null) { contador++; listaColumnas.Add(ws.Cells[1, 2].Text); }
                        if (ws.Cells[row, 3].Value == null) { contador++; listaColumnas.Add(ws.Cells[1, 3].Text); }
                        if (ws.Cells[row, 4].Value == null) { contador++; listaColumnas.Add(ws.Cells[1, 4].Text); }
                        if (ws.Cells[row, 5].Value == null) { contador++; listaColumnas.Add(ws.Cells[1, 5].Text); }
                        if (ws.Cells[row, 6].Value == null) { contador++; listaColumnas.Add(ws.Cells[1, 6].Text); }
                        if (ws.Cells[row, 7].Value == null) { contador++; listaColumnas.Add(ws.Cells[1, 7].Text); }
                        if (ws.Cells[row, 8].Value == null) { contador++; listaColumnas.Add(ws.Cells[1, 8].Text); }
                        if (ws.Cells[row, 9].Value == null) { contador++; listaColumnas.Add(ws.Cells[1, 9].Text); }
                        if (ws.Cells[row, 10].Value == null) { contador++; listaColumnas.Add(ws.Cells[1, 10].Text); }
                        if (ws.Cells[row, 11].Value == null) { contador++; listaColumnas.Add(ws.Cells[1, 11].Text); }
                        if (ws.Cells[row, 12].Value == null) { contador++; listaColumnas.Add(ws.Cells[1, 12].Text); }
                        if (ws.Cells[row, 13].Value == null) { contador++; listaColumnas.Add(ws.Cells[1, 13].Text); }
                        if (ws.Cells[row, 14].Value == null) { contador++; listaColumnas.Add(ws.Cells[1, 14].Text); }
                        if (ws.Cells[row, 15].Value == null) { contador++; listaColumnas.Add(ws.Cells[1, 15].Text); }
                        if (ws.Cells[row, 16].Value == null) { contador++; listaColumnas.Add(ws.Cells[1, 16].Text); }

                        if (contador > 0)
                        {
                            listaSumary.Add($"La fila {row}, la(s) siguiente(s) columna(s) '{string.Join(", ", listaColumnas)}' no cuentan con datos. <br />");
                        }
                        contador = 0;
                        listaColumnas.Clear();
                    }

                    if (listaSumary.Any())
                    {
                        ViewBag.Message = $"Se encontraron los siguientes errores en el archivo: <br /> {string.Join("", listaSumary)}";
                        return View(model);
                    }

                    contador = 0;
                    listaColumnas.Clear();

                    for (int row = 2; row <= rowCount; row++)
                    {
                        if (!(ws.Cells[row, 12].Value.ToString()).All(char.IsDigit)) { contador++; listaColumnas.Add(ws.Cells[1, 12].Text); }
                        if (!(ws.Cells[row, 13].Value.ToString()).All(char.IsDigit)) { contador++; listaColumnas.Add(ws.Cells[1, 13].Text); }
                        if (!(ws.Cells[row, 14].Value.ToString()).All(char.IsDigit)) { contador++; listaColumnas.Add(ws.Cells[1, 14].Text); }

                        if (contador > 0)
                        {
                            listaSumary.Add($"La fila {row}, la(s) siguiente(s) columna(s) '{string.Join(", ", listaColumnas)}' no cuentan con valores numéricos. <br />");
                        }
                        contador = 0;
                        listaColumnas.Clear();
                    }

                    if (listaSumary.Any())
                    {
                        ViewBag.Message = $"Se encontraron los siguientes errores en el archivo: <br /> {string.Join("", listaSumary)}";
                        return View(model);
                    }

                    for (int row = 2; row <= rowCount; row++)
                    {
                        if (ws.Cells[row, 2].Value != null)
                        {
                            var obj = new PrediccionBE
                            {
                                DNI = ws.Cells[row, 1].Value.ToString().Trim(),
                                ApellidosNombres = ws.Cells[row, 2].Value.ToString().Trim(),
                                Sexo = ws.Cells[row, 3].Value.ToString().Trim(),
                                Curso = new CursoBE()
                                {
                                    PersonalSocial = ws.Cells[row, 4].Value.ToString().Trim(),
                                    EducacionReligiosa = ws.Cells[row, 5].Value.ToString().Trim(),
                                    EducacionFisica = ws.Cells[row, 6].Value.ToString().Trim(),
                                    Comunicacion = ws.Cells[row, 7].Value.ToString().Trim(),
                                    ArteYCultura = ws.Cells[row, 8].Value.ToString().Trim(),
                                    Ingles = ws.Cells[row, 9].Value.ToString().Trim(),
                                    Matematica = ws.Cells[row, 10].Value.ToString().Trim(),
                                    CienciaYTecnologia = ws.Cells[row, 11].Value.ToString().Trim()
                                },
                                Inasistencias = Convert.ToInt32(ws.Cells[row, 12].Value),
                                HorasDedicacion = Convert.ToInt32(ws.Cells[row, 13].Value),
                                HorasDormir = Convert.ToInt32(ws.Cells[row, 14].Value),
                                GradoDeInstruccion = ws.Cells[row, 15].Value.ToString().Trim(),
                                DistritoDeResidencia = ws.Cells[row, 16].Value.ToString().Trim()
                            };
                            lista.Add(obj);
                        }
                    }

                    if (lista.Any())
                    {
                        int flag = repositoryPrediccion.PostResultadoPredAgregar(lista);

                        if (flag > 0)
                        {
                            ViewBag.Message = "Se realizó la predicción exitosamente.";
                        }
                        else
                        {
                            ViewBag.Message = "Ocurrió un error al ejecutar la predicción.";
                        }

                    }
                }
                else
                {
                    ViewBag.Message = "El archivo no cuenta el nombre correcto de la pestaña: 'Hoja1'.";
                }
            }

            return View(model);
        }
    }
}