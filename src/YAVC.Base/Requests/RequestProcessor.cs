using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace YAVC.Base.Requests
{
    public class RequestProcessor : IRequestProcesser
    {

        private static Encoding CommandEncoding { get { return Encoding.UTF8; } }

        private readonly string _Hostname;

        public RequestProcessor(string hostname)
        {
            _Hostname = hostname;
        }

        #region IProcessRequest Members
        public async Task<SendResult> Process(Queue<RequestInfo> requests, Func<string, SendResult> onResult)
        {
            while (requests.Count > 0)
            {
                try
                {
                    var req = requests.Dequeue();
                    var result = await ProcessImp(req, onResult);
                    if (!result.Success)
                        return result;
                }
                catch (Exception exp)
                {
                    return SendResult.Error(exp);
                }
            }

            return SendResult.Succcess;
        }
        #endregion

        protected virtual async Task<SendResult> ProcessImp(RequestInfo info, Func<string, SendResult> onResult)
        {
            var uri = string.Format("http://{0}:{1}/{2}", _Hostname, info.Port, info.RelativeUri);
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

