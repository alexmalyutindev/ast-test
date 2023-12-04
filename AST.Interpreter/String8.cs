namespace AST.Interpreter;

public struct String8
{
    public readonly int Length;
    public readonly char v0, v1, v2, v3, v4, v5, v6, v7;

    private String8(string s)
    {
        Length = s.Length;
        var charArray = s.ToCharArray(0, Length);

        v0 = charArray[0];
        if (Length > 1)
            v1 = charArray[1];
        if (Length > 2)
            v2 = charArray[2];
        if (Length > 3)
            v3 = charArray[3];
        if (Length > 4)
            v4 = charArray[4];
        if (Length > 5)
            v5 = charArray[5];
        if (Length > 6)
            v6 = charArray[6];
        if (Length > 7)
            v7 = charArray[7];
    }

    public static implicit operator string(String8 s)
    {
        return s.Length switch
        {
            1 => "" + s.v0,
            2 => "" + s.v0 + s.v1,
            3 => "" + s.v0 + s.v1 + s.v2,
            4 => "" + s.v0 + s.v1 + s.v2 + s.v3,
            5 => "" + s.v0 + s.v1 + s.v2 + s.v3 + s.v4,
            6 => "" + s.v0 + s.v1 + s.v2 + s.v3 + s.v4 + s.v5,
            7 => "" + s.v0 + s.v1 + s.v2 + s.v3 + s.v4 + s.v5 + s.v6,
            8 => "" + s.v0 + s.v1 + s.v2 + s.v3 + s.v4 + s.v5 + s.v6 + s.v7,
            _ => "",
        };
    }

    public static explicit operator String8(string s) => new(s);
}