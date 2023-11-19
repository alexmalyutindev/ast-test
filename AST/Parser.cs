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
            _tokens.Add(token); // Add end token
        }
    }
    
    private Token? Current => _cursor >= _tokens.Count ? null : _tokens[_cursor];
    private Token? PeekNext => _cursor + 1 >= _tokens.Count ? null : _tokens[_cursor + 1];

    private Token Eat(TokenKind kind)
    {
        if (Current == null)
        {
            throw new Exception($"Syntax error: Expected token is `NULL`!");
        }
        
        if (Current?.Kind != kind)
        {
            throw new Exception($"Syntax error: Expected token `{kind}`, but current token: `{PeekNext}`");
        }

        return _tokens[_cursor++];
    }

    public INode Parse()
    {
        return Program();
    }

    private INode Program()
    {
        return Expression();
    }

    private INode Expression()
    {
        return Literal();
    }

    /// Literal
    /// : NumericLiteral
    /// ;
    private INode Literal()
    {
        var token = Current!;

        var node = token.Kind switch
        {
            TokenKind.NumberLiteral => NumberLiteral(),
            TokenKind.StringLiteral => StringLiteral(),
            _ => new Node { Token = token },
        };

        return node;
        
        if (PeekNext?.Kind is TokenKind.PlusToken or TokenKind.MinusToken)
        {
            var b= BinaryExpression();
            b.Left = node;
            node = b;
        }

        return node;
    }

    private INode StringLiteral()
    {
        return new Node()
        {
            Token = Eat(TokenKind.StringLiteral),
        };
    }

    private INode NumberLiteral()
    {
        var token = Eat(TokenKind.NumberLiteral);
        return new Node
        {
            Token = token
        };
    }

    private BinaryNode BinaryExpression()
    {
        var token = Eat(TokenKind.PlusToken);
        return new BinaryNode()
        {
            Token = token,
            Right = NumberLiteral()
        };
    }
}