using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketIOClient.Messages
{
	/// <summary>
	/// Signals disconnection. If no endpoint is specified, disconnects the entire socket.
	/// </summary>
	/// <remarks>Disconnect a socket connected to the /test endpoint:
	///		0::/test
	/// </remarks>
	public class DisconnectMessage : Message
	{

		public override string Event
		{
			get { return "disconnect"; }
		}

		public DisconnectMessage() : base()
		{
			this.MessageType = SocketIOMessageTypes.Disconnect;
		}
		public DisconnectMessage(string endPoint)
			: this()
		{
			this.Endpoint = endPoint;
		}
		public static DisconnectMessage Deserialize(string rawMessage)
		{
			DisconnectMessage msg = new DisconnectMessage();
			//  0::
			//  0::/test
			msg.RawMessage = rawMessage;

			IList<string> args = rawMessage.Split(Separator, 3);
			if (args.Count == 3)
			{
				if (!string.IsNullOrWhiteSpace(args[2]))
					msg.Endpoint = args[2];
			}
			return msg;
		}
		public override string Encoded
		{
			get
			{
				return string.Format("0::{0}", this.Endpoint);
			}
		}
	}
}
