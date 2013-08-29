using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketIOClient
{
    public static class StringSplitHelper
    {
        public static IList<string> Split(this string input, char separator, int maxElements)
        {
            List<string> list = new List<string>(maxElements);

            int length = input.Length;
            int anchor = 0;
            int numberOfElements = 0;

            for (int i = 0; i < length; i++)
            {
                if (input[i] == separator)
                {
                    int strLength = i - anchor;
                    string slice = input.Substring(anchor, strLength);
                    list.Add(slice);

                    anchor = i + 1;
                    numberOfElements++;
                }
                if (numberOfElements == maxElements - 1)
                {
                    list.Add(input.Substring(anchor));

                    break;
                }
            }

            return list;
        }
    }
}
