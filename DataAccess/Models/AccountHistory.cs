using System;

namespace DataAccess.Models
{
	public class AccountHistory: EntityBase<int>
	{
		public decimal Amount { get; set; }
		public DateTime ChangedAt { get; set; }
		
		public int AccountId { get; set; }
		public UserAccount Account { get; set; }
	}
}