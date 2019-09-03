using System;

namespace Core.Exceptions
{
	public class InternalErrorException: Exception
	{
		public InternalErrorException(string message) : base(message)
		{
		}
	}
}