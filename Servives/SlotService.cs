namespace SmartCanteen.Services
{
	public interface ISlotService
	{
		SlotResult Calculate(
			DateTime orderTime,
			int longestPrep,
			int totalQty,
			int pendingOrders);
	}

	public record SlotResult(
		int PrepMinutes,
		int QueueMinutes,
		DateTime ReadyAt,
		DateTime Slot);

	public class SlotService : ISlotService
	{
		public SlotResult Calculate(
			DateTime orderTime,
			int longestPrep,
			int totalQty,
			int pendingOrders)
		{
			var prepMinutes = Math.Min(longestPrep, 30);

			var queueMinutes = (int)Math.Ceiling(
				pendingOrders / 2.0);

			var readyAt = orderTime.AddMinutes(
				prepMinutes + queueMinutes);

			var slotMinutes =
				(int)Math.Ceiling(
					readyAt.Minute / 10.0) * 10;

			var slot = new DateTime(
				readyAt.Year,
				readyAt.Month,
				readyAt.Day,
				readyAt.Hour,
				0,
				0);

			if (slotMinutes == 60)
			{
				slot = slot.AddHours(1);
			}
			else
			{
				slot = slot.AddMinutes(slotMinutes);
			}

			return new SlotResult(
				prepMinutes,
				queueMinutes,
				readyAt,
				slot);
		}
	}
}