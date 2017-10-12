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
using System.Security.Cryptography;
using System.IO;
using System.Security;
using PagedList;

namespace test_app
{
    public partial class AgregarUsuario : Form
    {
        long sizeOfUpdate = 0;
        private bool conexion = false;
        bool flag = false;
        List<string> files;
        SistemaAdministracion sadmon;
        TestConexion tsc;
        int pageNumber = 1;
        int totalPages = 0;
        IPagedList<usuarios> listUsuarios;
        

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
            //txtPass2.Text = "";
            txtUsuario.Text = "";            
            cargarGridUsuario();

        }

        private void AgregarUsuario_Load(object sender, EventArgs e)
        {
            //tsc = new TestConexion();
            InitializeOpenFileDialog();
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

            

        }

        private async void cargarGridUsuario()
        {
            try
            {
                listUsuarios = await GetPagedListAsync();
                btnPrimero.Enabled = !listUsuarios.IsFirstPage;
                btnUltimo.Enabled = !listUsuarios.IsLastPage;
                btnAnterior.Enabled = listUsuarios.HasPreviousPage;
                btnSiguiente.Enabled = listUsuarios.HasNextPage;

                dgvUsuarios.DataSource = new BindingSource(listUsuarios.ToList(), null);

                lblTotalPag.Text = string.Format("Pág {0}/{1}",pageNumber, totalPages);

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
                string source = txtPass1.Text;
                string hash = ""; 
                using (MD5 md5Hash = MD5.Create())
                {
                    hash = SistemaAdministracion.GetMd5Hash(md5Hash, source);                    
                }
                usuarios usr = new usuarios();
                usr.nombre = txtNombre.Text;
                usr.apellido = txtApellido.Text;
                usr.nombre_formal = txtNombreFormal.Text;
                usr.correo = txtUsuario.Text;
                usr.contrasena = hash;

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
                bool res = false;
                //for(int x = 0; x < 20; x++)
                //{
                //    res = sadmon.sincronizarDatos();

                //}
                res = sadmon.sincronizarDatos();
                if (res)
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
            dgvUsuarios.Rows.Clear();
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

        private async Task<IPagedList<usuarios>> GetPagedListAsync(int pageNumber = 1, int pageSize = 10)
        {
            return await Task.Factory.StartNew(() =>
            {
                totalPages = sadmon.totalUsuarios();
                if(totalPages > 0)
                {
                   double res = (double)totalPages / (double)pageSize;
                   double flor = Math.Ceiling(res);
                   totalPages = Convert.ToInt32(flor);
                }
                return sadmon.getUsuarios(pageNumber, pageSize);           
            });
        }

        private void InitializeOpenFileDialog()
        {
            // Set the file dialog to filter for graphics files.
            this.openFileDialog1.Filter =
                "Images (*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF|" +
                "All files (*.*)|*.*";

            // Allow the user to select multiple images.
            this.openFileDialog1.Multiselect = true;
            this.openFileDialog1.Title = "My Image Browser";
        }


        private void btnArchivo_Click(object sender, EventArgs e)
        {
            DialogResult dr = this.openFileDialog1.ShowDialog();
            if (dr == System.Windows.Forms.DialogResult.OK)
            {
                string desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)+"";
                // Read the files
                foreach (String file in openFileDialog1.FileNames)
                {
                    // Create a PictureBox.
                    try
                    {
                        PictureBox pb = new PictureBox();
                        Image loadedImage = Image.FromFile(file);
                        files.Add(file);
                        pb.Height = loadedImage.Height;
                        pb.Width = loadedImage.Width;
                        pb.Image = loadedImage;
                        flowLayoutPanel1.Controls.Add(pb);
                    }
                    catch (SecurityException ex)
                    {
                        // The user lacks appropriate permissions to read files, discover paths, etc.
                        MessageBox.Show("Security error. Please contact your administrator for details.\n\n" +
                            "Error message: " + ex.Message + "\n\n" +
                            "Details (send to Support):\n\n" + ex.StackTrace
                        );
                    }
                    catch (Exception ex)
                    {
                        // Could not load the image - probably related to Windows file system permissions.
                        MessageBox.Show("Cannot display the image: " + file.Substring(file.LastIndexOf('\\'))
                            + ". You may not have permission to read the file, or " +
                            "it may be corrupt.\n\nReported error: " + ex.Message);
                    }
                }
            }
        }

        private async void btnAnterior_Click(object sender, EventArgs e)
        {
            if (listUsuarios.HasPreviousPage)
            {
                listUsuarios = await GetPagedListAsync(--pageNumber);
                btnPrimero.Enabled = !listUsuarios.IsFirstPage;
                btnUltimo.Enabled = true;
                btnAnterior.Enabled = listUsuarios.HasPreviousPage;
                btnSiguiente.Enabled = listUsuarios.HasNextPage;

                dgvUsuarios.DataSource = new BindingSource(listUsuarios.ToList(), null);

                lblTotalPag.Text = string.Format("Pág {0}/{1}", pageNumber, totalPages);

                dgvUsuarios.Update();
                dgvUsuarios.Refresh();
            }

        }

        private async void btnSiguiente_Click(object sender, EventArgs e)
        {
            if (listUsuarios.HasNextPage)
            {
                listUsuarios = await GetPagedListAsync(++pageNumber);
                btnPrimero.Enabled = true;
                btnUltimo.Enabled = !listUsuarios.IsLastPage;
                btnAnterior.Enabled = listUsuarios.HasPreviousPage;
                btnSiguiente.Enabled = listUsuarios.HasNextPage;

                dgvUsuarios.DataSource = new BindingSource(listUsuarios.ToList(), null);

                lblTotalPag.Text = string.Format("Pág {0}/{1}", pageNumber, totalPages);

                dgvUsuarios.Update();
                dgvUsuarios.Refresh();
            }

        }

        private async void btnPrimero_Click(object sender, EventArgs e)
        {
            
            btnPrimero.Enabled = false;
            btnUltimo.Enabled = true;
            pageNumber = 1;

            listUsuarios = await GetPagedListAsync(pageNumber);
            btnAnterior.Enabled = listUsuarios.HasPreviousPage;
            btnSiguiente.Enabled = listUsuarios.HasNextPage;

            dgvUsuarios.DataSource = new BindingSource(listUsuarios.ToList(), null);

            lblTotalPag.Text = string.Format("Pág {0}/{1}", pageNumber, totalPages);

            dgvUsuarios.Update();
            dgvUsuarios.Refresh();
            
        }

        private async void btnUltimo_Click(object sender, EventArgs e)
        {
            
            btnUltimo.Enabled = false;
            btnPrimero.Enabled = true;
            pageNumber = totalPages;

            listUsuarios = await GetPagedListAsync(pageNumber);
            btnAnterior.Enabled = listUsuarios.HasPreviousPage;
            btnSiguiente.Enabled = listUsuarios.HasNextPage;

            dgvUsuarios.DataSource = new BindingSource(listUsuarios.ToList(), null);

            lblTotalPag.Text = string.Format("Pág {0}/{1}", pageNumber, totalPages);

            dgvUsuarios.Update();
            dgvUsuarios.Refresh();

            
        }
    }
}
