using AST.Nodes;

namespace AST.Interpreter;

public class Interpreter
{
    public readonly Stack<int> Stack = new();
    public readonly Stack<string> StringStack = new();
    public readonly Dictionary<string, object> Variables = new();

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
                if (n.Token.Kind == TokenKind.NumberLiteral)
                    Stack.Push(Int32.Parse(n.Token.Value));
                else
                    StringStack.Push(n.Token.Value);
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
                if (n.Token.Kind == TokenKind.NumberLiteral)
                    Stack.Push(int.Parse(n.Token.Value));
                else
                    StringStack.Push(n.Token.Value);
                break;
            case IdentifierNode id:
                Stack.Push((int) Variables[id.Token.Value]);
                break;
        }

        if (type == TokenKind.NumberLiteral)
        {
            var (right, left) = (Stack.Pop(), Stack.Pop());
            switch (binaryExpressionNode.Token.Kind)
            {
                case TokenKind.PlusToken:
                    Stack.Push(left + right);
                    break;
                case TokenKind.MinusToken:
                    Stack.Push(left - right);
                    break;
                
                case TokenKind.MultiplyToken:
                    Stack.Push(left * right);
                    break;

                case TokenKind.DivideToken:
                    Stack.Push(left / right);
                    break;
            }
        }
        else
        {
            StringStack.Push(StringStack.Pop() + StringStack.Pop());
        }
    }
}