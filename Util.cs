using System;
using System.Collections.Generic;
using System.Text;

namespace telegram_bot
{
    abstract class Util
    {
        public static string getCommandoString(string message)
        {
            if (message.StartsWith('/'))
            {
                return message.Substring(1, message.IndexOf(' '));
            }
            return null;
        }
    }
}
