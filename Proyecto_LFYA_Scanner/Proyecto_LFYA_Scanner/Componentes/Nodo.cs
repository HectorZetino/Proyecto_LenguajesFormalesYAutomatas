using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_LFYA_Scanner.Componentes
{
    class Nodo
    {
        public string DatosContinuos { get; set; }
        public string ElementosContenidos { get; set; }
        public string AsignacionFirst { get; set; }
        public string AsignacionLast { get; set; }
        public bool IsNull { get; set; }
        public Nodo Data1 { get; set; }
        public Nodo Data2 { get; set; }

        public Nodo(string _DatosContinuos, string _ElementosContenidos, string _AsignacionFirst, string _AsignacionLast, bool _IsNull)
        {
            DatosContinuos = _DatosContinuos;
            ElementosContenidos = _ElementosContenidos;
            AsignacionFirst = _AsignacionFirst;
            AsignacionLast = _AsignacionLast;
            IsNull = _IsNull;
            Data1 = null;
            Data2 = null;
        }


    }
}
