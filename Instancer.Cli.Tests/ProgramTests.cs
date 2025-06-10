using System;
using System.Reflection;
using Xunit;

namespace Instancer.Cli.Tests;

public class ProgramTests
{
    [Fact]
    public void EntryPoint_WritesHelloWorld()
    {
        var assembly = typeof(Program).Assembly;
        var entry = assembly.EntryPoint!;
        var writer = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(writer);
        entry.Invoke(null, entry.GetParameters().Length == 0 ? null : new object[] { Array.Empty<string>() });
        Console.SetOut(originalOut);
        Assert.Contains("Hello, World!", writer.ToString());
    }
}
