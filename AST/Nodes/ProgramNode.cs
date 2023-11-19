namespace AST.Nodes;

public class ProgramNode : INode
{
    public Token Token { get; init; }
    public int ChildCount => Children.Length;
    public INode[] Children { get; init; }
}