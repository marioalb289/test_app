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
using System.Net;

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
        MsgBox msgBox;



        public AgregarUsuario()
        {
            InitializeComponent();

            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "";
            string path = @desktop + "\\sice_archivos";
            //This the list of random files we want to copy into a single new directory
            //List<String> TempFiles = new List<String>();

            List<String> TempFiles = Directory.GetFiles(path).ToList();
            if (TempFiles.Count == 0)
                btnSincronizarArchivo.Enabled = false;
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
                    msgBox = new MsgBox(this, "Datos Guardados Correctamente", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    msgBox.ShowDialog(this);
                    this.Focus();
                    //MessageBox.Show("Datos Guardados correctamente");
                    this.recargar();
                    break;
                case 2:
                    msgBox = new MsgBox(this, "No hay Conexion. \n Trabajndo en modo SIN CONEXIÓN", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    msgBox.ShowDialog(this);
                    //MessageBox.Show("No hay Conexion. \n Trabajndo en modo SIN CONEXIÓN");
                    this.recargar();
                    break;
                case 3:
                    msgBox = new MsgBox(this, "Error al Guardar los Datos.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    msgBox.ShowDialog(this);
                    //MessageBox.Show("Error al Guardar los Datos.");
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
            Loading loading = new Loading(this);
            loading.StartPosition = FormStartPosition.Manual;
            loading.Location = new Point(this.Location.X + (this.Width - loading.Width) / 2, this.Location.Y + (this.Height - loading.Height) / 2);
            loading.Show(this);
            this.agregarUsuario();

            loading.Close();





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
            this.files = new List<string>();
            DialogResult dr = this.openFileDialog1.ShowDialog();
            if (dr == System.Windows.Forms.DialogResult.OK)
            {
                string desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)+"";
                string path = @desktop+"\\sice_archivos";
                // Read the files
                int cont = 0;
                foreach (String file in openFileDialog1.FileNames)
                {
                    // Create a PictureBox.
                    try
                    {
                        if (!Directory.Exists(path))
                        {
                            // Try to create the directory.
                            DirectoryInfo di = Directory.CreateDirectory(path);
                            Console.WriteLine("The directory was created successfully at {0}.", Directory.GetCreationTime(path));
                        }
                        Console.WriteLine(openFileDialog1.SafeFileNames[cont]);
                        string fName = openFileDialog1.SafeFileNames[cont];

                        //var networkPath = @"\\192.168.1.146\test2";
                        //var credentials = new NetworkCredential("estu2", "123");

                        //using (new NetworkConnection(networkPath, credentials))
                        //{
                        //    System.IO.File.Copy(file, "\\\\192.168.1.146\\test2" + "\\" + "prueba.jpg",true);
                        //}                        

                        //then do whatever, such as getting a list of folders:
                        //string[] theFolders = System.IO.Directory.GetDirectories("@\\computer\share");
                        File.Copy(file, path+"\\"+"prueba.jpg",true);
                        


                        //this.files.Add(file);
                        cont++;
                    }
                    catch (SecurityException ex)
                    {
                        // The user lacks appropriate permissions to read files, discover paths, etc.
                        MessageBox.Show("Error de Seguridad. Contacte al administrador para detalles.\n\n" +
                            "Mensaje de Error: " + ex.Message + "\n\n" +
                            "Detalles (notifcar para soporte):\n\n" + ex.StackTrace
                        );
                    }
                    catch (IOException copyError)
                    {
                        Console.WriteLine(copyError.Message);
                    }
                    catch (Exception ex)
                    {
                        // Could not load the image - probably related to Windows file system permissions.
                        MessageBox.Show("No se puede cargar la imagen: " + file.Substring(file.LastIndexOf('\\'))
                            + ". Posiblemente no tenga permisos para leer el archivo, o " +
                            "tal vez esta dañado.\n\nMensaje de Error: " + ex.Message);
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

        private void btnSincronizarArchivo_Click(object sender, EventArgs e)
        {
            try
            {
                string desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "";
                string path = @desktop + "\\sice_archivos";
                //This the list of random files we want to copy into a single new directory
                //List<String> TempFiles = new List<String>();

                List<String> TempFiles = Directory.GetFiles(path).ToList();

                if(TempFiles.Count > 0)
                {
                    var networkPath = @"\\192.168.1.146\test2\archivos";
                    var credentials = new NetworkCredential("estu2", "123");

                    using (new NetworkConnection(networkPath, credentials))
                    {
                        //I would recommend you put at least one large file in this folder
                        //to see the progress bar in action.
                        CopyFiles.CopyFiles Temp = new CopyFiles.CopyFiles(TempFiles, @"\\192.168.1.146\test2");

                        //Uncomment the next line to copy the file tree.
                        //CopyFiles.CopyFiles Temp = new CopyFiles.CopyFiles("C:\\Copy Test Folder", "C:\\Test");

                        //Create the default Copy Files Dialog window from our Copy Files assembly
                        //and sync it with our main/current thread
                        CopyFiles.DIA_CopyFiles TempDiag = new CopyFiles.DIA_CopyFiles(this);
                        TempDiag.SynchronizationObject = this;

                        //Copy the files anysncrinsuly
                        Temp.CopyAsync(TempDiag);

                        //Uncomment this line to do a synchronous copy.
                        ///Temp.Copy();
                    }
                }
                else
                {
                    msgBox = new MsgBox(this, "No hay archivos para sincronizar", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    msgBox.ShowDialog(this);
                    this.Focus();

                }

                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
           
        }

        private void cargando(object sender, ProgressChangeDelegate e)
        {
            //pgrBarSincronizar.Value = e.

        }

        private void subido(object sender, Completedelegate e)
        {
            MessageBox.Show("Completo");

        }
    }
}
