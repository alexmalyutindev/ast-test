namespace AST;

public enum TokenKind
{
    Program,
    End,
    WhiteSpace,

    NumberLiteral,
    PlusToken = '+',
    MinusToken = '-',
    MultiplyToken = '*',
    DivideToken = '/',

    OpenParentheses = '(',
    CloseParentheses = ')',

    StringLiteral = 48,
    Semicolon = ';',

    OpenCurlyBrace = '{',
    CloseCurlyBrace = '}',

    Unknown,
    StatementList
}