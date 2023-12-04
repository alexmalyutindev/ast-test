using AST.Nodes;

namespace AST.Interpreter;

public class Interpreter
{
    public INode Program => _ast;

    public readonly Stack Stack = new();
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
        foreach (var declaration in node.Declarations)
        {
            var id = declaration.Identifier as IdentifierNode;
            Variables[id!.Token.Value] = declaration.Initializer switch
            {
                LiteralNode<int> l => l.Value,
                LiteralNode<string> l => l.Value,
                ExpressionStatementNode exp => EvalPop(exp),
                BinaryExpressionNode exp => EvalPop(exp),
                _ => 0,
            };
        }
    }

    private object EvalPop(INode exp)
    {
        // TODO: 
        Eval(exp);
        return Stack.Pop<int>();
    }

    private void EvalBinary(BinaryExpressionNode binaryExpressionNode)
    {
        var type = TokenKind.NumberLiteral;

        switch (binaryExpressionNode.Right)
        {
            case BinaryExpressionNode b:
                EvalBinary(b);
                break;
            case LiteralNode<int> n:
                type = TokenKind.NumberLiteral;
                Stack.Push(n.Value);
                break;
            case LiteralNode<string> s:
                type = TokenKind.StringLiteral;
                Stack.Push(s.Value);
                break;
            case IdentifierNode id:
                Stack.Push((int) Variables[id.Token.Value]);
                break;
        }

        switch (binaryExpressionNode.Left)
        {
            case BinaryExpressionNode b:
                EvalBinary(b);
                break;
            case LiteralNode<int> n:
                type = TokenKind.NumberLiteral;
                Stack.Push(n.Value);
                break;
            case LiteralNode<string> s:
                type = TokenKind.StringLiteral;
                Stack.Push(s.Value);
                break;

            case IdentifierNode id:
                Stack.Push((int) Variables[id.Token.Value]);
                break;
        }

        switch (type)
        {
            case TokenKind.NumberLiteral:
            {
                var (left, right) = (Stack.Pop<int>(), Stack.Pop<int>());
                // TODO: Compare operators support!
                switch (binaryExpressionNode.Operator)
                {
                    case BinaryOperator.Plus:
                        Stack.Push(left + right);
                        break;
                    case BinaryOperator.Minus:
                        Stack.Push(left - right);
                        break;

                    case BinaryOperator.Multiply:
                        Stack.Push(left * right);
                        break;
                    case BinaryOperator.Divide:
                        Stack.Push(left / right);
                        break;
                }

                break;
            }
            case TokenKind.StringLiteral:
            {
                if (binaryExpressionNode.Operator != BinaryOperator.Plus)
                {
                    throw new Exception("Not supported operation on stings!");
                }

                var (right, left) = (Stack.PopString(), Stack.PopString());
                Stack.Push(left + right);
                break;
            }
        }
    }
}
