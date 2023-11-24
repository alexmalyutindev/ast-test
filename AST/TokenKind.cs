namespace AST;

public enum TokenKind
{
    Unknown = 0,
    End,

    // Keywords
    IfToken,
    ElseToken,

    SimpleAssign,
    ComplexAssign,
    
    // Literals
    NumberLiteral,
    BooleanLiteral,
    StringLiteral,
    NullLiteral,

    // Logical
    LogicalAnd, // '&&'
    LogicalOr,  // '||'
    LogicalNot, // '!'
    
    WhiteSpace = ' ',

    PlusToken = '+',
    MinusToken = '-',
    MultiplyToken = '*',
    DivideToken = '/',

    OpenParentheses = '(',
    CloseParentheses = ')',
    CommaToken = ',',

    Semicolon = ';',

    OpenCurlyBrace = '{',
    CloseCurlyBrace = '}',

    VariableDeclarationToken = 'v',
    Identifier = 'i',

    EqualityOperator = 'q',
    GreaterToken = '<',
    LessToken = '>',
}