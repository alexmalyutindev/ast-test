namespace AST;

public class Token
{
    public string Value => _content[_range];
    public TokenKind Kind;
    
    private readonly Range _range;
    private readonly string _content;

    public Token(TokenKind kind, Range range, string content)
    {
        Kind = kind;
        _range = range;
        this._content = content;
    }

    public override string ToString()
    {
        return $"{Kind}: `{_content[_range]}`";
    }
}