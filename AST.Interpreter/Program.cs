// See https://aka.ms/new-console-template for more information

using System.Text;
using AST.Interpreter;

var src = """
          2 + 3 - 1 + 10;
          "abc" + "def";
          """;
var interpreter = new Interpreter(src);

interpreter.Run();

var sb = new StringBuilder()
    .AppendLine(">Source:")
    .AppendLine(src)
    .AppendLine(">Stack:")
    .AppendLine(String.Join(", ", interpreter.Stack))
    .AppendLine(">StringStack:")
    .AppendLine(String.Join(", ", interpreter.StringStack));

Console.WriteLine(sb);

namespace AST.Interpreter
{ }