using System.Collections.ObjectModel;
using System.Text;
using AST.Nodes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using JsonConverter = Newtonsoft.Json.JsonConverter;

namespace AST.Tests;

public class Tests
{
    private JsonSerializerSettings _settings = default!;
    private readonly SyntaxTests _syntaxTests = new SyntaxTests();

    [SetUp]
    public void Setup()
    {
        _settings = new JsonSerializerSettings()
        {
            Converters = new Collection<JsonConverter>()
            {
                new StringEnumConverter(),
                new TokenConverter(),
            },
            Formatting = Formatting.Indented,
            ContractResolver = new AstContractResolver(),
        };
    }

    [Test]
    public void Test01_NumberLiteral()
    {
        var src = "42;";
        var ast = new Parser(src).Parse();
        var ast2 = new ProgramNode()
        {
            ProgramName = "Program",
            Body = new INode[]
            {
                new ExpressionStatementNode()
                {
                    Expression = new LiteralNode()
                    {
                        Token = new Token(TokenKind.NumberLiteral, new Range(0, 0 + 2), src)
                    }
                },
            }
        };

        Compare(ast, ast2);
    }

    [Test]
    public void Test02_StringLiteral()
    {
        var src = """ "abc 69" ;""";
        var ast = new Parser(src).Parse();
        var ast2 = new ProgramNode()
        {
            ProgramName = "Program",
            Body = new INode[]
            {
                new ExpressionStatementNode()
                {
                    Expression = new LiteralNode()
                    {
                        Token = new Token(TokenKind.StringLiteral, new Range(1, 1 + 8), src)
                    }
                },
            }
        };

        Compare(ast, ast2);
    }

    [Test]
    public void Test03_MultipleStatements()
    {
        var src =
            """
            42;
            "abc";
            """;

        var ast = new Parser(src).Parse();
        var ast2 = new ProgramNode()
        {
            ProgramName = "Program",
            Body = new INode[]
            {
                new ExpressionStatementNode()
                {
                    Expression = new LiteralNode()
                    {
                        Token = new Token(TokenKind.NumberLiteral, new Range(0, 0 + 2), src)
                    }
                },
                new ExpressionStatementNode()
                {
                    Expression = new LiteralNode()
                    {
                        Token = new Token(TokenKind.StringLiteral, new Range(4, 4 + 5), src)
                    }
                },
            }
        };

        Compare(ast, ast2);
    }

    [Test]
    public void Test04_Block()
    {
        var src = """{ 42; "abc"; }""";
        var ast = new Parser(src).Parse();
        var ast2 = new ProgramNode()
        {
            ProgramName = "Program",
            Body = new INode[]
            {
                new StatementListNode()
                {
                    Children = new INode[]
                    {
                        new ExpressionStatementNode()
                        {
                            Expression = new LiteralNode()
                            {
                                Token = new Token(TokenKind.NumberLiteral, new Range(2, 4), src)
                            }
                        },
                        new ExpressionStatementNode()
                        {
                            Expression = new LiteralNode()
                            {
                                Token = new Token(TokenKind.StringLiteral, new Range(6, 11), src)
                            }
                        },
                    },
                }
            }
        };

        Compare(ast, ast2);
    }

    [Test]
    public void Test05_Sum()
    {
        var src = "2 + 3 - 1;";
        var ast = new Parser(src).Parse();
        var ast2 = new ProgramNode()
        {
            ProgramName = "Program",
            Body = new INode[]
            {
                new ExpressionStatementNode()
                {
                    Expression = new BinaryExpressionNode()
                    {
                        Token = new Token(TokenKind.AdditiveOperator, new Range(6, 7), src),
                        Operator = BinaryOperator.Minus,
                        Left = new BinaryExpressionNode()
                        {
                            Token = new Token(TokenKind.AdditiveOperator, new Range(2, 3), src),
                            Operator = BinaryOperator.Plus,
                            Left = new LiteralNode()
                            {
                                Token = new Token(TokenKind.NumberLiteral, new Range(0, 1), src)
                            },
                            Right = new LiteralNode()
                            {
                                Token = new Token(TokenKind.NumberLiteral, new Range(4, 5), src)
                            },
                        },
                        Right = new LiteralNode()
                        {
                            Token = new Token(TokenKind.NumberLiteral, new Range(8, 9), src)
                        }
                    }
                }
            }
        };

        Compare(ast, ast2);
    }

    [Test]
    public void Test06_SumAndMul()
    {
        var src = "1+2*3;";
        var ast = new Parser(src).Parse();
        var ast2 = new ProgramNode()
        {
            ProgramName = "Program",
            Body = new INode[]
            {
                new ExpressionStatementNode()
                {
                    Expression = new BinaryExpressionNode()
                    {
                        Operator = BinaryOperator.Plus,
                        Token = new Token(TokenKind.AdditiveOperator, new Range(1, 2), src),
                        Left = new LiteralNode()
                        {
                            Token = new Token(TokenKind.NumberLiteral, new Range(0, 1), src)
                        },
                        Right = new BinaryExpressionNode()
                        {
                            Operator = BinaryOperator.Multiply,
                            Token = new Token(TokenKind.MultiplicativeOperator, new Range(3, 4), src),
                            Left = new LiteralNode()
                            {
                                Token = new Token(TokenKind.NumberLiteral, new Range(2, 3), src)
                            },
                            Right = new LiteralNode()
                            {
                                Token = new Token(TokenKind.NumberLiteral, new Range(4, 5), src)
                            },
                        }
                    }
                }
            }
        };

        Compare(ast, ast2);
    }

    [Test]
    public void Test07_Parentheses()
    {
        var src = "(1+2)*3;";
        var ast = new Parser(src).Parse();
        var ast2 = new ProgramNode()
        {
            ProgramName = "Program",
            Body = new INode[]
            {
                new ExpressionStatementNode()
                {
                    Expression = new BinaryExpressionNode()
                    {
                        Operator = BinaryOperator.Multiply,
                        Token = new Token(TokenKind.MultiplicativeOperator, new Range(5, 6), src),
                        Left = new BinaryExpressionNode()
                        {
                            Operator = BinaryOperator.Plus,
                            Token = new Token(TokenKind.AdditiveOperator, new Range(2, 3), src),
                            Left = new LiteralNode()
                            {
                                Token = new Token(TokenKind.NumberLiteral, new Range(1, 2), src)
                            },
                            Right = new LiteralNode()
                            {
                                Token = new Token(TokenKind.NumberLiteral, new Range(3, 4), src)
                            },
                        },
                        Right = new LiteralNode()
                        {
                            Token = new Token(TokenKind.NumberLiteral, new Range(6, 7), src)
                        },
                    }
                }
            }
        };

        Compare(ast, ast2);
    }

    [Test]
    public void Test08_Block()
    {
        var src = """{ 2 + 123; { "456"; } } { }""";
        var ast = new Parser(src).Parse();
        var expected = new ProgramNode()
        {
            ProgramName = "Program",
            Body = new INode[]
            {
                new StatementListNode()
                {
                    Children = new INode[]
                    {
                        new ExpressionStatementNode()
                        {
                            Expression = new BinaryExpressionNode()
                            {
                                Operator = BinaryOperator.Plus,
                                Token = new Token(TokenKind.AdditiveOperator, new Range(4, 5), src),
                                Left = new LiteralNode
                                    { Token = new Token(TokenKind.NumberLiteral, new Range(2, 3), src) },
                                Right = new LiteralNode
                                    { Token = new Token(TokenKind.NumberLiteral, new Range(6, 9), src) }
                            }
                        },
                        new StatementListNode()
                        {
                            Children = new INode[]
                            {
                                new ExpressionStatementNode()
                                {
                                    Expression = new LiteralNode()
                                        { Token = new Token(TokenKind.StringLiteral, new Range(13, 18), src) }
                                },
                            },
                        },
                    },
                },
                new StatementListNode { Children = Array.Empty<INode>() }
            }
        };

        Compare(ast, expected);
    }

    private void Compare(INode current, INode expected)
    {
        var json = ToJson(current);
        var expectedJson = ToJson(expected);

        var sb = new StringBuilder();
        var splitA = json.Split('\n');
        var splitB = expectedJson.Split('\n');
        var lines = Math.Max(splitA.Length, splitB.Length);

        const int alignment = -60;

        sb.AppendLine($"{"AST:",alignment} | EXPECTED:");
        for (int i = 0; i < lines; i++)
        {
            var a = i >= splitA.Length ? " " : splitA[i];
            var b = i >= splitB.Length ? " " : splitB[i];
            sb.AppendLine($"{a,alignment} | {b}");
        }

        Assert.That(
            json,
            Is.EqualTo(expectedJson),
            () => sb.ToString()
        );

        Console.WriteLine("AST:\n" + json);
    }

    private string ToJson(INode ast)
    {
        return JsonConvert.SerializeObject(ast, _settings);
    }
}