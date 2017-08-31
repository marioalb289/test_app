using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Sistema.DataModel
{
    public class DatabaseContext: DbContext
    {
        public DatabaseContext(string name)
            : base("name="+name)
        {
        }
        public DbSet<usuarios> usuarios { get; set; }
        public DbSet<areas> areas { get; set; }
        public DbSet<cosas_requi> cosas_requi { get; set; }
        public DbSet<lugares> lugares { get; set; }
        public DbSet<requerimientos> requerimientos { get; set; }
        public DbSet<sigi_contador_folios> sigi_contador_folios { get; set; }
        public DbSet<sigi_documentos> sigi_documentos { get; set; }
        public DbSet<sigi_oficios> sigi_oficios { get; set; }
        public DbSet<sigi_oficios_documentos_recepcion> sigi_oficios_documentos_recepcion { get; set; }
        public DbSet<sigi_reportes_param> sigi_reportes_param { get; set; }
        public DbSet<solicitudes> solicitudes { get; set; }
    }
}
