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

        public MallDbContext(DbContextOptions<MallDbContext> options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
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

        public virtual DbSet<NonMbrShoppingCartItem> NonMbrShoppingCartItems { get; set; }

        public virtual DbSet<PushMessage> PushMessages { get; set; }

        public virtual DbSet<ProductQty> ProductQties { get; set; }
    }
}