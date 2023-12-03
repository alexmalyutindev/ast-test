using System.Collections.ObjectModel;
using System.Text;
using AST.Nodes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AST.Tests;

public class InterpreterTests
{
    private JsonSerializerSettings _settings;

    [SetUp]
    public void Setup()
    {
        _settings = new JsonSerializerSettings()
        {
            Converters = new Collection<JsonConverter>()
            {
                new StringEnumConverter(),
                new TokenConverter(),
            },
            Formatting = Formatting.Indented,
            ContractResolver = new AstContractResolver(),
        };
    }
    
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
        
        Console.WriteLine(ToJson(interpreter.Program));

        Assert.That(
            interpreter.Stack.Reverse().ToArray(),
            Is.EqualTo(
                new IComparable[]
                {
                    45 + 25 - 1,
                    2 + 2 * 2,
                    (2 + 2) * 2,
                }
            )
        );

        Assert.That(
            interpreter.StringStack.Reverse().ToArray(),
            Is.EqualTo(
                new IComparable[]
                {
                    "abc" + "def",
                }
            )
        );
    }
    
    private string ToJson(INode ast)
    {
        return JsonConvert.SerializeObject(ast, _settings);
    }
}