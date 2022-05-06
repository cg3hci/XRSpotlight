parser grammar ECARulesParser;
options {tokenVocab=ECARulesLexer;}

// program
program:
    declaration+
    behaviourDeclaration*
    ecarule+
    alias*;

// variables declaration
declaration:
    objectDeclaration | positionDeclaration | pathDeclaration | colorDeclaration;

objectDeclaration :
    DEFINE type IDENTIFIER SEMI;

positionDeclaration :
    DEFINE POSITION IDENTIFIER EQUAL positionLiteral SEMI;

positionLiteral :
    LPAREN floatLiteral COMMA
    floatLiteral  COMMA
    floatLiteral RPAREN;

pathDeclaration :
    DEFINE PATH IDENTIFIER EQUAL LBRACE positionLiteral (COMMA positionLiteral)? RBRACE SEMI;

colorDeclaration :
    DEFINE COLOR IDENTIFIER EQUAL COLOR_LITERAL SEMI;

floatLiteral :
    FLOAT_LITERAL | DECIMAL_LITERAL;
type:
    IDENTIFIER;

// complex types
position:
    THE POSITION IDENTIFIER;
path:
    THE PATH IDENTIFIER;
angle:
    floatLiteral DEGREES (AROUND (X|Y|Z) AXIS)?;
color:
    THE COLOR IDENTIFIER;

behaviourDeclaration:
    THE type IDENTIFIER HAS A type SEMI;

// type alias
alias:
    DEFINE type AS STRING_LITERAL FOR IDENTIFIER SEMI;

ecarule :
    WHEN action (IF condition)? THEN (action SEMI)* ;

action :
    subject verb |
    subject verb (preposition)? object |
    subject verb (preposition)? value |
    subject verb (preposition)? property modifier value (MEASURE_UNIT)?|
    ;
    
subject: 
    THE type IDENTIFIER;

verb: 
    IDENTIFIER;

object:
    THE type IDENTIFIER;

value: 
    floatLiteral |
    position | path |
    angle | color |
    POV_LITERAL |
    BOOL_LITERAL |
    BOOL_YES_NO |
    ON | OFF |
    COLOR_LITERAL |
    TIME_LITERAL |
    STRING_LITERAL;

modifier:
    preposition;

preposition:
    A |
    AS |
    AT |
    AROUND |
    BY |
    FOR |
    FROM |
    IN |
    ON |
    OF |
    OFF |
    TO |
    WITH;

property:
    IDENTIFIER | COLOR | POSITION | PATH;

condition :
    baseCondition |
    NOT condition |
    condition (AND condition)+ |
    condition (OR condition)+ |
    LPAREN condition RPAREN;

baseCondition:
        THE type IDENTIFIER (NOT)? (property)? (operator)? (value | object)
    ;

operator:
    GT |
    LT |
    EQUAL | 
    LE |   
    GE |
    NOTEQUAL |
    AND |
    OR |
    ADD |
    SUB |
    MUL |
    DIV |
    MOD |
    IS ;