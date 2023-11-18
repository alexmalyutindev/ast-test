using System.Text.Json;

namespace AST.Tests;

public class Tests
{
    private JsonSerializerOptions _serializerOptions;

    [SetUp]
    public void Setup()
    {
        _serializerOptions = new JsonSerializerOptions()
        {
            WriteIndented = true,
        };
    }

    [Test]
    [TestCase("2 + 2")]
    [TestCase("2 + 2 * 2")]
    [TestCase("(2 + 2) * 2")]
    public void Test1(string src)
    {
        var parser = new Parser(src);
        var root = parser.Parse();

        var json = JsonSerializer.Serialize(root, _serializerOptions);
        Assert.Pass($"Source:\n{src}\nAST:\n{json}");
    }
}