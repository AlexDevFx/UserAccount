using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
	public class UsersDbContext: DbContext
	{
		protected UsersDbContext()
		{
		}

		public UsersDbContext(DbContextOptions options) : base(options)
		{
		}

		public DbSet<UserAccount> UserAccounts { get; set; }
		public DbSet<AccountHistory> AccountHistory { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			base.OnConfiguring(optionsBuilder);
		}

		protected override void OnModelCreating(ModelBuilder builder)
		{
			builder.Entity<UserAccount>(b =>
			{
				b.ToTable("UserAccounts");
				b.Property(p => p.AccountNumber).HasMaxLength(128);
				b.HasMany(p => p.History)
					.WithOne(p => p.Account)
					.HasForeignKey(p => p.AccountId);
			});
			
			builder.Entity<AccountHistory>(b =>
			{
				b.ToTable("AccountHistory");
			});
		}
	}
}