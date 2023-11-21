using AST.Nodes;

namespace AST;

public class ExpressionStatementNode : INode
{
    public string Name => nameof(ExpressionStatementNode);
    public INode Expression { get; set; }
}