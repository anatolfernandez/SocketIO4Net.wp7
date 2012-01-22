using System;
using System.Diagnostics;
using System.Threading;
using SocketIOClient;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Text.RegularExpressions;

namespace TestProject
{
	/// <summary>
	/// Example usage class for SocketIO4Net
	/// </summary>
	public class TestSocketIOClient
	{
		Client socket;
		public void Execute()
		{
			Console.WriteLine("Starting SocketIOClient...");
			socket = new Client("http://127.0.0.1:3000/"); // url to the nodejs / socket.io instance

			socket.Opened += socket_Opened;
			socket.Message += socket_OnMessage;
			socket.SocketConnectionClosed +=socket_SocketConnectionClosed;
			socket.Error += socket_Error;
			
			socket.On("connect", (fn) =>
			{
				Console.WriteLine("\r\nConnected event...");
				Console.WriteLine("\tsending 'event1' event");
				socket.Emit("event1", new { Item = "5678", Code = "A", Points = 1 });
			});

			socket.On("news", (data) =>
			{
				Console.WriteLine("\r\nrecv'd 'news' event: {0}",data.JsonEncodedMessage.ToJsonString());
				Console.WriteLine("\tsending 'event2' event");
                socket.Emit("event2", new { my = "from a .net client instance" });
			});
			
			socket.Connect();
		}

		
		public void SendEvent2()
		{
			string payload = string.Format("from .net client at {0}", DateTime.Now.ToLongTimeString());
			socket.Emit("event2", new { my = payload });
		}

		void socket_Error(object sender, ErrorEventArgs e)
		{
			Console.WriteLine("socket client error:");
			Console.WriteLine(e.Message);
		}

		void socket_SocketConnectionClosed(object sender, EventArgs e)
		{
			Console.WriteLine("WebSocketConnection was terminated!");
		}

		void socket_OnMessage(object sender, MessageEventArgs e)
		{
			if (string.IsNullOrEmpty(e.Message.Event))
				Console.WriteLine("socket_OnMessage: {0}",e.Message.MessageText);
			else
				Console.WriteLine("socket_OnMessage: {0} : {1}",e.Message.Event, e.Message.JsonEncodedMessage.ToJsonString());
		}

		void socket_Opened(object sender, EventArgs e)
		{
			//socket.Emit("messageAck", new { hello = "ma" },
			//    (data) =>
			//    {
			//        Console.WriteLine(string.Format("Ack Message received: {0} ",data.Args));
			//    });
		}

		public void Close()
		{
			if (this.socket != null)
			{
				socket.Opened -=socket_Opened;
				socket.Message -= socket_OnMessage;
				socket.SocketConnectionClosed -= socket_SocketConnectionClosed;
				socket.Error -= socket_Error;
				this.socket.Dispose(); // close & dispose of socket client
			}
		}
	}
	
}
