using System;
namespace SocketIOClient
{
	/// <summary>
	/// C# Socket.IO client interface
	/// </summary>
	interface IClient
	{
		event EventHandler Opened;
		event EventHandler<MessageEventArgs> Message;
		event EventHandler SocketConnectionClosed;
		event EventHandler<ErrorEventArgs> Error;

		SocketIOHandshake HandShake { get; }
		bool IsConnected { get; }
		WebSocket4Net.WebSocketState ReadyState { get; }

		void Connect();
		void Close();
		void Dispose();

		void On(string eventName, Action<SocketIOClient.Messages.IMessage> action);
		void Emit(string eventName, dynamic payload, Action<dynamic> callBack = null);

		void Send(SocketIOClient.Messages.IMessage msg);
		//void Send(string rawEncodedMessageText);
	}
}
