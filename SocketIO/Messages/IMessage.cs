using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace SocketIOClient.Messages
{
    public interface IMessage
    {
		SocketIOMessageTypes MessageType { get; }
		string RawMessage { get; }

		string Event { get; set; }
		int? AckId { get; }
		string Endpoint { get; }
		
        string MessageText { get; }
		JsonEncodedEventMessage JsonEncodedMessage { get; }

        string Encoded { get; }
        
    }
}