using Core.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core
{
    public class BillingContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(SystemHelper.GetConnectionString("billing"));
        }
        public DbSet<HangfireJob> Job { get; set; }
        public DbSet<SINDetails> SINDetails { get; set; }
        public DbSet<Transfer> Transfer { get; set; }
        public DbSet<Wallet> Wallet { get; set; }
        public DbSet<SIN> SIN { get; set; }

        public DbSet<Character> Character { get; set; }

        //public DbSet<SystemSettings> SystemSettings { get; set; }
    }
}
