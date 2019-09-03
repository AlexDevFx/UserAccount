using System;

namespace Core
{
	public interface IClockProvider
	{
		DateTime Now();
	}

	public class ClockProvider : IClockProvider
	{
		public DateTime Now()
		{
			return DateTime.UtcNow;
		}
	}
}