using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_LFYA_Scanner.Componentes
{
    class Token
    {
        public Token()
        {
            NumeroTokenData = 0;
            ElementoTokenData = "";
        }
        public string ElementoTokenData { get; set; }
        public int NumeroTokenData { get; set; }

        public void setElementoTokenData(string _ElementoTokenData) {
            this.ElementoTokenData = _ElementoTokenData;
        }
        public void setNumeroTokenData(int _NumeroTokenData) {
            this.NumeroTokenData = _NumeroTokenData;
        }
        public string getElementoTokenData() {
            return this.ElementoTokenData;
        }
        public int getNumeroTokenData() {
            return this.NumeroTokenData;
        }
    }
}
