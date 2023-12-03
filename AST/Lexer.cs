using System.Text.RegularExpressions;

namespace AST;

public partial class Lexer
{
    public string Source => _content;

    private readonly string _content;
    private int _cursor;

    private readonly (Regex, TokenKind)[] _specs =
    {
        (WhiteSpace(), TokenKind.WhiteSpace),

        // Separators
        (new Regex(@"\G\;", RegexOptions.Compiled), TokenKind.Semicolon),
        (new Regex(@"\G\,", RegexOptions.Compiled), TokenKind.CommaToken),

        // Parentheses
        (new Regex(@"\G\{", RegexOptions.Compiled), TokenKind.OpenCurlyBrace),
        (new Regex(@"\G\}", RegexOptions.Compiled), TokenKind.CloseCurlyBrace),
        (new Regex(@"\G\(", RegexOptions.Compiled), TokenKind.OpenParentheses),
        (new Regex(@"\G\)", RegexOptions.Compiled), TokenKind.CloseParentheses),

        // Compare
        (new Regex(@"\G[=!]=", RegexOptions.Compiled), TokenKind.EqualityOperator),
        (new Regex(@"\G\>", RegexOptions.Compiled), TokenKind.GreaterToken),
        (new Regex(@"\G\<", RegexOptions.Compiled), TokenKind.LessToken),
        // TODO: Add '<=', '>='
        
        // Logical
        (new Regex(@"\G\&\&", RegexOptions.Compiled), TokenKind.LogicalAnd),
        (new Regex(@"\G\|\|", RegexOptions.Compiled), TokenKind.LogicalOr),
        (new Regex(@"\G\!", RegexOptions.Compiled), TokenKind.LogicalNot),

        
        (new Regex(@"\G\=", RegexOptions.Compiled), TokenKind.SimpleAssign),
        (new Regex(@"\G[\+\-\*\/]=", RegexOptions.Compiled), TokenKind.ComplexAssign), // '+=', '-=', '*=', '/='

        // Keywords
        (new Regex(@"\G\bvar\b", RegexOptions.Compiled), TokenKind.VariableDeclarationToken),
        (new Regex(@"\G\bif\b", RegexOptions.Compiled), TokenKind.IfToken),
        (new Regex(@"\G\belse\b", RegexOptions.Compiled), TokenKind.ElseToken),

        (new Regex(@"\G\btrue\b", RegexOptions.Compiled), TokenKind.BooleanLiteral),
        (new Regex(@"\G\bfalse\b", RegexOptions.Compiled), TokenKind.BooleanLiteral),
        (new Regex(@"\G\bnull\b", RegexOptions.Compiled), TokenKind.NullLiteral),

        // Numbers
        (NumberLiteral(), TokenKind.NumberLiteral),

        (Identifier(), TokenKind.Identifier),

        // Math
        (new Regex(@"\G[+-]", RegexOptions.Compiled), TokenKind.AdditiveOperator),
        // (new Regex(@"\G\-", RegexOptions.Compiled), TokenKind.MinusToken),
        (new Regex(@"\G[*/]", RegexOptions.Compiled), TokenKind.MultiplicativeOperator),
        // (new Regex(@"\G\/", RegexOptions.Compiled), TokenKind.DivideToken),

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

        throw new SyntaxError(
            "Unknown token kind: ",
            _content,
            new Token(TokenKind.Unknown, new Range(_cursor, ++_cursor), _content)
        );
    }

    [GeneratedRegex(@"\G\s+", RegexOptions.Compiled)]
    private static partial Regex WhiteSpace();

    [GeneratedRegex(@"\G-?\d+", RegexOptions.Compiled)]
    private static partial Regex NumberLiteral();

    [GeneratedRegex("\"(.*?)\"", RegexOptions.Compiled)]
    private static partial Regex StringLiteral();

    [GeneratedRegex(@"\G[a-zA-Z_@][\w]*", RegexOptions.Compiled)]
    private static partial Regex Identifier();
}