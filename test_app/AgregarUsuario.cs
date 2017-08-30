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
            sadmon = new SistemaAdministracion();
            tsc = new TestConexion();
            //cargarGridUsuario();

            if (tsc.IsServerConnected())
            {
                this.conexion = true;
                MessageBox.Show("Conexion Establecida");
                cargarGridUsuario();
            }
            else
            {
                MessageBox.Show("No hay Conexion. \n Trabajndo en modo SIN CONEXIÓN");
                this.conexion = false;
            }

        }       

        private void cargarGridUsuario()
        {
            try
            {
                if (this.conexion)
                {
                    dgvUsuarios.DataSource = new BindingSource(sadmon.getUsuarios(), null);                    
                }
                else
                {
                    dgvUsuarios.DataSource = new BindingSource(sadmon.getUsuarios(), null);
                }

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
                
                using (datacenterEntities contexto = new datacenterEntities())
                {
                    usuarios usr = new usuarios();
                    usr.nombre = txtNombre.Text;
                    usr.apellido = txtApellido.Text;
                    usr.nombre_formal = txtNombreFormal.Text;
                    usr.correo = txtUsuario.Text;
                    usr.contrasena = txtPass1.Text;

                    contexto.usuarios.Add(usr);
                    contexto.SaveChanges();

                    cargarGridUsuario();

                    MessageBox.Show("Datos Guardados Correctamente");
                }
                

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

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
    }
}
