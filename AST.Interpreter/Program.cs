// See https://aka.ms/new-console-template for more information

using System.Text;
using AST.Interpreter;
using AST.Nodes;

var src = """
          2 + 3 - 1 + 10;
          "abc" + "def";
          """;
var interpreter = new Interpreter(src);

interpreter.Run();

var sb = new StringBuilder()
    .AppendLine(">Source:")
    .AppendLine(src)
    .AppendLine(">Stack:")
    .AppendLine(String.Join(", ", interpreter.Stack))
    .AppendLine(">StringStack:")
    .AppendLine(String.Join(", ", interpreter.StringStack));

Console.WriteLine(sb);

namespace AST.Interpreter
{
    public class Interpreter
    {
        public readonly Stack<int> Stack = new();
        public readonly Stack<string> StringStack = new();

        private readonly Parser _parser;
        private readonly INode _ast;

        public Interpreter(string src)
        {
            _parser = new Parser(src);
            _ast = _parser.Parse();
        }

        public void Run()
        {
            if (_ast is not ProgramNode program)
            {
                return;
            }

            foreach (var node in program.Body)
            {
                Eval(node);
            }
        }

        private void Eval(INode node)
        {
            switch (node)
            {
                case BinaryNode binaryNode:
                    EvalBinary(binaryNode);
                    break;
                case ExpressionNode exp:
                    Eval(exp.Expression);
                    break;
            }
        }

        private void EvalBinary(BinaryNode binaryNode)
        {
            var type = TokenKind.NumberLiteral;

            switch (binaryNode.Left)
            {
                case BinaryNode b:
                    EvalBinary(b);
                    break;
                case Node n:
                    type = n.Token.Kind;
                    if (n.Token.Kind == TokenKind.NumberLiteral)
                        Stack.Push(Int32.Parse(n.Token.Value));
                    else
                        StringStack.Push(n.Token.Value);
                    break;
            }

            switch (binaryNode.Right)
            {
                case BinaryNode b:
                    EvalBinary(b);
                    break;
                case Node n:
                    type = n.Token.Kind;
                    if (n.Token.Kind == TokenKind.NumberLiteral)
                        Stack.Push(Int32.Parse(n.Token.Value));
                    else
                        StringStack.Push(n.Token.Value);
                    break;
            }

            if (type == TokenKind.NumberLiteral)
            {
                switch (binaryNode.Token.Kind)
                {
                    case TokenKind.PlusToken:
                        Stack.Push(Stack.Pop() + Stack.Pop());
                        break;
                    case TokenKind.MinusToken:
                        Stack.Push(Stack.Pop() - Stack.Pop());
                        break;
                }
            }
            else
            {
                StringStack.Push(StringStack.Pop() + StringStack.Pop());
            }
        }
    }
}