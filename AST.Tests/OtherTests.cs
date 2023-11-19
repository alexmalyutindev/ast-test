using Newtonsoft.Json;

namespace AST.Tests;

public class OtherTests
{
    [Test]
    [TestCase("2 + 2")]
    [TestCase("2 + 2 * 2")]
    [TestCase("(2 + 2) * 2")]
    public void Test1(string src)
    {
        var parser = new Parser(src);
        var root = parser.Parse();

        var json = JsonConvert.SerializeObject(root, Formatting.Indented);
        Assert.Pass($"Source:\n{src}\nAST:\n{json}");
    }
}