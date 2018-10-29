using System;
using System.Collections.Generic;
using System.Text;

namespace telegram_bot
{
    /// <summary>
    /// Represents a command from the user.
    /// </summary>
    public class Command
    {
        public string CommandCode { get; }

        public string[] Parameters { get; }

        Command(string commandCode, string[] parameters)
        {
            CommandCode = commandCode;
            Parameters = parameters;
        }

        /// <summary>
        /// Trys to parse the user text as a command.
        /// </summary>
        /// <param name="UserText"></param>
        /// <returns>The command if it could be successfully parsed, <c>null</c> otherwise.</returns>
        public static Command TryParse(string UserText)
        {
            UserText = UserText.Trim();
            if(UserText.StartsWith('/'))
            {
                int firstSpace = UserText.IndexOf(' ');
                string command;
                string[] parameters = null;

                if (firstSpace == -1)
                    //only command, no parameters
                    command = UserText.Substring(1);
                else {
                    //extract command and parameters
                    command = UserText.Substring(1, firstSpace - 1);
                    parameters = UserText.Substring(firstSpace + 1).Split(' ');
                }
                //remove the name of the bot if included
                int firstAt = command.IndexOf('@');
                if(firstAt != -1)
                {
                    string botName = command.Substring(firstAt + 1);
                    if (!Controller.BOT_NAME.Equals(botName))
                    {
                        //not meant for our bot
                        return null;
                    }
                    else
                        command = command.Substring(0, firstAt);
                }

                return new Command(command, parameters);
            }
            return null;
        }
    }
}
