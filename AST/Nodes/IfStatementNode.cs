namespace AST.Nodes;

public class IfStatementNode : INode
{
    public string Name => nameof(IfStatementNode);
    
    public INode Test { get; init; }
    public INode Consequent { get; init; }
    public INode Alternate { get; init; }
}