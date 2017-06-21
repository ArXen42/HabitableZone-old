namespace HabitableZone.Core.World.Society
{
	/// <summary>
	///    Provides access to game characters collection. For now we have only player.
	/// </summary>
	public class Captains
	{
		public Captains(WorldContext worldContext, CaptainsData data)
		{
			Player = data.PlayerData.GetInstanceFromData(worldContext);
		}

		public CaptainsData GetSerializationData() => new CaptainsData(this);

		/// <summary>
		///    It's you.
		/// </summary>
		public Captain Player { get; set; }
	}

	public struct CaptainsData
	{
		public CaptainsData(Captains captains)
		{
			PlayerData = captains.Player.GetSerializationData();
		}

		public Captains GetInstanceFromData(WorldContext worldContext) => new Captains(worldContext, this);

		public CaptainData PlayerData;
	}
}