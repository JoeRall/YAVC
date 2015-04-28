using System;
using YAVC.Base.Requests;

namespace YAVC.Base.Commands {
	public interface ICommand {
		void Send(IController c, Action<SendResult> onCompleted);
	}
}
