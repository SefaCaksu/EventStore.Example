namespace Shared.Events
{
    public class AvailableChangedEvent
	{
		public string ProductId { get; set; }
		public bool IsAvailable { get; set; }
    }
}

