namespace AST.Nodes;

public class VariableStatementNode : INode
{
    public string Name => nameof(VariableStatementNode);
    public INode[] Declarations { get; init; }
}