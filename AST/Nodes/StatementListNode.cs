namespace AST.Nodes;

public class StatementListNode : INode
{
    public string Name => nameof(StatementListNode);

    public int ChildCount => Children.Length;
    public INode[] Children { get; init; }
}