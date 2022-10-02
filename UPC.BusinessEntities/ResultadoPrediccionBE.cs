using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UPC.BusinessEntities
{
    public class ResultadoPrediccionBE
    {
        public virtual int CodPrediccion
        {
            get;
            set;
        }

        public virtual string DNI
        {
            get;
            set;
        }

        public virtual string ApellidosNombres
        {
            get;
            set;
        }

        public virtual string Sexo
        {
            get;
            set;
        }

        public virtual CursoBE Curso
        {
            get;
            set;
        }

        public virtual int Inasistencias
        {
            get;
            set;
        }

        public virtual int HorasDedicacion
        {
            get;
            set;
        }

        public virtual int HorasDormir
        {
            get;
            set;
        }

        public virtual string GradoDeInstruccion
        {
            get;
            set;
        }

        public virtual string DistritoDeResidencia
        {
            get;
            set;
        }

        public virtual int Resultado
        {
            get;
            set;
        }
    }
}
