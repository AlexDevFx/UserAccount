namespace UsersAccounts.Models
{
	public class ApiResponse
	{
		public ApiResponse(object result, string message = "", string status = "")
		{
			Result = result;
			Message = message;
			Status = status;
		}

		public object Result { get; set; }
		public string Message { get; set; }
		public string Status { get; set; }
	}
}