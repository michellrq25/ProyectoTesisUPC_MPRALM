using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UPC.BusinessEntities
{
    public class UsuarioBE
    {
        public virtual string cod_posicion
        {
            get;
            set;
        }
        public virtual int cod_TipoPosicion
        {
            get;
            set;
        }
        public virtual System.DateTime fec_crea
        {
            get;
            set;
        }
        public virtual System.DateTime fec_mod
        {
            get;
            set;
        }
        public virtual string flag_activo
        {
            get;
            set;
        }
        [Display(Name = "Documento")]
        public virtual string DNIUsuario
        {
            get;
            set;
        }
        public virtual string ApeUsuario
        {
            get;
            set;
        }
        public virtual string sel
        {
            get;
            set;
        }
        [Display(Name = "Descripción Admnistrador")]
        public virtual string vc_DescripcionAdministrador
        {
            get;
            set;
        }

        public virtual EstadoPersonaBE EstadoPersonaBE
        {
            get;
            set;
        }
        public virtual RolBE RolBE
        {
            get;
            set;
        }
        public virtual SexoBE SexoBE
        {
            get;
            set;
        }
        public virtual RolBE rolBE
        {
            get;
            set;
        }
        public virtual string DescripPositicion { get; set; }
        public UsuarioBE()
        {

        }
        public virtual int intermed { get; set; }
        public virtual String valor { get; set; }
        public virtual String descript { get; set; }
        public virtual String e_mail { get; set; }
        public string CodigoCip { get; set; }
        public string Nombres { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public Int32 id_usu { get; set; }
        public string cod_rol { get; set; }
        [Display(Name = "Logín")]
        public string login { get; set; }
        public string password { get; set; }
        public string confirmacion { get; set; }
        public string sucursal { get; set; }
        [Display(Name = "Nombre")]
        public string NomUsuario { get; set; }
        public string sexo { get; set; }
        public string celular { get; set; }
        public string agent
        {
            get;
            set;
        }
        public string cod_sexo
        {
            get;
            set;
        }
        public string cod_Posicion
        {
            get;
            set;
        }
        public string Estado { get; set; }
    }
}
