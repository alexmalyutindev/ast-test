using AST.Nodes;

namespace AST;

public class ExpressionNode : INode
{
    public string Name => nameof(ExpressionNode);
    public INode Expression { get; set; }
}