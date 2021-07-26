using Core.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Text;

namespace Core
{
    class TraceLoggerProvider : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName)
        {
            return new TraceLogger(categoryName);
        }

        public void Dispose()
        {
        }
    }
    public class TraceLogger : ILogger
    {
        private readonly string categoryName;
        public TraceLogger(string categoryName) => this.categoryName = categoryName;
        public bool IsEnabled(LogLevel logLevel) => true;
        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception exception,
            Func<TState, Exception, string> formatter)
        {
            Trace.WriteLine($"{DateTime.Now.ToString("o")} {logLevel} {eventId.Id} {this.categoryName}");
            Trace.WriteLine(formatter(state, exception));
        }
        public IDisposable BeginScope<TState>(TState state) => null;
    }

    public class SecondBillingContext: BillingContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(SystemHelper.GetConnectionString());
        }
    }

    public class BillingContext : DbContext
    {
        public BillingContext(bool autodetect = false)
        {
            ChangeTracker.AutoDetectChangesEnabled = autodetect;
        }
        private static readonly Lazy<LoggerFactory> LoggerFactory = new Lazy<LoggerFactory>(() => new LoggerFactory(new[] { new TraceLoggerProvider() }), false);

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(SystemHelper.GetConnectionString());
            
        }
        public DbSet<BillingAbilityLog> AbilityLogs { get; set; }
        public DbSet<BillingBeat> BillingCycle { get; set; }
        public DbSet<BillingInit> BillingInit { get; set; }
        public DbSet<CacheQRContent> CacheQR { get; set; }
        public DbSet<Character> Character { get; set; }
        public DbSet<Contract> Contract { get; set; }
        public DbSet<CorporationWallet> CorporationWallet { get; set; }
        public DbSet<CorporationSpecialisation> CorporationSpecialisation { get; set; }
        public DbSet<CurrentCategory> CurrentCategory { get; set; }
        public DbSet<CurrentFactor> CurrentFactor { get; set; }
        public DbSet<ScoringEventLifestyle> EventLifestyle { get; set; }
        public DbSet<HangfireJob> Job { get; set; }
        public DbSet<HackerHistory> HackerHistory { get; set; }
        public DbSet<JoinCharacter> JoinCharacter { get; set; }
        public DbSet<JoinFieldValue> JoinFieldValue { get; set; }
        public DbSet<JoinField> JoinField { get; set; }
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
        public DbSet<ShopTrusted> ShopTrusted { get; set; }
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
