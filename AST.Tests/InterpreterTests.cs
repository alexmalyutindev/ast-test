using System.Collections.ObjectModel;
using System.Text;
using AST.Interpreter;
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
    public void TestStr()
    {
        var src = """
                  "abc" + "def";
                  """;

        var interpreter = new Interpreter.Interpreter(src);


        interpreter.Run();

        Assert.That(interpreter.Stack.PopString(), Is.EqualTo("abc" + "def"));

        var sb = new StringBuilder()
            .AppendLine(">Source:")
            .AppendLine(src)
            .AppendLine(">Stack:").Append('[')
            .Append(String.Join(" | ", interpreter.Stack)).AppendLine("]");
        Console.WriteLine(sb);

        Console.WriteLine(ToJson(interpreter.Program));
    }


    [Test]
    public void TestInt()
    {
        var src = """
                  45 + 25 - 1;
                  2 + 2 * 2;
                  (2 + 2) * 2;
                  """;

        var interpreter = new Interpreter.Interpreter(src);
        interpreter.Run();

        Assert.That(interpreter.Stack.Pop<int>(), Is.EqualTo((2 + 2) * 2));
        Assert.That(interpreter.Stack.Pop<int>(), Is.EqualTo(2 + 2 * 2));
        Assert.That(interpreter.Stack.Pop<int>(), Is.EqualTo(45 + 25 - 1));

        var sb = new StringBuilder()
            .AppendLine(">Source:")
            .AppendLine(src)
            .AppendLine(">Stack:").Append('[')
            .Append(String.Join(" | ", interpreter.Stack)).AppendLine("]");
        Console.WriteLine(sb);

        Console.WriteLine(ToJson(interpreter.Program));
    }

    private string ToJson(INode ast)
    {
        return JsonConvert.SerializeObject(ast, _settings);
    }
}
