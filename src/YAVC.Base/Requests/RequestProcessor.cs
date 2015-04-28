using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace YAVC.Base.Requests
{
    public class RequestProcessor : IProcessRequest
    {

        private static Encoding CommandEncoding { get { return Encoding.UTF8; } }

        private void Process(RequestState state)
        {
            Process(state.Infos, state.HostName, state.OnResponse, state.OnCompleted);
        }

        #region IProcessRequest Members
        public void Process(Queue<RequestInfo> requests, string hostname, Action<string> onResult, Action<SendResult> onCompleted)
        {
        }
        public async Task<SendResult> Process(Queue<RequestInfo> requests, string hostname, Action<string> onResult)
        {
            while (requests.Count > 0)
            {
                try
                {
                    var req = requests.Dequeue();
                    await ProcessImp(req, hostname, onResult);
                }
                catch (Exception exp)
                {
                    return SendResult.Error(exp);
                }
            }

            return SendResult.Succcess;
        }
        #endregion

        protected virtual async Task<SendResult> ProcessImp(RequestInfo info, string hostname, Action<string> onResult)
        {
            var uri = string.Format("http://{0}:{1}/{2}", hostname, info.Port, info.RelativeUri);
            var req = (HttpWebRequest)HttpWebRequest.CreateHttp(uri);

            foreach (var header in info.Headers)
            {
                req.Headers[header.Key] = header.Value;
                if (header.Key == "User-Agent") //-- Special header
                    req.Headers[HttpRequestHeader.UserAgent] = header.Value;
            }

            req.Method = info.Method == yavcMethod.Get ? "GET" : "POST";

            if (info.Method == yavcMethod.Post)
            {
                var body = CommandEncoding.GetBytes(info.Body);

                using (var reqStream = await req.GetRequestStreamAsync())
                {
                    await reqStream.WriteAsync(body, 0, body.Length);
                }
            }

            using (var response = await req.GetResponseAsync())
            using (var resStream = response.GetResponseStream())
            using (var sr = new StreamReader(resStream))
            {
                var result = await sr.ReadToEndAsync();
                onResult(result);
            }

            return SendResult.Succcess;
        }
    }
}

