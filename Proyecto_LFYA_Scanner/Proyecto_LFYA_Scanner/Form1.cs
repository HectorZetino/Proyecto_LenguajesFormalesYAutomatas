using Microsoft.CSharp;
using Proyecto_LFYA_Scanner.Archivo;
using Proyecto_LFYA_Scanner.Componentes;
using Proyecto_LFYA_Scanner.RecursosTabla;
using System;
using System.CodeDom.Compiler;
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
        int contadorAumento = 1;
         
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
            foreach (var item in ListaSets)
            {
                lst_Sets.Items.Add("Listado de Sets: "+contadorAumento);
                foreach (var item2 in item.ElementosSetData)
                {
                    lst_Sets.Items.Add(item.NombreSetData + " = " + item2);
                }
                contadorAumento++;
            }
            FaseDeScanner();

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

        //////////////////////////////////////////////////////////////////METODOS DE LA SEGUNDA FASE////////////////////////////////////////////////////////////////////////////////
                                    ///..archivo necesario para su ejecucion ,este es el archivo de entrada, (Program x a :=b c = d const a)..///
       
        /// <summary>
        /// Metodo para la creacion de la segunda fase del proyecto
        /// </summary>
        private void FaseDeScanner()
        {
            StreamWriter DataStreamWriter = new StreamWriter("C:\\Users\\HRZCz\\OneDrive\\Escritorio\\Program.cs");//Escritura del archivo, donde contendra todos los metodos necesarios, dentro de este escribira el archivo a ejecutar
            //SE PROCEDE A ESCRIBIR TODO EL ENCABEZADO DEL PROGRAM.CS
            DataStreamWriter.WriteLine("using System;");
            DataStreamWriter.WriteLine("using System.Collections.Generic;");
            DataStreamWriter.WriteLine("using System.IO;");
            DataStreamWriter.WriteLine("using System.Linq;");
            DataStreamWriter.WriteLine("using System.Text;");
            DataStreamWriter.WriteLine("using System.Threading.Tasks;");
            DataStreamWriter.WriteLine("");
            DataStreamWriter.WriteLine("namespace Scanner");
            DataStreamWriter.WriteLine("{");
            DataStreamWriter.WriteLine("\tclass Program");
            DataStreamWriter.WriteLine("\t{");
            DataStreamWriter.WriteLine("\t\tpublic static int simboloTerminal = 0;");
            DataStreamWriter.WriteLine("\t\tpublic static Procesos procesos = new Procesos();");
            DataStreamWriter.WriteLine("\t\t");
            DataStreamWriter.WriteLine("\t\tstatic void Main(string[] args)");
            DataStreamWriter.WriteLine("\t\t{");
            DataStreamWriter.WriteLine("\t\t\t");
            //SE PROCEDE A REALIZAR TODOS LOS METODOS CORRESPONDIENTES
            VariablesDeScanner(ref DataStreamWriter);
            LecturaArchivoScanner(ref DataStreamWriter);
            ClasificacionDatosScanner(ref DataStreamWriter);
        
            DataStreamWriter.WriteLine("\t\t\tConsole.ReadKey();");
            DataStreamWriter.WriteLine("\t\t}");

            GenerarAutomataScanner(ref DataStreamWriter);
            DataStreamWriter.WriteLine("\t}");
            DataStreamWriter.WriteLine("\t");
            ClasesScanner(ref DataStreamWriter);
            DataStreamWriter.WriteLine("");
            DataStreamWriter.WriteLine("}");
            DataStreamWriter.WriteLine("");

            DataStreamWriter.Close();
           
            var RutaDelPrograma = "C:\\Users\\HRZCz\\OneDrive\\Escritorio\\SegundafaseArchivo.exe"; /////Parte donde se genera en el .exe , para poder visualizarlo dentro de la carpeta de resultado
            var csc = new CSharpCodeProvider();
            var parameters = new CompilerParameters(new[] { "mscorlib.dll", "System.Core.dll", "System.Data.dll", "System.dll", "System.Collections.dll" }, RutaDelPrograma, true); parameters.GenerateExecutable = true;/*esta parte ayuda para compilar de una forma dinamica 
            y ademas para la ejecucion de fragmento del codigo de c#*/
            parameters.GenerateExecutable = true;

            
            var code = File.ReadAllText("C:\\Users\\HRZCz\\OneDrive\\Escritorio\\Program.cs");//Se procede a leer y alamacenar dentro de la variable code todo las lineas del codigo 
            CompilerResults result = csc.CompileAssemblyFromSource(parameters, code);/*Compila un ensamblaje a partir de la matriz de cadenas especificada que contiene el código fuente,
            utilizando la configuración del compilador especificada.*/
            System.Diagnostics.Process.Start(@"C:\\Users\\HRZCz\\OneDrive\\Escritorio\\SegundafaseArchivo.exe");//mediante esa linea se ejecuta el archivo luego de ingresar el archivo de entrada


        }
        
        
        /// <summary>
         /// Dentro de este metodo se comienza la escritura de los sets,actions,tokens
         /// </summary>
         /// <param name="DataStreamWriter">Continuacion del archivo que se esta creando, en este momento se envia lo que lleva el archivo escrito</param>
        private void VariablesDeScanner(ref StreamWriter DataStreamWriter) {
            DataStreamWriter.WriteLine("\t\t\tint error = " + LineaErrorInte + ";");
            DataStreamWriter.WriteLine("\t\t");

            DataStreamWriter.WriteLine("\t\t\tList<Set> Sets = new List<Set>();");//Se comienza con la escritura del contenido de los SETS
            DataStreamWriter.WriteLine("\t\t\tSet SetTemporal;");
            DataStreamWriter.WriteLine("\t\t\tList<string> elementos;");

            foreach (var setData in ListaSets)//se procede a la busqueda dentro de la lista de set almacenados para agregarlos
            {
                DataStreamWriter.WriteLine("\t\t\t");
                DataStreamWriter.WriteLine("\t\t\tSetTemporal = new Set(\"" + setData.getNombreSetData() + "\");");
                DataStreamWriter.WriteLine("\t\t\telementos = new List<string>();");
                DataStreamWriter.WriteLine("\t\t\t");
                foreach (var elemento in setData.getElementosSetData())
                {
                    char[] ArregloCaracterLetras = elemento.ToCharArray();
                    if (ArregloCaracterLetras.Length == 1)
                    {
                        DataStreamWriter.WriteLine("\t\t\telementos.Add(Convert.ToString(Convert.ToChar(" + Convert.ToInt16(ArregloCaracterLetras[0]) + ")));");
                    }
                    else
                    {
                        DataStreamWriter.WriteLine("\t\t\telementos.Add(\"" + ArregloCaracterLetras + "\");");
                    } 
                }
                DataStreamWriter.WriteLine("\t\t\tSetTemporal.setElementos(elementos);");
                DataStreamWriter.WriteLine("\t\t\tSets.Add(SetTemporal);");

                DataStreamWriter.WriteLine("\t\t\tSetTemporal = new Set();");//reinicio del temporal 
                DataStreamWriter.WriteLine("\t\t\telementos = new List<string>();");//reinicio de elementos
                DataStreamWriter.WriteLine("\t\t\t");

            }
            DataStreamWriter.WriteLine("\t\t\t");

            DataStreamWriter.WriteLine("\t\t\tDictionary<string, int> Tokens = new Dictionary<string, int>();");//se procede a al escritura de los tokens almacenados
            DataStreamWriter.WriteLine("\t\t\t");
            foreach (var token in ListaTokens)
            {
                string CadenaElemento = "";
                char[] ArregloCaracter = token.getElementoTokenData().ToCharArray();
                foreach (var value in ArregloCaracter)
                {
                    if (value == '"')
                    {
                        CadenaElemento += Convert.ToString(Convert.ToChar(92)) + "\"";
                    }
                    else
                    {
                        CadenaElemento += value;
                    }
                }
                DataStreamWriter.WriteLine("\t\t\tTokens.Add(\"" + CadenaElemento + "\", " + token.getNumeroTokenData() + ");");
            }
            //se almacenan los actions almacenadas dentro del diccionario 
            DataStreamWriter.WriteLine("\t\t\t");
            DataStreamWriter.WriteLine("\t\t\t");
            DataStreamWriter.WriteLine("\t\t\tDictionary<string,int> Actions = new Dictionary<string, int>();");
            DataStreamWriter.WriteLine("\t\t\t");
            foreach (var action in DiccionarioActions)
            {
                DataStreamWriter.WriteLine("\t\t\tActions.Add(\"" + action.Key + "\"," + action.Value + ");");
            }
            DataStreamWriter.WriteLine("\t\t\t");
        }

        /// <summary>
        /// En este metodo se recibira el archivo con las entradas, se recibe el archivo que se continua creando.
        /// </summary>
        /// <param name="DataStreamWriter">Continuacion del archivo que se esta creando, en este momento se envia lo que lleva el archivo escrito</param>
        private void LecturaArchivoScanner(ref StreamWriter DataStreamWriter) {

            openFileDialog1.Title = "Ingresa el Archivo contenedor de entradas ||**Archivo Segunda Fase**||";
            openFileDialog1.Filter = "Archivo texto|*.txt";
            openFileDialog1.FileName = "";
            if (openFileDialog1.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            ArchivoRuta = openFileDialog1.FileName;
            DataStreamWriter.WriteLine("\t\t\t");
            DataStreamWriter.WriteLine("\t\t\tstring line;");
            DataStreamWriter.WriteLine("\t\t\t");
            char[] _Path = ArchivoRuta.ToCharArray();
            ArchivoRuta = "";

            foreach (var item in _Path)
            {
                if (item == Convert.ToChar(92))
                {
                    ArchivoRuta += (Convert.ToString(Convert.ToChar(92)) + Convert.ToString(Convert.ToChar(92)));
                }
                else
                {
                    ArchivoRuta += Convert.ToString(item);
                }
            }
            DataStreamWriter.WriteLine("\t\t\tStreamReader _StreamR = new StreamReader(\"" + ArchivoRuta + "\");"); //metodo escrito dentro del archivo con la ruta del archivo de entrada que se ingresara.
            DataStreamWriter.WriteLine("\t\t\tline = _StreamR.ReadLine();");//Proceso dentro del archivo que leera por completo al archivo de entrada.
            DataStreamWriter.WriteLine("\t\t\t");
        }


        /// <summary>
        /// Proceso que realiza la revision del archivo que se ingresa a la aplicacion, se procede a realizar una separacion de las entradas
        /// </summary>
        /// <param name="DataStreamWriter">Continuacion del archivo que se esta creando, en este momento se envia lo que lleva el archivo escrito</param>
        private void ClasificacionDatosScanner(ref StreamWriter DataStreamWriter) {
            DataStreamWriter.WriteLine("\t\t\tstring[] _fragmento = line.Split(' ');");//se toman lo que ingrese en archivo de entrada y se separa por espacios
            DataStreamWriter.WriteLine("\t\t\t");
            DataStreamWriter.WriteLine("\t\t\tforeach (var fragmento in _fragmento) //Al momento de ser separados por espacios cada una de las entradas se logra obtener cada uno de los valores");//cada fragmento del archivo de entrada se tomara de los que se encuentran en el arreglo de fragmento
            DataStreamWriter.WriteLine("\t\t\t{");
            DataStreamWriter.WriteLine("\t\t\t\tint TokenI = -1;");
            DataStreamWriter.WriteLine("\t\t\t\tint TokenL = -1;");
            DataStreamWriter.WriteLine("\t\t\t\t");
            DataStreamWriter.WriteLine("\t\t\t\tif (Actions.ContainsKey(fragmento)) //se verifica si dentro de los actions ya existe el fragmento ingresado. ");
            DataStreamWriter.WriteLine("\t\t\t\t{");
            DataStreamWriter.WriteLine("\t\t\t\tConsole.WriteLine(fragmento + \"\t=\t\"+ Actions[fragmento]);");
            DataStreamWriter.WriteLine("\t\t\t\t}");
            DataStreamWriter.WriteLine("\t\t\t\telse");
            DataStreamWriter.WriteLine("\t\t\t\t{");
            DataStreamWriter.WriteLine("\t\t\t\t\tforeach (var item in Tokens) //Metodo para poder analizar cada token existente y recorrerlo");
            DataStreamWriter.WriteLine("\t\t\t\t\t{");
            DataStreamWriter.WriteLine("\t\t\t\t\t\tList<string> TokenDirect = procesos.VerificarTokenDirecto(item.Key);");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t");
            DataStreamWriter.WriteLine("\t\t\t\t\t\tif (TokenDirect.Count() != 0)");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t{");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\tchar[] caracteres = fragmento.ToCharArray();");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\tbool coincide = true;");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\ttry");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t{");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\tfor (int i = 0; i < caracteres.Length; i++)");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\t{");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\t\tif (!Convert.ToString(caracteres[i]).Equals(TokenDirect[i]))");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\t\t\tcoincide = false;");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\t}");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t}");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\tcatch");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t{");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\tcoincide = false;");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t}");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\tif (coincide)");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\tTokenI = item.Value;");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t}");
            DataStreamWriter.WriteLine("\t\t\t\t\t\telse");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t{");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\tQueue<string> Expresion = procesos.ConvertToSets(Sets, fragmento);");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\tColumna[] Encabezado = new Columna[100];");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t");

            DataStreamWriter.WriteLine("\t\t\t\t\t\t\tTransicion[,] TablaDeTransiciones = OperarArchivo(Sets, item.Key, ref Encabezado);");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\tif (procesos.AnalizarEntrada(Encabezado, TablaDeTransiciones, Expresion, simboloTerminal))");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\tTokenL = item.Value;");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t}");
            DataStreamWriter.WriteLine("\t\t\t\t\t}");
            DataStreamWriter.WriteLine("\t\t\t\t\t");
            DataStreamWriter.WriteLine("\t\t\t\t\tif (TokenI != -1)");
            DataStreamWriter.WriteLine("\t\t\t\t\t\tConsole.WriteLine(fragmento + \"\t=\t\" + TokenI);");
            DataStreamWriter.WriteLine("\t\t\t\t\telse if (TokenL != -1)");
            DataStreamWriter.WriteLine("\t\t\t\t\t\tConsole.WriteLine(fragmento + \"\t=\t\" + TokenL);");
            DataStreamWriter.WriteLine("\t\t\t\t\telse");
            DataStreamWriter.WriteLine("\t\t\t\t\t\tConsole.WriteLine(\"ERROR\t\t\" + error);");
            DataStreamWriter.WriteLine("\t\t\t\t\t");
            DataStreamWriter.WriteLine("\t\t\t\t}");
            DataStreamWriter.WriteLine("\t\t\t}");
            DataStreamWriter.WriteLine("\t\t\t");

        }
        
        /// <summary>
        /// Maneja todo el proceso de nuevamente la creacion del automata, ademas de tomar los valores y crear nuevamente la tabla de transiciones
        /// </summary>
        /// <param name="DataStreamWriter">Continuacion del archivo que se esta creando, en este momento se envia lo que lleva el archivo escrito</param>
        private void GenerarAutomataScanner(ref StreamWriter DataStreamWriter) {
            DataStreamWriter.WriteLine("\t\t");
            DataStreamWriter.WriteLine("\t\tstatic private Transicion[,] OperarArchivo(List<Set> Sets, string Token, ref Columna[] Encabezado)");
            DataStreamWriter.WriteLine("\t\t{");
            DataStreamWriter.WriteLine("\t\t\tint leaf = 1;");
            DataStreamWriter.WriteLine("\t\t\tint cont = 1;");
            DataStreamWriter.WriteLine("\t\t\tStack<Node> Posfijo = new Stack<Node>();");
            DataStreamWriter.WriteLine("\t\t\tList<Node> Leafs = new List<Node>();");
            DataStreamWriter.WriteLine("\t\t\t");
            DataStreamWriter.WriteLine("\t\t\t//La expresion regular se valida que es el token que se esta trabajando");
            DataStreamWriter.WriteLine("\t\t\tstring ER = Token;");
            DataStreamWriter.WriteLine("\t\t\t");
            DataStreamWriter.WriteLine("\t\t\tProcesos procesos = new Procesos();");
            DataStreamWriter.WriteLine("\t\t\tprocesos.ObtenerPosfijo(ref Posfijo, ref Leafs, Sets, ER, ref cont, ref leaf);");
            DataStreamWriter.WriteLine("\t\t\t");
            DataStreamWriter.WriteLine("\t\t\tif (Posfijo.Count == 2)");
            DataStreamWriter.WriteLine("\t\t\t{");
            DataStreamWriter.WriteLine("\t\t\t\tNode Operador = new Node();");
            DataStreamWriter.WriteLine("\t\t\t\tOperador.setContenido(Convert.ToString('|'));");
            DataStreamWriter.WriteLine("\t\t\t\t");
            DataStreamWriter.WriteLine("\t\t\t\tNode C2 = Posfijo.Pop();");
            DataStreamWriter.WriteLine("\t\t\t\tNode C1 = Posfijo.Pop();");
            DataStreamWriter.WriteLine("\t\t\t\t");
            DataStreamWriter.WriteLine("\t\t\t\t//se validan los Nulables");
            DataStreamWriter.WriteLine("\t\t\t\t");
            DataStreamWriter.WriteLine("\t\t\t\tif (C1.getNulable() || C2.getNulable())");
            DataStreamWriter.WriteLine("\t\t\t\t\tOperador.setNulable(true);");
            DataStreamWriter.WriteLine("\t\t\t\telse");
            DataStreamWriter.WriteLine("\t\t\t\t\tOperador.setNulable(false);");
            DataStreamWriter.WriteLine("\t\t\t\t");
            DataStreamWriter.WriteLine("\t\t\t\tOperador.setFirst(C1.getFirst() + \", \" + C2.getFirst());");
            DataStreamWriter.WriteLine("\t\t\t\tOperador.setLast(C1.getLast() + \", \" + C2.getLast());");
            DataStreamWriter.WriteLine("\t\t\t\t");
            DataStreamWriter.WriteLine("\t\t\t\tOperador.setC1(C1);");
            DataStreamWriter.WriteLine("\t\t\t\tOperador.setC2(C2);");
            DataStreamWriter.WriteLine("\t\t\t\t");
            DataStreamWriter.WriteLine("\t\t\t\tOperador.setExpresionAcumulada(C1.getExpresionAcumulada() + Operador.getContenido() + C2.getExpresionAcumulada());");
            DataStreamWriter.WriteLine("\t\t\t\tPosfijo.Push(Operador);");
            DataStreamWriter.WriteLine("\t\t\t}");
            DataStreamWriter.WriteLine("\t\t\t");
            DataStreamWriter.WriteLine("\t\t\tcont = 1;");
            DataStreamWriter.WriteLine("\t\t\t");
            DataStreamWriter.WriteLine("\t\t\tprocesos.ObtenerPosfijo(ref Posfijo, ref Leafs, Sets, \"(#)\", ref cont, ref leaf);");
            DataStreamWriter.WriteLine("\t\t\t");
            DataStreamWriter.WriteLine("\t\t\t// Se realiza la union de los valores resultantes con el '#'");
            DataStreamWriter.WriteLine("\t\t\tNode FOperador = new Node();");
            DataStreamWriter.WriteLine("\t\t\tFOperador.setContenido(Convert.ToString('.'));");
            DataStreamWriter.WriteLine("\t\t\t");
            DataStreamWriter.WriteLine("\t\t\tNode FC2 = Posfijo.Pop();");
            DataStreamWriter.WriteLine("\t\t\tNode FC1 = Posfijo.Pop();");
            DataStreamWriter.WriteLine("\t\t\t");
            DataStreamWriter.WriteLine("\t\t\t//se validan los Nulables");
            DataStreamWriter.WriteLine("\t\t\tif (FC1.getNulable() || FC2.getNulable())");
            DataStreamWriter.WriteLine("\t\t\t\tFOperador.setNulable(true);");
            DataStreamWriter.WriteLine("\t\t\telse");
            DataStreamWriter.WriteLine("\t\t\t\tFOperador.setNulable(false);");
            DataStreamWriter.WriteLine("\t\t\t");
            DataStreamWriter.WriteLine("\t\t\t// Se realiza el proceso de first y last de los nodos");
            DataStreamWriter.WriteLine("\t\t\tif (FC1.getNulable())");
            DataStreamWriter.WriteLine("\t\t\t\tFOperador.setFirst(FC1.getFirst() + \", \" + FC2.getFirst());");
            DataStreamWriter.WriteLine("\t\t\telse");
            DataStreamWriter.WriteLine("\t\t\t\tFOperador.setFirst(FC1.getFirst());");
            DataStreamWriter.WriteLine("\t\t\t");
            DataStreamWriter.WriteLine("\t\t\tif (FC2.getNulable())");
            DataStreamWriter.WriteLine("\t\t\t\tFOperador.setLast(FC1.getLast() + \", \" + FC2.getLast());");
            DataStreamWriter.WriteLine("\t\t\telse");
            DataStreamWriter.WriteLine("\t\t\t\tFOperador.setLast(FC2.getLast());");
            DataStreamWriter.WriteLine("\t\t\t");
            DataStreamWriter.WriteLine("\t\t\tFOperador.setC1(FC1);");
            DataStreamWriter.WriteLine("\t\t\tFOperador.setC2(FC2);");
            DataStreamWriter.WriteLine("\t\t\t");
            DataStreamWriter.WriteLine("\t\t\tFOperador.setExpresionAcumulada(FC1.getExpresionAcumulada() + FOperador.getContenido() + FC2.getExpresionAcumulada());");
            DataStreamWriter.WriteLine("\t\t\t");
            DataStreamWriter.WriteLine("\t\t\tPosfijo.Push(FOperador); //valida y asigna el operador final");
            DataStreamWriter.WriteLine("\t\t\t");
            DataStreamWriter.WriteLine("\t\t\t// -----------------------------------");
            DataStreamWriter.WriteLine("\t\t\t");
            DataStreamWriter.WriteLine("\t\t\tDictionary<int, List<int>> Follows = new Dictionary<int, List<int>>();");
            DataStreamWriter.WriteLine("\t\t\t");
            DataStreamWriter.WriteLine("\t\t\tfor (int i = 1; i < leaf; i++)");
            DataStreamWriter.WriteLine("\t\t\t{");
            DataStreamWriter.WriteLine("\t\t\t\tList<int> Follow = new List<int>();");
            DataStreamWriter.WriteLine("\t\t\t\tFollows.Add(i, Follow);");
            DataStreamWriter.WriteLine("\t\t\t}");
            DataStreamWriter.WriteLine("\t\t\t");
            DataStreamWriter.WriteLine("\t\t\tInOrden(Posfijo.Peek(), ref Follows); // Realiza una busqueda follows de los nodos");
            DataStreamWriter.WriteLine("\t\t\t");
            DataStreamWriter.WriteLine("\t\t\tforeach (var item in Follows)");
            DataStreamWriter.WriteLine("\t\t\t{");
            DataStreamWriter.WriteLine("\t\t\t\tstring elemento = \"\";");
            DataStreamWriter.WriteLine("\t\t\t\tforeach (var elementos in item.Value)");
            DataStreamWriter.WriteLine("\t\t\t\t{");
            DataStreamWriter.WriteLine("\t\t\t\t\telemento += elementos + \", \";");
            DataStreamWriter.WriteLine("\t\t\t\t}");
            DataStreamWriter.WriteLine("\t\t\t}");
            DataStreamWriter.WriteLine("\t\t\t");
            DataStreamWriter.WriteLine("\t\t\tList<string> FirstPadre = new List<string>(); // Se transforma el first en una lista, Convierte el first en una lista manipulable");
            DataStreamWriter.WriteLine("\t\t\tstring[] FPadre = FOperador.getFirst().Split(',');");
            DataStreamWriter.WriteLine("\t\t\t");
            DataStreamWriter.WriteLine("\t\t\tforeach (var item in FPadre)");
            DataStreamWriter.WriteLine("\t\t\t{");
            DataStreamWriter.WriteLine("\t\t\t\tFirstPadre.Add(item);");
            DataStreamWriter.WriteLine("\t\t\t}");
            DataStreamWriter.WriteLine("\t\t\t");
            DataStreamWriter.WriteLine("\t\t\tTransicion[,] TablaDeTransiciones = TablaTransiciones(FirstPadre, Leafs, Follows, ref Encabezado);");
            DataStreamWriter.WriteLine("\t\t\treturn TablaDeTransiciones;");
            DataStreamWriter.WriteLine("\t\t}");
            DataStreamWriter.WriteLine("\t\t");
            DataStreamWriter.WriteLine("\t\tstatic private void InOrden(Node nodoAuxiliar, ref Dictionary<int, List<int>> Follows)");
            DataStreamWriter.WriteLine("\t\t{");
            DataStreamWriter.WriteLine("\t\t\tif (nodoAuxiliar != null)");
            DataStreamWriter.WriteLine("\t\t\t{");
            DataStreamWriter.WriteLine("\t\t\t\tInOrden(nodoAuxiliar.getC1(), ref Follows);");
            DataStreamWriter.WriteLine("\t\t\t\t");
            DataStreamWriter.WriteLine("\t\t\t\tstring contNodo = nodoAuxiliar.getContenido();");
            DataStreamWriter.WriteLine("\t\t\t\tif (contNodo == \".\" && (nodoAuxiliar.getC1() != null && nodoAuxiliar.getC2() != null))");
            DataStreamWriter.WriteLine("\t\t\t\t{");
            DataStreamWriter.WriteLine("\t\t\t\t\tstring[] lasts = nodoAuxiliar.getC1().getLast().Split(',');");
            DataStreamWriter.WriteLine("\t\t\t\t\tstring[] first = nodoAuxiliar.getC2().getFirst().Split(',');");
            DataStreamWriter.WriteLine("\t\t\t\t\t");
            DataStreamWriter.WriteLine("\t\t\t\t\tList<int> Lasts = new List<int>();");
            DataStreamWriter.WriteLine("\t\t\t\t\tList<int> First = new List<int>();");
            DataStreamWriter.WriteLine("\t\t\t\t\t");
            DataStreamWriter.WriteLine("\t\t\t\t\tforeach (var item in lasts)");
            DataStreamWriter.WriteLine("\t\t\t\t\t\tLasts.Add(Convert.ToInt16(item));");
            DataStreamWriter.WriteLine("\t\t\t\t\t");
            DataStreamWriter.WriteLine("\t\t\t\t\tforeach (var item in first)");
            DataStreamWriter.WriteLine("\t\t\t\t\t\tFirst.Add(Convert.ToInt16(item));");
            DataStreamWriter.WriteLine("\t\t\t\t\t");
            DataStreamWriter.WriteLine("\t\t\t\t\tforeach (var itemLast in Lasts)");
            DataStreamWriter.WriteLine("\t\t\t\t\t{");
            DataStreamWriter.WriteLine("\t\t\t\t\t\tforeach (var itemFirst in First)");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t{");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\tif (!Follows[itemLast].Contains(itemFirst))");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\tFollows[itemLast].Add(itemFirst);");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t}");
            DataStreamWriter.WriteLine("\t\t\t\t\t}");
            DataStreamWriter.WriteLine("\t\t\t\t}");
            DataStreamWriter.WriteLine("\t\t\t\telse if ((nodoAuxiliar.getContenido() == \"*\" || nodoAuxiliar.getContenido() == \"+\") && (nodoAuxiliar.getC1() != null))");
            DataStreamWriter.WriteLine("\t\t\t\t{");
            DataStreamWriter.WriteLine("\t\t\t\t\tstring[] lasts = nodoAuxiliar.getC1().getLast().Split(',');");
            DataStreamWriter.WriteLine("\t\t\t\t\tstring[] first = nodoAuxiliar.getC1().getFirst().Split(',');");
            DataStreamWriter.WriteLine("\t\t\t\t\t");
            DataStreamWriter.WriteLine("\t\t\t\t\tList<int> Lasts = new List<int>();");
            DataStreamWriter.WriteLine("\t\t\t\t\tList<int> First = new List<int>();");
            DataStreamWriter.WriteLine("\t\t\t\t\t");
            DataStreamWriter.WriteLine("\t\t\t\t\tforeach (var item in lasts)");
            DataStreamWriter.WriteLine("\t\t\t\t\t\tLasts.Add(Convert.ToInt16(item));");
            DataStreamWriter.WriteLine("\t\t\t\t\t");
            DataStreamWriter.WriteLine("\t\t\t\t\tforeach (var item in first)");
            DataStreamWriter.WriteLine("\t\t\t\t\t\tFirst.Add(Convert.ToInt16(item));");
            DataStreamWriter.WriteLine("\t\t\t\t\t");
            DataStreamWriter.WriteLine("\t\t\t\t\tforeach (var itemLast in Lasts)");
            DataStreamWriter.WriteLine("\t\t\t\t\t{");
            DataStreamWriter.WriteLine("\t\t\t\t\t\tforeach (var itemFirst in First)");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t{");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\tif (!Follows[itemLast].Contains(itemFirst))");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t{");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\tFollows[itemLast].Add(itemFirst);");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t}");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t}");
            DataStreamWriter.WriteLine("\t\t\t\t\t}");
            DataStreamWriter.WriteLine("\t\t\t\t}");
            DataStreamWriter.WriteLine("\t\t\t\t");
            DataStreamWriter.WriteLine("\t\t\tInOrden(nodoAuxiliar.getC2(), ref Follows);");
            DataStreamWriter.WriteLine("\t\t\t\t");
            DataStreamWriter.WriteLine("\t\t\t}");
            DataStreamWriter.WriteLine("\t\t}");
            DataStreamWriter.WriteLine("\t\t\t");
            DataStreamWriter.WriteLine("\t\t\t");
            DataStreamWriter.WriteLine("\t\tstatic private Transicion[,] TablaTransiciones(List<string> FirstPadre, List<Node> Leafs, Dictionary<int, List<int>> Follows, ref Columna[] Encabezado)");
            DataStreamWriter.WriteLine("\t\t{");
            DataStreamWriter.WriteLine("\t\t\tEncabezado = ConstruirColumnas(Leafs);");
            DataStreamWriter.WriteLine("\t\t\tint filas = CantFilas(FirstPadre, Encabezado, Follows);");
            DataStreamWriter.WriteLine("\t\t\t");
            DataStreamWriter.WriteLine("\t\t\tTransicion[,] TablaDeTransiciones = new Transicion[filas, Encabezado.Length + 1];");
            DataStreamWriter.WriteLine("\t\t\t");
            DataStreamWriter.WriteLine("\t\t\tint caracter = 65;");
            DataStreamWriter.WriteLine("\t\t\tchar letra = Convert.ToChar(caracter);");
            DataStreamWriter.WriteLine("\t\t\t");
            DataStreamWriter.WriteLine("\t\t\tList<Transicion> Transiciones = new List<Transicion>(); // Captura toda la cantidad de estados de la tabla de transiciones");
            DataStreamWriter.WriteLine("\t\t\tQueue<Transicion> Pendientes = new Queue<Transicion>(); //Captura todos los estados que aun se encuentran pendientes de ser analizados que se encuentran en la tabla de transiciones");
            DataStreamWriter.WriteLine("\t\t\t");
            DataStreamWriter.WriteLine("\t\t\t// Inicializacion de la matriz");
            DataStreamWriter.WriteLine("\t\t\tfor (int i = 0; i < filas; i++) //En este momento se realiza un inicio de la matriz ");
            DataStreamWriter.WriteLine("\t\t\t{");
            DataStreamWriter.WriteLine("\t\t\t\tfor (int j = 0; j < (Encabezado.Length + 1); j++)");
            DataStreamWriter.WriteLine("\t\t\t\t{");
            DataStreamWriter.WriteLine("\t\t\t\t\tTablaDeTransiciones[i, j] = new Transicion();");
            DataStreamWriter.WriteLine("\t\t\t\t}");
            DataStreamWriter.WriteLine("\t\t\t}");
            DataStreamWriter.WriteLine("\t\t\t");
            DataStreamWriter.WriteLine("\t\t\tTablaDeTransiciones[0, 0].setElementos(FirstPadre);");
            DataStreamWriter.WriteLine("\t\t\tTablaDeTransiciones[0, 0].setIdentificador(Convert.ToString(letra));");
            DataStreamWriter.WriteLine("\t\t\t");
            DataStreamWriter.WriteLine("\t\t\tTransicion temp = new Transicion(Convert.ToString(letra), FirstPadre); // Se asigna el valor de que sera el punto de inicio de la tabla transiciones");
            DataStreamWriter.WriteLine("\t\t\tTransiciones.Add(temp);");
            DataStreamWriter.WriteLine("\t\t\tletra++;");
            DataStreamWriter.WriteLine("\t\t\t");
            DataStreamWriter.WriteLine("\t\t\tint fila = 0;");
            DataStreamWriter.WriteLine("\t\t\tbool analizar = true;");
            DataStreamWriter.WriteLine("\t\t\tbool UltimaIteracion = false;");
            DataStreamWriter.WriteLine("\t\t\t");
            DataStreamWriter.WriteLine("\t\t\tdo");
            DataStreamWriter.WriteLine("\t\t\t{");
            DataStreamWriter.WriteLine("\t\t\t\tif (UltimaIteracion == true)// comodin para no continuar analizando validnando solo la ultima iteracion que es la posicion actual");
            DataStreamWriter.WriteLine("\t\t\t\t\tanalizar = false;");
            DataStreamWriter.WriteLine("\t\t\t\t");
            DataStreamWriter.WriteLine("\t\t\t\tforeach (var Columna in Encabezado)// recorrido de cada una de las hojas por columna");
            DataStreamWriter.WriteLine("\t\t\t\t{");
            DataStreamWriter.WriteLine("\t\t\t\t\tforeach (var Elemento in temp.getElementos())//capturar los elementos del first que se esta trabajando");
            DataStreamWriter.WriteLine("\t\t\t\t\t{");
            DataStreamWriter.WriteLine("\t\t\t\t\t\tif (Columna.getHojas().Contains(Convert.ToInt16(Elemento)))");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t{");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\tList<int> ElementosFollow = Follows[Convert.ToInt16(Elemento)];");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\tList<string> Elementos = new List<string>();// metodo que realiza la conversion de los follows de int a string.");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\tforeach (var item in ElementosFollow)");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\tElementos.Add(Convert.ToString(item));");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\tint posicion = Columna.getNumColumna();");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\tTablaDeTransiciones[fila, posicion].setElementos(Elementos);");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\tbool añadir = true;");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\tforeach (var item in Transiciones)");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t{");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\tif (TablaDeTransiciones[fila, Columna.getNumColumna()].getElementos() == item.getElementos())");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\t\tañadir = false;");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t}");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\tif (añadir)");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t{");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\tbool letraExistente = false;");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\t");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\tstring letraAnterior = \"\";");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\t");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\tforeach (var item in Transiciones)");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\t{");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\t\t//Se procede a la conversion en estring de los valores para comparar si son el mismo elemento existente en la tabla");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\t\tstring cadena1 = \"\";");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\t\tstring cadena2 = \"\";");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\t\t");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\t\tforeach (var elemento in item.getElementos())");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\t\t\tcadena1 += elemento;");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\t\t");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\t\tforeach (var elemento in Elementos)");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\t\t\tcadena2 += elemento;");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\t\t");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\t\tif (cadena1.Equals(cadena2))");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\t\t{");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\t\t\tletraExistente = true;");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\t\t\tletraAnterior = item.getIdentificador();");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\t\t\tbreak;");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\t\t}");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\t}");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\t");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\tif (letraExistente)");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\t\tTablaDeTransiciones[fila, Columna.getNumColumna()].setIdentificador(letraAnterior);");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\telse");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\t{");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\t\tTablaDeTransiciones[fila, Columna.getNumColumna()].setIdentificador(Convert.ToString(letra));");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\t\tletra++;");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\t\tTransiciones.Add(TablaDeTransiciones[fila, Columna.getNumColumna()]);");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\t\tPendientes.Enqueue(TablaDeTransiciones[fila, Columna.getNumColumna()]);");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\t\t");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\t\tif (Pendientes.Count != 0 && analizar == false)");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\t\t\tanalizar = true;");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\t}");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t}");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t}");
            DataStreamWriter.WriteLine("\t\t\t\t\t}");
            DataStreamWriter.WriteLine("\t\t\t\t}");
            DataStreamWriter.WriteLine("\t\t\t\t");
            DataStreamWriter.WriteLine("\t\t\t\ttry");
            DataStreamWriter.WriteLine("\t\t\t\t{");
            DataStreamWriter.WriteLine("\t\t\t\t\tfila++;");
            DataStreamWriter.WriteLine("\t\t\t\t\ttemp = Pendientes.Dequeue();");
            DataStreamWriter.WriteLine("\t\t\t\t\tFirstPadre = temp.getElementos();");
            DataStreamWriter.WriteLine("\t\t\t\t\t");
            DataStreamWriter.WriteLine("\t\t\t\t\tTablaDeTransiciones[fila, 0].setIdentificador(temp.getIdentificador());");
            DataStreamWriter.WriteLine("\t\t\t\t\tTablaDeTransiciones[fila, 0].setElementos(temp.getElementos());");
            DataStreamWriter.WriteLine("\t\t\t\t\t");
            DataStreamWriter.WriteLine("\t\t\t\t\tstring identificador = temp.getIdentificador();");
            DataStreamWriter.WriteLine("\t\t\t\t\t");
            DataStreamWriter.WriteLine("\t\t\t\t}");
            DataStreamWriter.WriteLine("\t\t\t\tcatch (Exception e) { }");
            DataStreamWriter.WriteLine("\t\t\t");
            DataStreamWriter.WriteLine("\t\t\tif (Pendientes.Count == 0) // Cuando no existan elementos dentro cola se procede a realizar el metodo de UltimaIteracion");
            DataStreamWriter.WriteLine("\t\t\tUltimaIteracion = true;");
            DataStreamWriter.WriteLine("\t\t\t");
            DataStreamWriter.WriteLine("\t\t\t} while (analizar);");
            DataStreamWriter.WriteLine("\t\t\t");
            DataStreamWriter.WriteLine("\t\t\tsimboloTerminal = Leafs.Count;//realiza el recorrido para encontrar o verificar el simbolo terminal");
            DataStreamWriter.WriteLine("\t\t\t");
            DataStreamWriter.WriteLine("\t\t\treturn TablaDeTransiciones;");
            DataStreamWriter.WriteLine("\t\t}");
            DataStreamWriter.WriteLine("\t\t");
            DataStreamWriter.WriteLine("\t\tstatic private Columna[] ConstruirColumnas(List<Node> Leafs)");
            DataStreamWriter.WriteLine("\t\t{");
            DataStreamWriter.WriteLine("\t\t\tList<string> nombres = new List<string>();");
            DataStreamWriter.WriteLine("\t\t\tforeach (var item in Leafs)");
            DataStreamWriter.WriteLine("\t\t\t{");
            DataStreamWriter.WriteLine("\t\t\t\tif (!nombres.Contains(item.getContenido()) && item.getContenido() != \"#\")");
            DataStreamWriter.WriteLine("\t\t\t\t\tnombres.Add(item.getContenido());");
            DataStreamWriter.WriteLine("\t\t\t}");
            DataStreamWriter.WriteLine("\t\t\t");
            DataStreamWriter.WriteLine("\t\t\tColumna[] Encabezado = new Columna[nombres.Count];");
            DataStreamWriter.WriteLine("\t\t\t");
            DataStreamWriter.WriteLine("\t\t\tfor (int i = 0; i < Encabezado.Length; i++)");
            DataStreamWriter.WriteLine("\t\t\t{");
            DataStreamWriter.WriteLine("\t\t\t\tEncabezado[i] = new Columna();");
            DataStreamWriter.WriteLine("\t\t\t\tif (nombres[i] == \"α\")");
            DataStreamWriter.WriteLine("\t\t\t\t\tEncabezado[i].setNombre(\"(\");");
            DataStreamWriter.WriteLine("\t\t\t\telse if (nombres[i] == \"β\")");
            DataStreamWriter.WriteLine("\t\t\t\t\tEncabezado[i].setNombre(\")\");");
            DataStreamWriter.WriteLine("\t\t\t\telse if (nombres[i] == \"ɣ\")");
            DataStreamWriter.WriteLine("\t\t\t\t\tEncabezado[i].setNombre(\".\");");
            DataStreamWriter.WriteLine("\t\t\t\telse if (nombres[i] == \"δ\")");
            DataStreamWriter.WriteLine("\t\t\t\t\tEncabezado[i].setNombre(\"*\");");
            DataStreamWriter.WriteLine("\t\t\t\telse if (nombres[i] == \"ε\")");
            DataStreamWriter.WriteLine("\t\t\t\t\tEncabezado[i].setNombre(\"+\");");
            DataStreamWriter.WriteLine("\t\t\t\telse if (nombres[i] == \"ϑ\")");
            DataStreamWriter.WriteLine("\t\t\t\t\tEncabezado[i].setNombre(\"?\");");
            DataStreamWriter.WriteLine("\t\t\t\telse");
            DataStreamWriter.WriteLine("\t\t\t\t\tEncabezado[i].setNombre(nombres[i]);");
            DataStreamWriter.WriteLine("\t\t\t\tEncabezado[i].setNombre(nombres[i]);");
            DataStreamWriter.WriteLine("\t\t\t\tEncabezado[i].setNumColumna(i + 1);");
            DataStreamWriter.WriteLine("\t\t\t\t");
            DataStreamWriter.WriteLine("\t\t\t\tList<int> hojas = new List<int>();");
            DataStreamWriter.WriteLine("\t\t\t\t");
            DataStreamWriter.WriteLine("\t\t\t\tforeach (var item in Leafs)");
            DataStreamWriter.WriteLine("\t\t\t\t{");
            DataStreamWriter.WriteLine("\t\t\t\t\tif (item.getContenido() == Encabezado[i].getNombre())");
            DataStreamWriter.WriteLine("\t\t\t\t\t\thojas.Add(Convert.ToInt16(item.getFirst()));");
            DataStreamWriter.WriteLine("\t\t\t\t}");
            DataStreamWriter.WriteLine("\t\t\t\t");
            DataStreamWriter.WriteLine("\t\t\t\tEncabezado[i].setHojas(hojas);");
            DataStreamWriter.WriteLine("\t\t\t}");
            DataStreamWriter.WriteLine("\t\t\t");
            DataStreamWriter.WriteLine("\t\t\t");
            DataStreamWriter.WriteLine("\t\t\treturn Encabezado;");
            DataStreamWriter.WriteLine("\t\t}");
            DataStreamWriter.WriteLine("\t\t");
            DataStreamWriter.WriteLine("\t\t");
            DataStreamWriter.WriteLine("\t\tstatic private int CantFilas(List<string> FirstPadre, Columna[] Encabezado, Dictionary<int, List<int>> Follows)");
            DataStreamWriter.WriteLine("\t\t{");
            DataStreamWriter.WriteLine("\t\t\tint cant = 1;");
            DataStreamWriter.WriteLine("\t\t\tint caracter = 65;");
            DataStreamWriter.WriteLine("\t\t\tchar letra = Convert.ToChar(caracter);");
            DataStreamWriter.WriteLine("\t\t\t");
            DataStreamWriter.WriteLine("\t\t\tList<Transicion> Transiciones = new List<Transicion>();");
            DataStreamWriter.WriteLine("\t\t\tQueue<Transicion> Pendientes = new Queue<Transicion>();");
            DataStreamWriter.WriteLine("\t\t\t");
            DataStreamWriter.WriteLine("\t\t\tTransicion[] Linea = new Transicion[Encabezado.Length + 1];");
            DataStreamWriter.WriteLine("\t\t\t");
            DataStreamWriter.WriteLine("\t\t\t// Inicializacion de la lista");
            DataStreamWriter.WriteLine("\t\t\tfor (int i = 0; i < Linea.Length; i++)");
            DataStreamWriter.WriteLine("\t\t\t\tLinea[i] = new Transicion();");
            DataStreamWriter.WriteLine("\t\t\t");
            DataStreamWriter.WriteLine("\t\t\tLinea[0].setElementos(FirstPadre);");
            DataStreamWriter.WriteLine("\t\t\tLinea[0].setIdentificador(Convert.ToString(letra));");
            DataStreamWriter.WriteLine("\t\t\t");
            DataStreamWriter.WriteLine("\t\t\t// Punto inicial de la tabla de transiciones");
            DataStreamWriter.WriteLine("\t\t\tTransicion temp = new Transicion(Convert.ToString(letra), FirstPadre);");
            DataStreamWriter.WriteLine("\t\t\tTransiciones.Add(temp);");
            DataStreamWriter.WriteLine("\t\t\tletra++;");
            DataStreamWriter.WriteLine("\t\t\t");
            DataStreamWriter.WriteLine("\t\t\tbool analizar = true;");
            DataStreamWriter.WriteLine("\t\t\tbool UltimaIteracion = false;");
            DataStreamWriter.WriteLine("\t\t\t");
            DataStreamWriter.WriteLine("\t\t\tdo");
            DataStreamWriter.WriteLine("\t\t\t{");
            DataStreamWriter.WriteLine("\t\t\t\tif (UltimaIteracion == true)//  comodin para no continuar analizando validnando solo la ultima iteracion que es la posicion actual");
            DataStreamWriter.WriteLine("\t\t\t\t\tanalizar = false;");
            DataStreamWriter.WriteLine("\t\t\t\t");
            DataStreamWriter.WriteLine("\t\t\t\t// recorrido de cada una de las hojas por columna");
            DataStreamWriter.WriteLine("\t\t\t\tforeach (var Columna in Encabezado)");
            DataStreamWriter.WriteLine("\t\t\t\t{");
            DataStreamWriter.WriteLine("\t\t\t\t\t//Se capturan los elementos existentes dentro del firts que se esta visualizando en este momento");
            DataStreamWriter.WriteLine("\t\t\t\t\tforeach (var Elemento in temp.getElementos())");
            DataStreamWriter.WriteLine("\t\t\t\t\t{");
            DataStreamWriter.WriteLine("\t\t\t\t\t\tif (Columna.getHojas().Contains(Convert.ToInt16(Elemento)))");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t{");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\tList<int> ElementosFollow = Follows[Convert.ToInt16(Elemento)];");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t//realiza una conversion de los elementos del follow de un elemento en int a string");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\tList<string> Elementos = new List<string>();");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\tforeach (var item in ElementosFollow)");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\tElementos.Add(Convert.ToString(item));");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\tint posicion = Columna.getNumColumna();");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\tLinea[posicion].setElementos(Elementos);");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\tbool añadir = true;");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\tforeach (var item in Transiciones)");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t{");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\tif (Linea[Columna.getNumColumna()].getElementos() == item.getElementos())");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\t\tañadir = false;");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t}");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\tif (añadir)");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t{");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\tbool letraExistente = false;");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\tstring letraAnterior = \"\";");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\t");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\tforeach (var item in Transiciones)");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\t{");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\t\t//Se procede a la conversion en estring de los valores para comparar si son el mismo elemento existente en la tabla");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\t\tstring cadena1 = \"\";");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\t\tstring cadena2 = \"\";");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\t\t");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\t\tforeach (var elemento in item.getElementos())");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\t\t\tcadena1 += elemento;");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\t\t");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\t\tforeach (var elemento in Elementos)");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\t\t\tcadena2 += elemento;");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\t\t");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\t\tif (cadena1.Equals(cadena2))");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\t\t{");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\t\t\tletraExistente = true;");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\t\t\tletraAnterior = item.getIdentificador();");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\t\t\tbreak;");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\t\t}");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\t}");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\t");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\tif (letraExistente)");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\t{");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\t\tLinea[Columna.getNumColumna()].setIdentificador(letraAnterior);");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\t}");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\telse");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\t{");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\t\tLinea[Columna.getNumColumna()].setIdentificador(Convert.ToString(letra));");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\t\tletra++;");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\t\tTransiciones.Add(Linea[Columna.getNumColumna()]);");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\t\tPendientes.Enqueue(Linea[Columna.getNumColumna()]);");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\t\t");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\t\tif (Pendientes.Count != 0 && analizar == false)");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\t\t\tanalizar = true;");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t\t}");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t\t}");
            DataStreamWriter.WriteLine("\t\t\t\t\t\t}");
            DataStreamWriter.WriteLine("\t\t\t\t\t}");
            DataStreamWriter.WriteLine("\t\t\t\t}");
            DataStreamWriter.WriteLine("\t\t\t\t");
            DataStreamWriter.WriteLine("\t\t\t\ttry");
            DataStreamWriter.WriteLine("\t\t\t\t{");
            DataStreamWriter.WriteLine("\t\t\t\t\tcant++;");
            DataStreamWriter.WriteLine("\t\t\t\t\ttemp = Pendientes.Dequeue();");
            DataStreamWriter.WriteLine("\t\t\t\t\tFirstPadre = temp.getElementos();");
            DataStreamWriter.WriteLine("\t\t\t\t\tstring identificador = temp.getIdentificador();");
            DataStreamWriter.WriteLine("\t\t\t\t\t");
            DataStreamWriter.WriteLine("\t\t\t\t\tLinea = new Transicion[Encabezado.Length + 1];");
            DataStreamWriter.WriteLine("\t\t\t\t\t");
            DataStreamWriter.WriteLine("\t\t\t\t\t// Inicializacion de la lista");
            DataStreamWriter.WriteLine("\t\t\t\t\tfor (int i = 0; i < Linea.Length; i++)");
            DataStreamWriter.WriteLine("\t\t\t\t\t\tLinea[i] = new Transicion();");
            DataStreamWriter.WriteLine("\t\t\t\t\t");
            DataStreamWriter.WriteLine("\t\t\t\t\tLinea[0].setElementos(FirstPadre);");
            DataStreamWriter.WriteLine("\t\t\t\t\tLinea[0].setIdentificador(identificador);");
            DataStreamWriter.WriteLine("\t\t\t\t");
            DataStreamWriter.WriteLine("\t\t\t\t}");
            DataStreamWriter.WriteLine("\t\t\t\tcatch (Exception e) { }");
            DataStreamWriter.WriteLine("\t\t\t\t");
            DataStreamWriter.WriteLine("\t\t\t\t// Cuando no existan elementos dentro cola se procede a realizar el metodo de UltimaIteracion");
            DataStreamWriter.WriteLine("\t\t\t\tif (Pendientes.Count == 0)");
            DataStreamWriter.WriteLine("\t\t\t\t\tUltimaIteracion = true; ");
            DataStreamWriter.WriteLine("\t\t\t} while (analizar);");
            DataStreamWriter.WriteLine("\t\t\t");
            DataStreamWriter.WriteLine("\t\t\treturn cant;");
            DataStreamWriter.WriteLine("\t\t}");
            DataStreamWriter.WriteLine("\t\t");

        }
        /// <summary>
        /// Metodo que extrae el archivo contenedor de los metodos para la correcta interpretacion de la segunda fase
        /// </summary>
        /// <param name="DataStreamWriter">Continuacion del archivo que se esta creando, en este momento se envia lo que lleva el archivo escrito</param>
        private void ClasesScanner(ref StreamWriter DataStreamWriter)
        {
            StreamReader DataClass = new StreamReader("C:\\Users\\HRZCz\\OneDrive\\Escritorio\\URL-Primer Ciclo 2020\\Lenguajes\\ProyectoLenguajes\\Proyecto_LenguajesFormalesYAutomatas\\Proyecto_LFYA_Scanner\\MetodosNecesarios.txt");// se inyectan las clases necesarias con los metodos para funcionamiento correcto.
            DataStreamWriter.Write(DataClass.ReadToEnd());
        }
    }
}
