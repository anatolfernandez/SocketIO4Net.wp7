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
			Console.WriteLine("--> Examples");
			Console.WriteLine(" Callback sample: 'b'");
			Console.WriteLine(" Namespace sample: 'n'");
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
						case 'b':
						case 'B':
							tClient.CallbackExample();
							break;

						case 'n':
						case 'N':
							tClient.NamespaceExample();
							break;

						case 'q':
						case 'Q':
							run = false;
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
