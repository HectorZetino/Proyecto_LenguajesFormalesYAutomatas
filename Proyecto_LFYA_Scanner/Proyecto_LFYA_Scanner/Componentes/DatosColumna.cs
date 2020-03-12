using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_LFYA_Scanner.Componentes
{
    class DatosColumna
    {
        public DatosColumna()
        {

        }
        public int id_columna { get; set; }
        public string NombreC { get; set; }
        public List<int> ElementosC { get; set; }

        public void setid_columna(int _id_columna) {
            id_columna = _id_columna;
        }
        public void setNombreC(string _NombreC) {
            NombreC = _NombreC;
        }
        public void setElementosC(List<int> _ElementosC) {
            ElementosC = _ElementosC;
        }
        public int getid_columna() {
            return this.id_columna;
        }
        public string getNombreC() {
            return this.NombreC;
        }
        public List<int> getElementosC() {
            return this.ElementosC;
        }
            
    }
}
