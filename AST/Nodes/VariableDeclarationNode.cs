namespace AST.Nodes;

public class VariableDeclarationNode : INode
{
    public string Name => nameof(VariableDeclarationNode);
    public INode Identifier { get; init; }
    public INode Initializer { get; init; }
}