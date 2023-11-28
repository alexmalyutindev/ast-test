// See https://aka.ms/new-console-template for more information

using System.Text;
using AST.Interpreter;

var src = """
          var a = 5;
          var b = 44 + 25;
          var s = "abc";

          var c = b - a;

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
    .AppendLine(String.Join(", ", interpreter.StringStack))
    .AppendLine(">Variables:")
    .AppendLine(string.Join(", ", interpreter.Variables.Select(pair => $"{pair.Key}: {pair.Value}")))
    ;

Console.WriteLine(sb);

namespace AST.Interpreter
{ }