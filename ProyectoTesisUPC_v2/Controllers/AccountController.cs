using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using ProyectoTesisUPC_v2.Models;
using UPC.BusinessEntities;
using UPC.Repositories;
using UPC.DataAccess;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using ProyectoTesisUPC_v2.Permisos;
using System.Security.Cryptography;
using System.Text;
using System.Net.Mail;
using System.IO;
using UPC.Common.Seguridad;
using UPC.Common;

namespace ProyectoTesisUPC_v2.Controllers
{
    public class AccountController : Controller
    {
        string cadena = ConfigurationManager.AppSettings["USU.Cnx_DB"].ToString();
        string usuarioSMTP = ConfigurationManager.AppSettings["usuarioSMTP"].ToString();
        string claveSMTP = ConfigurationManager.AppSettings["claveSMTP"].ToString();

        RepositoryLogueo repositorioLogueo = new RepositoryLogueo();
        RepositoryUsuario repositorioUsuario = new RepositoryUsuario();
        Encriptacion encriptacion = new Encriptacion();

        public AccountController()
        {
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            UsuarioBE obj;
            string strMensaje = "";
            String clave = "";

            clave = EncriptarTexto(model.Password.Trim());

            obj = repositorioLogueo.f_login(model.Usuario, clave, out strMensaje);

            if (obj.id_usu != 0)
            {
                Session["Usuario"] = obj;
                Session["cod_rol"] = obj.cod_rol;
                Session["IdUsuario"] = obj.id_usu;
                Session["ApeUsuario"] = obj.ApeUsuario;
                Session["NomUsuario"] = obj.NomUsuario;
                Session["DniVendedor"] = obj.DNIUsuario;
                Session["Login"] = model.Usuario.Trim();

                string descripRol = repositorioUsuario.ObtenerRol(obj.cod_rol);
                Session["descripRol"] = descripRol;

                return RedirectToAction("Prediccion", "Prediccion");
            }
            else
            {
                ModelState.AddModelError("", strMensaje);
                return View(model);
            }
        }

        public ActionResult CerrarSesion()
        {
            Session["Usuario"] = null;
            Session["cod_rol"] = null;
            Session["DniVendedor"] = null;
            Session["IdUsuario"] = null;
            Session["ApeUsuario"] = "";
            Session["NomUsuario"] = "";
            Session["Login"] = null;
            Session["descripRol"] = null;

            Session.Abandon();

            return RedirectToAction("Login", "Account");
        }

        ////
        // GET: /Account/ForgotPassword
        [HttpGet]
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("ForgotPassword", model);
            }

            string MensajeSalida = string.Empty;
            string usuario = string.Empty;
            string clave = string.Empty;

            repositorioUsuario.RecuperarPassword(model.Email, out MensajeSalida, out clave, out usuario);

            if (MensajeSalida.Equals("OK"))
            {
                if (EnviaCorreo(model.Email, usuario, clave))
                {
                    MensajeSalida = "La clave ha sido enviada satisfactoriamente al correo.";
                }
                else
                {
                    MensajeSalida = "Ocurrió un error al enviar el correo.";
                }
            }
            else
            {
                ModelState.Clear();
            }

            ViewBag.mensaje = MensajeSalida;

            return View(model);
        }

        private bool EnviaCorreo(string EmailDestino, string usuario, string clave)
        {
            bool blSalida = false;
            string EmailOrigen = usuarioSMTP;
            string Contraseña = claveSMTP;
            string sBody = string.Empty;
            string file = "";

            file = AppDomain.CurrentDomain.BaseDirectory.ToString() + "Plantillas/EmailRecuperaClave_v1.0.html";

            GeneraCuerpoCorreoWeb(clave, usuario, file, ref sBody);

            MailMessage oMailMessage = new MailMessage(EmailOrigen, EmailDestino, "Recuperación de contraseña", sBody);

            oMailMessage.IsBodyHtml = true;

            SmtpClient oSmtpClient = new SmtpClient("smtp.office365.com");
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

            return blSalida;
        }

        public void GeneraCuerpoCorreoWeb(string clave, string usuario, string file, ref string sBody)
        {
            try
            {
                StreamReader sr = new StreamReader(file);
                sBody = sr.ReadToEnd();
                sBody = sBody.Replace("@Clave", clave);
                sBody = sBody.Replace("@Usuario", usuario);
                sr.Close();
            }
            catch (Exception) { throw; }
        }

        private string EncriptarTexto(string strText)
        {
            try
            {
                return encriptacion.Encrypt(strText, "&%#@?,:*");
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
    }
}