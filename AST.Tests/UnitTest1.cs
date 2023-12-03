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
                    Expression = new LiteralNode<int>(42),
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
        var expected = new ProgramNode()
        {
            ProgramName = "Program",
            Body = new INode[]
            {
                new ExpressionStatementNode()
                {
                    Expression = new LiteralNode<string>("abc 69"),
                },
            }
        };

        Compare(ast, expected);
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
                    Expression = new LiteralNode<int>(42)
                },
                new ExpressionStatementNode()
                {
                    Expression = new LiteralNode<string>("abc"),
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
                            Expression = new LiteralNode<int>(42)
                        },
                        new ExpressionStatementNode()
                        {
                            Expression = new LiteralNode<string>("abc"),
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
                    Expression = new BinaryExpressionNode(
                        op: BinaryOperator.Minus,
                        left: new BinaryExpressionNode(
                            op: BinaryOperator.Plus,
                            left: new LiteralNode<int>(2),
                            right: new LiteralNode<int>(3)
                        ),
                        right: new LiteralNode<int>(1)
                    )
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
                    Expression = new BinaryExpressionNode(
                        op: BinaryOperator.Plus,
                        left: new LiteralNode<int>(1),
                        right: new BinaryExpressionNode(
                            op: BinaryOperator.Multiply,
                            left: new LiteralNode<int>(2),
                            right: new LiteralNode<int>(3)
                        )
                    )
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
                    Expression = new BinaryExpressionNode(
                        op: BinaryOperator.Multiply,
                        left: new BinaryExpressionNode(
                            op: BinaryOperator.Plus,
                            left: new LiteralNode<int>(1),
                            right: new LiteralNode<int>(2)
                        ),
                        right: new LiteralNode<int>(3)
                    )
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
                            Expression = new BinaryExpressionNode(
                                BinaryOperator.Plus,
                                left: new LiteralNode<int>(2),
                                right: new LiteralNode<int>(123)
                            )
                        },
                        new StatementListNode()
                        {
                            Children = new INode[]
                            {
                                new ExpressionStatementNode { Expression = new LiteralNode<string>("456") },
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