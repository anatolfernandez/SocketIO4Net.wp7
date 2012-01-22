using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestProject
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("To Exit  Console: 'q'");
			Console.WriteLine("To Clear Console: 'c'");
			Console.WriteLine("To Send  event2:  '2'");
			Console.WriteLine("=============================");
			Console.WriteLine("");
			Console.ResetColor();

			TestSocketIOClient tClient = new TestSocketIOClient();
			tClient.Execute();

			bool run = true;
			while (run)
			{
				string line = Console.ReadLine();
				if (!string.IsNullOrWhiteSpace(line))
				{
					char key = line.FirstOrDefault();
					switch (key)
					{
						case 'c':
						case 'C':
							Console.Clear();
							break;

						case 'q':
						case 'Q':
							run = false;
							break;
						
						case '2':
							tClient.SendEvent2();
							break;
						default:
							break;
					}
				}
			}
			tClient.Close();
		}
	}
}
