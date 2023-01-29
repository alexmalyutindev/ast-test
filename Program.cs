using System.Collections;
using System.Text.RegularExpressions;

public class Program
{
    public static void Main()
    {
        var prog = "123 / 3 + (456 - 69) * 2";
        var parser = new Parser(prog);
    }
}

public class Node
{
    public Node? Parrent;
    public Token? Token;
}

public class BinaryNode : Node
{
    public Node? Left;
    public Node? Right;
}

public class UnaryNode : Node
{
    public Node? Child;
}

public class Parser
{
    private string _content;

    public Parser(string content)
    {
        _content = content;
        var lexer = new Lexer(_content);

        var tokens = new List<Token>();
        {
            var token = lexer.Next();
            while (token.Kind != TokenKind.End)
            {
                tokens.Add(token);
                token = lexer.Next();
            }
        }

        foreach (var token in tokens)
        {
            Console.WriteLine($"{token.Kind}: `{_content[token.Range]}`");
        }
    }

    public Node ParseNode(List<Token> tokens, int id)
    {
        var token = tokens[id];

        switch (token.Kind)
        {
            case TokenKind.PlusToken:
            case TokenKind.MinusToken:
            case TokenKind.MultiplyToken:
            case TokenKind.DivideToken:
                return new BinaryNode
                {
                    Token = token,
                    Left = ParseNode(tokens, id + 1),
                    Right = ParseNode(tokens, id + 1),
                };

            case TokenKind.NumberLiteral:
                return new Node
                {
                    Token = token
                };
        }

        return new Node();
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
    public TokenKind Kind;
    public Range Range;

    public Token(TokenKind kind, Range range)
    {
        Kind = kind;
        Range = range;
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
            return new Token(TokenKind.End, new Range(_cursor, _cursor + 1));
        }

        foreach (var spec in Specs)
        {
            Match match = spec.Item1.Match(_content, _cursor);
            if (match.Success)
            {
                var start = _cursor;
                _cursor += match.Length;
                return new Token(spec.Item2, new Range(start, _cursor));
            }
        }

        return new Token(TokenKind.Unknown, new Range(_cursor++, _cursor));
    }
}
