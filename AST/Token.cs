using System.Text.Json.Serialization;

namespace AST;

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