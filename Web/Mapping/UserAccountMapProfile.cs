using AutoMapper;
using DataAccess.Models;
using UsersAccounts.Models.UsersAccounts;

namespace UsersAccounts.Mapping
{
	public class UserAccountMapProfile: Profile
	{
		public UserAccountMapProfile()
		{
			CreateMap<AccountHistory, AccountHistoryDto>();
		}
	}
}