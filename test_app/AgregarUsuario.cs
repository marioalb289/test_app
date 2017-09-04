using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Deployment.Application;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Data.SqlClient;
using Sistema.DataModel;
using Sistema.Generales;

namespace test_app
{
    public partial class AgregarUsuario : Form
    {
        long sizeOfUpdate = 0;
        private bool conexion = false;
        bool flag = false;
        SistemaAdministracion sadmon;
        TestConexion tsc;
        

        public AgregarUsuario()
        {
            InitializeComponent();
        }

        public void recargar()
        {
            txtNombre.Text = "";
            txtApellido.Text = "";
            txtNombreFormal.Text = "";
            txtPass1.Text = "";
            txtPass2.Text = "";
            txtUsuario.Text = "";

            cargarGridUsuario();

        }

        private void AgregarUsuario_Load(object sender, EventArgs e)
        {
            //tsc = new TestConexion();
            if (SistemaAdministracion.Conexion)
            {
                btnModoSinConexion.Text = "Modo sin Conexión";
            }
            else
            {
                btnModoSinConexion.Text = "Modo con Conexión";
            }
            sadmon = new SistemaAdministracion();
            cargarGridUsuario();

            //if (tsc.IsServerConnected())
            //{
            //    this.conexion = true;
            //    sadmon = new SistemaAdministracion(this.conexion);
            //    SistemaAdministracion.Conexion = true;
            //    MessageBox.Show("Conexion Establecida");
            //    cargarGridUsuario();
            //}
            //else
            //{
            //    MessageBox.Show("No hay Conexion. \n Trabajndo en modo SIN CONEXIÓN");
            //    this.conexion = false;
            //    sadmon = new SistemaAdministracion(this.conexion);
            //    SistemaAdministracion.Conexion = false;
            //    cargarGridUsuario();
            //}

            

        }

        private void cargarGridUsuario()
        {
            try
            {
                dgvUsuarios.DataSource = new BindingSource(sadmon.getUsuarios(), null);

                dgvUsuarios.Update();
                dgvUsuarios.Refresh();




            }
            catch(Exception ex){
                MessageBox.Show(ex.Message);
            }
        }

        private void agregarUsuario()
        {
            try
            {
                usuarios usr = new usuarios();
                usr.nombre = txtNombre.Text;
                usr.apellido = txtApellido.Text;
                usr.nombre_formal = txtNombreFormal.Text;
                usr.correo = txtUsuario.Text;
                usr.contrasena = txtPass1.Text;

                this.messageRes(sadmon.guardarUsuarios(usr));
                
                

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
            
                       

        }

        public void messageRes(int res)
        {
            switch (res)
            {
                case 1:
                    MessageBox.Show("Datos Guardados correctamente");
                    this.recargar();
                    break;
                case 2:
                    MessageBox.Show("No hay Conexion. \n Trabajndo en modo SIN CONEXIÓN");
                    this.recargar();
                    break;
                case 3:
                    MessageBox.Show("Error al Guardar los Datos.");
                    break;
            }
            
        }

        public bool IsServerConnected()
        {
            try
            {
                datacenterEntities contexto = new datacenterEntities();

                var connectionString = contexto.Database.Connection.ConnectionString;
                MySqlConnection connection = new MySqlConnection(connectionString);

                try
                {
                    connection.Open();
                    connection.Close();
                    return true;
                }
                catch (MySqlException)
                {
                    return false;
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            
            

        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            this.agregarUsuario();
        }

        private void btnActualizarSis_Click(object sender, EventArgs e)
        {
            BuscarActualizaciones update = new BuscarActualizaciones();
            update.UpdateApplication(downloadStatus);
            //this.UpdateApplication();
        }

        private void btnSincronizar_Click(object sender, EventArgs e)
        {
            try
            {
                if (sadmon.sincronizarDatos())
                {
                    MessageBox.Show("Datos Sincronizados Correctamentes");
                    this.recargar();
                }

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private void btnModoSinConexion_Click(object sender, EventArgs e)
        {
            if (SistemaAdministracion.Conexion)
            {
                SistemaAdministracion.Conexion = false;
                MessageBox.Show("Trabajando en modo sin Conexión");
                btnModoSinConexion.Text = "Modo con Conexión";
            }
            else
            {
                TestConexion test = new TestConexion();
                if (!test.IsServerConnected())
                {
                    btnModoSinConexion.Text = "Modo con Conexión";
                }
                else
                {
                    btnModoSinConexion.Text = "Modo sin Conexión";
                }
                
            }
            this.recargar();
        }
    }
}
