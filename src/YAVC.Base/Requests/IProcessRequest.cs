using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace YAVC.Base.Requests {
	public interface IProcessRequest {
		Task<SendResult> Process(Queue<RequestInfo> requests, string hostname, Action<string> onResult);
	}
}
