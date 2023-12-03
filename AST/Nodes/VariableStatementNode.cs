namespace AST.Nodes;

public class VariableStatementNode : INode
{
    public string Name => nameof(VariableStatementNode);
    public VariableDeclarationNode[] Declarations { get; init; }
}