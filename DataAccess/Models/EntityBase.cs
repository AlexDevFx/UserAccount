namespace DataAccess.Models
{
	public interface IEntityBase<TKey>
	{
		TKey Id { get;  }
	}
	
	public class EntityBase<TKey>: IEntityBase<TKey>
	{
		public TKey Id { get; set; }
	}
}