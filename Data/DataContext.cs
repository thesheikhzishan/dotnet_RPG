using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace dotnet_RPG.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        //Table for storing Characters
        public DbSet<Character> Characters => Set<Character>();
        public DbSet<User> Users => Set<User>();

    }
    
}