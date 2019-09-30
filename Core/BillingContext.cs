using Core.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core
{
    public class BillingContext : DbContext
    {
        public BillingContext()
            :base()
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(UnitOfWork.GetConnectionString("billing"));
        }

        public DbSet<SystemSettings> SystemSettings { get; set; }
    }
}
