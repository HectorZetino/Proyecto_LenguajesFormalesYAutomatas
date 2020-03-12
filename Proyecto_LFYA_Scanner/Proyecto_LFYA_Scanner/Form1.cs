using Proyecto_LFYA_Scanner.Componentes;
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
        int LineaError = 0;
        int LineaTamano = 0;
        string error = "";
        string ArchivoRuta = "";
        List<string> ArchivoTexto = new List<string>();
        List<Set> ListSets = new List<Set>();
        List<Token> ListTokens = new List<Token>();
        Dictionary<string, int> DiccionarioActions = new Dictionary<string, int>();
         
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
            List<string> ListadoNFollowLast = new List<string>();
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
            LecturaArchivo.Close();

        }
    }
}
