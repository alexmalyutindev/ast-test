using System.Text.Json;
using AST.Nodes;

namespace AST;

public static class Program
{
    public static void Main()
    {
        var program = "123 / 3 + (456 - 69) * 2";
        var parser = new Parser(program);

        Console.WriteLine();
        var root = parser.Parse();


        Console.WriteLine();
        Console.WriteLine(program);

        var q = new Queue<INode>();
        q.Enqueue(root);

        var options = new JsonSerializerOptions(JsonSerializerDefaults.General)
        {
            WriteIndented = true,
        };
        var json = JsonSerializer.Serialize(root, options);
        Console.WriteLine(json);
    }
}