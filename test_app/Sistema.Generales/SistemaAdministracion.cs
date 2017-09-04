using Sistema.DataModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Data;
using System.Data.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Metadata.Edm;
using System.Reflection;
using System.Windows.Forms;

namespace Sistema.Generales
{
    public class SistemaAdministracion
    {
        static bool conexion = true;
        private string db;

        public static bool Conexion { get => conexion; set => conexion = value; }

        public SistemaAdministracion()
        {
            
            
        }
        public string inicializarBd(bool cnx)
        {
            Database.SetInitializer<DatabaseContext>(new CreateDatabaseIfNotExists<DatabaseContext>());
            return cnx ? "MYSQLSERVER" : "MYSQLLOCAL";
            
        }
        public List<usuarios> getUsuarios()
        {
            try
            {
                
                //string source = "Hello World!";
                //using (MD5 md5Hash = MD5.Create())
                //{
                //    string hash = GetMd5Hash(md5Hash, source);
                //}
                using (DatabaseContext contexto = new DatabaseContext(inicializarBd(Conexion)))
                {
                    List<usuarios> listaUsuarios = (from i in contexto.usuarios where i.importado == 0 orderby i.id descending select i).ToList();
                    return listaUsuarios;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int guardarUsuarios(usuarios data)
        {
            try
            {
                using (DatabaseContext contexto = new DatabaseContext(inicializarBd(Conexion)))
                {        
                    contexto.usuarios.Add(data);
                    contexto.SaveChanges();

                    return 1;
                }


            }
            catch (Exception ex)
            {
                string innerEx = ex.InnerException.Message;
                if(innerEx == "Unable to connect to any of the specified MySQL hosts.")
                {
                    Conexion = false;
                    return 2;
                }
                else
                {
                    return 3;
                }
                

            }

        }

        public bool sincronizarDatos()
        {
            try
            {
                using (datacenterEntities context = new datacenterEntities()) {

                    ObjectContext objContext = ((IObjectContextAdapter)context).ObjectContext;
                    IEnumerable<EntityType> tables = objContext.MetadataWorkspace.GetItems<EntityType>(System.Data.Metadata.Edm.DataSpace.CSpace);
                    foreach (EntityType t in tables)
                    {

                        int res = this.guardarServidor(t.Name);

                        if(res == 2)
                        {
                            MessageBox.Show("No hay conexión. No se pueden sincronizar los datos.\n Intentálo de nuevo mas tarde.");
                            return false;

                        }
                        if(res == 3)
                        {
                            MessageBox.Show("Error al Sincronizar los Datos");
                            return false;
                        }
                        if (res == 4)
                        {
                            MessageBox.Show("Error no hay datos para sincronizar");
                            return false;
                        }

                        Console.WriteLine(t);
                    }
                    return true;
                    //IEnumerable<EntityType> tables = workspace.GetItems<EntityType>(DataSpace.SSpace);
                }

            }
            catch(Exception ex)
            {
                throw ex;
            }
            


        }

        public int guardarServidor(string model)
        {
            try
            {
                switch (model)
                {
                    case "usuarios":
                        using (DatabaseContext contextoLocal = new DatabaseContext("MYSQLLOCAL"))
                        {
                            List<usuarios> lista = (from i in contextoLocal.usuarios where i.importado == 0 select i).ToList();
                            if (lista.Count == 0)
                                throw new Exception("4");
                            foreach (usuarios usuario in lista)
                            {
                                int tempid = usuario.id;
                                using (DatabaseContext contextoServer = new DatabaseContext("MYSQLSERVER"))
                                {
                                    usuarios nuevoUsuario = new usuarios();
                                    nuevoUsuario.id = 0;
                                    nuevoUsuario = usuario;
                                    contextoServer.usuarios.Add(nuevoUsuario);
                                    contextoServer.SaveChanges();

                                }
                                usuario.id = tempid;
                                usuario.importado = 1;
                                contextoLocal.SaveChanges();

                                //break;
                            }

                        }
                        break;

                }
                return 1;

            }
            catch (Exception ex)
            {
                if(ex.Message == "4")
                {
                    return 4;
                }
                string innerEx = ex.InnerException.Message;
                if (innerEx == "Unable to connect to any of the specified MySQL hosts.")
                {
                    Conexion = false;
                    return 2;
                }
                else
                {
                    return 3;
                }
            }
            

        }



        public static string GetMd5Hash(MD5 md5Hash, string input)
        {

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        // Verify a hash against a string.
        public static bool VerifyMd5Hash(MD5 md5Hash, string input, string hash)
        {
            // Hash the input.
            string hashOfInput = GetMd5Hash(md5Hash, input);

            // Create a StringComparer an compare the hashes.
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (0 == comparer.Compare(hashOfInput, hash))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
