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
            throw new SyntaxError($"Syntax error: Current token is `NULL` at {_cursor}!");
        }

        if (Current?.Kind != kind)
        {
            throw new SyntaxError(
                $"Syntax error: Expected token `{kind}`, but current token: `{PeekNext}`\n" +
                "Tokens:\n " + String.Join("\n ", _tokens)
            );
        }

        return _tokens[_cursor++];
    }

    public INode Parse()
    {
        return Program();
    }

    /// Program
    /// : StatementList
    /// ;
    private INode Program()
    {
        return new ProgramNode()
        {
            ProgramName = "Program",
            Body = StatementList(),
        };
    }

    /// StatementList
    /// : Statement
    /// | StatementList Statement
    /// ;
    private INode[] StatementList()
    {
        var statementList = new List<INode>() { Statement() };

        while (Current!.Kind is not (TokenKind.End or TokenKind.CloseCurlyBrace))
        {
            statementList.Add(Statement());
        }

        return statementList.ToArray();
    }

    /// Statement
    /// : ExpressionStatement
    /// | BlockStatement
    /// ;
    private INode Statement()
    {
        return Current!.Kind switch
        {
            TokenKind.OpenCurlyBrace => BlockStatement(),
            _ => ExpressionStatement(),
        };
    }

    /// BlockStatement
    /// : '{' OptStatementList '}'
    /// ;
    private INode BlockStatement()
    {
        Eat(TokenKind.OpenCurlyBrace);

        INode[] body = Current!.Kind switch
        {
            TokenKind.CloseCurlyBrace => Array.Empty<INode>(),
            _ => StatementList(),
        };

        Eat(TokenKind.CloseCurlyBrace);

        return new StatementListNode()
        {
            Children = body,
        };
    }

    /// ExpressionStatement
    /// : Expression ';'
    /// ;
    private INode ExpressionStatement()
    {
        var expression = new ExpressionStatementNode()
        {
            Expression = Expression()
        };

        Eat(TokenKind.Semicolon);
        return expression;
    }

    /// Expression
    /// : AdditiveExpression
    /// ;
    private INode Expression()
    {
        return AdditiveExpression();
    }

    /// AdditiveExpression
    /// : MultiplicativeExpression
    /// | AdditiveExpression ADDITIVE_OPERATOR MultiplicativeExpression -> MultiplicativeExpression ADDITIVE_OPERATOR MultiplicativeExpression
    /// ;
    private INode AdditiveExpression()
    {
        var left = MultiplicativeExpression();

        while (Current!.Kind is TokenKind.PlusToken or TokenKind.MinusToken)
        {
            var op = Eat(Current.Kind);
            var right = MultiplicativeExpression();

            left = new BinaryExpressionNode()
            {
                Token = op,
                Left = left,
                Right = right
            };
        }

        return left;
    }


    /// MultiplicativeExpression
    /// : PrimaryExpression
    /// | MultiplicativeExpression MULTIPLICATIVE_OPERATOR PrimaryExpression -> PrimaryExpression MULTIPLICATIVE_OPERATOR PrimaryExpression
    /// ;
    private INode MultiplicativeExpression()
    {
        var left = PrimaryExpression();

        while (Current!.Kind is TokenKind.MultiplyToken or TokenKind.DivideToken)
        {
            var op = Eat(Current.Kind);
            var right = PrimaryExpression();

            left = new BinaryExpressionNode()
            {
                Token = op,
                Left = left,
                Right = right
            };
        }

        return left;
    }

    /// PrimaryExpression
    /// : ParenthesizedExpression
    /// | Literal
    /// ;
    private INode PrimaryExpression()
    {
        return Current!.Kind switch
        {
            TokenKind.OpenParentheses => ParenthesizedExpression(),
            _ => Literal()
        };
    }

    /// ParenthesizedExpression
    /// : '(' Expression ')'
    /// ;
    private INode ParenthesizedExpression()
    {
        Eat(TokenKind.OpenParentheses);
        var expression = Expression();
        Eat(TokenKind.CloseParentheses);

        return expression;
    }

    /// Literal
    /// : NumericLiteral
    /// | StringLiteral
    /// ;
    private INode Literal()
    {
        var token = Current!;

        var node = token.Kind switch
        {
            TokenKind.NumberLiteral => NumberLiteral(),
            TokenKind.StringLiteral => StringLiteral(),
            _ => new LiteralNode { Token = token },
        };

        return node;
    }

    private INode StringLiteral()
    {
        return new LiteralNode()
        {
            Token = Eat(TokenKind.StringLiteral),
        };
    }

    private INode NumberLiteral()
    {
        var token = Eat(TokenKind.NumberLiteral);
        return new LiteralNode
        {
            Token = token
        };
    }

    private BinaryExpressionNode BinaryExpression()
    {
        var token = Eat(TokenKind.PlusToken);
        return new BinaryExpressionNode()
        {
            Token = token,
            Right = NumberLiteral()
        };
    }
}

internal class SyntaxError : Exception
{
    public SyntaxError(string message) : base(message) { }
}