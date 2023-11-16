using System.Collections;
using System.Drawing;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace AST;

public class Program
{
    public static void Main()
    {
        var prog = "123 / 3 + (456 - 69) * 2";
        var parser = new Parser(prog);

        Console.WriteLine();
        var root = parser.Parse();


        Console.WriteLine();
        Console.WriteLine(prog);

        var q = new Queue<INode>();
        q.Enqueue(root);

        var options = new JsonSerializerOptions(JsonSerializerDefaults.General)
        {
            WriteIndented = true,
        };
        var json = JsonSerializer.Serialize(root, options);
        Console.WriteLine(json);

        // int stop = 10;
        // while (q.Count > 0 && stop > 0)
        // {
        //     stop--;

        //     var node = q.Dequeue();
        //     Console.WriteLine(node.Token.ToString(prog));
        //     Console.WriteLine("  " + node.Children.Length);

        //     foreach (var child in root.Children)
        //     {
        //         Console.WriteLine("+-> " + child.Token.ToString(prog));
        //         q.Enqueue(child);
        //     }
        // }
    }
}

public interface INode
{
    public Token Token { get; }

    public int ChildCount { get; }
    public INode[] Children { get; }
}

public class Node : INode
{
    public Token Token { get; set; } = default!;

    public int ChildCount => 0;
    public INode[] Children => Array.Empty<Node>();
}

public class BinaryNode : INode
{
    public Token Token { get; set; } = default!;

    public INode? Left;
    public INode? Right;

    public int ChildCount => 2;
    public INode[] Children => new INode[] { Left!, Right! };
}

public class Parser
{
    public List<Token> Tokens;
    private string _content;
    private int _cursor;

    public Parser(string content)
    {
        _content = content;
        var lexer = new Lexer(_content);

        Tokens = new List<Token>();
        {
            var token = lexer.Next();
            while (token.Kind != TokenKind.End)
            {
                if (token.Kind != TokenKind.WhiteSpace)
                {
                    Tokens.Add(token);
                }
                token = lexer.Next();
            }
        }

        foreach (var token in Tokens)
        {
            Console.WriteLine(token.ToString(_content));
        }
    }

    public INode Parse()
    {
        var token = Tokens[_cursor];

        switch (token.Kind)
        {
            case TokenKind.PlusToken:
            case TokenKind.MinusToken:
            case TokenKind.MultiplyToken:
            case TokenKind.DivideToken:
                _cursor++;
                return new BinaryNode
                {
                    Token = token,
                    Left = Parse(),
                    Right = Parse(),
                };

            case TokenKind.NumberLiteral:
                return Literal();

            case TokenKind.OpenPrantecies:
            case TokenKind.ClosePrantecies:

            case TokenKind.End:
                return new Node()
                {
                    Token = token
                };

            default:
                return new Node()
                {
                    Token = token,
                };
        }
    }

    private INode Literal()
    {
        Token token = Tokens[_cursor];
        return token.Kind switch
        {
            TokenKind.NumberLiteral => NumberLiteral(),
            _ => new Node { Token = token },
        };
    }

    private INode NumberLiteral()
    {
        return new Node()
        {
            Token = Tokens[_cursor]
        };
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
    OpenPrantecies = '(',
    ClosePrantecies = ')',
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

    public string ToString(string content)
    {
        return $"{Kind}: `{content[Range]}`";
    }
}

public class Lexer
{
    private readonly string _content;
    private int _cursor;

    private (Regex, TokenKind)[] Specs = new[]
    {
        (new Regex(@"\G\s+", RegexOptions.Compiled), TokenKind.WhiteSpace),
        (new Regex(@"\G\+", RegexOptions.Compiled), TokenKind.PlusToken),
        (new Regex(@"\G\-", RegexOptions.Compiled), TokenKind.MinusToken),
        (new Regex(@"\G\*", RegexOptions.Compiled), TokenKind.MultiplyToken),
        (new Regex(@"\G\/", RegexOptions.Compiled), TokenKind.DivideToken),
        (new Regex(@"\G\(", RegexOptions.Compiled), TokenKind.OpenPrantecies),
        (new Regex(@"\G\)", RegexOptions.Compiled), TokenKind.ClosePrantecies),
        (new Regex(@"\G\d+", RegexOptions.Compiled), TokenKind.NumberLiteral),
        (new Regex("\\G\"[^\"]*$", RegexOptions.Compiled), TokenKind.StringLiteral),
    };

    public Lexer(string content)
    {
        _content = content;
        _cursor = 0;
    }

    public Token Next()
    {
        if (_cursor >= _content.Length)
        {
            return new Token(TokenKind.End, new Range(_cursor, _cursor + 1), _content);
        }

        foreach (var spec in Specs)
        {
            Match match = spec.Item1.Match(_content, _cursor);
            if (match.Success)
            {
                var start = _cursor;
                _cursor += match.Length;

                return new Token(spec.Item2, new Range(start, _cursor), _content);
            }
        }

        return new Token(TokenKind.Unknown, new Range(_cursor++, _cursor), _content);
    }
}
