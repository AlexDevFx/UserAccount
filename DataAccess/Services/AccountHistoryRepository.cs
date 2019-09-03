using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Services
{
	public class AccountHistoryRepository: Repository<AccountHistory>, IRepository<AccountHistory>
	{
		public AccountHistoryRepository(DbContext context) : base(context)
		{
		}
	}
}