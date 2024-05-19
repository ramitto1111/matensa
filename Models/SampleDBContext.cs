//using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
namespace matensa.Models
{
    public partial class SampleDBContext
    {
        /*
        public SampleDBContext(DbContextOptions
        <SampleDBContext> options)
            : base(options)
        {
        }
        public virtual DbSet<User> User { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity => {
                entity.HasKey(k => k.CustomerId);
            });
            OnModelCreatingPartial(modelBuilder);
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
        */
    }
}
