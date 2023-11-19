namespace AST;

public enum TokenKind
{
    Unknown,
    End,
    NumberLiteral,
    PlusToken = '+',
    MinusToken = '-',
    MultiplyToken = '*',
    DivideToken = '/',
    OpenParentheses = '(',
    CloseParentheses = ')',
    StringLiteral = 48,
    WhiteSpace,
    Semicolon = ';',
    Program
}