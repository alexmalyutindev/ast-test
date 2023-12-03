using AST.Nodes;

namespace AST.Interpreter;

public class Interpreter
{
    public readonly Stack<int> Stack = new();
    public readonly Stack<string> StringStack = new();
    public readonly Dictionary<string, object> Variables = new();

    private Memory<byte> _mem;

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
            case BinaryExpressionNode binaryNode:
                EvalBinary(binaryNode);
                break;
            case ExpressionStatementNode exp:
                Eval(exp.Expression);
                break;
            case VariableStatementNode variableDeclarationNode:
                VariableDeclaration(variableDeclarationNode);
                break;
        }
    }

    private void VariableDeclaration(VariableStatementNode node)
    {
        foreach (VariableDeclarationNode declaration in node.Declarations)
        {
            var id = declaration.Identifier as IdentifierNode;
            Variables[id!.Token.Value] = declaration.Initializer switch
            {
                LiteralNode l => l.Token.Kind == TokenKind.NumberLiteral ? int.Parse(l.Token.Value) : l.Token.Value,
                ExpressionStatementNode exp => EvalPop(exp),
                BinaryExpressionNode exp => EvalPop(exp),
                _ => 0,
            };
        }
    }

    private object EvalPop(INode exp)
    {
        Eval(exp);
        return Stack.Pop();
    }

    private void EvalBinary(BinaryExpressionNode binaryExpressionNode)
    {
        var type = TokenKind.NumberLiteral;

        switch (binaryExpressionNode.Left)
        {
            case BinaryExpressionNode b:
                EvalBinary(b);
                break;
            case LiteralNode n:
                type = n.Token.Kind;
                switch (n.Token.Kind)
                {
                    case TokenKind.NumberLiteral:
                        Stack.Push(int.Parse(n.Token.Value));
                        break;
                    case TokenKind.StringLiteral:
                        StringStack.Push(n.Token.Value[1..^1]);
                        break;
                    default:
                        throw new Exception($"Not supported Literal: {n.Token.Kind}");
                }

                break;
            case IdentifierNode id:
                Stack.Push((int) Variables[id.Token.Value]);
                break;
        }

        switch (binaryExpressionNode.Right)
        {
            case BinaryExpressionNode b:
                EvalBinary(b);
                break;
            case LiteralNode n:
                type = n.Token.Kind;
                switch (n.Token.Kind)
                {
                    case TokenKind.NumberLiteral:
                        Stack.Push(int.Parse(n.Token.Value));
                        break;
                    case TokenKind.StringLiteral:
                        StringStack.Push(n.Token.Value[1..^1]);
                        break;
                    default:
                        throw new Exception($"Not supported Literal: {n.Token.Kind}");
                }

                break;
            case IdentifierNode id:
                Stack.Push((int) Variables[id.Token.Value]);
                break;
        }

        if (type == TokenKind.NumberLiteral)
        {
            var (right, left) = (Stack.Pop(), Stack.Pop());
            // TODO: Optimize
            switch (binaryExpressionNode.Token.Value)
            {
                case "+":
                    Stack.Push(left + right);
                    break;
                case "-":
                    Stack.Push(left - right);
                    break;

                case "*":
                    Stack.Push(left * right);
                    break;

                case "/":
                    Stack.Push(left / right);
                    break;
            }
        }
        else
        {
            var (right, left) = (StringStack.Pop(), StringStack.Pop());
            StringStack.Push(left + right);
        }
    }
}