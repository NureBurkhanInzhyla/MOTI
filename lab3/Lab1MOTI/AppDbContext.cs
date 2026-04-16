using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Lab1MOTI
{
    public class AppDbContext: DbContext
    {
        public DbSet<Alternative> Alternatives { get; set; }
        public DbSet<Criterion> Criterion { get; set; }
        public DbSet<Vector> Vectors { get; set; }
        public DbSet<LPR> LPRs { get; set; }
        public DbSet<Result> Results { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=lab1_moti.db");
        }
    }
}
