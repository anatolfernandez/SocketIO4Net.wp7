using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketIOClient.Messages;

namespace SocketIOClient.Eventing
{
	public class RegistrationManager : IDisposable
	{
		private ConcurrentDictionary<int, Action<dynamic>> callBackRegistry;
		private ConcurrentDictionary<string, Action<IMessage>> eventNameRegistry;

		public RegistrationManager()
		{
			this.callBackRegistry = new ConcurrentDictionary<int, Action<dynamic>>();
			this.eventNameRegistry = new ConcurrentDictionary<string, Action<IMessage>>();
		}
		public void AddCallBack(IMessage message)
		{
			EventMessage eventMessage = message as EventMessage;
			if (eventMessage != null)
				this.callBackRegistry.AddOrUpdate(eventMessage.AckId.Value, eventMessage.Callback, (key, oldValue) => eventMessage.Callback);
		}
		public void AddCallBack(int ackId, Action<dynamic> callback)
		{
			this.callBackRegistry.AddOrUpdate(ackId, callback, (key, oldValue) => callback);
		}
		
		public void InvokeCallBack(int? ackId, string value)
		{
			Action<dynamic> target = null;
			if (ackId.HasValue)
			{
				if (this.callBackRegistry.TryRemove(ackId.Value, out target))
				{
					target.BeginInvoke(value, target.EndInvoke, null);
				}
			}
		}
		public void InvokeCallBack(int? ackId, JsonEncodedEventMessage value)
		{
			Action<dynamic> target = null;
			if (ackId.HasValue)
			{
				if (this.callBackRegistry.TryRemove(ackId.Value, out target))
				{
					target.BeginInvoke(value, target.EndInvoke, null);
				}
			}
		}

		public void AddOnEvent(string eventName, Action<IMessage> callback)
		{
			this.eventNameRegistry.AddOrUpdate(eventName, callback, (key, oldValue) => callback);
		}

		public void InvokeOnEvent(string eventName, IMessage value)
		{
			Action<IMessage> target;
			if (this.eventNameRegistry.TryRemove(eventName, out target))
				target.BeginInvoke(value, target.EndInvoke, null);
		}
	
		public void  Dispose()
		{
			this.callBackRegistry.Clear();
			this.eventNameRegistry.Clear();
		}
}
}
