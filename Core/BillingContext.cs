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
            optionsBuilder.UseNpgsql(SystemHelper.GetConnectionString());
            optionsBuilder.EnableSensitiveDataLogging();
        }
        public DbSet<BillingAbilityLog> AbilityLogs { get; set; }
        public DbSet<BillingBeat> BillingCycle { get; set; }
        public DbSet<BillingInit> BillingInit { get; set; }
        public DbSet<CacheQRContent> CacheQR { get; set; }
        public DbSet<Character> Character { get; set; }
        public DbSet<Contract> Contract { get; set; }
        public DbSet<CorporationWallet> CorporationWallet { get; set; }
        public DbSet<CurrentCategory> CurrentCategory { get; set; }
        public DbSet<CurrentFactor> CurrentFactor { get; set; }
        public DbSet<ScoringEventLifestyle> EventLifestyle { get; set; }
        public DbSet<HangfireJob> Job { get; set; }
        public DbSet<JoinCharacter> JoinCharacter { get; set; }
        public DbSet<Metatype> Metatype { get; set; }
        public DbSet<Price> Price { get; set; }
        public DbSet<ProductType> ProductTypes { get; set; }
        public DbSet<Renta> Renta { get; set; }
        public DbSet<Scoring> Scoring { get; set; }
        public DbSet<ScoringCategory> ScoringCategory { get; set; }
        public DbSet<ScoringEvent> ScoringEvent { get; set; }
        public DbSet<ScoringFactor> ScoringFactor { get; set; }
        public DbSet<ShopQR> ShopQR { get; set; }
        public DbSet<ShopSpecialisation> ShopSpecialisation { get; set; }
        public DbSet<ShopWallet> ShopWallet { get; set; }
        public DbSet<SIN> SIN { get; set; }
        public DbSet<Sku> Sku { get; set; }
        public DbSet<Specialisation> Specialisation { get; set; }
        public DbSet<SystemSettings> SystemSettings { get; set; }
        public DbSet<Transfer> Transfer { get; set; }
        public DbSet<Wallet> Wallet { get; set; }
        public DbSet<BeatHistory> BeatHistory { get; set; }
    }
}
