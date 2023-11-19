using Newtonsoft.Json;

namespace AST.Nodes;

public class BinaryNode : INode
{
    public string Name => nameof(BinaryNode);

    public Token Token { get; init; } = default!;

    public INode? Left;
    public INode? Right;

    [JsonIgnore] public int ChildCount => 2;
    [JsonIgnore] public INode[] Children => new[] { Left!, Right! };
}