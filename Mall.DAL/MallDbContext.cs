using Microsoft.EntityFrameworkCore;
using System;
using Web.Framework;
using WS.Model;

namespace WS.DAL
{
    public class MallDbContext : DbContext
    {
       
        public MallDbContext() 
        {
           
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseLoggerFactory(new CustomEFLoggerFactory());
            
            optionsBuilder.UseSqlServer(Globals.Configuration["ConnectionStrings:sqlcon"]);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            

        }

        public virtual DbSet<Inventory> Inventories { get; set; }

        /// <summary>
        /// 庫存預留資料
        /// </summary>
        public virtual DbSet<InventoryReserved> InventoryReserveds { get; set; }

        public virtual DbSet<ShoppingCartItem> ShoppingCartItems { get; set; }
    }
}