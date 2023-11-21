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
            Eat(TokenKind.AssignToken);
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
        /// : RelationalExpression
        /// | LeftHandSideExpression AssignmentExpression AssignmentExpression
        /// ;
        private INode AssignmentExpression()
        {
            var left = RelationalExpression();
            if (Current!.Kind != TokenKind.AssignToken)
            {
                return left;
            }

            var token = Eat(TokenKind.AssignToken);

            return new AssignmentExpressionNode()
            {
                Token = token, // TODO: AssignmentOperator
                Left = left as IdentifierNode ??
                    throw new SyntaxError($"Invalid left-hand side expression: {left}!"),
                Right = AssignmentExpression(),
            };
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

        /// RelationalExpression
        /// : AdditionalExpression
        /// | AdditionalExpression RELATIONAL_OPERATOR RelationalExpression
        /// ;
        private INode RelationalExpression()
        {
            var left = AdditiveExpression();
            
            while (Current!.Kind is TokenKind.GreaterToken or TokenKind.LessToken or TokenKind.EqualsToken)
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
        /// : Literal
        /// | ParenthesizedExpression
        /// | LeftHandSideExpression
        /// ;
        private INode PrimaryExpression()
        {
            if (Current!.Kind is TokenKind.NumberLiteral or TokenKind.StringLiteral)
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

        public SyntaxError(string message, string src, Token token) : base(
            message + $"\n{src}\n" + new string(' ', token.Location) + new string('^', token.Length)
        ) { }
    }
}