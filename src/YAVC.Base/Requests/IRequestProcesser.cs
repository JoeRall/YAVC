using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace YAVC.Base.Requests {
	public interface IRequestProcesser {
		Task<SendResult> Process(Queue<RequestInfo> requests, Func<string, SendResult> onResult);
	}
}
