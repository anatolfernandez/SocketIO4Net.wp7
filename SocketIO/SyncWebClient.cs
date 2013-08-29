using System;
using System.Collections.Specialized;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;

namespace SocketIOClient
{
    internal class SyncWebClient : IDisposable
    {
        private readonly WebClient _webClient;
        private readonly AutoResetEvent _autoResetEvent = new AutoResetEvent(false);

        private string _result;

        public SyncWebClient()
        {
            _webClient = new WebClient();
            _webClient.DownloadStringCompleted += onDownloadStringCompleted;
        }

        public void AddHeaders(NameValueCollection headers)
        {
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    _webClient.Headers[header.Key] = header.Value;
                }
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public string DownloadString(string uri)
        {
            _webClient.DownloadStringAsync(new Uri(uri));
            _autoResetEvent.WaitOne();

            return _result;
        }

        private void onDownloadStringCompleted(object sender, DownloadStringCompletedEventArgs downloadStringCompletedEventArgs)
        {
            _result = downloadStringCompletedEventArgs.Result;
            _autoResetEvent.Set();
        }

        public void Dispose()
        {
            _autoResetEvent.Close();
        }
    }
}
