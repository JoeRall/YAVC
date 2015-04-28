using System;
using System.Collections.Generic;

namespace YAVC.Base.Commands {
	public interface IProcessRequest {
		void Process(Queue<RequestInfo> requests, string hostname, Action<string> onResult, Action<SendResult> onCompleted);
	}
}
