using System.Text;

namespace AST.Tests;

public class InterpreterTests
{
    [Test]
    public void Test0()
    {
        var src = """
                  45 + 25 - 1;
                  2 + 2 * 2;
                  (2 + 2) * 2;
                  "abc" + "def";
                  """;

        var interpreter = new Interpreter.Interpreter(src);
        
        interpreter.Run();
        
        var sb = new StringBuilder()
            .AppendLine(">Source:")
            .AppendLine(src)
            .AppendLine(">Stack:").Append('[')
            .Append(String.Join(" | ", interpreter.Stack)).AppendLine("]")
            .AppendLine(">StringStack:").Append('[')
            .Append(String.Join(", ", interpreter.StringStack)).AppendLine("]");

        Console.WriteLine(sb);
    }
}