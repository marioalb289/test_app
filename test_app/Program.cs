using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace test_app
{
    static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Sistema.Generales.TestConexion test = new Sistema.Generales.TestConexion();
            test.IsServerConnected();
            Application.Run(new AgregarUsuario());
        }
    }
}
