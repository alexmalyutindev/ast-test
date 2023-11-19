using Newtonsoft.Json;

namespace AST.Nodes;

public class Node : INode
{
    public string Name => nameof(Node);

    public Token Token { get; init; } = default!;

    [JsonIgnore] public int ChildCount => 0;
    [JsonIgnore] public INode[] Children => Array.Empty<INode>();
}