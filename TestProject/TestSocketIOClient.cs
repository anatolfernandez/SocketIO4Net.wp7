using System;
using System.Diagnostics;
using System.Threading;
using SocketIOClient;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Text.RegularExpressions;
using SocketIOClient.Messages;

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
			Console.WriteLine("Starting TestSocketIOClient Example...");

			socket = new Client("http://127.0.0.1:3000/"); // url to the nodejs / socket.io instance

			socket.Opened += SocketOpened;
			socket.Message += SocketMessage;
			socket.SocketConnectionClosed += SocketConnectionClosed;
			socket.Error += SocketError;

			// register for 'connect' event with io server
			socket.On("connect", (fn) =>
			{
				Console.WriteLine("\r\nConnected event...\r\n");
				Console.WriteLine("Emit Part object");

				// emit Json Serializable object, anonymous types, or strings
				Part newPart = new Part() { PartNumber = "K4P2G324EC", Code = "DDR2", Level = 1 };
				socket.Emit("partInfo", newPart);
			});

			// register for 'update' events - message is a json 'Part' object
			socket.On("update", (data) =>
			{
				Console.WriteLine("recv [socket].[update] event");
				//Console.WriteLine("  raw message:      {0}", data.RawMessage);
				//Console.WriteLine("  string message:   {0}", data.MessageText);
				//Console.WriteLine("  json data string: {0}", data.Json.ToJsonString());
				//Console.WriteLine("  json raw:         {0}", data.Json.Args[0]);

				// cast message as Part - use type cast helper
				Part part = data.Json.GetFirstArgAs<Part>();
				Console.WriteLine(" Part Level:   {0}\r\n", part.Level);
			});

			// make the socket.io connection
			socket.Connect();
		}
		public void CallbackExample()
		{
			Console.WriteLine("Emit [socket].[messageAck] - should recv callback [root].[messageAck]");
			socket.Emit("messageAck", new { hello = "mama" }, null,
				(callback) =>
				{
					var jsonMsg = callback as JsonEncodedEventMessage; // callback will be of type JsonEncodedEventMessage, cast for intellisense
					Console.WriteLine(string.Format("callback [root].[messageAck]: {0} \r\n", jsonMsg.Args));
				});
		}

		private ManualResetEvent testHold = new ManualResetEvent(false);
		void direct_Example()
		{

			var directSocket = new Client("http://127.0.0.1:3000/logger"); // url to the nodejs
			directSocket.Connect();
			directSocket.On("connect", (fn) =>
			{
				Console.WriteLine("\r\nConnected event...\r\n");
			});


			directSocket.On("traceEvent", (eventLog) =>
				{
					// do something with eventLog
				});

			directSocket.Emit("messageAck", new { hello = "papa" });
		}
		void similar_Example()
		{
			var socket = new Client("http://127.0.0.1:3000/"); // url to the nodejs
			socket.Connect();
			socket.On("connect", (fn) =>
			{
				Console.WriteLine("\r\nConnected event...\r\n");
			});

			var logger = socket.Connect("/logger"); // connect to the logger ns
			logger.On("traceEvent", (eventLog) =>
			{
				// do something with eventLog
			});
		}
		void explicit_Example()
		{
			var socket = new Client("http://127.0.0.1:3000/"); // url to the nodejs
			socket.Connect();
			socket.Connect("/logger");

			// EventMessage by namespace
			socket.On("traceEvent", "/logger", (eventLog) =>
			{
				Console.WriteLine("recv #1 [logger].[traceEvent] : {0}\r\n", eventLog.Json.GetFirstArgAs<EventLog>().ToJsonString());
			});
			socket.Emit("messageAck", new { hello = "papa" }, "/logger");
		}
		//			testHold.WaitOne(5000);
		IEndPointClient logger;
		public void NamespaceExample()
		{
			// two ways to work with namespace - your preference - they function exactly the same
			//   note: only one handler is allowed per event name, the last handler registered wins!
			//   event names are logically == (eventName + endpoint)

			// traditional socket.io syntax style
			if (logger == null)
				logger = socket.Connect("/logger"); // connect to the logger ns on server, must use '/' dir prefix

			logger.On("traceEvent", (eventLog) =>
			{
				Console.WriteLine("recv #2 [logger].[traceEvent] : {0}", eventLog.Json.GetFirstArgAs<EventLog>().ToJsonString());
			});
			// optional way - using EventMessage by namespace
			//socket.On("traceEvent", "/logger", (eventLog) =>
			//{
			//    Console.WriteLine("recv #1 [logger].[traceEvent] : {0}\r\n", eventLog.Json.GetFirstArgAs<EventLog>().ToJsonString());
			//});

			Console.WriteLine("Emit [logger].[messageAck] - should recv callback [logger].[messageAck]");
			logger.Emit("messageAck", new { hello = "papa" }, (callback) =>
			{
				var jsonMsg = callback as JsonEncodedEventMessage; // callback will be of type JsonEncodedEventMessage, cast for intellisense
				Console.WriteLine(string.Format("recv [logger].[messageAck]: {0} \r\n", jsonMsg.ToJsonString()));
			});
		}

		void MiscExamples()
		{
			// random examples of different styles of sending / recv payloads - will add to...
			socket.Send(new TextMessage("Hello Nodejs !")); // send plain string message
			socket.Send(new TextMessage("Hello Nodejs !") { Endpoint = "/logger" });  // send plain string message to endpoint (namespace)

			socket.Emit("simple", "Hello nodejs!");
			socket.Emit("partInfo", new { PartNumber = "AT80601000741AA", Code = "SLBEJ", Level = 1 });

			// EventMessage by namespace
			socket.On("traceEvent", "/logger", (eventLog) =>
			{
				Console.WriteLine("recv #1 [logger].[traceEvent] : {0}\r\n", eventLog.Json.GetFirstArgAs<EventLog>().ToJsonString());
			});

			// callback using namespace example 
			Console.WriteLine("Emit [socket.logger].[messageAck] - should recv callback [socket::logger].[messageAck]");
			socket.Emit("messageAck", new { hello = "papa" }, "/logger",
				(callback) =>
				{
					var jsonMsg = callback as JsonEncodedEventMessage; // callback will be of type JsonEncodedEventMessage, cast for intellisense
					Console.WriteLine(string.Format("callback [socket::logger].[messageAck]: {0} \r\n", jsonMsg.ToJsonString()));
				});
		}

		void SocketError(object sender, ErrorEventArgs e)
		{
			Console.WriteLine("socket client error:");
			Console.WriteLine(e.Message);
		}

		void SocketConnectionClosed(object sender, EventArgs e)
		{
			Console.WriteLine("WebSocketConnection was terminated!");
		}

		void SocketMessage(object sender, MessageEventArgs e)
		{
			// uncomment to show any non-registered messages
			//if (string.IsNullOrEmpty(e.Message.Event))
			//    Console.WriteLine("Generic SocketMessage: {0}", e.Message.MessageText);
			//else
			//    Console.WriteLine("Generic SocketMessage: {0} : {1}", e.Message.Event, e.Message.JsonEncodedMessage.ToJsonString());
		}

		void SocketOpened(object sender, EventArgs e)
		{

		}

		public void Close()
		{
			if (this.socket != null)
			{
				socket.Opened -= SocketOpened;
				socket.Message -= SocketMessage;
				socket.SocketConnectionClosed -= SocketConnectionClosed;
				socket.Error -= SocketError;
				this.socket.Dispose(); // close & dispose of socket client
			}
		}
	}

}
