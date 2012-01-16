using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketIOClient.Messages
{
    public class NoopMessage : Message
    {
        public NoopMessage()
        {
            this.MessageType = SocketIOMessageTypes.Noop;
        }
        public static NoopMessage Deserialize(string rawMessage)
        {
			return new NoopMessage();
        }
    }
}
