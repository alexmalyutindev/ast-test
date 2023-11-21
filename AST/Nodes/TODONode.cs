namespace AST.Nodes;

public class TODONode : INode
{
    public string Name => "TODO";
    public string Message { get; }

    public TODONode(string message)
    {
        Message = message;
    }
}