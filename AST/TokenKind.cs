namespace AST;

public enum TokenKind
{
    Unknown,
    End,
    WhiteSpace,

    NumberLiteral,
    PlusToken = '+',
    MinusToken = '-',
    MultiplyToken = '*',
    DivideToken = '/',
    
    AssignToken = '=',
    EqualsToken,

    OpenParentheses = '(',
    CloseParentheses = ')',
    CommaToken = ',',

    StringLiteral = 48,
    Semicolon = ';',

    OpenCurlyBrace = '{',
    CloseCurlyBrace = '}',

    VariableDeclarationToken,
    Identifier,
}