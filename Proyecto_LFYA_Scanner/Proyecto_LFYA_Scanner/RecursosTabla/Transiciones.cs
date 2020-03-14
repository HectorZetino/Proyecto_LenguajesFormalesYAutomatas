using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_LFYA_Scanner.RecursosTabla
{
    class Transiciones
    {
        public string id_Transicion { get; set; }
        public List<string> ElementosTransicion { get; set; }

        public Transiciones(string _id_Transicion, List<string> _ElementosTransicion)
        {
            id_Transicion = _id_Transicion;
            ElementosTransicion = _ElementosTransicion;
        }

        public Transiciones()
        {
            id_Transicion = "";
            ElementosTransicion = null;
        }

        public void setid_Transicion(string _id_Transicion) {
            this.id_Transicion = _id_Transicion;
        }
        public void setElementosTransicion(List<string> _ElementosTransicion) {
            if (this.ElementosTransicion != null)
            {
                foreach (var data in _ElementosTransicion)
                {
                    this.ElementosTransicion.Add(data);
                }
                this.ElementosTransicion.Sort();
            }
            else
            {
                this.ElementosTransicion = _ElementosTransicion;
            }
        }
        public string getid_Transicion() {
            return this.id_Transicion;
        }

        public List<string> getElementosTransicion() {
            return this.ElementosTransicion;
        }

        public string getCadenaDeElementosTransicion() {
            
            if (ElementosTransicion != null)
            {
                string Caracter = "(";
                for (int i = 0; i < ElementosTransicion.Count; i++)
                {
                    if (i != (ElementosTransicion.Count - 1))
                    {
                        Caracter += ElementosTransicion[i] + ",";
                    }
                    else
                    {
                        Caracter += ElementosTransicion[i];
                    }
                }
                Caracter += ")";
                return Caracter;
            }
            else
            {
                return "";
            }
        }
    }
}
