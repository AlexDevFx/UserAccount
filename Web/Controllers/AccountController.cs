using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Models;
using Core.Services;
using Microsoft.AspNetCore.Mvc;
using UsersAccounts.Models.UsersAccounts;

namespace UsersAccounts.Controllers
{
	[Produces("application/json")]
	[Route("api/account")]
	[ApiController]
	public class AccountController : ControllerBase
	{
		private readonly IUsersAcccountsManager _usersAcccountsManager;
		private readonly IMapper _objectsMapper;

		public AccountController(IUsersAcccountsManager usersAcccountsManager, IMapper objectsMapper)
		{
			_objectsMapper = objectsMapper;
			_usersAcccountsManager = usersAcccountsManager;
		}
		
		
		[HttpGet("{accountId}/history")]
		public ActionResult<IList<AccountHistoryDto>> History(int accountId)
		{
			var accountHistory = _usersAcccountsManager.GetHistory(accountId);
			var accountHistoryDto = _objectsMapper.Map<IList<AccountHistoryDto>>(accountHistory
				.OrderByDescending(e => e.ChangedAt));
			return Ok(accountHistoryDto);
		}
		
		[HttpPost("{accountId}/top-up")]
		public async Task<ActionResult<decimal>> TopUp(int accountId, [FromBody] TransferInput input)
		{
			decimal balance = await _usersAcccountsManager.Deposit(accountId, input.Amount);
			return Ok(balance);
		}

		[HttpPost("{accountId}/withdraw")]
		public async Task<ActionResult<decimal>> Withdraw(int accountId, [FromBody] TransferInput input)
		{
			decimal balance = await _usersAcccountsManager.Withdraw(accountId, input.Amount);
			return Ok(balance);
		}

		[HttpPost("{sourceAccountId}/transfer/{destinationAccountId}")]
		public async Task<ActionResult<TransferResult>> Transfer(int sourceAccountId, int destinationAccountId, [FromBody] TransferInput input)
		{
			TransferResult result = await _usersAcccountsManager.Transfer(sourceAccountId, destinationAccountId, input.Amount);
			return Ok(result);
		}
	}

	public class TransferInput
	{
		public decimal Amount { get; set; }
	}
}