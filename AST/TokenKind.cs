namespace AST;

public enum TokenKind
{
    Unknown = '0',
    End = '1',
    WhiteSpace = ' ',

    NumberLiteral = 'n',
    PlusToken = '+',
    MinusToken = '-',
    MultiplyToken = '*',
    DivideToken = '/',
    
    AssignToken = '=',

    OpenParentheses = '(',
    CloseParentheses = ')',
    CommaToken = ',',

    StringLiteral = 's',
    Semicolon = ';',

    OpenCurlyBrace = '{',
    CloseCurlyBrace = '}',

    VariableDeclarationToken = 'v',
    Identifier = 'i',

    IfToken = 'f',
    ElseToken = 'e',

    // Compare
    EqualsToken = 'q',
    GreaterToken = '<',
    LessToken = '>',
}