namespace AST.Nodes;

public interface INode
{
    public Token Token { get; }

    public int ChildCount { get; }
    public INode[] Children { get; }
}