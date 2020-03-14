using Proyecto_LFYA_Scanner.Archivo;
using Proyecto_LFYA_Scanner.Componentes;
using Proyecto_LFYA_Scanner.RecursosTabla;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Proyecto_LFYA_Scanner
{
    public partial class FormFuncionamiento : Form
    {
        int LineaErrorInte = 0;
        string errorAsignado = "";
        string ArchivoRuta = "";
        List<string> ArchivoTexto = new List<string>();
        List<Set> ListaSets;
        List<Token> ListaTokens;
        Dictionary<string, int> DiccionarioActions;
        LecturaArchivo EjeccucionArchivo = new LecturaArchivo();
         
        public FormFuncionamiento()
        {
            InitializeComponent();
            openFileDialog1.Title = "Seleccione el archivo";
            openFileDialog1.Filter = "Archivos Tipo Texto|*.txt";
            openFileDialog1.FileName = "";
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            List<string> ListadoNullsFirstLast = new List<string>();
            List<string> ListadoFollows = new List<string>();

            if (openFileDialog1.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            ArchivoRuta = openFileDialog1.FileName;
            StreamReader LecturaArchivo = new StreamReader(ArchivoRuta);
            String LineaLeida = LecturaArchivo.ReadLine();
            while (LineaLeida != null)
            {
                ArchivoTexto.Add(LineaLeida);
                LineaLeida = LecturaArchivo.ReadLine();
            }
            int LineaTamano = 0;

            ListaSets = new List<Set>();
            ListaTokens = new List<Token>();
            DiccionarioActions = new Dictionary<string, int>();
            LecturaArchivo.Close();

            if (EjeccucionArchivo.VerificacionArchivoAnalizado(ArchivoTexto, ref errorAsignado, ref LineaTamano, ref ListaSets, ref ListaTokens, ref DiccionarioActions, ref LineaErrorInte) == false)
            {
                MessageBox.Show(("Se encontro un ERROR, en linea: " + (LineaTamano + 1) + "\n" + errorAsignado) ,"ERROR",MessageBoxButtons.OK,MessageBoxIcon.Error); 
                return;
            }


            

            ManejoArchivoAFD(ListaSets, ListaTokens, ref ListadoFollows, ref ListadoNullsFirstLast);

            LST_FirstLastFollow.Items.Add("FIRSTS - LASTS");
            foreach (var data in ListadoNullsFirstLast)
            {
                LST_FirstLastFollow.Items.Add(data);
            }

            LST_FirstLastFollow.Items.Add(""); LST_FirstLastFollow.Items.Add("Lista de FOLLOWS");
            foreach (var data in ListadoFollows)
            {
                LST_FirstLastFollow.Items.Add(data);
            }

        }

        private void ManejoArchivoAFD(List<Set> ListaSets, List<Token> ListaTokens, ref List<string> ListaFollows, ref List<string> ListadoNullsFirstLast) { //Obtencion de first last y follows del archivo
            int CantHojas = 1;
            int Contador = 1;
            Stack<Nodo> PilaPosFijo = new Stack<Nodo>();
            List<Nodo> ListaHojas = new List<Nodo>();

            foreach (var data in ListaTokens)//proceso para los tokens
            {
                Contador = 1;
                string ExpresionRegular = data.getElementoTokenData();
               
                EjeccucionArchivo.DevolverPosFijo(ref PilaPosFijo, ref ListaHojas, ListaSets, ExpresionRegular, ref Contador, ref CantHojas);
                if (PilaPosFijo.Count == 2)
                {
                    Nodo Simbolo = new Nodo();
                    Simbolo.setElementosContenidos(Convert.ToString('|'));
                    Nodo C2Right = PilaPosFijo.Pop();
                    Nodo C1Left = PilaPosFijo.Pop();
                    

                    if (C1Left.getIsNull() || C2Right.getIsNull()) //validaciones para verificar su nullabilidad
                    {
                        Simbolo.setIsNull(true);
                    }
                    else
                    {
                        Simbolo.setIsNull(false);
                    }

                    Simbolo.setAsignacionFirst(C1Left.getAsignacionFirst() + "," + C2Right.getAsignacionFirst());
                    Simbolo.setAsignacionLast(C1Left.getAsignacionLast() + "," + C2Right.getAsignacionLast());


                    Simbolo.setc1(C1Left);
                    Simbolo.setc2(C2Right);
                    
                    Simbolo.SetExpresionObtenida(C1Left.getExpresionObtenida() + Simbolo.getElementosContenidos() + C2Right.getExpresionObtenida());
                    PilaPosFijo.Push(Simbolo);
                }
            }
            Contador = 1;
            EjeccucionArchivo.DevolverPosFijo(ref PilaPosFijo, ref ListaHojas, ListaSets, "(#)", ref Contador, ref CantHojas); // momento magico donde se concatena el # al final de la expresion

            Nodo SimboloAlFinal = new Nodo();
            SimboloAlFinal.setElementosContenidos(Convert.ToString('.'));

            Nodo C2RightFinal = PilaPosFijo.Pop();
            Nodo C1LeftFinal = PilaPosFijo.Pop();

            if (C1LeftFinal.getIsNull() || C2RightFinal.getIsNull()) //asignacion de su nullabilidad
            {
                SimboloAlFinal.setIsNull(true);
            }
            else
            {
                SimboloAlFinal.setIsNull(false);
            }

            if (C1LeftFinal.getIsNull())// si c1 es nullable se concatenan los first de c1 y c2
            {
                SimboloAlFinal.setAsignacionFirst(C1LeftFinal.getAsignacionFirst() + "," + C2RightFinal.getAsignacionFirst());
            }
            else
            {
                SimboloAlFinal.setAsignacionFirst(C1LeftFinal.getAsignacionFirst());
            }

            if (C2RightFinal.getIsNull())
            {
                SimboloAlFinal.setAsignacionLast(C1LeftFinal.getAsignacionLast() + "," + C2RightFinal.getAsignacionLast());
            }
            else
            {
                SimboloAlFinal.setAsignacionLast(C2RightFinal.getAsignacionLast());
            }

            SimboloAlFinal.setc1(C1LeftFinal);
            SimboloAlFinal.setc2(C2RightFinal);
            SimboloAlFinal.SetExpresionObtenida(C1LeftFinal.getExpresionObtenida() + SimboloAlFinal.getElementosContenidos() + C2RightFinal.getExpresionObtenida());//momento de adiccion del operador al final
            PilaPosFijo.Push(SimboloAlFinal);

            //Proceso de Follows
            Dictionary<int, List<int>> DiccionarioFollows = new Dictionary<int, List<int>>();

            for (int i = 1; i < CantHojas; i++)
            {
                List<int> FAux = new List<int>();
                DiccionarioFollows.Add(i, FAux);
            }
           
            RecorridoInOrden(PilaPosFijo.Peek(), ref DiccionarioFollows);
            RecorridoPostOrden(PilaPosFijo.Peek(), ref ListadoNullsFirstLast);//en este momento se realizara la busqueda de los follows de la expresion

            foreach (var data in DiccionarioFollows)
            {
                string elementoInData1 = "";
                foreach (var elementoInData2 in data.Value)
                {
                    elementoInData1 += elementoInData2 + ",";
                }
                ListaFollows.Add(Convert.ToString(data.Key) + "===>" + elementoInData1);
            }
            List<string> FirstRoot = new List<string>();
            string[] FirstDelRoot = SimboloAlFinal.getAsignacionFirst().Split(',');

            foreach (var data in FirstDelRoot)
            {
                FirstRoot.Add(data);
            }

            Transiciones[,] TablaDeTransiciones = CreacionTablaTransiciones(FirstRoot, ListaHojas, DiccionarioFollows);


        }

        private Transiciones[,] CreacionTablaTransiciones(List<string> FirstRoot, List<Nodo> ListaHojas, Dictionary<int, List<int>> DiccionarioFollows) { //metodo que construye la matriz de la tabla de transiciones

          
            DatosColumna[] ParteDelEncabezado = CreacionColumnas(ListaHojas);
            int filasX = RecuentoCantidadFilas(FirstRoot, ParteDelEncabezado, DiccionarioFollows);

            Transiciones[,] TablaDeTransicionesAFD = new Transiciones[filasX, ParteDelEncabezado.Length + 1];

            int caracteres = 65;
            char CaracterLetras = Convert.ToChar(caracteres);

            List<Transiciones> ListaTransicionesEstado = new List<Transiciones>();// coloca aqui todos los estados de la tabla de transicion

            Queue<Transiciones> ColaPendientesEstado = new Queue<Transiciones>();//coloca dentro de la cola todos los estados pendientes de analizar

            for (int i = 0; i < filasX; i++)//comienza la matriz
            {
                for (int j = 0; j < (ParteDelEncabezado.Length + 1); j++)
                {
                    TablaDeTransicionesAFD[i, j] = new Transiciones();
                }
            }
            TablaDeTransicionesAFD[0, 0].setElementosTransicion(FirstRoot);
            TablaDeTransicionesAFD[0, 0].setid_Transicion(Convert.ToString(CaracterLetras));
            Transiciones TranAuxiliar = new Transiciones(Convert.ToString(CaracterLetras), FirstRoot);
            ListaTransicionesEstado.Add(TranAuxiliar);
            CaracterLetras++;
            int NFila = 0;

            bool esAnalizable = true;
            bool EselUltimoDato = false;

            do
            {
                if (EselUltimoDato == true)//verficar que no es la ultima iteracion para no continuar con el analisis
                {
                    esAnalizable = false;
                }
                foreach (var Columnas in ParteDelEncabezado)
                {
                    foreach (var DataElemento in TranAuxiliar.getElementosTransicion())
                    {//se obtienen los valores del first que se tiene en ese momento
                        if (Columnas.getElementosC().Contains(Convert.ToInt16(DataElemento)))
                        {
                            List<int> ListadoDeFollows = DiccionarioFollows[Convert.ToInt16(DataElemento)];

                            List<string> ContenidoF = new List<string>();//convertir los follows a string(int)
                            foreach (var data in ListadoDeFollows)
                            {
                                ContenidoF.Add(Convert.ToString(data));
                            }
                            int localizacion = Columnas.getid_columna();
                            TablaDeTransicionesAFD[NFila, localizacion].setElementosTransicion(ContenidoF);
                            bool Agregar = true;
                            foreach (var item in ListaTransicionesEstado)
                            {
                                if (TablaDeTransicionesAFD[NFila, Columnas.getid_columna()].getElementosTransicion() == item.getElementosTransicion())
                                {
                                    Agregar = false;
                                }
                            }
                            if (Agregar)
                            {
                                bool EsCaracterConocido = false;
                                string LetraAtras = "";

                                foreach (var data in ListaTransicionesEstado)//aqui todo se convierte en string para la comprobacion si el elemento ya existe en la tabla
                                {
                                    string ConjuntoA = "";
                                    string ConjuntoB = "";
                                    foreach (var NElemento in data.getElementosTransicion())
                                    {
                                        ConjuntoA += NElemento;
                                    }
                                    foreach (var NElemento in ContenidoF)
                                    {
                                        ConjuntoB += NElemento;
                                    }
                                    if (ConjuntoA.Equals(ConjuntoB))
                                    {
                                        EsCaracterConocido = true;
                                        LetraAtras = data.getid_Transicion();
                                        break;
                                    }

                                }
                                if (EsCaracterConocido)
                                {
                                    TablaDeTransicionesAFD[NFila, Columnas.getid_columna()].setid_Transicion(LetraAtras);
                                }
                                else
                                {
                                    TablaDeTransicionesAFD[NFila, Columnas.getid_columna()].setid_Transicion(Convert.ToString(CaracterLetras));
                                    CaracterLetras++;
                                    ListaTransicionesEstado.Add(TablaDeTransicionesAFD[NFila, Columnas.getid_columna()]);
                                    ColaPendientesEstado.Enqueue(TablaDeTransicionesAFD[NFila, Columnas.getid_columna()]);
                                    if (ColaPendientesEstado.Count != 0 && esAnalizable == false)
                                    {
                                        esAnalizable = true;
                                    }
                                }
                            }
                            
                        }
                    }
                }
                try
                {
                    NFila++;
                    TranAuxiliar = ColaPendientesEstado.Dequeue();
                    FirstRoot = TranAuxiliar.getElementosTransicion();

                    TablaDeTransicionesAFD[NFila, 0].setid_Transicion(TranAuxiliar.getid_Transicion());
                    TablaDeTransicionesAFD[NFila, 0].setElementosTransicion(TranAuxiliar.getElementosTransicion());
                    string id = TranAuxiliar.getid_Transicion();
                }
                catch (Exception e)
                {

                }
                if (ColaPendientesEstado.Count == 0)//cuando ya no existan elemntos procede con lo demas
                {
                    EselUltimoDato = true;
                }
            } while (esAnalizable);

            NFila++;
            int CantidadSimbolosT = ListaHojas.Count;

            for (int i = 0; i < (NFila - 1); i++)
            {
                for (int j = 0; j < (ParteDelEncabezado.Length + 1); j++)
                {
                    if (TablaDeTransicionesAFD[i,j].getElementosTransicion() == null)
                    {
                        DGV_transiciones.Rows[i].Cells[j].Style.BackColor = Color.LightGoldenrodYellow; 
                    }
                    if (i == 0 && j == 0)
                    {
                        DGV_transiciones.Rows[i].Cells[j].Style.BackColor = Color.LightCyan;
                    }
                    if (TablaDeTransicionesAFD[i,0].getElementosTransicion().Contains(Convert.ToString(CantidadSimbolosT)))
                    {
                        DGV_transiciones.Rows[i].Cells[0].Style.BackColor = Color.LightGreen;
                    }

                    if (TablaDeTransicionesAFD[i,j].getElementosTransicion() != null)
                    {//momento magico de ingreso de datos al DGV
                        DGV_transiciones.Rows[i].Cells[j].Value = TablaDeTransicionesAFD[i, j].getCadenaDeElementosTransicion() + " " + TablaDeTransicionesAFD[i, j].getid_Transicion();
                        DGV_transiciones.Rows[i].Cells[j].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    }
                }
            }
            return TablaDeTransicionesAFD;
        }


        private DatosColumna[] CreacionColumnas(List<Nodo> ListaHojas) {// metodo que ayuda para la obtencion del encabecador de la preAFD
            List<string> listaAsignacionIdentificador = new List<string>();
            foreach (var data in ListaHojas)
            {
                if (!listaAsignacionIdentificador.Contains(data.getElementosContenidos()) && data.getElementosContenidos() != "#")
                {
                    listaAsignacionIdentificador.Add(data.getElementosContenidos());
                }
            }

            DatosColumna[] TituloColumna = new DatosColumna[listaAsignacionIdentificador.Count];

            for (int i = 0; i < TituloColumna.Length; i++)
            {
                TituloColumna[i] = new DatosColumna();
                if (listaAsignacionIdentificador[i] == "α")
                {
                    TituloColumna[i].setNombreC("(");
                }
                else if (listaAsignacionIdentificador[i] == "β")
                {
                    TituloColumna[i].setNombreC(")");
                }
                else if (listaAsignacionIdentificador[i] == "ɣ")
                {
                    TituloColumna[i].setNombreC(".");
                }
                else if (listaAsignacionIdentificador[i] == "δ")
                {
                    TituloColumna[i].setNombreC("*");
                }
                else if (listaAsignacionIdentificador[i] == "ε")
                {
                    TituloColumna[i].setNombreC("+");
                }
                else if (listaAsignacionIdentificador[i] == "ϑ")
                {
                    TituloColumna[i].setNombreC("?");
                }
                else
                {
                    TituloColumna[i].setNombreC(listaAsignacionIdentificador[i]);
                }

                TituloColumna[i].setid_columna(i + 1);
                List<int> LHojas = new List<int>();
                foreach (var data in ListaHojas)
                {
                    if (data.getElementosContenidos() == TituloColumna[i].getNombreC()) 
                    {
                        LHojas.Add(Convert.ToInt16(data.getAsignacionFirst()));
                    }
                }
                TituloColumna[i].setElementosC(LHojas);
            }
            DGV_transiciones.Columns.Add(0.ToString(), "");

            for (int i = 0; i < TituloColumna.Length; i++)
            {
                DGV_transiciones.Columns.Add(i.ToString(), TituloColumna[i].getNombreC());
            }
            return TituloColumna;
        }

        private int RecuentoCantidadFilas(List<string> FirstRoot, DatosColumna[] TituloColumna, Dictionary<int, List<int>> DiccionarioFollows) { // este metodo es para encontrar la cantidad de las lineas de la matriz(tabla transicion)

           

            int Cantidad = 1;
            int Caracter = 65;
            char AllLetra = Convert.ToChar(Caracter);
            List<Transiciones> ListaTransicion = new List<Transiciones>();
            Queue<Transiciones> ColaPendientes = new Queue<Transiciones>();
            Transiciones[] LineaActual = new Transiciones[TituloColumna.Length + 1];

            for (int i = 0; i < LineaActual.Length; i++)//cominezo de la lista
            {
                LineaActual[i] = new Transiciones();
            }
            LineaActual[0].setElementosTransicion(FirstRoot);
            LineaActual[0].setid_Transicion(Convert.ToString(AllLetra));

            Transiciones TransAux = new Transiciones(Convert.ToString(AllLetra), FirstRoot);// punto de partida de la tabla de transicion
            ListaTransicion.Add(TransAux);
            AllLetra++;

            bool EsTransicion = true;
            bool EsUltimaParte = false;

            do
            {
                if (EsUltimaParte == true) //opcion para ya no continuar analizando, ya que se analiza solo la ultima iteracion
                {
                    EsTransicion = false;
                }
                foreach (var DataC in TituloColumna) //cantidadd d filas por la cada columna
                {
                    foreach (var DataE in TransAux.getElementosTransicion())//se almacena el primer first
                    {
                        if (DataC.getElementosC().Contains(Convert.ToInt16(DataE)))
                        {
                            List<int> ContenidoFollows = DiccionarioFollows[Convert.ToInt16(DataE)];

                            List<string> ContenidoConvertidoE = new List<string>();
                            foreach (var data in ContenidoFollows)
                            {
                                ContenidoConvertidoE.Add(Convert.ToString(data));
                            }

                            int Localizacion = DataC.getid_columna();
                            LineaActual[Localizacion].setElementosTransicion(ContenidoConvertidoE);
                            bool Agregar = true;
                            foreach (var data in ListaTransicion)
                            {
                                if (LineaActual[DataC.getid_columna()].getElementosTransicion() == data.getElementosTransicion())
                                {
                                    Agregar = false;
                                }
                            }
                            if (Agregar)
                            {
                                bool LetraEsEncontrar = false;
                                string LetraAtras = "";
                                foreach (var data in ListaTransicion)
                                {
                                    string Caracteres1 = "";
                                    string Caracteres2 = "";

                                    foreach (var DataElemento in data.getElementosTransicion())
                                    {
                                        Caracteres1 += DataElemento;
                                    }
                                    foreach (var DataElemento in ContenidoConvertidoE)
                                    {
                                        Caracteres2 += DataElemento;
                                    }
                                    if (Caracteres1.Equals(Caracteres2))
                                    {
                                        LetraEsEncontrar = true;
                                        LetraAtras = data.getid_Transicion();
                                        break;
                                    }
                                }
                                if (LetraEsEncontrar)
                                {
                                    LineaActual[DataC.getid_columna()].setid_Transicion(LetraAtras);
                                }
                                else
                                {
                                    LineaActual[DataC.getid_columna()].setid_Transicion(Convert.ToString(AllLetra));
                                    AllLetra++;
                                    ListaTransicion.Add(LineaActual[DataC.getid_columna()]);
                                    ColaPendientes.Enqueue(LineaActual[DataC.getid_columna()]);
                                    if (ColaPendientes.Count != 0 && EsTransicion == false )
                                    {
                                        EsTransicion = true;
                                    }
                                }
                            }

                        }
                    }
                }
                try
                {
                    Cantidad++;
                    TransAux = ColaPendientes.Dequeue();
                    FirstRoot = TransAux.getElementosTransicion();
                    string id = TransAux.getid_Transicion();
                    LineaActual = new Transiciones[TituloColumna.Length + 1];

                    for (int i = 0; i < LineaActual.Length; i++)
                    {
                        LineaActual[i] = new Transiciones();
                    }
                    LineaActual[0].setElementosTransicion(FirstRoot);
                    LineaActual[0].setid_Transicion(id);
                }
                catch (Exception e)
                {

                }
                if (ColaPendientes.Count == 0)
                {
                    EsUltimaParte = true;
                }
            } while (EsTransicion);

            DGV_transiciones.Rows.Add(Cantidad - 2);

            return Cantidad;
        }
        private void RecorridoInOrden(Nodo NodoAux, ref Dictionary<int, List<int>> DiccionarioFollows) {//Recorre el arbol generado en el modo inorden y luego analiza los follows 
            if (NodoAux != null)
            {
                RecorridoInOrden(NodoAux.getc1(), ref DiccionarioFollows);
                string ContenidoDelNodo = NodoAux.getElementosContenidos();
                if (ContenidoDelNodo == "." && (NodoAux.getc1() != null && NodoAux.getc2() != null))
                {
                    string[] AllLast = NodoAux.getc1().getAsignacionLast().Split(',');
                    string[] AllFirst = NodoAux.getc2().getAsignacionFirst().Split(',');

                    List<int> AllLastL = new List<int>();
                    List<int> AllFirstL = new List<int>();

                    foreach (var Data in AllLast)
                    {
                        AllLastL.Add(Convert.ToInt16(Data));
                    }
                    foreach (var Data in AllFirst)
                    {
                        AllFirstL.Add(Convert.ToInt16(Data));
                    }

                    foreach (var DataLast in AllLastL)
                    {
                        foreach (var DataFirst in AllFirstL)
                        {
                            if (!DiccionarioFollows[DataLast].Contains(DataFirst))
                            {
                                DiccionarioFollows[DataLast].Add(DataFirst);
                            }
                        }
                    }
                }
                else if ((NodoAux.getElementosContenidos() == "*" || NodoAux.getElementosContenidos() == "+") && (NodoAux.getc1() != null))
                {
                    string[] AllLast = NodoAux.getc1().getAsignacionLast().Split(',');
                    string[] AllFirst = NodoAux.getc1().getAsignacionFirst().Split(',');

                    List<int> AllLastL = new List<int>();
                    List<int> AllFirstL = new List<int>();

                    foreach (var Data in AllLast)
                    {
                        AllLastL.Add(Convert.ToInt16(Data));
                    }
                    foreach (var Data in AllFirst)
                    {
                        AllFirstL.Add(Convert.ToInt16(Data));
                    }

                    foreach (var DataLast in AllLastL)
                    {
                        foreach (var DataFirst in AllFirstL)
                        {
                            if (!DiccionarioFollows[DataLast].Contains(DataFirst))
                            {
                                DiccionarioFollows[DataLast].Add(DataFirst);
                            }
                        }
                    }
                }
                RecorridoInOrden(NodoAux.getc2(), ref DiccionarioFollows);
            }
        
        }

        private void RecorridoPostOrden(Nodo NodoAux, ref List<string> ListadoNullsFirstLast) { // en este metodo se recorre el arbol generado para la busqueda de los first y last para la vista del usuario
            if (NodoAux != null)
            {
                RecorridoPostOrden(NodoAux.getc1(), ref ListadoNullsFirstLast);
                RecorridoPostOrden(NodoAux.getc2(), ref ListadoNullsFirstLast);
                string ContenidoDelNodo = NodoAux.getExpresionObtenida();
                string LastNodo = NodoAux.getAsignacionLast();
                string FirstNodo = NodoAux.getAsignacionFirst();

                if (NodoAux.getIsNull())
                {
                    ListadoNullsFirstLast.Add("N \t" + ContenidoDelNodo + "\t\t" + "F(" + FirstNodo + ")" + "  " + "L(" + LastNodo + ")                                              ");                                                          
                }
                else
                {
                    ListadoNullsFirstLast.Add("NN \t" + ContenidoDelNodo + "\t\t" + "F(" + FirstNodo + ")" + "  " + "L(" + LastNodo + ")                                              ");
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
