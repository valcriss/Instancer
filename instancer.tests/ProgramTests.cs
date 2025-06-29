using System.Reflection;
using instancer.cli;
using instancer.cli.Commands;

namespace instancer.tests;

public class ProgramTests
{
    [Fact]
    public void CommandsAreRegisteredCorrectly()
    {
        var field = typeof(Program).GetField("_commands", BindingFlags.NonPublic | BindingFlags.Static)!;
        var commands = (string[])field.GetValue(null)!;
        Assert.Equal(new[] { "up", "down", "help" }, commands);
    }

    [Fact]
    public void SuggestReturnsClosestAlias()
    {
        var suggestion = Utils.Suggest("hep", new[] { "up", "down", "help" });
        Assert.Equal("help", suggestion);
    }

    [Fact]
    public void ProgramParsesUpParameters()
    {
        var output = new StringWriter();
        var original = Console.Out;
        Console.SetOut(output);
        try
        {
            var result = Program.Main(new[] { "up", "--file", "stack.yml", "--name", "mystack", "--verbose" });
            Assert.Equal(0, result);
        }
        finally
        {
            Console.SetOut(original);
        }

        var text = output.ToString();
        Assert.Contains("Executing 'up' command...", text);
        Assert.Contains("File: stack.yml", text);
        Assert.Contains("Name of the stack : mystack", text);
        Assert.Contains("Verbose mode enabled.", text);
    }

    [Fact]
    public void ProgramHandlesUnknownCommand()
    {
        var output = new StringWriter();
        var error = new StringWriter();
        var originalOut = Console.Out;
        var originalErr = Console.Error;
        Console.SetOut(output);
        Console.SetError(error);
        try
        {
            var result = Program.Main(new[] { "dwn" });
            Assert.Equal(1, result);
        }
        finally
        {
            Console.SetOut(originalOut);
            Console.SetError(originalErr);
        }

        var errText = error.ToString();
        Assert.Contains("Unknown command 'dwn'.", errText);
        Assert.Contains("Did you mean this?", errText);
    }
}

