using System.Text.Json;
using System.Text.Json.Serialization;
using AST.Nodes;

namespace AST;

public static class Program
{
    public static void Main()
    {
        var program = "123 / 3 + (456 - 69) * 2";
        var parser = new Parser(program);

        Console.WriteLine();
        var root = parser.Parse();


        Console.WriteLine();
        Console.WriteLine(program);

        var q = new Queue<INode>();
        q.Enqueue(root);

        var options = new JsonSerializerOptions(JsonSerializerDefaults.General)
        {
            WriteIndented = true,
        };
        var json = JsonSerializer.Serialize(root, options);
        Console.WriteLine(json);
    }
}

public enum TokenKind
{
    Unknown,
    End,
    NumberLiteral,
    PlusToken = '+',
    MinusToken = '-',
    MultiplyToken = '*',
    DivideToken = '/',
    OpenParentheses = '(',
    CloseParentheses = ')',
    StringLiteral = 48,
    WhiteSpace
}

public class Token
{
    [JsonInclude]
    public string Value => _content[Range];

    [JsonInclude, JsonConverter(typeof(JsonStringEnumConverter))]
    public TokenKind Kind;
    public Range Range;

    private readonly string _content;

    public Token(TokenKind kind, Range range, string content)
    {
        Kind = kind;
        Range = range;
        this._content = content;
    }

    public override string ToString()
    {
        return $"{Kind}: `{_content[Range]}`";
    }
}