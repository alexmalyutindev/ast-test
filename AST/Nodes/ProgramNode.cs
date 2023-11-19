namespace AST.Nodes;

public class ProgramNode : INode
{
    public string Name => nameof(ProgramNode);

    public string ProgramName { get; init; }
    public int ChildCount => Body.Length;
    public INode[] Body { get; init; }
}