using Proyecto_LFYA_Scanner.Componentes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_LFYA_Scanner.Archivo
{
    class LecturaArchivo
    {
        public string CapturarCharsets(string LineaLeida, ref int CaracterLeido) { //Captura el numero del char que se ha encontrado
            char[] caracteres = LineaLeida.ToCharArray();
            string CharsetLine = "";
            while (caracteres[CaracterLeido] != ')')
            {
                CharsetLine += Convert.ToString(caracteres[CaracterLeido]);
                CaracterLeido++;
            }
            CaracterLeido++;
            return CharsetLine;
        
        }
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
                    if (caracteres[i] == '\'' && caracteres[i + 1] == '\'' && caracteres[i + 2] == '\'')
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
                if (Caracter != ' ' && Caracter != '\t')
                {
                    CadenaMenosEspacio += Convert.ToString(Caracter);
                }
            }
            return CadenaMenosEspacio;
        }
        public string CapturarTabs(string LineaLeida) { // Quitar los Tabs
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

        public bool VerificacionTokenID(string LineaLeida, ref int idToken, ref string errorAsignado) { // Verifica escritura y asignacion del id del token
            char[] caracteres = CapturarTabs(LineaLeida.ToUpper()).ToCharArray();
            string numero = "";
            if (caracteres[0] == 'T' && caracteres[1] == 'O' && caracteres[2] == 'K' && caracteres[3] == 'E' && caracteres[4] == 'N' && caracteres[5] == ' ')
            {
                for (int i = 6; i < caracteres.Length; i++)
                {
                    numero += caracteres[i];
                }
                if (int.TryParse(numero, out idToken) == true)
                {
                    return true;
                }
                else
                {
                    errorAsignado = "Corregir y/o Verificar el id del Token no se encuentra en un formato correcto";
                    return false; 
                }
            }
            else
            {
                errorAsignado = "Corregir y/o Verificar la palabra TOKEN no esta escrita correctamente";
                return false;
            }
        
        }

        public bool VerificacionOperador(char[] ConjuntoCaracteres, ref int Caracter, ref string errorAsignado) { // Valida que vengan asignados de forma correcta los operadores ?,*,+
            if (ConjuntoCaracteres[Caracter] == '?' || ConjuntoCaracteres[Caracter] == '*' || ConjuntoCaracteres[Caracter] == '+')
            {
                if (Caracter + 1 == ConjuntoCaracteres.Length)
                {
                    Caracter++;
                    return true;
                }
                if (ConjuntoCaracteres[Caracter + 1] != ' ')
                {
                    errorAsignado = "El Operador utilizado ( " + ConjuntoCaracteres[Caracter] + " ) colocado en un lugar no permitido";
                    return false;
                }
                try
                {
                    if (ConjuntoCaracteres[Caracter - 1] == '|' || ConjuntoCaracteres[Caracter -1] == '(')
                    {
                        errorAsignado = "Revisar, no se le puede asignar ningun tipo operador : " + ConjuntoCaracteres[Caracter - 1];
                        return false;
                    }
                }
                catch
                {
                    errorAsignado = "Revisar, No puede usar al inicio de la expresion : " + ConjuntoCaracteres[Caracter - 1];
                    return false;
                }
                try
                {
                    if (ConjuntoCaracteres[Caracter + 1] == '?' || ConjuntoCaracteres[Caracter + 1] == '*' || ConjuntoCaracteres[Caracter + 1] == '+' )
                    {
                        errorAsignado = "Revisar, No pueden mantenerse dos operadores (?,*,+) juntos ( " + ConjuntoCaracteres[Caracter] + " concatenado con : " + ConjuntoCaracteres[Caracter + 1] + " )";
                        return false;
                    }
                    else
                    {
                        Caracter++;
                        return true;
                    }
                }
                catch 
                {
                    Caracter++;
                    return true;
                }
            }
            return true;
        }
        public string ManejoExpresionGenerada(string LineaLeida) { //Metodo para colocar puntos de concatenacion en los lugares necesarios
            string DataF = "";
            LineaLeida = LineaLeida.TrimEnd(' ');
            char[] AllCaracteres = LineaLeida.ToCharArray();
            string ExpresionRGenerada = "";
            int Contador = 0;

            while (AllCaracteres[Contador] == ' ') //espacios <==
            {
                Contador++;
            }
            while (Contador != AllCaracteres.Length) // Interpretar las concatenaciones de las expresiones
            {
                if (AllCaracteres[Contador] != ' ')
                {
                    ExpresionRGenerada += Convert.ToString(AllCaracteres[Contador]);
                    Contador++;
                }
                else
                {
                    if (AllCaracteres[Contador - 1] == '(' || AllCaracteres[Contador + 1] == ')' || AllCaracteres[Contador + 1] == '|' || AllCaracteres[Contador - 1] == '|' || AllCaracteres[Contador + 1] == '*' || AllCaracteres[Contador + 1] == '+' || AllCaracteres[Contador + 1] == '?' )
                    {
                        Contador++;
                    }
                    else
                    {
                        if (!(AllCaracteres[Contador - 1] == ' '))
                        {
                            ExpresionRGenerada += ".";
                        }
                        Contador++;
                    }
                }
            }
            if (ExpresionRGenerada.Contains("''")) //Signo concatenacion entre comillas simples
            {
                char[] ExpresionSeparada = ExpresionRGenerada.ToCharArray();
                string ExpresionCompleta = Convert.ToString(ExpresionSeparada[0]);

                try
                {
                    for (int i = 1; i < (AllCaracteres.Length - 1) ; i++)
                    {
                        if (ExpresionSeparada[i - 1] != '\'' && ExpresionSeparada[i] == '\'' && ExpresionSeparada[i + 1] == '\'' && ExpresionSeparada[i + 2] != '\'' )
                        {
                            ExpresionCompleta += Convert.ToString(ExpresionSeparada[i] + ".");
                        }
                        else
                        {
                            ExpresionCompleta += Convert.ToString(ExpresionSeparada[i]);
                        }
                    }
                }
                catch 
                {

                }
                ExpresionCompleta += Convert.ToString(ExpresionSeparada[ExpresionSeparada.Length - 1]);
                Contador = 0; // obviamos las comillas simples
                char[] FragmentoExpresion = ExpresionCompleta.ToCharArray();
                while (Contador != FragmentoExpresion.Length)
                {
                    try
                    {
                        if (FragmentoExpresion[Contador] != '\'')
                        {
                            DataF += Convert.ToString(FragmentoExpresion[Contador]);
                        }
                        else if (FragmentoExpresion[Contador - 1] == '\'' && FragmentoExpresion[Contador] == '\'' && FragmentoExpresion[Contador + 1] == '\'') 
                        {
                            DataF += Convert.ToString('\'');
                        }
                    }
                    catch 
                    {

                    }
                    Contador++;
                }
                string DataExpReg = "";
                string[] SignoSeparador = DataF.Split('|');

                if (SignoSeparador.Length > 1)
                {
                    for (int i = 0; i < SignoSeparador.Length; i++)
                    {
                        if (i != SignoSeparador.Length -1)
                        {
                            DataExpReg += "(" + SignoSeparador[i] + ")|";
                        }
                        else
                        {
                            DataExpReg += "(" + SignoSeparador[i] + ")";
                        }
                    }
                    return DataExpReg;
                }
                else
                {
                    return DataF;
                }
            }
            else
            {
                Contador = 0; //regresamos el contador al valor inicial , aqui eliminamos comillas s
                char[] NFragmentoExpresion = ExpresionRGenerada.ToCharArray();
                while (Contador != NFragmentoExpresion.Length)
                {
                    try
                    {
                        if (NFragmentoExpresion[Contador] != '\'')
                        {
                            DataF += Convert.ToString(NFragmentoExpresion[Contador]);
                        }
                        else if (NFragmentoExpresion[Contador - 1] == '\'' && NFragmentoExpresion[Contador] == '\'' && NFragmentoExpresion[Contador + 1] == '\'')
                        {
                            DataF += Convert.ToString('\'');
                        }
                    }
                    catch
                    {

                    }
                    Contador++;
                }
                string DataExpReg = "";
                string[] SignoSeparador = DataF.Split('|');

                if (SignoSeparador.Length > 1)
                {
                    for (int i = 0; i < SignoSeparador.Length; i++)
                    {
                        int ParentesisApertura = 0;
                        int ParentesisCerrar = 0;
                        bool EsConcatenado = false;
                        char[] Particion = SignoSeparador[i].ToCharArray();
                        for (int j = 1; j < Particion.Length; j++)
                        {
                            if (Particion[j] == '(')
                            {
                                ParentesisApertura++;
                            }
                            else if (Particion[j] == ')')
                            {
                                ParentesisCerrar++;
                            }
                            else if (Particion[j] == '.')
                            {
                                EsConcatenado = true;
                            }
                        }
                        if (i != SignoSeparador.Length - 1)
                        {
                            if (EsConcatenado)
                            {
                                if (ParentesisApertura == ParentesisCerrar && ParentesisApertura != 0)
                                {
                                    DataExpReg += "(" + SignoSeparador[i] + ")|";
                                }
                                else
                                {
                                    DataExpReg += SignoSeparador[i] + "|";
                                }
                            }
                            else
                            {
                                DataExpReg += SignoSeparador[i] + "|";
                            }
                        }
                        else
                        {
                            if (EsConcatenado)
                            {
                                DataExpReg += "(" + SignoSeparador[i] + ")";
                            }
                            else
                            {
                                DataExpReg += SignoSeparador[i];
                            }
                        }
                    }
                    return ConjuncionDeOperadores(DataExpReg);
                }
                else
                {
                    return ConjuncionDeOperadores(DataF);
                }
            }

        }

        public string ConjuncionDeOperadores(string LineaLeida) { // cuando un operador se encuentre que cambia en algo la expresion, se colocara parentesis correspondiente a la ER
            char[] AllCarateres = LineaLeida.ToCharArray();
            bool EsUnParentesis = false;
            int ParentesisApertura = 0;
            int ParentesisCierre = 0;
            string ExpresionRGenerada = "";
            List<int> DataAOperadores = new List<int>();
            List<int> DataBOperadores = new List<int>();

            if (LineaLeida.Contains('*') || LineaLeida.Contains('+') || LineaLeida.Contains('?'))
            {
                for (int i = 0; i < AllCarateres.Length; i++)
                {
                    if (AllCarateres[i] == '*' || AllCarateres[i] == '+' || AllCarateres[i] == '?')
                    {
                        DataAOperadores.Add(i);
                        try
                        {
                            if (AllCarateres[i - 1] == ')')//para cuando con el apertura("(")
                            {
                                EsUnParentesis = true;
                            }

                            for (int j = DataAOperadores[DataAOperadores.Count - 1]; j >= 0 ; j--)
                            {
                                if (EsUnParentesis)
                                {//el punto donde debe abrir parentesis
                                    if (AllCarateres[j] == ')')
                                    {
                                        ParentesisCierre++;
                                    }
                                    else if (AllCarateres[j] == '(')
                                    {
                                        ParentesisApertura++;
                                    }

                                    if (AllCarateres[j] == '(' && ParentesisCierre == ParentesisApertura)
                                    {
                                        DataBOperadores.Add(j);
                                    }
                                }
                                else
                                {
                                    if (AllCarateres[j] == '.' || AllCarateres[j] == '|')
                                    {
                                        DataBOperadores.Add(j);
                                        break;
                                    }
                                }
                            }
                        }
                        catch 
                        {

                        }
                    }
                }
                if (DataAOperadores.Count != 0 && DataBOperadores.Count != 0)
                {
                    for (int i = 0; i < LineaLeida.Length; i++)
                    {
                        ExpresionRGenerada += AllCarateres[i];
                        if (DataAOperadores.Contains(i))
                        {
                            ExpresionRGenerada += ")";
                        }
                        if (DataBOperadores.Contains(i))
                        {
                            ExpresionRGenerada += "(";
                        }
                    }
                    return ExpresionRGenerada;
                }
                else
                {
                    return LineaLeida;
                }
            }
            else
            {
                return LineaLeida;
            }
        }

        public bool CapturaSets(List<string> txtCompleto, ref string errorAsignado, ref int LineaT, ref List<Set> ListaSets) {
            string CadenaSet = "";
            while (CapturarEspacios(txtCompleto[LineaT]).ToUpper() != "TOKENS")//validacion que unicamente se analice las lineas de los sets
            {
                while (CapturarEspacios(txtCompleto[LineaT]) == "")
                {
                    LineaT++;
                }
                if (CapturarEspacios(txtCompleto[LineaT]).ToUpper() == "TOKENS")
                {
                    break;
                }

                CadenaSet = CapturarEspaciosFormaEspecial(txtCompleto[LineaT]);
                string[] Particion = CadenaSet.Split('=');
                string LElementosSets = "";

                if (Particion.Length == 1)//si no contiene el '='
                {
                    errorAsignado = "No existe el signo (=) que hace valido el set";
                    return false;
                }
                for (int i = 1; i < Particion.Length; i++)//Exceso de signos '='
                {
                    LElementosSets += Particion[i];
                    if (Particion.Length > 2 && i != (Particion.Length - 1))
                    {
                        LElementosSets += "=";
                    }
                }

                Set SetProvicional = new Set();
                SetProvicional.setNombreSetData(Particion[0]);
                List<string> ElementosDelSet = new List<string>();
                if (CapturarElementosParaSets(LElementosSets, 0, ref errorAsignado, ref ElementosDelSet) == false)
                {
                    return false;
                }

                foreach (Set Sets in ListaSets)
                {
                    if (Sets.getNombreSetData() == SetProvicional.getNombreSetData())
                    {
                        errorAsignado = "Set no autorizado, identificador ingresado repetido ( " + SetProvicional.getNombreSetData() + " )";
                        return false;
                    }
                }
                SetProvicional.setElementoSetData(ElementosDelSet);
                ListaSets.Add(SetProvicional);
                LineaT++;
            }
            if (ListaSets.Count == 0)
            {
                errorAsignado = "No existe ningun SET en el segmento SETS para ingresar";
                return false;
            }
            return true;
        }

        public bool CapturarElementosParaSets(string LineaLeida, int Caracter, ref string errorAsignado , ref List<string> ElementosDelSet) {
            LineaLeida = LineaLeida.TrimEnd(' ');
            char[] Caracteres = LineaLeida.ToCharArray();
            string EnEsteMomento = "";
            string Final = "";
            bool ProcesarCaptura = true;
            

            if (Caracteres.Length == 0)//si no existe nada 
            {
                errorAsignado = "La linea se encuentra vacía";
                return false;
            }
            if (Caracteres[Caracter] == '+')//al encontrar un '+' se procede a leer
            {
                Caracter++;

                try
                {
                    if (Caracteres[Caracter + 1 ] == ' ')
                    {

                    }
                }
                catch 
                {
                    errorAsignado = "El Siguiente Signo '+' no cuenta con elementos consiguientes a el";
                    return false;
                }

            }

            if (Caracteres[Caracter] != '\'' && (Caracteres[Caracter] != 'C' && Caracteres[Caracter + 1] != 'H' && Caracteres[Caracter + 2] != 'R'))//validacion del 'CHR' en la parte de charsets
            {
                errorAsignado = "Caracter no permitido ( " + Caracteres[Caracter] + " )";
                return false;
            }

            if (Caracteres[Caracter] == '\'')
            {
                Caracter++;//verifica comillas simple
                try
                {
                    if (Caracteres[Caracter]== '\'')
                    {
                        try
                        {
                            if (Caracteres[Caracter + 1] != '\'')
                            {
                                errorAsignado = "El elemento se encuentra Vacio";
                                return false;
                            }
                            else
                            {
                                ElementosDelSet.Add(Convert.ToString('\''));

                                Caracter += 2;
                                if (Caracter == Caracteres.Length)
                                {
                                    return false;
                                }
                                else
                                {
                                    Caracter++;
                                }

                            }
                        }
                        catch 
                        {
                            errorAsignado = "El elemento se encuentra vacio";
                            return false;
                        }
                    }
                    else
                    {
                        Caracter--;
                    }
                }
                catch 
                {
                    Caracter--;
                }
            }
            if (Caracteres[Caracter] == 'C' && Caracteres[Caracter + 1] == 'H' && Caracteres[Caracter + 2] == 'R')
            {
                Caracter = Caracter + 4;
                EnEsteMomento = CapturarCharsets(LineaLeida, ref Caracter);

                try
                {//cadena corta
                    if (Caracteres[Caracter] == '.' && Caracteres[Caracter + 1] == '.' && Caracteres[Caracter + 2] == 'C' && Caracteres[Caracter + 3] == 'H' && Caracteres[Caracter + 4] == 'R')//analizador de limite si llegara al final
                    {
                        Caracter = Caracter + 6;
                        Final = CapturarCharsets(LineaLeida, ref Caracter);
                        for (int i = Convert.ToInt32(EnEsteMomento); i <= Convert.ToInt16(Final); i++)
                        {
                            ElementosDelSet.Add(Convert.ToString(Convert.ToChar(i)));
                        }
                    }
                    else if (Caracteres[Caracter] == '.' && Caracteres[Caracter + 1] != '.')
                    {
                        errorAsignado = "Simbolo no Aceptado y/o no reconocido ( " + Caracteres[Caracter] + " )";
                        return false;
                    }
                    else
                    {
                        ElementosDelSet.Add(Convert.ToString(Convert.ToChar(Convert.ToInt16(EnEsteMomento)))); // si no pues solo agrega el CHR nuevo
                    }
                }
                catch 
                {
                }
                if (Caracteres.Length == Caracter)
                {
                    return true;
                }
                if (Caracteres[Caracter] != '+')
                {
                    errorAsignado = "Simbolo no Aceptado y/o no reconocido ( " + Caracteres[Caracter] + " )";
                    return false;
                }
            }
            while (ProcesarCaptura)
            {
                if (Caracteres[Caracter] != '\'')//si no llega a encontrar algo como comillas lo almacena
                {
                    EnEsteMomento += Convert.ToString(Caracteres[Caracter]);
                }
                Caracter++;
                try
                {
                    if (Caracteres[Caracter] == '\'')
                    {
                        ProcesarCaptura = false;
                        Caracter++;
                    }
                }
                catch
                {
                    errorAsignado = "Falta el Simbolo de comilla simple (')";
                    return false;
                }
            }
            try
            {
                if (Caracteres[Caracter] == '+')
                {
                    ElementosDelSet.Add(EnEsteMomento);
                    try
                    {
                        if (CapturarElementosParaSets(LineaLeida,Caracter,ref errorAsignado, ref ElementosDelSet) == false)
                        {
                            return false;
                        }
                    }
                    catch 
                    {
                        errorAsignado = "El signo (+) no cuenta con elementos consiguientes";
                        return false;
                    }
                }
                else if (Caracteres[Caracter] == '.')
                {
                    try
                    {   
                        if (Caracteres[Caracter] == '.' && Caracteres[Caracter + 1] == '.')
                        {//verificacion de que no exista otro espacio dentro de la cadena
                            Caracter = Caracter + 2;
                            ProcesarCaptura = true;

                            
                            while (ProcesarCaptura)
                            {//se procesa si es el tope del rango
                                if (Caracteres[Caracter] != '\'')
                                {
                                    Final += Convert.ToString(Caracteres[Caracter]);
                                }
                                Caracter++;

                                try
                                {
                                    if (Caracteres[Caracter] == '\'')
                                    {// si llega a encotnrar una comilla simple no continua el proceso
                                        ProcesarCaptura = false;
                                        Caracter++;
                                    }
                                }
                                catch 
                                {
                                    errorAsignado = "Falta el Simbolo de comilla simple (')";
                                    return false;
                                }
                            }
                            for (int i = (int)Convert.ToChar(EnEsteMomento); i <= (int)Convert.ToChar(Final); i++)
                            {
                                ElementosDelSet.Add(Convert.ToString((char)i));
                            }
                            try//comprobacion que este dentro del rango!
                            {
                                if (Caracteres[Caracter] != '+')
                                {
                                    errorAsignado = "Simbolo no Aceptado y/o no reconocido ( " + Caracteres[Caracter] + " )";
                                    return false;
                                }
                                else
                                {
                                    if (CapturarElementosParaSets(LineaLeida, Caracter, ref errorAsignado, ref ElementosDelSet) == false)
                                    {
                                        return false;
                                    }
                                }
                            }
                            catch 
                            {

                            }
                        }
                        else if (Caracteres[Caracter] == '.' && Caracteres[Caracter + 1] != '.')
                        {
                            errorAsignado = "Simbolo no Aceptado y/o no reconocido ( " + Caracteres[Caracter] + " )";
                            return false;
                        }
                    }
                    catch 
                    {
                        errorAsignado = "Simbolo no Aceptado y/o no reconocido ( " + Caracteres[Caracter] + " )";
                        return false;
                    }
                }
                else
                {
                    errorAsignado = "El simbolo ingresado es invalido ( " + Caracteres[Caracter] + " )";
                    return false;
                }
            }
            catch
            {
                ElementosDelSet.Add(EnEsteMomento);
            }// si sobre pasa el indice se alamacena el ultimo digito 
            return true;
        }

        public bool CapturarTokens(List<string> txtCompleto, ref string errorAsignado, ref int LineaT, ref List<Token> ListaTokens,  List<Set> ListaSets) {
            string CadenaToken = "";
            while (CapturarEspacios(txtCompleto[LineaT]).ToUpper() != "ACTIONS")//siempre y cuando no este en la linea de actions
            {
                while (CapturarEspacios(txtCompleto[LineaT]) == "")
                {
                    LineaT++;
                }
                if (CapturarEspacios(txtCompleto[LineaT]).ToUpper() == "ACTIONS")
                {
                    break;
                }
                CadenaToken = txtCompleto[LineaT].Trim();
                string[] Particion = CadenaToken.Split('=');
                string LElementoToken = "";

                if (Particion.Length == 1)// Por si no existe ningun '='
                {
                    errorAsignado = "No existe el signo (=) que hace valido el Token";
                    return false;
                }
                for (int i = 1; i < Particion.Length; i++) // Por si existe mas de un signo "=", Se unen todos los elementos exceptuando el titulo  
                {
                    LElementoToken += Particion[i];
                    if (Particion.Length > 2 && i != (Particion.Length - 1))
                    {
                        LElementoToken += "=";
                    }
                }

                Token TokenProvicional = new Token();//token provicional para ingresarlo a un listado de token
                int Id_Token = 0;
                if (VerificacionTokenID(Particion[0], ref Id_Token,ref errorAsignado) == false)
                {
                    return false;
                }
                TokenProvicional.setNumeroTokenData(Id_Token);

                List<string> ListaDeLenguaje = new List<string>();
                List<string> ListaDePalabras = new List<string>();

                if (CapturarEER (LElementoToken, ref errorAsignado, ref ListaDeLenguaje, ref ListaDePalabras)== false)
                {
                    return false;
                }
                if (ListaDeLenguaje.Count == 0 && ListaDePalabras.Count == 0) //si no tienen ningun valor quiere decir que no se escribio ningun token
                {
                    errorAsignado = "El token no contenia elementos, o no se han encontrado elementos dentro del token";
                    return false;
                }
                if (ListaDeLenguaje.Count != 0)
                {
                    if (BusquedaLenguajes(ListaSets, ListaDeLenguaje, ref errorAsignado) == false)
                    {
                        return false;
                    }
                }

                if (ListaDePalabras.Count != 0)
                {
                    if (BusquerdaPalabra(ListaSets, ListaDePalabras, ref errorAsignado) == false)
                    {
                        return false;
                    }
                }
                bool ParentesisAbierto = false;
                string LineaDeToken = "";
                char[] Caracteres = LElementoToken.ToCharArray();
                foreach (var Data in Caracteres)
                {
                    if (Data == '\'')
                    {
                        ParentesisAbierto = !ParentesisAbierto;
                        LineaDeToken += Convert.ToString(Data);
                    }
                    else if (Data == '(' && ParentesisAbierto)
                    {
                        LineaDeToken += "α";
                    }
                    else if (Data == ')' && ParentesisAbierto)
                    {
                        LineaDeToken += "β";
                    }
                    else if (Data == '.' && ParentesisAbierto)
                    {
                        LineaDeToken += "ɣ";
                    }
                    else if (Data == '*' && ParentesisAbierto)
                    {
                        LineaDeToken += "δ";
                    }
                    else if (Data == '+' && ParentesisAbierto)
                    {
                        LineaDeToken += "ε";
                    }
                    else if (Data == '?' && ParentesisAbierto)
                    {
                        LineaDeToken += "ϑ";
                    }
                    else
                    {
                        LineaDeToken += Convert.ToString(Data);
                    }
                }
                bool AgregoPar = false;
                char[] AllCaracteres = ManejoExpresionGenerada(LineaDeToken).ToCharArray();

                if (AllCaracteres[0] == '(' && AllCaracteres[AllCaracteres.Length - 1] == ')' )
                {
                    for (int i = 1; i < AllCaracteres.Length; i++)
                    {
                        if (AllCaracteres[i] == ')')
                        {
                            AgregoPar = true;
                            break;
                        }
                    }
                }
                else
                {
                    AgregoPar = true;
                }

                if (AgregoPar)
                {
                    TokenProvicional.setElementoTokenData("(" + ManejoExpresionGenerada(LineaDeToken) + ")");
                }
                else
                {
                    TokenProvicional.setElementoTokenData("(" + ManejoExpresionGenerada(LineaDeToken));
                }

                foreach (var FindTokens in ListaTokens)
                {
                    if (FindTokens.getNumeroTokenData() == TokenProvicional.getNumeroTokenData())
                    {
                        errorAsignado = "Token Erroneo, el identificado ya fue utilizado y esta repetido: ( " + TokenProvicional.getNumeroTokenData() + " )";
                        return false;
                    }
                }
                ListaTokens.Add(TokenProvicional);
                LineaT++;
            }
            if (ListaSets.Count == 0)
            {
                errorAsignado = "Revisar, No existe ningun TOKEN para ser analizado";
                return false;
            }
            return true;
        
        }

        public bool CapturarActions(List<string> txtOriginal, ref string errorAsignado, ref int LineaT, ref Dictionary<string, int> DiccionarioActions) {
            bool lectura = true;
            string LineaCapturada = CapturarEspacios(txtOriginal[LineaT]);
            char[] ValorC = LineaCapturada.ToCharArray();
            if (ValorC[0] == 'E' && ValorC[1] == 'R' && ValorC[2] == 'R' && ValorC[3] == 'O' && ValorC[4] == 'R')
            {
                lectura = false;
                errorAsignado = "Revisar, El archivo no tiene declaracion de ACTIONS";
                return false;
            }

            while (lectura)
            {//siempre y cuando no este leyendo la parte de los actions
                while (CapturarEspacios(txtOriginal[LineaT]) == "")
                {
                    LineaT++;
                }

                LineaCapturada = CapturarEspacios(txtOriginal[LineaT]);
                ValorC = LineaCapturada.ToCharArray();

                if (ValorC[0] == 'E' && ValorC[1] == 'R' && ValorC[2] == 'R' && ValorC[3] == 'O' && ValorC[4] == 'R')
                {
                    break;
                }
                int NumeroIncremental = 0;
                string LElementoAction = "";

                try
                {//que este bien escrito el nombre action
                    while (ValorC[NumeroIncremental] != '(')
                    {
                        LElementoAction += Convert.ToString(ValorC[NumeroIncremental]);
                        NumeroIncremental++;
                    }
                }
                catch
                {
                    errorAsignado = "Verificar, Hace falta el Caracter ('(')";
                    return false;
                }//si sobre pasa el max, es porque no tiene parentesis de apertura
                try
                {// si no existe el parentesis de cerradura
                    if (ValorC[NumeroIncremental] == '(' && ValorC[NumeroIncremental + 1] != ')')
                    {
                        errorAsignado = "Simbolo no Aceptado y / o no reconocido( " + ValorC[NumeroIncremental + 1] + " )";
                        return false;
                    }
                }
                catch
                {
                    errorAsignado = "Verificar, Hace falta el caracter (')')";
                    return false;
                }
                LineaT++;

                if (CapturarEspacios(txtOriginal[LineaT]) != "{")
                {
                    errorAsignado = "El caracter ingresaro no es valido ( " + CapturarEspacios(txtOriginal[LineaT]) + " )\n" + "Se esperaba que ingresar el Caracter Abrir Llave ({)";
                    return false;
                }
                else
                {
                    LineaT++;
                }

                try
                {
                    while (!CapturarEspacios(txtOriginal[LineaT]).Equals("}"))
                    {
                        string LineaDeAction = CapturarEspacios(txtOriginal[LineaT]);
                        string[] Particion = LineaDeAction.Split('=');
                        if (Particion.Length == 1)
                        {
                            errorAsignado = "Revisar, la linea no cuenta con el caracter (=)";
                            return false;
                        }
                        int IdentificadorAction = 0;
                        if (!int.TryParse(Particion[0], out IdentificadorAction))
                        {
                            errorAsignado = "Revisar, Se tiene un identificador no autorizado";
                            return false;
                        }
                        string ParticionesAgrupadas = "";
                        if (Particion.Length > 2)
                        {
                            for (int i = 1; i < Particion.Length; i++)
                            {
                                ParticionesAgrupadas += Particion[i];
                                if (i != Particion.Length - 1)
                                {
                                    ParticionesAgrupadas += "=";
                                }
                            }
                        }
                        else
                        {
                            ParticionesAgrupadas = Particion[1];
                        }

                        char[] DataCargada = ParticionesAgrupadas.ToCharArray();

                        if (DataCargada[0] != '\'' || DataCargada[DataCargada.Length -1] != '\'')
                        {
                            errorAsignado = "Sintaxis Erronea, No existe la Comilla simple (') en un lugar necesario";
                            return false;
                        }
                        else
                        {
                            string ActionMenosComilla = "";
                            for (int i = 1; i < DataCargada.Length - 1; i++)
                            {
                                ActionMenosComilla += Convert.ToString(DataCargada[i]);
                            }

                            DiccionarioActions.Add(ActionMenosComilla, IdentificadorAction);
                        }
                        LineaT++;
                    }
                }
                catch 
                {
                    errorAsignado = "Revisar, la linea no cuenta con el caracter de LLave de cerradura (}) ";
                    return false;
                }
                if (CapturarEspacios(txtOriginal[LineaT]).Equals("}"))
                {
                    LineaT++;
                }
                while (CapturarEspacios(txtOriginal[LineaT]) == "")
                {
                    LineaT++;
                }
                LineaCapturada = CapturarEspacios(txtOriginal[LineaT]);
                ValorC = LineaCapturada.ToCharArray();

                if (ValorC[0] == 'E' && ValorC[1] == 'R' && ValorC[2] == 'R' && ValorC[3] == 'O' && ValorC[4] == 'R')
                {
                    lectura = false;
                }
            }
            return true;
        }

        public bool CapturarEER(string LineaLeida, ref string errorAsignado, ref List<string> ListaDeLenguajes, ref List<string> ListaDePalabras) {
            LineaLeida = LineaLeida.TrimEnd(' ');
            char[] Caracteres = LineaLeida.ToCharArray();
            string LPalabra = "";
            string LLenguaje = "";
            int Contador = 0;

            while (Contador < Caracteres.Length)
            {
                while (Caracteres[Contador] == ' ')
                {
                    Contador++;
                    if (Contador == Caracteres.Length)
                    {
                        break;
                    }
                }
                if (Caracteres[Contador] == '*' || Caracteres[Contador] == '+' || Caracteres[Contador] == '?')
                {
                    errorAsignado = "Simbolo operador ( " + Caracteres[Contador] + " ) dentro de un lugar incorrecto";
                    return false;
                }
                if (Caracteres[Contador] == '(' || Caracteres[Contador] == '|' || Caracteres[Contador] == ')')
                {
                    Contador++;
                    if (Contador == Caracteres.Length)
                    {
                        break;
                    }
                }
                while (Caracteres[Contador] == ' ')
                {
                    Contador++;
                    if (Contador == Caracteres.Length)
                    {
                        break;
                    }
                }
                if (VerificacionOperador(Caracteres, ref Contador, ref errorAsignado) == false)
                {
                    return false;
                }
                if (Contador == Caracteres.Length)
                {
                    break;
                }
                if (Caracteres[Contador] == '\'')
                {
                    Contador++;
                    try
                    {
                        if (Caracteres[Contador] == '\'')
                        {
                            try
                            {
                                if (Caracteres[Contador + 1] != '\'')
                                {
                                    errorAsignado = "El Elemento se encuentra vacio";
                                    return false;
                                }
                                else
                                {
                                    LPalabra = Convert.ToString('\'');
                                    ListaDePalabras.Add(LPalabra);
                                    LPalabra = "";
                                    Contador += 2;
                                    if (Contador == Caracteres.Length)
                                    {
                                        break;
                                    }
                                    if (VerificacionOperador(Caracteres, ref Contador, ref errorAsignado) == false)
                                    {
                                        return false;
                                    }
                                }
                            }
                            catch
                            {
                                errorAsignado = "El Elemento se encuentra vacio";
                                return false;
                            }
                        }
                        else
                        {//Remplaza las palabras de reserva por letras griegas
                            try
                            {
                                if ((Caracteres[Contador] == '(' && Caracteres[Contador + 1] == '\'' ))
                                {
                                    LPalabra += Convert.ToString('α');
                                    Contador = Contador + 2;
                                }
                                else if ((Caracteres[Contador] == ')' && Caracteres[Contador + 1] == '\''))
                                {
                                    LPalabra += Convert.ToString('β');
                                    Contador = Contador + 2;
                                }
                                else if ((Caracteres[Contador] == '.' && Caracteres[Contador + 1] == '\''))
                                {
                                    LPalabra += Convert.ToString('ɣ');
                                    Contador = Contador + 2;
                                }
                                else if ((Caracteres[Contador] == '*' && Caracteres[Contador + 1] == '\''))
                                {
                                    LPalabra += Convert.ToString('δ');
                                    Contador = Contador + 2;
                                }
                                else if ((Caracteres[Contador] == '+' && Caracteres[Contador + 1] == '\''))
                                {
                                    LPalabra += Convert.ToString('ε');
                                    Contador = Contador + 2;
                                }
                                else if ((Caracteres[Contador] == '?' && Caracteres[Contador] == '\''))
                                {
                                    LPalabra += Convert.ToString('ϑ');
                                    Contador = Contador + 2;
                                }
                                else
                                {
                                    while (Caracteres[Contador] != '\'')
                                    {
                                        LPalabra += Convert.ToString(Caracteres[Contador]);
                                        Contador++;
                                    }
                                    Contador++;
                                }
                                ListaDePalabras.Add(LPalabra);
                                LPalabra = "";
                                if (Contador == Caracteres.Length)
                                {
                                    break;
                                }
                                if (VerificacionOperador(Caracteres, ref Contador, ref errorAsignado) == false)
                                {
                                    return false;
                                }
                            }
                            catch 
                            {
                                errorAsignado = "Falta el Simbolo de comilla simple (')";
                                return false;
                            }
                        }
                    }
                    catch //Al llegar aqui se sabe que se capturo elementros entra las comillas 
                    {
                        errorAsignado = "Es posible que comilla simple (') no este relacionado o esta fuera de lugar";
                        return false;
                    }
                }
                else
                {
                    while (Caracteres[Contador] != ' ' && Caracteres[Contador] != '*' && Caracteres[Contador] != '+' && Caracteres[Contador] != '?' 
                        && Caracteres[Contador] != '(' && Caracteres[Contador] != '|' && Caracteres[Contador] != ')') // si al encontrar algo no esta detro de comillas se almacena el lenguaje
                    {
                        LLenguaje += Convert.ToString(Caracteres[Contador]);
                        Contador++;
                        if (Contador == Caracteres.Length)
                        {
                            break;
                        }
                    }
                    ListaDeLenguajes.Add(LLenguaje);
                    LLenguaje = "";
                    if (Contador == Caracteres.Length)
                    {
                        break;
                    }
                    if (VerificacionOperador(Caracteres, ref Contador , ref errorAsignado) == false)
                    {
                        return false;
                    }
                }
                if (Contador == Caracteres.Length)
                {
                    break;
                }

                if (!(Caracteres[Contador - 2] != '\'' && Caracteres[Contador -1] == '\'' && Caracteres[Contador] == '\'' && Caracteres[Contador + 1] != '\''))//una posibilidad que no traiga espacio entre las comillas
                {
                    if (Caracteres[Contador] == ' '|| Caracteres[Contador] == '|' || Caracteres[Contador] == '(' || Caracteres[Contador] == ')')
                    {
                        Contador++;
                        if (Contador == Caracteres.Length)
                        {
                            break;
                        }
                        if (VerificacionOperador(Caracteres, ref Contador, ref errorAsignado) == false)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        errorAsignado = "Simbolo no Aceptado y/o no reconocido ( " + Caracteres[Contador] + " )";
                        return false;
                    }
                }
                if (Contador == Caracteres.Length)
                {
                    break;
                }
            }
            return true;
        }

        public bool CapturarError(List<string> txtOriginal, ref string errorAsignado, ref int LineaT, ref int OutError) { // Error con mostrado para el error en el TXT
            string[] LineasError = CapturarEspacios(txtOriginal[LineaT]).Split('=');
            if (LineasError.Length > 2)
            {
                errorAsignado = "Caracter incorrecto (=)";
                return false;
            }
            if (LineasError[0] != "ERROR")
            {
                errorAsignado = "Palabra no valida o no reconocida: ( " + LineasError[0] + " )";
                return false;
            }
            if (!int.TryParse(LineasError[1], out OutError))
            {
                errorAsignado = "Numero identificador de error incorrecto: ( " + LineasError[1] + " )";
                return false;
            }
            return true;
        }
        public bool BusquedaLenguajes(List<Set> ListaSets, List<string> ListaLenguajes, ref string errorAsignado) {
            foreach (string FindLeng in ListaLenguajes)
            {
                bool EsLenguaje = false;
                foreach (Set FindSet in ListaSets)
                {
                    if (FindSet.getNombreSetData() == FindLeng)
                    {
                        EsLenguaje = true;
                        break;
                    }
                }
                if (EsLenguaje == false)
                {
                    if (FindLeng == "α" || FindLeng == "β" || FindLeng == "ɣ" || FindLeng == "δ" || FindLeng == "ε" || FindLeng == "ϑ")
                    {
                        return true;
                    }
                    else
                    {
                        errorAsignado = "Verificacion: No se ha encontrado el dato \"" + FindLeng + "\" declarado en los SETS";
                        return false;
                    }
                }
            }
            return true;
        }

        public bool BusquerdaPalabra(List<Set> ListaSets, List<string> ListaPalabras, ref string errorAsignado) {
            foreach (string FindPalabra in ListaPalabras)
            {
                bool EsPalabra = false;
                foreach (Set FindSet in ListaSets)
                {
                    EsPalabra = false;
                    List<string> ElementosSet = FindSet.getElementosSetData();
                    if (ElementosSet.Contains(FindPalabra))
                    {
                        EsPalabra = true;
                        break;
                    }
                }
                if (EsPalabra == false)
                {
                    if (FindPalabra == "α" || FindPalabra == "β" || FindPalabra == "ɣ" || FindPalabra == "δ" || FindPalabra == "ε" || FindPalabra == "ϑ")
                    {
                        return true;
                    }
                    else
                    {
                        errorAsignado = "Verificacion: No se ha encontrado el dato \"" + FindPalabra + "\" declarado en los SETS";
                        return false;
                    }
                }
            }

            return true;
        }

        public bool VerificacionArchivoAnalizado(List<string> txtOriginal, ref string errorAsignado, ref int LineaT, ref List<Set> ListaSets, ref List<Token> ListaTokens, ref Dictionary<string, int> DiccionarioActions, ref int OutError) {
            while (CapturarEspacios(txtOriginal[LineaT]) == "")
            {
                LineaT++;
            }
            if (CapturarEspacios(txtOriginal[LineaT]).ToUpper() == "SETS")//verificacion que la primera linea que encuentre sean los SETS
            {
                LineaT++;
                if (CapturaSets(txtOriginal, ref errorAsignado, ref LineaT, ref ListaSets) == false)
                {
                    return false;
                }

                if (CapturarEspacios(txtOriginal[LineaT]).ToUpper() == "TOKENS")// verificacion que la siguiente parte sea la implementacion de los tokens
                {
                    LineaT++;
                    if (CapturarTokens(txtOriginal, ref errorAsignado, ref LineaT, ref ListaTokens, ListaSets) == false)
                    {
                        return false;
                    }
                    if (CapturarEspacios(txtOriginal[LineaT]).ToUpper() == "ACTIONS")// verificacion que posteriormente viene la parte de actions
                    {
                        LineaT++;
                        if (CapturarActions(txtOriginal,ref errorAsignado, ref LineaT, ref DiccionarioActions) == false)
                        {
                            return false;
                        }
                        CapturarError(txtOriginal, ref errorAsignado, ref LineaT, ref OutError);// verificacion de la parte de Error
                    }
                    else
                    {
                        errorAsignado = "No se ha encontrado la parte de ACTIONS dentro del archivo a analizar";
                        return false;
                    }

                }
                else
                {
                    errorAsignado = "No se ha encontrado la parte de TOKENS dentro del archivo a analizar";
                    return false;
                }
            }
            else
            {
                errorAsignado = "No se ha encontrado la parte de SETS al inicio dentro del archivo a analizar";
                return false;
            }
            return true;
        }

        public void ObtenerPosFijoDeCadena(ref Stack<Nodo> PilaPosFijo, ref List<Nodo> ListaHojas, char[] Caracteres, ref int Contador, ref int NoHojas, List<Set> ListaSets) { // Metodo que si encuentra una palabra en el ExpresionRegular y ese dato se ingreesa a una pila posfijo
            bool ProcederAnalisis = true;
            bool ParentesisAbierto = false;
            string errorAsignado = "";
            String CadenaCompleta = "";
            List<string> CadenaPalabras = new List<string>();

            if (Caracteres[Contador - 1] == '(')
            {
                ParentesisAbierto = true;
            }

            if (Caracteres[Contador] == '(')
            {
                Contador++;
            }

            while (ProcederAnalisis)
            {
                if ((Caracteres[Contador] == '*' || Caracteres[Contador] == '+' || Caracteres[Contador] == '?') && ParentesisAbierto == true)
                {
                    CadenaCompleta += Caracteres[Contador];
                    Contador++;
                }

                if (Caracteres[Contador] != '.' && Caracteres[Contador] != '*' && Caracteres[Contador] != '+' && Caracteres[Contador] != '?' && Caracteres[Contador] != '|' && Caracteres[Contador] != ')' )
                {
                    CadenaCompleta += Caracteres[Contador];
                    Contador++;
                    ParentesisAbierto = false;
                }
                else
                {
                    CadenaPalabras.Add(CadenaCompleta);
                    if (CadenaCompleta == "#")
                    {
                        ProcederAnalisis = false;
                        Nodo NodoProvisional = new Nodo(CadenaCompleta, CadenaCompleta, Convert.ToString(NoHojas), Convert.ToString(NoHojas), false);
                        PilaPosFijo.Push(NodoProvisional);
                        ListaHojas.Add(NodoProvisional);
                        NoHojas++;
                        CadenaPalabras.RemoveAt(0);
                    }
                    else
                    
                    if (BusquedaLenguajes(ListaSets,CadenaPalabras,ref errorAsignado) || BusquerdaPalabra(ListaSets, CadenaPalabras, ref errorAsignado))
                    {
                        ProcederAnalisis = false;
                        Nodo NodoProvisional = new Nodo(CadenaCompleta, CadenaCompleta, Convert.ToString(NoHojas), Convert.ToString(NoHojas), false);
                        PilaPosFijo.Push(NodoProvisional);
                        ListaHojas.Add(NodoProvisional);
                        NoHojas++;
                        CadenaPalabras.RemoveAt(0);

                    }
                }
            }

            
        
        }

        public void DevolverPosFijo(ref Stack<Nodo> PilaPosfijo, ref List<Nodo> ListaHojas, List<Set> ListaSets, string ERTokenRecibido, ref int Contador, ref int NoHojas) { // este metodo realiza el analisis al posfijo de cada token
            string error = "";//en esta parte basicamente es para asignar los first last a cada uno de los datos
            char[] Caracteres = ERTokenRecibido.ToCharArray();
            ObtenerPosFijoDeCadena(ref PilaPosfijo, ref ListaHojas, Caracteres, ref Contador, ref NoHojas, ListaSets);

            if (Caracteres[Contador] == ')')
            {
                Contador++;
            }
            while (Caracteres.Length != Contador)
            {
                Nodo Simbolo = new Nodo();

                if (Caracteres[Contador] == '*' || Caracteres[Contador] == '+' || Caracteres[Contador] == '?')
                {
                    Nodo C1Left = PilaPosfijo.Pop();

                    Simbolo.setElementosContenidos(Convert.ToString(Caracteres[Contador]));
                    if (Caracteres[Contador] == '*' || Caracteres[Contador] == '?')
                    {
                        Simbolo.setIsNull(true);
                    }
                    else if (Caracteres[Contador] == '+')
                    {
                        Simbolo.setIsNull(C1Left.getIsNull());
                    }

                    Simbolo.setAsignacionFirst(C1Left.getAsignacionFirst());
                    Simbolo.setAsignacionLast(C1Left.getAsignacionLast());
                    Simbolo.setc1(C1Left);
                    Simbolo.setc2(null);

                    Simbolo.SetExpresionObtenida(C1Left.getExpresionObtenida() + Simbolo.getElementosContenidos());
                    PilaPosfijo.Push(Simbolo);
                    Contador++;
                    
                }
                else if (Caracteres[Contador] == '.' || Caracteres[Contador] == '|')
                {
                    Simbolo.setElementosContenidos(Convert.ToString(Caracteres[Contador]));
                    Contador++;
                    if (Caracteres[Contador] == '(')
                    {
                        string fragmento = "";
                        for (int i = Contador; i < Caracteres.Length; i++)
                        {
                            fragmento += Convert.ToString(Caracteres[i]);
                        }
                        int ContadorAux = 1;
                        DevolverPosFijo(ref PilaPosfijo, ref ListaHojas, ListaSets, fragmento, ref ContadorAux, ref NoHojas);
                        Contador += ContadorAux - 1;
                    }
                    else
                    {
                        ObtenerPosFijoDeCadena(ref PilaPosfijo, ref ListaHojas, Caracteres, ref Contador, ref NoHojas, ListaSets);
                    }

                    Nodo C2right = PilaPosfijo.Pop();
                    Nodo C1Left = PilaPosfijo.Pop();
                    

                    if (Simbolo.getElementosContenidos() == ".")//Reglas para la CONCATENACION
                    {
                        if (C1Left.getIsNull() && C2right.getIsNull()) //si c1 y c2 son nullables se considera la concatenacion nullabe
                        {
                            Simbolo.setIsNull(true);
                        }
                        else
                        {
                            Simbolo.setIsNull(false);
                        }

                        if (C1Left.getIsNull())// si c1 es nullable en el first se concatena el first de c1 y c1
                        {
                            Simbolo.setAsignacionFirst(C1Left.getAsignacionFirst() + "," + C2right.getAsignacionFirst());
                        }
                        else
                        {
                            Simbolo.setAsignacionFirst(C1Left.getAsignacionFirst());//si c1 no es nullable solo se toma el first de c1
                        }

                        if (C2right.getIsNull())// si c2 es nullable en el last se concatena el last de C1 y C2
                        {
                            Simbolo.setAsignacionLast(C1Left.getAsignacionLast() + "," + C2right.getAsignacionLast());
                        }
                        else
                        {
                            Simbolo.setAsignacionLast(C2right.getAsignacionLast()); //si c2 no es nullable solo se toma el last de c2
                        }
                    }
                    else if (Simbolo.getElementosContenidos() == "|")//reglas del OR
                    {
                        if (C1Left.getIsNull() || C2right.getIsNull())// si c1 o c2 son nullables el OR es nullable
                        {
                            Simbolo.setIsNull(true);
                        }
                        else
                        {
                            Simbolo.setIsNull(false);
                        }

                        Simbolo.setAsignacionFirst(C1Left.getAsignacionFirst() + "," + C2right.getAsignacionFirst());//para el first del OR se concatena los dos first
                        Simbolo.setAsignacionLast(C1Left.getAsignacionLast() + "," + C2right.getAsignacionLast()); //para el last del OR se concatena los dos last
                    }
                    Simbolo.setc1(C1Left);
                    Simbolo.setc2(C2right);
                    Simbolo.SetExpresionObtenida(C1Left.getExpresionObtenida() + Simbolo.getElementosContenidos() + C2right.getExpresionObtenida());
                    PilaPosfijo.Push(Simbolo);
                }
                try
                {
                    while (Caracteres[Contador] == ')')
                    {
                        Contador++;
                    }
                }
                catch
                {

                }
            }

        }
    }
}
