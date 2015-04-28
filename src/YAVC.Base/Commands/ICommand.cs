using System;
using System.Threading.Tasks;
using YAVC.Base.Requests;

namespace YAVC.Base.Commands {
	public interface ICommand {
        Task<SendResult> SendAsync(IRequestProcesser requestProcessor);
	}
}
