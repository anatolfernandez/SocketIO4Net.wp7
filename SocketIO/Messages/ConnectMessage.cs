using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketIOClient.Messages
{
	public class ConnectMessage : Message
	{
		public string Path { get; set; }
		public string Query { get; set; }

		public override string Event
		{
			get { return "connect"; }
		}

		public ConnectMessage() : base()
		{
			this.MessageType = SocketIOMessageTypes.Connect;
		}

		public static ConnectMessage Deserialize(string rawMessage)
		{
			ConnectMessage msg = new ConnectMessage();
			//  1:: [path] [query]
			//  1::/test?my=param
			msg.RawMessage = rawMessage;

			string[] args = rawMessage.Split(SPLITCHARS, 3);
			if (args.Length == 3)
			{
				string[] pq = args[2].Split(new char[] { '?' });

				if (pq.Length > 0)
					msg.Path = pq[0];
				
				if (pq.Length > 1)
					msg.Query = pq[1];
			}
			return msg;
		}
	}
}
