using System.Collections.ObjectModel;
using AST.Nodes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using JsonConverter = Newtonsoft.Json.JsonConverter;

namespace AST.Tests;

public class Tests
{
    private JsonSerializerSettings _settings = default!;
    private readonly OtherTests _otherTests = new OtherTests();

    [SetUp]
    public void Setup()
    {
        _settings = new JsonSerializerSettings()
        {
            Converters = new Collection<JsonConverter>()
            {
                new StringEnumConverter()
            },
            Formatting = Formatting.Indented,
            ContractResolver = new AstContractResolver(),
            // TypeNameHandling = TypeNameHandling.All,
            // TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
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
                new ExpressionNode()
                {
                    Expression = new Node()
                    {
                        Token = new Token(TokenKind.NumberLiteral, new Range(0, 0 + 2), src)
                    },
                }
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
                new ExpressionNode()
                {
                    Expression = new Node()
                    {
                        Token = new Token(TokenKind.StringLiteral, new Range(1, 1 + 8), src)
                    },
                }
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
                new ExpressionNode()
                {
                    Expression = new Node()
                    {
                        Token = new Token(TokenKind.NumberLiteral, new Range(0, 0 + 2), src)
                    }
                },
                new ExpressionNode()
                {
                    Expression = new Node()
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
                        new ExpressionNode()
                        {
                            Expression = new Node()
                            {
                                Token = new Token(TokenKind.NumberLiteral, new Range(2, 4), src)
                            }
                        },
                        new ExpressionNode()
                        {
                            Expression = new Node()
                            {
                                Token = new Token(TokenKind.StringLiteral, new Range(6, 11), src)
                            }
                        }
                    },
                }
            }
        };

        Compare(ast, ast2);
    }

    [Test]
    public void Test05_Sum()
    {
        var src = "2 + 3;";
        var ast = new Parser(src).Parse();
        var ast2 = new BinaryNode()
        {
            Token = new Token(TokenKind.PlusToken, new Range(2, 3), src),
            Left = new Node()
            {
                Token = new Token(TokenKind.NumberLiteral, new Range(0, 1), src)
            },
            Right = new Node()
            {
                Token = new Token(TokenKind.NumberLiteral, new Range(4, 5), src)
            },
        };

        Compare(ast, ast2);
    }

    private void Compare(INode current, INode expected)
    {
        var json = ToJson(current);
        var expectedJson = ToJson(expected);

        Assert.That(
            expectedJson,
            Is.EqualTo(json),
            () => "AST:\n" + json + "\nEXPECTED:\n" + expectedJson
        );

        Console.WriteLine("AST:\n" + json);
    }

    private string ToJson(INode ast)
    {
        return JsonConvert.SerializeObject(ast, _settings);
    }
}