using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Web;
using _036_MoviesMvcWissen.Entities;

namespace _036_MoviesMvcWissen.Contexts
{
    public class MoviesContext : DbContext
    {
        public MoviesContext() : base("MoviesContext")
        {
            //codefirst yapısında kullanmak önemli
            //Disable initilazer
            Database.SetInitializer<MoviesContext>(null);
        }

        public virtual DbSet<Movie> Movies { get; set; }
        public virtual DbSet<Director> Directors { get; set; }
        public virtual DbSet<Review> Reviews { get; set; }
        public virtual DbSet<MovieDirector> MovieDirectors { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public virtual DbSet<vwUsers> vwUsers { get; set; }


    }
}