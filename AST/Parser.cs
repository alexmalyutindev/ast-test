using System.Text;
using AST.Nodes;

namespace AST.Nodes { }

namespace AST
{
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
                    $"Expected token `{kind}`, but current token: `{Current}`.\n" +
                    "Tokens : " + String.Join(", ", _tokens),
                    _content,
                    Current!
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
        /// | VariableStatement
        /// | IfStatement
        /// ;
        private INode Statement()
        {
            return Current!.Kind switch
            {
                TokenKind.Semicolon => EmptyStatement(),
                TokenKind.OpenCurlyBrace => BlockStatement(),
                TokenKind.VariableDeclarationToken => VariableStatement(),
                TokenKind.IfToken => IfStatement(),
                _ => ExpressionStatement(),
            };
        }

        /// EmptyStatement
        /// : ';'
        /// ;
        private INode EmptyStatement()
        {
            Eat(TokenKind.Semicolon);
            return new EmptyStatementNode();
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

            // TODO: BlockStatementNode
            return new StatementListNode()
            {
                Children = body,
            };
        }

        /// VariableStatement
        /// : 'var' VariableDeclarationList ';'
        /// ;
        private INode VariableStatement()
        {
            Eat(TokenKind.VariableDeclarationToken);
            var declarations = VariableDeclarationList();
            Eat(TokenKind.Semicolon);

            return new VariableStatementNode()
            {
                Declarations = declarations,
            };
        }

        /// IfStatement
        /// : 'if' '(' Expression ')' Statement
        /// : 'if' '(' Expression ')' Statement 'else' Statement
        /// ;
        private INode IfStatement()
        {
            Eat(TokenKind.IfToken);
            Eat(TokenKind.OpenParentheses);
            var test = Expression();
            Eat(TokenKind.CloseParentheses);

            var consequent = Statement();

            INode? alternate = null;
            if (Current!.Kind == TokenKind.ElseToken)
            {
                Eat(TokenKind.ElseToken);
                alternate = Statement();
            }

            return new IfStatementNode()
            {
                Test = test,
                Consequent = consequent,
                Alternate = alternate!,
            };
        }

        /// VariableDeclarationList
        /// : VariableDeclaration
        /// | VariableDeclarationList ',' VariableDeclaration
        /// ;
        private INode[] VariableDeclarationList()
        {
            var declarations = new List<INode>()
            {
                VariableDeclaration()
            };

            while (Current!.Kind == TokenKind.CommaToken)
            {
                Eat(TokenKind.CommaToken);
                declarations.Add(VariableDeclaration());
            }

            return declarations.ToArray();
        }

        /// VariableDeclaration
        /// : Identifier OptVariableInitializer
        /// ;
        private INode VariableDeclaration()
        {
            var identifier = Identifier();

            var initializer = Current!.Kind is not (TokenKind.Semicolon or TokenKind.CommaToken)
                ? VariableInitializer()
                : null;

            return new VariableDeclarationNode()
            {
                Identifier = identifier,
                Initializer = initializer,
            };
        }

        /// VariableInitializer
        /// : '=' AssignmentExpression
        private INode VariableInitializer()
        {
            Eat(TokenKind.SimpleAssign);
            return AssignmentExpression();
        }

        /// ExpressionStatement
        /// : Expression ';'
        /// ;
        private INode ExpressionStatement()
        {
            var expression = Expression();
            Eat(TokenKind.Semicolon);

            return new ExpressionStatementNode()
            {
                Expression = expression,
            };
        }

        /// Expression
        /// : AssignmentExpression
        /// ;
        private INode Expression()
        {
            return AssignmentExpression();
        }

        /// AssignmentExpression
        /// : LogicalOrExpression
        /// | LeftHandSideExpression AssignmentOperator AssignmentExpression
        /// ;
        private INode AssignmentExpression()
        {
            var left = LogicalOrExpression();

            if (Current!.Kind is TokenKind.SimpleAssign or TokenKind.ComplexAssign)
            {
                var token = Eat(Current!.Kind);

                return new AssignmentExpressionNode()
                {
                    Token = token, // TODO: AssignmentOperator
                    Left = left as IdentifierNode ??
                        throw new SyntaxError($"Invalid left-hand side expression: {left}!"),
                    Right = AssignmentExpression(),
                };
            }

            return left;
        }

        /// LeftHandSideExpression
        /// : Identifier
        /// ;
        private INode LeftHandSideExpression()
        {
            return Identifier();
        }

        /// Identifier
        /// : IDENTIFIER
        /// ;
        private INode Identifier()
        {
            return new IdentifierNode()
            {
                Token = Eat(TokenKind.Identifier),
            };
        }

        /// LogicalOrExpression
        /// : LogicalAndExpression LOGICAL_OR LogicalOrExpression
        /// | LogicalAndExpression
        /// ;
        private INode LogicalOrExpression()
        {
            return LogicalExpression(TokenKind.LogicalOr, LogicalAndExpression);
        }
        
        /// LogicalAndExpression
        /// : EqualityExpression LOGICAL_AND LogicalAndExpression
        /// | EqualityExpression
        /// ;
        private INode LogicalAndExpression()
        {
            return LogicalExpression(TokenKind.LogicalAnd, EqualityExpression);
        }

        /// EQUALITY_OPERATOR: '==', '!='
        /// EqualityExpression
        /// : RelationalExpression EQUALITY_OPERATOR EqualityExpression
        /// | RelationalExpression
        /// ;
        private INode EqualityExpression()
        {
            return BinaryExpression(TokenKind.EqualityOperator, RelationalExpression);
        }

        /// RelationalExpression
        /// : AdditionalExpression
        /// | AdditionalExpression RELATIONAL_OPERATOR RelationalExpression
        /// ;
        private INode RelationalExpression()
        {
            var left = AdditiveExpression();

            while (Current!.Kind is TokenKind.GreaterToken or TokenKind.LessToken or TokenKind.EqualityOperator)
            {
                var op = Eat(Current.Kind);
                var right = AdditiveExpression();

                left = new BinaryExpressionNode()
                {
                    Token = op,
                    Left = left,
                    Right = right
                };
            }

            return left;
        }

        /// AdditiveExpression
        /// : MultiplicativeExpression
        /// | AdditiveExpression ADDITIVE_OPERATOR MultiplicativeExpression
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
        /// : Literal
        /// | ParenthesizedExpression
        /// | LeftHandSideExpression
        /// ;
        private INode PrimaryExpression()
        {
            if (Current!.Kind is TokenKind.NumberLiteral or TokenKind.StringLiteral or TokenKind.BooleanLiteral)
            {
                return Literal();
            }

            return Current!.Kind switch
            {
                TokenKind.OpenParentheses => ParenthesizedExpression(),
                TokenKind.Identifier => LeftHandSideExpression(),
                TokenKind.VariableDeclarationToken => throw new SyntaxError(
                    "TODO: VariableDeclaration",
                    _content,
                    Current
                ),
                _ => throw new SyntaxError(
                    "PrimaryExpression parsing error!\nTokens: " + String.Join(", ", _tokens),
                    _content,
                    Current
                ),
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
        /// | BooleanLiteral
        /// | NullLiteral
        /// ;
        private INode Literal()
        {
            var node = Current!.Kind switch
            {
                TokenKind.NumberLiteral => NumberLiteral(),
                TokenKind.StringLiteral => StringLiteral(),
                TokenKind.BooleanLiteral => BooleanLiteral(),
                TokenKind.NullLiteral => NullLiteral(),
                _ => throw new SyntaxError($"Expected literal but get: {Current}", _content, Current),
            };

            return node;
        }

        /// StringLiteral
        /// : STRING
        /// ;
        private INode StringLiteral()
        {
            return new LiteralNode()
            {
                Token = Eat(TokenKind.StringLiteral),
            };
        }

        /// NumberLiteral
        /// : NUMBER
        /// ;
        private INode NumberLiteral()
        {
            var token = Eat(TokenKind.NumberLiteral);
            return new LiteralNode
            {
                Token = token
            };
        }

        /// BooleanLiteral
        /// : BOOL
        /// ;
        private INode BooleanLiteral()
        {
            var token = Current!.Kind switch
            {
                TokenKind.BooleanLiteral => Eat(TokenKind.BooleanLiteral),
                _ => throw new SyntaxError($"Expected boolean literal but get {Current}", _content, Current),
            };

            return new LiteralNode
            {
                Token = token
            };
        }

        /// NullLiteral
        /// : NULL
        /// ;
        private INode NullLiteral()
        {
            var token = Eat(TokenKind.NullLiteral);
            return new LiteralNode
            {
                Token = token
            };
        }

        // TODO: Group operators
        private INode BinaryExpression(TokenKind token, Func<INode> expression)
        {
            var left = expression();

            while (Current!.Kind == token)
            {
                var op = Eat(Current.Kind);
                var right = expression();

                left = new BinaryExpressionNode()
                {
                    Token = op,
                    Left = left,
                    Right = right
                };
            }

            return left;
        }
        
        private INode LogicalExpression(TokenKind token, Func<INode> expression)
        {
            var left = expression();

            while (Current!.Kind == token)
            {
                var op = Eat(Current.Kind);
                var right = expression();

                left = new LogicalExpressionNode()
                {
                    Token = op,
                    Left = left,
                    Right = right
                };
            }

            return left;
        }
    }

    internal class SyntaxError : Exception
    {
        public SyntaxError(string message) : base(message) { }

        public SyntaxError(string message, string src, Token token) : base(
            message + '\n' + GetErrorLine(src, token)
        ) { }

        private static string GetErrorLine(string src, Token token)
        {
            // TODO: Add multiline support!
            var sb = new StringBuilder()
                .AppendLine(src)
                .Append(new string(' ', token.Location)).Append('~', token.Length)
                .AppendLine();
            return sb.ToString();
        }
    }
}