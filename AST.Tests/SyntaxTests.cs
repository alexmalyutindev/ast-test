using System.Collections.ObjectModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AST.Tests;

public class SyntaxTests
{
    private JsonSerializerSettings _settings = default!;

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

    private void GenericSyntaxTest(string src)
    {
        var parser = new Parser(src);
        var root = parser.Parse();

        var json = JsonConvert.SerializeObject(root, _settings);
        Assert.Pass($"Source:\n{src}\nAST:\n{json}");
    }

    [TestCase("2 + 2;")]
    [TestCase("2 + 2 * 2;")]
    [TestCase("(2 + 2) * 2;")]
    [TestCase("(2);")]
    [TestCase("{ }")]
    public void MathTests(string src) => GenericSyntaxTest(src);

    [TestCase("var a;")]
    [TestCase("var a = 2;")]
    [TestCase("var a, b = 2;")]
    [TestCase("var a = 2, b = 2;")]
    [TestCase("var a = b = 2;")]
    [TestCase("a = 2;")]
    [TestCase("a = b = 2;")]
    public void VariableTests(string src) => GenericSyntaxTest(src);

    [TestCase("if (a) b = 69;")]
    [TestCase("if (a) if (b) c = 69;")]
    [TestCase("if (a) { if (b) { c = 69; } } else { }")]
    public void IfStatementTests(string src) => GenericSyntaxTest(src);

    [TestCase("a > 0;")]
    [TestCase("a + 5 > 0;")]
    [TestCase("a = b + 5 > 0;")]
    public void RelationalTests(string src) => GenericSyntaxTest(src);
}