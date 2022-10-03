using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using UPC.BusinessEntities;
using UPC.DataAccess;
using ProyectoTesisUPC_v2.Permisos;
using System.Text.RegularExpressions;
using System.Configuration;
using System.Text;
using System.Net.Mail;
using System.IO;
using UPC.Common.Seguridad;
using UPC.Common;
using System.Globalization;

namespace ProyectoTesisUPC_v2.Controllers
{
    [SessionAuthorize]
    public class UsuariosController : Controller
    {
        readonly RepositoryUsuario repositoryUsuario = new RepositoryUsuario();
        private readonly string usuarioSMTP = ConfigurationManager.AppSettings["usuarioSMTP"].ToString();
        private readonly string claveSMTP = ConfigurationManager.AppSettings["claveSMTP"].ToString();
        readonly Encriptacion encriptacion = new Encriptacion();

        // GET: Usuario
        public ActionResult Index()
        {
            return View();
        }
        private List<SelectListItem> ListaEstadosResultados()
        {
            try
            {

                List<SelectListItem> listaEstados = new List<SelectListItem>
                {
                    new SelectListItem { Value = "999", Text = "--Todos--" },
                    new SelectListItem { Value = "0", Text = "Inactivo" },
                    new SelectListItem { Value = "1", Text = "Activo" }
                };
                return listaEstados.ToList().OrderByDescending(o => o.Text).ToList();
            }
            catch (Exception ex) { throw ex; }
        }

        private List<SelectListItem> ListaRoles()
        {
            try
            {
                string cod_rol = (string)Session["cod_rol"];
                List<RolBE> listaRoles = repositoryUsuario.ListarRoles();
                listaRoles.Add(new RolBE { CodigoRol = "999", DescripcionRol = "--Todos--" });
                if (cod_rol != "SA")
                {
                    var itemToRemove = listaRoles.Single(r => r.CodigoRol == "SA");
                    listaRoles.Remove(itemToRemove);
                }
                return (from tab in listaRoles select new SelectListItem { Text = tab.DescripcionRol, Value = tab.CodigoRol.ToString() }).ToList().OrderByDescending(o => o.Text).ToList();
            }
            catch (Exception ex) { throw ex; }
        }

        private List<SelectListItem> ListaSexos()
        {
            try
            {

                List<SelectListItem> listaSexos = new List<SelectListItem>
                {
                    new SelectListItem { Value = "F", Text = "Femenino" },
                    new SelectListItem { Value = "M", Text = "Masculino" }
                };
                return listaSexos.ToList().OrderByDescending(o => o.Text).ToList();
            }
            catch (Exception ex) { throw ex; }
        }

        private string ReemplazarCaracteresDNI(string dni)
        {
            string cadena = dni;
            string caracteres = cadena.Substring(0, 3);
            string firstcadena = caracteres.Replace(caracteres, "****");
            string lastCadena = cadena.Substring(4, 4);
            string newCadena = string.Concat(firstcadena, lastCadena);

            return newCadena;
        }

        private string ParsearFechaToString(DateTime fecha)
        {
            return fecha.ToString("dd/MM/yyyy");
        }

        [ValidarRol]
        public ActionResult ListarUsuarios()
        {
            var listaEstados = ListaEstadosResultados();
            var listaRolesUsuarios = ListaRoles();
            ViewBag.ddlEstado = listaEstados;
            ViewBag.ddlRoles = listaRolesUsuarios;
            return View();
        }

        [HttpGet]
        public ActionResult ListarUsuariosPrincipal(string login, string dniUsuario, string rol, string estado)
        {
            try
            {
                int contador = 0;
                List<UsuarioBE> objListaUsuarioGeneral;
                List<UsuarioBE> objListaUsuarioGeneralFinal = new List<UsuarioBE>();
                objListaUsuarioGeneral = repositoryUsuario.ListarUsuariosPorFiltro(login, dniUsuario, rol, estado);
                foreach (UsuarioBE entity in objListaUsuarioGeneral)
                {
                    if (objListaUsuarioGeneralFinal.Where(f => f.login == entity.login).ToList().Count == 0)
                    {
                        if (contador == 400)
                        {
                            break;
                        }
                        else
                        {
                            contador++;
                        }
                        entity.DNIUsuario = ReemplazarCaracteresDNI(entity.DNIUsuario);
                        objListaUsuarioGeneralFinal.Add(entity);
                    }
                }
                ViewBag.CantidadRegistrosFiltrados = objListaUsuarioGeneralFinal.Count;
                return PartialView("_ListarUsuariosPrincipal", objListaUsuarioGeneralFinal);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        [HttpGet]
        public ActionResult EditarUsuario(int idUsuario)
        {
            try
            {
                UsuarioBE objUsuario = new UsuarioBE();

                var listarRoles = ListaRoles();
                var itemToRemove = listarRoles.Single(r => r.Value == "999");
                listarRoles.Remove(itemToRemove);
                var listarEstados = ListaEstadosResultados();
                var listarSexos = ListaSexos();

                if (idUsuario > 0)
                {
                    objUsuario = repositoryUsuario.CargaUsuarios_ID(idUsuario);
                    Session["loginActual"] = objUsuario.login;
                    Session["dniActual"] = objUsuario.DNIUsuario;
                }
                else
                {
                    objUsuario.login = "";
                    objUsuario.ApeUsuario = "";
                    objUsuario.NomUsuario = "";
                    objUsuario.DNIUsuario = "";
                    objUsuario.e_mail = "";
                    objUsuario.cod_rol = "";
                }

                ViewBag.ddlSexo = listarSexos;
                ViewBag.ddlEstado = listarEstados;
                ViewBag.ddlRoles = listarRoles;

                return PartialView("_EditUsuario", objUsuario);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        [HttpPost]
        public ActionResult Save(string id_usu, string cod_rol, string login,
                                 string dniusuario, string ApeUsuario, string NomUsuario,
                                 string cod_sexo, string Estado, string email)
        {
            UsuarioBE c = new UsuarioBE
            {
                id_usu = Convert.ToInt32(id_usu),
                cod_rol = cod_rol,
                login = login,
                DNIUsuario = dniusuario,
                ApeUsuario = ApeUsuario,
                NomUsuario = NomUsuario,
                cod_sexo = cod_sexo,
                Estado = Estado
            };

            try
            {
                bool status = false;
                string MensajeB = "";
                string MensajeError = "";
                String loginExistente = "";
                String dniExistente = "";

                if (email == "")
                {
                    MensajeB = "Debe ingresar un correo";
                }
                else if (!ValidarEmail(email))
                {
                    MensajeB = "El correo ingresado no tiene un formato válido";
                }
                else
                {
                    MensajeB = repositoryUsuario.CorreoValido(Convert.ToInt32(id_usu), email);
                }

                if (ModelState.IsValid && MensajeB == "")
                {
                    if (c.id_usu <= 0)
                    {
                        //tipo = "Registro de Usuario";

                        if (repositoryUsuario.ExisteLogin(c.login))
                        {
                            MensajeB = "El login ingresado ya existe.";
                            loginExistente = login;
                        }

                        else if (repositoryUsuario.ExisteDNI(c.DNIUsuario))
                        {
                            MensajeB = "El DNI ingresado ya existe.";
                            dniExistente = dniusuario;
                        }

                        else
                        {
                            string claveDefault = GenerarClave();

                            int intUsuario = repositoryUsuario.RegistraUsuario(c.cod_rol, c.login, EncriptarTexto(claveDefault), c.DNIUsuario, c.ApeUsuario, c.NomUsuario,
                            c.cod_sexo, Convert.ToInt32(c.Estado), email, ref MensajeError);
                            if (MensajeError == "")
                            {
                                if (intUsuario == 0)
                                {
                                    MensajeB = "El Login de Usuario ingresado ya existe .";
                                }
                                else
                                {
                                    MensajeB = "El Usuario se ha registrado correctamente.";
                                    status = true;

                                    if (EnviaCorreo(email, c.NomUsuario + " " + c.ApeUsuario, c.login, claveDefault))
                                    {
                                        MensajeB = "Usuario creado, la clave fue enviada al correo del usuario.";
                                    }
                                    else
                                    {
                                        MensajeB = "Usuario creado, pero ocurrió un error al enviar el correo";
                                    }
                                }
                            }
                        }

                    }
                    else
                    {
                        //tipo = "Actualización de usuario";

                        if (Session["loginActual"].Equals(c.login))
                        {
                            if (Session["dniActual"].Equals(c.DNIUsuario))
                            {
                                repositoryUsuario.SW_SP_ModificaUsuario(c.cod_rol, c.login,
                                c.DNIUsuario, c.ApeUsuario, c.NomUsuario, c.cod_sexo, Convert.ToInt16(c.Estado),
                                c.id_usu, email, ref MensajeError);

                                if (MensajeError == "")
                                {
                                    MensajeB = "El Usuario se ha actualizado correctamente.";
                                    status = true;
                                }
                            }
                            else
                            {
                                MensajeB = "El DNI del Usuario ingresado no se puede modificar.";
                            }
                        }
                        else
                        {
                            MensajeB = "El Login del Usuario ingresado no se puede modificar.";
                        }
                    }
                }

                return new JsonResult { Data = new { message = MensajeB, status, loginExistente, dniExistente }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        [HttpPost]
        public ActionResult HabilitarInhabilitarEliminar(int tipo, int idUsuario)
        {
            string MensajeB = "";

            string login = "";
            bool respuesta = true;
            try
            {
                if (tipo == 1)
                {
                    MensajeB = repositoryUsuario.CorreoValido(idUsuario, "");
                }

                if (MensajeB == "")
                {
                    if (tipo == 1)
                    {
                        MensajeB = "El usuario se habilitó correctamente.";
                    }
                    else if (tipo == 0)
                    {
                        MensajeB = "El usuario se inhabilitó correctamente.";
                    }

                    login = repositoryUsuario.HabilitarInhabilitarEliminar(tipo, idUsuario);
                    respuesta = login != "";
                }
                return new JsonResult { Data = new { valida = respuesta, mensaje = MensajeB, login }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            catch (Exception ex)
            {
                return new JsonResult { Data = new { valida = false, mensaje = ex.Message, login = "" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }

        private string EncriptarTexto(string strText)
        {
            try
            {
                return encriptacion.Encrypt(strText, "&%#@?,:*");
            }
            catch (Exception ex) { throw new Exception(ex.Message, ex); }

        }

        private string DesencriptarTexto(string strText)
        {
            try
            {
                return encriptacion.Decrypt(strText, "&%#@?,:*");
            }
            catch (Exception ex) { throw new Exception(ex.Message, ex); }
        }

        private string GenerarClave()
        {
            Random rdn = new Random();
            string valores = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            string valoresEspeciales = "#$%&/?";
            int longitud = valores.Length;
            int longitud2 = valoresEspeciales.Length;
            char letra;
            int longitudclave = 10;

            int pEspecial = rdn.Next(longitudclave);

            StringBuilder clave = new StringBuilder();
            for (int i = 0; i < longitudclave; i++)
            {
                if (i == pEspecial)
                {
                    letra = valoresEspeciales[rdn.Next(longitud2)];
                }
                else
                {
                    letra = valores[rdn.Next(longitud)];
                }
                clave.Append(letra.ToString());
            }

            return clave.ToString();
        }

        private bool EnviaCorreo(string EmailDestino, string usuario, string login, string clave)
        {
            bool blSalida = false;
            string EmailOrigen = usuarioSMTP;
            string Contraseña = claveSMTP;
            string sBody = string.Empty;
            string file = AppDomain.CurrentDomain.BaseDirectory.ToString() + "Plantillas/EmailClave_v1.0.html";

            GeneraCuerpoCorreoWeb(clave, usuario, login, file, ref sBody);

            using (MailMessage oMailMessage = new MailMessage(EmailOrigen, EmailDestino, "MPRALM - Credenciales Web", sBody))
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

        public void GeneraCuerpoCorreoWeb(string clave, string usuario, string login, string file, ref string sBody)
        {
            try
            {
                StreamReader sr = new StreamReader(file);
                sBody = sr.ReadToEnd();
                sBody = sBody.Replace("@Clave", clave);
                sBody = sBody.Replace("@Usuario", usuario);
                sBody = sBody.Replace("@login", login);
                sr.Close();
            }
            catch (Exception) { throw; }
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