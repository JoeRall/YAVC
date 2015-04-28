using System;
using YAVC.Base.Data;

namespace YAVC.Base {
	public interface IFindDevice {
		void FindAllAsync(int seconds, Action<Device> FoundCallback, Action FinishedSearching);
	}
}
