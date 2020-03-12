using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_LFYA_Scanner.Archivo
{
    class LecturaArchivo
    {
        public string CapturarEspaciosFormaEspecial(string LineaLeida) { //Omite espacio no esten entre comillas simple
            char[] caracteres = LineaLeida.ToCharArray();
            bool A = false;
            string CadenaMenosEspacio = "";
            for (int i = 0; i < caracteres.Length; i++)
            {
                if (caracteres[i] == '\'' && A == false)
                {
                    A = true;
                }
                else if (caracteres[i] == '\'' && A == true)
                {
                    A = false;
                }

                try
                {
                    if (caracteres[i] == '\'' && caracteres[i+1] == '\'' && caracteres[i+2] == '\'')
                    {
                        CadenaMenosEspacio += Convert.ToString("'''");
                        i = i + 3;
                        A = false;
                    }
                }
                catch 
                {
                }

                if (A == false)
                {
                    if (caracteres[i] != ' ' && caracteres[i] != '\t')
                    {
                        CadenaMenosEspacio += Convert.ToString(caracteres[i]);
                    }
                }
                else
                {
                    CadenaMenosEspacio += Convert.ToString(caracteres[i]);
                }
            }
            return CadenaMenosEspacio;
        }
        public string CapturarEspacios(string LineaLeida) { //Quitar espacio en las cadenas
            char[] caracteres = LineaLeida.ToCharArray();
            string CadenaMenosEspacio = "";
            foreach (char Caracter in LineaLeida)
            {
                if (Caracter != '\t' && Caracter != ' ')
                {
                    CadenaMenosEspacio += Convert.ToString(Caracter);
                }
            }
            return CadenaMenosEspacio;
        }
        private string CapturarTabs(string LineaLeida) { // Quitar los Tabs
            char[] caracteres = LineaLeida.ToCharArray();
            string CadenaMenosTab = "";
            foreach (char Caracter in LineaLeida)
            {
                if (Caracter != '\t')
                {
                    CadenaMenosTab += Convert.ToString(Caracter);
                }
            }
            return CadenaMenosTab;
        }
    }
}
