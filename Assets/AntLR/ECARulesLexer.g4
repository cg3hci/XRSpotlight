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
AQUATIC:            'aquatic animal';
FLYING:             'flying animal';
TERRESTRIAL:        'terrestrial animal';
CREATURE:           'creature';
HUMAN:              'human';
ROBOT:              'robot';
ART:                'art';
BUILDING:           'building';
EXTERIOR:           'exterior';
FORNITURE:          'forniture';
SKY:                'sky';
VEGETATION:         'vegetation';
TERRAIN:            'terrain';
CLOTHING:           'clothing';
ELECTRONIC:         'electronic';
FOOD:               'food';
BULLET:             'bullet';
EDGED:              'edged weapon';
FIREARM:            'firearm';
SHIELD:             'shield';
AIRVEHICLE:         'air vehicle';
LANDVEHICLE:        'land vehicle';
SEAVEHICLE:         'sea vehicle';
SPACEVEHICLE:       'space vehicle';
BOUNDS:             'bounds';
BUTTON:             'button';
CAMERA:             'camera';
CONTAINER:          'container';
COLLECTABLE:        'collectable';
COUNTER:            'counter';
HIGHLIGHT:          'highlight';
IMAGE:              'image';
KEYPAD:             'keypad';
LIGHT:              'light';
PARTICLE:           'particle';
PLACEHOLDER:        'placeholder';
SCENE:              'scene';
SOUND:              'sound';
SWITCH:             'switch';
TEXT:               'text';
TRANSITION:         'transition';
LOCK:               'lock';
TIMER:              'timer';
TRIGGER:            'trigger';
VIDEO:              'video';

COLOR:              'color';
POSITION:           'position';
PATH:               'path';

// verbs
ACCELERATES:        'accelerates';
AIMS:               'aims';
APPENDS:            'appends';
BLOCKS:             'blocks';
CHANGES:            'changes';
CLOSES:             'closes';
COLLECTS:           'collects';
CONTAINS:           'contains';
DECREASES:          'decreases';
DELETES:            'deletes';
EATS:               'eats';
ENDS:               'ends';
ENTERS:             'enters';
EMPTIES:            'empties';
EXPIRES:            'expires';
FIRES:              'fires';
FLIES:              'flies';
GETS:               'gets';
HAS:                'has';
HASEATEN:           'has eaten';
INCREASES:          'increases';
INSERTS:            'inserts';
INTERACTS:          'interacts';
LANDS:              'lands';
LEAVES:             'leaves';
LOOKS:              'looks';
MOVES:              'moves';
JUMPS:              'jumps';
OPENS:              'opens';
PAUSES:             'pauses';
PLAYS:              'plays';
PUSHES:             'pushes';
RECHARGES:          'recharges';
REMOVES:            'removes';
RESETS:             'resets';
ROTATES:            'rotates';
RUNS:               'runs';
SCALES:             'scales';
SETS:               'sets';
SOUNDCURRENTTIME:   'sound current time';
SOUNDDURATION:      'sound duration';
SOUNDMAXVOLUME:     'sound max volume';
SOUNDPAUSE:         'pauses sound';
SOUNDPLAY:          'plays sound';
SOUNDSOURCE:        'sound source';
SOUNDSTOP:          'stops sound';
SOUNDVOLUME:        'sound volume';
SLICES:             'slices';
SLOWS:              'slows-down';
SPEAKS:             'speaks';
STABS:              'stabs';
STARTS:             'starts';
STARTSANIMATION:    'starts-animation';
STOPS:              'stops';
SWIMS:              'swims';
SWITCHES:           'switches';
TAKESOFF:           'takes-off';
TELEPORTS:          'teleports';
TIMERELAPSE:        'elapses timer';
TIMERPAUSE:         'pauses timer';
TIMERSTART:         'starts timer';
TIMERSTOP:          'stops timer';
TRIGGERS:           'triggers';
TURNS:              'turns';
UNWEARS:            'unwears';
WALKS:              'walks';
WEARS:              'wears';
ZOOMSIN:            'zooms-in';
ZOOMSOUT:           'zooms-out';

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
KILOS:              'Kg';
KMH:                'Km/h';
PX:                 'px';

// state variables
ACTIVABLE:          'activable';
AUTHOR:             'author';
BRAND:              'brand';
CAPACITY:           'capacity';
CHARGE:             'charge';
CONTENT:            'content';
COUNT:              'count';
COLLECTED:          'collected';
CURRENTTIME:        'current time';
DESCRIPTION:        'description';
DIMENSION:          'dimension';
DURATION:           'duration';
ENDED:              'ended';
GRAVITY:            'gravity';
HEIGHT:             'height';
HIGHLIGHTCOLOR:     'highlight color';
INPUT:              'input';
INSIDE:             'inside';
INTENSITY:          'intensity';
KEYCODE:            'key-code';
LIFE:               'life';
LOCKED:             'locked';
MAXINTENSITY:       'maximum intensity';
MAXVOLUME:          'maximum volume';
MODEL:              'model';
OBJECTSCOUNT:       'objects count';
OXYGEN:             'oxygen';
PAUSED:             'paused';
POWER:              'power';
PLAYING:            'playing';
POV:                'point of view';
PRICE:              'price';
ROTATION:           'rotation';
SCALE:              'scale';
SIZE:               'size';
SOURCE:             'source';
SPEED:              'speed';
STOPPED:            'stopped';
TARGET:             'target';
VISIBLE:            'visible';
VOLUME:             'volume';
WIDTH:              'width';
WEARED:             'weared';
WEIGHT:             'weight';
YEAR:               'year';
ZOOM:               'zoom';


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
    ;
fragment Letter
    : [a-zA-Z$_] // these are the "java letters" below 0x7F
    | ~[\u0000-\u007F\uD800-\uDBFF] // covers all characters above 0x7F which are not a surrogate
    | [\uD800-\uDBFF] [\uDC00-\uDFFF] // covers UTF-16 surrogate pairs encodings for U+10000 to U+10FFFF
    ;