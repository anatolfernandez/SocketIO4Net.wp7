using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketIOClient.Eventing
{
	public class WeakActionAndTokenList
	{
		public List<WeakActionAndToken> Items;
		//public BlockingCollection<WeakActionAndToken> Items;
		public WeakActionAndTokenList()
		{
			//this.Items = new BlockingCollection<WeakActionAndToken>(new ConcurrentBag<WeakActionAndToken>());
			this.Items = new List<WeakActionAndToken>();
		}
	}
}
