//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DataAccess
{
    using System;
    using System.Collections.Generic;
    
    public partial class UserConfiguration
    {
        public int userId { get; set; }
        public string selectedLanguage { get; set; }
        public Nullable<int> typography { get; set; }
        public string pieceSize { get; set; }
        public Nullable<int> pieceColor { get; set; }
        public int icon { get; set; }
        public Nullable<int> cursorDesign { get; set; }
    
        public virtual User User { get; set; }
    }
}
