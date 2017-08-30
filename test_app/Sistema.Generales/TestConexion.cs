using MySql.Data.MySqlClient;
using Sistema.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema.Generales
{
    public class TestConexion
    {
        public bool IsServerConnected()
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
    }
}
