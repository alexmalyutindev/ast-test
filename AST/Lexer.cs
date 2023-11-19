using System.Text.RegularExpressions;

namespace AST;

public partial class Lexer
{
    private readonly string _content;
    private int _cursor;

    private readonly (Regex, TokenKind)[] _specs =
    {
        (WhiteSpace(), TokenKind.WhiteSpace),
        (new Regex(@"\G\;", RegexOptions.Compiled), TokenKind.Semicolon),
        
        (new Regex(@"\G\{", RegexOptions.Compiled), TokenKind.OpenCurlyBrace),
        (new Regex(@"\G\}", RegexOptions.Compiled), TokenKind.CloseCurlyBrace),
        
        (new Regex(@"\G\+", RegexOptions.Compiled), TokenKind.PlusToken),
        (new Regex(@"\G\-", RegexOptions.Compiled), TokenKind.MinusToken),
        (new Regex(@"\G\*", RegexOptions.Compiled), TokenKind.MultiplyToken),
        (new Regex(@"\G\/", RegexOptions.Compiled), TokenKind.DivideToken),
        (new Regex(@"\G\(", RegexOptions.Compiled), TokenKind.OpenParentheses),
        (new Regex(@"\G\)", RegexOptions.Compiled), TokenKind.CloseParentheses),
        (NumberLiteral(), TokenKind.NumberLiteral),
        (StringLiteral(), TokenKind.StringLiteral),
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

        foreach (var spec in _specs)
        {
            Match match = spec.Item1.Match(_content, _cursor);
            if (!match.Success)
            {
                continue;
            }

            var start = _cursor;
            _cursor += match.Length;

            return new Token(spec.Item2, new Range(start, _cursor), _content);
        }

        return new Token(TokenKind.Unknown, new Range(_cursor, ++_cursor), _content);
    }

    [GeneratedRegex(@"\G\s+", RegexOptions.Compiled)]
    private static partial Regex WhiteSpace();

    [GeneratedRegex(@"\G\d+", RegexOptions.Compiled)]
    private static partial Regex NumberLiteral();

    [GeneratedRegex("\"(.*?)\"", RegexOptions.Compiled)]
    private static partial Regex StringLiteral();
}