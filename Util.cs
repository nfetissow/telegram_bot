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
                int firstSpace = message.IndexOf(' ');
                if (firstSpace == -1)
                    //the message only consists of a command string
                    return message.Substring(1);

                // return the command that comes after the / but ends before the first space
                return message.Substring(1, /*length*/firstSpace - 1);
            }
            return null;
        }
    }
}
