using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_LFYA_Scanner.Componentes
{
    class Set
    {
        public Set()
        {
            NombreSetData = "";
            ElementosSetData = new List<string>();
        }

        public string NombreSetData { get; set; }
        public List<string> ElementosSetData { get; set; }

        public void setNombreSetData(string _NombreSetData) {
            this.NombreSetData = _NombreSetData;
        }
        public void setElementoSetData(List<string> _ElementoSetData) {
            this.ElementosSetData = _ElementoSetData;
        }
        public string getNombreSetData() {
            return this.NombreSetData;
        }
        public List<string> getElementosSetData() {
            return this.ElementosSetData;
        }

    }
}
