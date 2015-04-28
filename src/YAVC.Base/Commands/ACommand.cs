using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YAVC.Base.Requests;

namespace YAVC.Base.Commands {

	public abstract class ACommand : ICommand {

		protected virtual string UserAgent { get { return @"AVcontrol/1.03 CFNetwork/485.12.30 Darwin/10.4.0"; } }
		protected virtual IEnumerable<KeyValuePair<string, string>> AdditionalHeaders { get { return new List<KeyValuePair<string, string>>(); } }

		protected abstract SendResult ParseResponseImp(string xml);
		protected abstract RequestInfo[] GetRequestInfo();
		
		public async Task<SendResult> SendAsync(IController c) {
			var requests = GetRequestInfo();
			var infos = new Queue<RequestInfo>(requests.Length);

			foreach (var req in requests) {
				//-- Setup the common Request properties
				req.AddHeaderIfNotExists("User-Agent", UserAgent);
				foreach (var header in AdditionalHeaders) {
					req.AddHeaderIfNotExists(header.Key, header.Value);
				}

				infos.Enqueue(req);
			}

            var sr = await c.RequestProccessor.Process(infos, c.HostNameorAddress, ParseResponseImp);

            return sr;
		}
	}
}
