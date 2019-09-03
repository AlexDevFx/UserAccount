using System.Collections.Generic;
using System.Linq;

namespace DataAccess.Models
{
	public class UserAccount: EntityBase<int>
	{
		public string AccountNumber { get; set; }
		public decimal Balance { get; set; }
		
		public List<AccountHistory> History { get; set; }

		public void RefreshBalance()
		{
			Balance = History?.Sum(e => e.Amount) ?? 0m;
		}
	}
}