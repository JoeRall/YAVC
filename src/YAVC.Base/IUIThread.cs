using System;

namespace YAVC.Base {
	public interface IUIThread {
		void Invoke(Action a);
		Action Wrap(Action a);
	}
}
