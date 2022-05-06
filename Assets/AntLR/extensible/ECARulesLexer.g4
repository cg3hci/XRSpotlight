lexer grammar ECARulesLexer;

WHITESPACE: [ \t\r\n]-> skip;
//keywords

WHEN:       'when';
THEN:       'then';
IF:         'if';
THE:        'the';
X:          'x';
Y:          'y';
Z:          'z';
AXIS:       'axis';
DEFINE:     'define';
CONDITION:  'condition';
HAS:    'has';



// Operators
GT:                 '>';
LT:                 '<';
NOT:               'not';
EQUAL:              '=';
LE:                 '<=';
GE:                 '>=';
NOTEQUAL:           '!=';
AND:                'and';
OR:                 'or';
ADD:                '+';
SUB:                '-';
MUL:                '*';
DIV:                '/';
MOD:                '%';
IS:                 'is';

// Separators
LPAREN:             '(';
RPAREN:             ')';
LBRACE:             '{';
RBRACE:             '}';
LBRACK:             '[';
RBRACK:             ']';
SEMI:               ';';
COMMA:              ',';
DOT:                '.';

// types
COLOR:              'color';
POSITION:           'position';
PATH:               'path';

// prepositions
A:                  'a';
AS:                 'as';
AT:                 'at';
AROUND:             'around';
BY:                 'by';
FOR:                'for';
FROM:               'from';
IN:                 'in';
ON:                 'on';
OF:                 'of';
OFF:                'off';
TO:                 'to';
WITH:               'with';

// units
DEGREES:            'degrees';

// values
POV_LITERAL: '1st person' | '3rd person';
BOOL_LITERAL:       'true'
            |       'false'
            ;
BOOL_YES_NO:        'yes'
            |       'no'
            ;
COLOR_LITERAL:      HexColor;
DECIMAL_LITERAL:    ('0' | [1-9] (Digits? | '_'+ Digits));
IDENTIFIER:         Letter LetterOrDigit*;
STRING_LITERAL:     '"' (~["\\\r\n] | EscapeSequence)* '"';
FLOAT_LITERAL:      (Digits '.' Digits? | '.' Digits) ExponentPart? [fFdD]?
             |       Digits (ExponentPart [fFdD]? | [fFdD])
             ;
TIME_LITERAL:       (Digits+'d:')?(('2'[0-3]|[01]?[0-9])'h:')?([0-5]?[0-9]'m:')?([0-5]?[0-9]'s')('.'Digits+)?;
MEASURE_UNIT:       Letter (LetterOrDigit | '/')*;

// Fragment rules
fragment HexColor
    : '#' HexDigit HexDigit HexDigit HexDigit HexDigit HexDigit;
fragment ExponentPart
    : [eE] [+-]? Digits
    ;

fragment EscapeSequence
    : '\\' [btnfr"'\\]
    | '\\' ([0-3]? [0-7])? [0-7]
    | '\\' 'u'+ HexDigit HexDigit HexDigit HexDigit
    ;
fragment HexDigits
    : HexDigit ((HexDigit | '_')* HexDigit)?
    ;
fragment HexDigit
    : [0-9a-fA-F]
    ;
fragment Digits
    : [0-9] ([0-9_]* [0-9])?
    ;
fragment LetterOrDigit
    : Letter
    | [0-9]
    | '-'
    ;
fragment Letter
    : [a-zA-Z$_] // these are the "java letters" below 0x7F
    | ~[\u0000-\u007F\uD800-\uDBFF] // covers all characters above 0x7F which are not a surrogate
    | [\uD800-\uDBFF] [\uDC00-\uDFFF] // covers UTF-16 surrogate pairs encodings for U+10000 to U+10FFFF
    ;