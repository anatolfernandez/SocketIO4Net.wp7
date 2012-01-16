using System;
using System.Diagnostics;
using System.Threading;
using SocketIOClient;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Text.RegularExpressions;

namespace TestProject
{
	public class TestSocketIOClient
	{
		ManualResetEvent competeEvent = new ManualResetEvent(false);
		ManualResetEvent waitEvent = new ManualResetEvent(false);
		ManualResetEvent msgEvent = new ManualResetEvent(false);
		ManualResetEvent sleepEvent = new ManualResetEvent(false);
		public void Connect()
		{
			
			string url = "http://127.0.0.1:8080/";
			Client socket = new Client(url,true );
			socket.Opened += new EventHandler(socket_Opened);
			socket.Message += new EventHandler<MessageEventArgs>(socket_Message);
			socket.SocketConnectionClosed += new EventHandler(socket_SocketConnectionClosed);
			
			socket.On("news", (data) =>
			{
				Trace.WriteLine(data.MessageText);
				msgEvent.Set();
			}); 
			socket.On("open", (fn) => 
			{
				socket.Emit("my other event", new { Assessment = "1234", Response = "A", Points = 5 });
				socket.Emit("my other event", new { Assessment = "5678", Response = "B", Points = 1 });
				sleepEvent.WaitOne(30000);
				socket.Emit("my other event", new { Assessment = "9123", Response = "C", Points = 1 });
				sleepEvent.WaitOne(30000);
				socket.Emit("my other event", new { Assessment = "9123", Response = "C", Points = 1 });

			});
			socket.Connect();
			competeEvent.WaitOne(60000);
			socket.Dispose();
		}
		Client socket;
		public void TestSocket()
		{
			string url = "http://127.0.0.1:8080/";
			socket = new Client(url, true);
			socket.Opened += new EventHandler(socket_Opened);
			socket.Message += new EventHandler<MessageEventArgs>(socket_Message);
			socket.SocketConnectionClosed += new EventHandler(socket_SocketConnectionClosed);

			socket.On("news", (data) =>
			{
				Console.WriteLine("news dynamic event:");
				Console.WriteLine(data.JsonEncodedMessage.ToJson());
			});
			socket.On("connect", (f) =>
			{
				Console.WriteLine("Connect dynamic event");
			});
			socket.On("open", (f) =>
			{
				Console.WriteLine("Open dynamic event");
			});
			
			socket.Connect();

			competeEvent.WaitOne(15000);
		}

		void socket_SocketConnectionClosed(object sender, EventArgs e)
		{
			Console.WriteLine("SocketConnection was terminated!");
		}

		void socket_Message(object sender, MessageEventArgs e)
		{
			//Console.WriteLine(e.Message.MessageText);
		}

		void socket_Opened(object sender, EventArgs e)
		{
			waitEvent.Set();
			socket.Emit("my other event", new { Assessment = "1234", Response = "A", Points = 5 });

			socket.Emit("messageAck", new { hello = "ma" },
				(data) =>
				{
					Console.WriteLine(string.Format("Ack Message received: {0} ",data.Args));
					competeEvent.Set();
				});
		}

		
	}
}
