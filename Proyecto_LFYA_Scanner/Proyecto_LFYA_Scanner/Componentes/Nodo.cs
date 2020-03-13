using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_LFYA_Scanner.Componentes
{
    class Nodo
    {
        public string ExpresionObtenida { get; set; }
        public string ElementosContenidos { get; set; }
        public string AsignacionFirst { get; set; }
        public string AsignacionLast { get; set; }
        public bool IsNull { get; set; }
        public Nodo c1 { get; set; }
        public Nodo c2 { get; set; }

        public Nodo(string _ExpresionObtenida, string _ElementosContenidos, string _AsignacionFirst, string _AsignacionLast, bool _IsNull)
        {
            ExpresionObtenida = _ExpresionObtenida;
            ElementosContenidos = _ElementosContenidos;
            AsignacionFirst = _AsignacionFirst;
            AsignacionLast = _AsignacionLast;
            IsNull = _IsNull;
            c1 = null;
            c2 = null;
        }

        public void SetExpresionObtenida(string _ExpresionObtenida){
            this.ExpresionObtenida = _ExpresionObtenida;
        }
        public void setElementosContenidos(string _ElementosContenidos) {
            this.ElementosContenidos = _ElementosContenidos;
        }

        public void setAsignacionFirst(string _AsignacionFirst) {
            this.AsignacionFirst = _AsignacionFirst;
        }
        public void setAsignacionLast(string _AsignacionLast) {
            this.AsignacionLast = _AsignacionLast;
        }
        public void setIsNull(bool _IsNull) {
            this.IsNull = _IsNull;
        }
        public void setc1(Nodo _c1) {
            this.c1 = _c1;
        }
        public void setc2(Nodo _c2) {
            this.c2 = _c2;
        }
        public string getExpresionObtenida() {
            return this.ExpresionObtenida;
        }
        public string getElementosContenidos() {
            return this.ElementosContenidos;
        }
        public string getAsignacionFirst() {
            return this.AsignacionFirst;
        }
        public string getAsignacionLast() {
            return this.AsignacionLast;
        }
        public bool getIsNull() {
            return this.IsNull;
        }
        public Nodo getc1() {
            return this.c1;
        }
        public Nodo getc2() {
            return this.c2;
        }



        public Nodo()
        {
            ExpresionObtenida = "";
            ElementosContenidos = "";
            IsNull = false;
            AsignacionFirst = "";
            AsignacionLast = "";
            c1 = null;
            c2 = null;

        }




    }
}
