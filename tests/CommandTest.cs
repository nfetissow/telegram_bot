using System;
using Xunit;
using telegram_bot;

namespace tests
{
    public class CommandTests
    {
        [Fact]
        public void TestCommandNoParameter()
        {
            Command myCommand = Command.TryParse("/help");
            Assert.Equal("help", myCommand.CommandCode);
        }
        [Fact]
        public void TestsCommandWithParameter()
        {
            Command myCommand = Command.TryParse("/help me");
            Assert.Equal("help", myCommand.CommandCode);
            Assert.Single(myCommand.Parameters);
            Assert.Equal("me", myCommand.Parameters[0]);
        }
    }
}
