using CadastroMercadoriaBiblioteca.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadastroMercadoriaBiblioteca.Data
{
    public class MercadoriaDbContext : DbContext
    {
        public MercadoriaDbContext() { }

        public MercadoriaDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Mercadoria> Mercadorias { get; set; }
        public DbSet<Entrada> Entradas { get; set; }
        public DbSet<Saida> Saidas { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string conn = "Server=localhost,1433;Database=CadastroMercadoriaDB;User ID=sa;Password=example_123;TrustServerCertificate=True";

            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(conn);
            }
        }
    }
}
