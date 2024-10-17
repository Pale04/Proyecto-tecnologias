﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class papayagramsEntities : DbContext
    {
        public papayagramsEntities()
            : base("name=papayagramsEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Achievement> Achievement { get; set; }
        public virtual DbSet<OriginalGameHistory> OriginalGameHistory { get; set; }
        public virtual DbSet<SuddenDeathHistory> SuddenDeathHistory { get; set; }
        public virtual DbSet<TimeAtackHistory> TimeAtackHistory { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserAchieved> UserAchieved { get; set; }
        public virtual DbSet<UserConfiguration> UserConfiguration { get; set; }
        public virtual DbSet<UserRelationship> UserRelationship { get; set; }
        public virtual DbSet<UserStatus> UserStatus { get; set; }
    
        public virtual int register_user(string parametro1, string parametro2, string parametro3)
        {
            var parametro1Parameter = parametro1 != null ?
                new ObjectParameter("Parametro1", parametro1) :
                new ObjectParameter("Parametro1", typeof(string));
    
            var parametro2Parameter = parametro2 != null ?
                new ObjectParameter("Parametro2", parametro2) :
                new ObjectParameter("Parametro2", typeof(string));
    
            var parametro3Parameter = parametro3 != null ?
                new ObjectParameter("Parametro3", parametro3) :
                new ObjectParameter("Parametro3", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("register_user", parametro1Parameter, parametro2Parameter, parametro3Parameter);
        }
    
        public virtual ObjectResult<login_Result> login(string username, string password)
        {
            var usernameParameter = username != null ?
                new ObjectParameter("username", username) :
                new ObjectParameter("username", typeof(string));
    
            var passwordParameter = password != null ?
                new ObjectParameter("password", password) :
                new ObjectParameter("password", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<login_Result>("login", usernameParameter, passwordParameter);
        }
    
        public virtual ObjectResult<get_player_Result> get_player(string username)
        {
            var usernameParameter = username != null ?
                new ObjectParameter("username", username) :
                new ObjectParameter("username", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<get_player_Result>("get_player", usernameParameter);
        }
    
        public virtual ObjectResult<get_player_by_email_Result> get_player_by_email(string email)
        {
            var emailParameter = email != null ?
                new ObjectParameter("email", email) :
                new ObjectParameter("email", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<get_player_by_email_Result>("get_player_by_email", emailParameter);
        }
    
        public virtual ObjectResult<get_player_by_username_Result> get_player_by_username(string username)
        {
            var usernameParameter = username != null ?
                new ObjectParameter("username", username) :
                new ObjectParameter("username", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<get_player_by_username_Result>("get_player_by_username", usernameParameter);
        }
    }
}