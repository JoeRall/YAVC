using YAVC.Base.Data;

namespace YAVC.Base.Commands {

	public abstract class AZoneCommand : ACommand {
		public Zone TheZone { get; protected set; }

		protected AZoneCommand(Zone z) {
			TheZone = z;
		}
	}
}
