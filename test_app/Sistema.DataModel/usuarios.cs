//------------------------------------------------------------------------------
// <auto-generated>
//    Este código se generó a partir de una plantilla.
//
//    Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//    Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Sistema.DataModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class usuarios
    {
        public int id { get; set; }
        public string nombre_formal { get; set; }
        public string nombre { get; set; }
        public string apellido { get; set; }
        public string contrasena { get; set; }
        public string correo { get; set; }
        public int area { get; set; }
        public int privilegios { get; set; }
        public int priv_sigi { get; set; }
        public int priv_sui { get; set; }
        public int priv_sia { get; set; }
        public int titular { get; set; }
        public int estado { get; set; }
    }
}