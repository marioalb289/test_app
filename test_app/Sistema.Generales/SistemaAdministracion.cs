using Sistema.DataModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema.Generales
{
    public class SistemaAdministracion
    {
        public List<sigi_oficios> getUsuarios()
        {
            try
            {
                Database.SetInitializer<DatabaseContext>(new CreateDatabaseIfNotExists<DatabaseContext>());
                using (DatabaseContext contexto = new DatabaseContext("MYSQL"))
                {
                    List<sigi_oficios> listaUsuarios = (from i in contexto.sigi_oficios orderby i.id descending select i).ToList();
                    return listaUsuarios;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<usuarios_local> getUsuariosLocal()
        {
            try
            {
                using (DataCenterLocalEntities contexto = new DataCenterLocalEntities())
                {
                    List<usuarios_local> listaUsuarios = (from i in contexto.usuarios_local orderby i.Id descending select i).ToList();
                    return listaUsuarios;
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
