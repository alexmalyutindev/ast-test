using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AST.Interpreter;

public class Stack
{
    // TODO: Move out
    private Heap _heap = new();
    
    private int _sp = 0;
    private readonly Memory<byte> _stack = new byte[64];

    public int Push<T>(T value) where T : struct
    {
        var size = Unsafe.SizeOf<T>();
        MemoryMarshal.Write(_stack.Slice(_sp, size).Span, ref value);

        _sp += size;
        return _sp;
    }

    public int Push(string value)
    {
        // NOTE: String saved as a pointer in stack referencing heap
        var size = Unsafe.SizeOf<int>();
        var ptr = _heap.Alloc(value);

        MemoryMarshal.Write(_stack.Span, ref ptr);

        _sp += size;
        return _sp;
    }

    public T Pop<T>() where T : struct
    {
        var size = Unsafe.SizeOf<T>();
        _sp -= size;
        return MemoryMarshal.Read<T>(_stack.Slice(_sp, size).Span);
    }
    
    public string PopString()
    {
        var size = Unsafe.SizeOf<int>();
        _sp -= size;
        var ptr = MemoryMarshal.Read<int>(_stack.Slice(_sp, size).Span);

        return _heap.GetValue<string>(ptr);
    }
}

public class Heap
{
    private int size;
    private readonly object[] _heap = new object[16];

    public int Alloc(string value)
    {
        var ptr = size++;
        _heap[ptr] = value;
        return ptr;
    }

    public T GetValue<T>(int ptr)
    {
        return (T) _heap[ptr];
    }
}
