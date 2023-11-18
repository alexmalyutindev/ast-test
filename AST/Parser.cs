using AST.Nodes;

namespace AST;

public class Parser
{
    private readonly List<Token> _tokens;
    private readonly string _content;
    private int _cursor;

    public Parser(string content)
    {
        _content = content;
        var lexer = new Lexer(_content);

        _tokens = new List<Token>();
        {
            var token = lexer.Next();
            while (token.Kind != TokenKind.End)
            {
                if (token.Kind != TokenKind.WhiteSpace)
                {
                    _tokens.Add(token);
                }
                token = lexer.Next();
            }
        }
    }

    public INode Parse()
    {
        var token = _tokens[_cursor];

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

            case TokenKind.OpenParentheses:
            case TokenKind.CloseParentheses:

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

    private Token Eat()
    {
        return _tokens[_cursor++];
    }

    private INode Literal()
    {
        Token token = _tokens[_cursor];
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
            Token = _tokens[_cursor]
        };
    }
}