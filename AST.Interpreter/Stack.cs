using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AST.Interpreter;

public class Stack
{
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
        var size = Unsafe.SizeOf<String8>();
        var string8 = (String8) value;
        MemoryMarshal.Write(_stack.Span, ref string8);
        _sp += size;
        return _sp;
    }

    public T Pop<T>() where T : struct
    {
        var size = Unsafe.SizeOf<T>();
        _sp -= size;
        return MemoryMarshal.Read<T>(_stack.Slice(_sp, size).Span);
    }
}
