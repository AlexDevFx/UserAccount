using System.Linq;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Services
{
	public interface IUsersAccountsRepository:  IRepository<UserAccount>
	{
		UserAccount Get(int accountId);
	}

	public class UsersAccountsRepository: Repository<UserAccount>,IUsersAccountsRepository
	{
		public UsersAccountsRepository(DbContext context)
			: base(context)
		{
		}

		public UserAccount Get(int accountId)
		{
			return base.Query()
				.Include(e => e.History)
				.FirstOrDefault(e => e.Id == accountId);
		}
	}
}