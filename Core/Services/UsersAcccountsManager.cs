using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Core.Exceptions;
using Core.Models;
using DataAccess.Models;
using DataAccess.Services;
using Microsoft.EntityFrameworkCore.Storage;

namespace Core.Services
{
	public interface IUsersAcccountsManager
	{
		IList<AccountHistory> GetHistory(int accountId);
		Task<decimal> Deposit(int accountId, decimal amount);
		Task<decimal> Withdraw(int accountId, decimal amount);
		Task<TransferResult> Transfer(int sourceAccountId, int destinationAccountId, decimal amount);
	}

	public class UsersAcccountsManager : IUsersAcccountsManager
	{
		private readonly IUsersAccountsRepository _usersAccountsRepository;
		private readonly IRepository<AccountHistory> _accountsHistoryRepository;
		private readonly IClockProvider _clockProvider;

		public UsersAcccountsManager(IClockProvider clockProvider,
			IUsersAccountsRepository usersAccountsRepository,
			IRepository<AccountHistory> accountsHistoryRepository)
		{
			_clockProvider = clockProvider;
			_accountsHistoryRepository = accountsHistoryRepository;
			_usersAccountsRepository = usersAccountsRepository;
		}

		public IList<AccountHistory> GetHistory(int accountId)
		{
			UserAccount account = GetUserAccount(accountId);

			return account.History;
		}

		public async Task<decimal> Deposit(int accountId, decimal amount)
		{
			if (amount <= 0)
			{
				throw new BadRequestException("Сумма не может быть отрицательной или нулевой");
			}
			
			UserAccount account = await ChangeBalance(accountId, amount);
			return account.Balance;
		}

		private async Task<UserAccount> ChangeBalance(int accountId, decimal amount)
		{
			UserAccount account = GetUserAccount(accountId);
			account = await ChangeBalance(account, amount);
			return account;
		}

		public async Task<decimal> Withdraw(int accountId, decimal amount)
		{
			if (amount <= 0)
			{
				throw new BadRequestException("Сумма не может быть отрицательной или нулевой");
			}
			
			UserAccount account = GetUserAccount(accountId);
			account = await ChangeBalance(account, (-1)*amount);
			return account.Balance;
		}
		
		public async Task<TransferResult> Transfer(int sourceAccountId, int destinationAccountId, decimal amount)
		{
			TransferResult result = new TransferResult();
			
			using (IDbContextTransaction transaction =
				_usersAccountsRepository.BeginTransaction(IsolationLevel.ReadUncommitted))
			{
				try
				{
					UserAccount sourceAccount = await CreatePayment(sourceAccountId, (-1)*amount);
					UserAccount destinationAccount = await CreatePayment(destinationAccountId, amount);
					
					transaction.Commit();
					
					result.SourceBalance = sourceAccount.Balance;
					result.DestinationBalance = destinationAccount.Balance;
				}
				catch (Exception)
				{
					transaction.Rollback();
					throw new InternalErrorException("Не удалось завершить транзакцию перевода средств");
				}
			}
			
			return result;
		}
		
		private async Task<UserAccount> ChangeBalance(UserAccount account, decimal amount)
		{
			if (amount < 0 && account.Balance + amount < 0)
			{
				throw new BadRequestException("Недостаточно средст на счёте");
			}
			
			using (IDbContextTransaction transaction =
				_usersAccountsRepository.BeginTransaction(IsolationLevel.ReadUncommitted))
			{
				try
				{
					await CreatePayment(account, amount);

					transaction.Commit();
				}
				catch (Exception)
				{
					transaction.Rollback();
					throw new InternalErrorException("Не удалось завершить транзакцию перевода средств");
				}
			}

			return account;
		}
		
		private async Task<UserAccount> CreatePayment(int accountId, decimal amount)
		{
			UserAccount account = GetUserAccount(accountId);
			await CreatePayment(account, amount);
			return account;
		}

		private async Task CreatePayment(UserAccount account, decimal amount)
		{
			await _accountsHistoryRepository.Create(new AccountHistory()
			{
				AccountId = account.Id,
				Amount = amount,
				ChangedAt = _clockProvider.Now()
			});

			account.RefreshBalance();
		}

		private UserAccount GetUserAccount(int accountId)
		{
			if (accountId <= 0 )
			{
				throw new BadRequestException("Номер счета не может быть 0 или отрицательным числом");
			}
			
			UserAccount account = _usersAccountsRepository.Get(accountId);
			if (account == null)
			{
				throw new NotFoundException("Счет не найден");
			}

			return account;
		}
	}
}