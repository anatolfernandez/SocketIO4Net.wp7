using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using System.Collections.Concurrent;
using SocketIOClient.Messages;

namespace SocketIOClient.Eventing
{
	/// <summary>
	/// EventManager provides a weak reference subscription event model.  As string-named events arrive from
	/// the Socket.IO server - subscribed Action<JsonEncodedEventMessage> events will be called.
	/// </summary>
	public class EventManager : IDisposable
	{
		// Dictionary of subscriber events
		private ConcurrentDictionary<string, WeakActionAndTokenList> _eventLibrary;

		public EventManager()
		{
			this._eventLibrary = new ConcurrentDictionary<string, WeakActionAndTokenList>();
		}

		/// <summary>
		/// Mimick the Socket.IO client 'socket.on('name',function(data){});' pattern
		/// </summary>
		/// <param name="eventName"></param>
		/// <param name="recipient"></param>
		/// <param name="action"></param>
		/// <param name="token"></param>
		public virtual void On(
			string eventName,
			Action<JsonEncodedEventMessage> action)
		{
			this.Register(eventName, action.Target, action, null);
		}

		/// <summary>
		/// Registers a recipient for a type of message TMessage.
		/// The action parameter will be executed when a corresponding 
		/// message is sent. 
		/// <para>Registering a recipient does not create a hard reference to it,
		/// so if this recipient is deleted, no memory leak is caused.</para>
		/// </summary>
		/// <param name="action">The action that will be executed when a message
		/// of type TMessage is sent.</param>
		public virtual void Register(
			string eventName,
			object recipient,
			Action<JsonEncodedEventMessage> action, object token = null)
		{
			var weakAction = new WeakAction<JsonEncodedEventMessage>(recipient, action);
			WeakActionAndToken addItem = new WeakActionAndToken() { WeakAction = weakAction, Token = token };
			WeakActionAndTokenList coll = _eventLibrary.GetOrAdd(eventName, val => new WeakActionAndTokenList());
			coll.Items.Add(addItem);
		}
		
		//public virtual void UnRegister(
		//    string eventName,
		//    object recipient,
		//    Action<JsonEncodedEventMessage> action, 
		//    object token = null)
		//{
		//    var weakAction = new WeakAction<JsonEncodedEventMessage>(recipient, action);
		//    WeakActionAndToken remItem = new WeakActionAndToken() { WeakAction = weakAction, Token = token };
		//     WeakActionAndTokenList coll = null;
		//     if (_eventLibrary.TryGetValue(eventName, out coll))
		//     {
		//         WeakActionAndToken found = coll.Items.Where(i => i.WeakAction.Action == remItem.WeakAction.Action).FirstOrDefault();
		//         if (found != null)
		//             coll.Items.Remove(found);
		//     }
		//}

		public virtual void Notify(string eventName, JsonEncodedEventMessage message, object token = null)
		{
			WeakActionAndTokenList coll = null;
			if (_eventLibrary.TryGetValue(eventName, out coll))
			{
				var items = coll.Items.ToArray();
				if (items.Count() == 0)
				{
					Trace.WriteLine("No subscribers found...");
				}
				else
				{
					foreach (var target in items)
					{
						var executeAction = target.WeakAction as IExecuteWithObject;
						if (executeAction != null
							&& target.WeakAction.IsAlive
							&& target.WeakAction.Target != null
							&& ((target.Token == null && token == null)
								|| target.Token != null && target.Token.Equals(token)))
						{
							executeAction.ExecuteWithObject(message);
						}

					}
				}
			}
			else
				Trace.WriteLine(string.Format("EventManager Notice: No event name found registered for '{0}' received from socket.io server",eventName));
		}

		private void Cleanup()
		{
			//var keys = this._eventLibrary.Keys;

			//var wa = from x in this._eventLibrary.Values
			//         from y in x.Items
			//         where y.WeakAction.IsAlive == false || y.WeakAction.Target == null
			//         select new { List = x.Items, Item = y };
			//wa.Item;

		}

		public void Dispose()
		{
			if (this._eventLibrary != null)
				this._eventLibrary.Clear();
		}
	}
}
