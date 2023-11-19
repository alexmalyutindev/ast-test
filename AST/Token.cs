namespace AST;

public class Token
{
    public string Value => _content[_range];
    public TokenKind Kind { get; }

    private readonly Range _range;
    private readonly string _content;

    public Token(TokenKind kind)
    {
        Kind = kind;
        _range = Range.All;
        _content = String.Empty;
    }

    public Token(TokenKind kind, Range range, string content)
    {
        Kind = kind;
        _range = range;
        _content = content;
    }

    public override string ToString()
    {
        if (_content.Length < _range.End.Value)
        {
            return $"{Kind}: [{_range.Start.Value}, {_range.End.Value}]";
        }

        if (!string.IsNullOrEmpty(_content))
        {
            return $"{Kind}: `{Value}`";
        }

        return $"{Kind}";
    }
}