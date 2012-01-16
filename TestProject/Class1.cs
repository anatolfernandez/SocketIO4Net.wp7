using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using SocketIO.

namespace TestProject
{
    public class Class1
    {
		ManualResetEvent waitEvent = new ManualResetEvent(false);
		
        public void TestWSC()
        {
            //Uri TargetUri = new Uri("ws://vr.conveyclassrooms.com:8080");
			Uri TargetUri = new Uri("ws://50.57.72.13:8080/");
            string path = this.getWSPath2(TargetUri);
            Uri fullUri = new Uri(TargetUri.ToString() + path);

            WebSocket4Net.WebSocket ws = new WebSocket4Net.WebSocket(fullUri.ToString(), WebSocket4Net.WebSocketVersion.DraftHybi10);
			ws.Opened += new EventHandler(ws_Opened);
			ws.Closed += new EventHandler(ws_Closed);
			ws.DataReceived += new EventHandler<WebSocket4Net.DataReceivedEventArgs>(ws_DataReceived);
			ws.MessageReceived += new EventHandler<WebSocket4Net.MessageReceivedEventArgs>(ws_MessageReceived);
            Debug.WriteLine(string.Format("WebSocket version: {0}", ws.Version));
			ws.Open();

            
           bool connected =  waitEvent.WaitOne(5000);
		   Debug.WriteLine("We connected:" + connected);
		   waitEvent.Reset();
		   //ws.Send("Hello World!");
		   waitEvent.WaitOne(10000);
			//ws.Send()
            ws.Close();
        }

		void ws_MessageReceived(object sender, WebSocket4Net.MessageReceivedEventArgs e)
		{
			Debug.WriteLine(e.Message);
		}

		void ws_Closed(object sender, EventArgs e)
		{
			Debug.WriteLine("SocketClient has closed");
		}

		void ws_Opened(object sender, EventArgs e)
		{
			WebSocket4Net.WebSocket ws = sender as WebSocket4Net.WebSocket;
			waitEvent.Set();
			//if (ws != null)
			//    ws.Send("Hello World!");

		}

		void ws_DataReceived(object sender, WebSocket4Net.DataReceivedEventArgs e)
		{
			
			string msg = Encoding.UTF8.GetString(e.Data);
			Debug.WriteLine(msg);
		}


        // this gets the socket.io request path
        protected string getWSPath(Uri uri)
        {
            string rd = string.Empty;
            using (TcpClient tcp = new TcpClient(uri.Host, uri.Port))
            {
				
                tcp.ReceiveTimeout = 5000;
                Stream stream = tcp.GetStream();
                StringBuilder writer = new StringBuilder();
                writer.AppendLine("GET /socket.io/1/ HTTP/1.1");
                writer.AppendLine("");
				Debug.WriteLine(writer.ToString());
                byte[] sendBuffer = Encoding.UTF8.GetBytes(writer.ToString());
                stream.Write(sendBuffer, 0, sendBuffer.Length);

                byte[] byteResp = new byte[tcp.ReceiveBufferSize];

                int bytesRead = stream.Read(byteResp, 0, byteResp.Length);
                rd += System.Text.Encoding.UTF8.GetString(byteResp, 0, bytesRead);
            }
            Debug.WriteLine(rd);
            string[] array = rd.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            string value = array[array.Length - 2];
               Debug.WriteLine(value);
            SocketIOHandshake handshake = SocketIOHandshake.LoadFromString(value);
            string path2 = string.Format("socket.io/1/websocket/{0}", handshake.SID);
            Debug.WriteLine(string.Format("Socket.io path: {0}", path2));
            return path2;
        }

		private void TestWithWebRequest()
		{
			Uri uri = new Uri("ws://50.57.72.13:8080/");
			string response = getWSPath2(uri);
			Debug.WriteLine(response);
		}

		protected string getWSPath2(Uri uri)
		{
			string output = "";
			string url = string.Format("http://{0}:{1}/socket.io/1/",uri.Host,uri.Port);
			Debug.WriteLine(url);
			WebRequest req = WebRequest.Create(url);
			req.Proxy = GlobalProxySelection.GetEmptyWebProxy(); // GlobalProxySelection.Select;
			using (WebResponse resp = req.GetResponse())
			{
				using (StreamReader sr = new StreamReader(resp.GetResponseStream()))
				{
					string line = sr.ReadToEnd().Trim();
					SocketIOHandshake handshake = SocketIOHandshake.LoadFromString(line);
					output = string.Format("socket.io/1/websocket/{0}", handshake.SID);
					Debug.WriteLine(string.Format("Socket.io path: {0}", output));
				}
			}
			return output;
		}
    }
}
