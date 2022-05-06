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

// variable references
reference:
    THE object IDENTIFIER;
type:
    object | behaviour;
object:
    character | scene | environment | prop | vehicle | interaction;
character:
    animal | CREATURE | HUMAN | ROBOT;
animal:
    AQUATIC | FLYING | TERRESTRIAL;
scene:
    SCENE;
environment:
    ART | BUILDING | EXTERIOR | FORNITURE | SKY | VEGETATION | TERRAIN;
prop:
    CLOTHING | ELECTRONIC | FOOD | weapon;
weapon:
    BULLET | EDGED | FIREARM | SHIELD;
vehicle:
    AIRVEHICLE | LANDVEHICLE | SEAVEHICLE | SPACEVEHICLE;
interaction:
    BOUNDS | BUTTON | CAMERA | IMAGE | LIGHT | TEXT | VIDEO;
behaviour:
    CONTAINER | COLLECTABLE | COUNTER | HIGHLIGHT | KEYPAD | LOCK |
    PARTICLE | PLACEHOLDER | SOUND | SWITCH | TRANSITION | TRIGGER | TIMER;

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
    THE object IDENTIFIER HAS A behaviour;

// type alias
alias:
    DEFINE type AS STRING_LITERAL FOR IDENTIFIER SEMI;

ecarule :
    WHEN action (IF condition)? THEN (action)* ;

action :
    aquaticAction | flyingAction | terrestrialAction |
    creatureAction | humanAction | robotAction |
    sceneAction |
    artAction | buildingAction | exteriorAction | fornitureAction |
    skyAction | vegetationAction | terrainAction |
    clothingAction | electronicAction | foodAction |
    bulletAction | edgedAction | firearmAction | shieldAction
    airVehicleAction | landVehicleAction | seaVehicleAction | spaceVehicleAction
    boundsAction | buttonAction | cameraAction | imageAction |
    lightAction | textAction | videoAction |
    containerAction | collectableAction | counterAction | highlightAction
    keypadAction | lockAction | particleAction | soundAction | placeholderAction
    switchAction | transitionAction | triggerAction | timerAction
    ;

// actions for each type
objectAction:
    moves | rotates | looksAt | setVisible | setActivable;

// character actions
characterAction:
    objectAction | jumps | interacts | setLife | setPlaying | startsAnimation;

animalAction:
    characterAction | speaks;

aquaticAction :
    THE AQUATIC IDENTIFIER (animalAction | swims);

flyingAction :
    THE FLYING IDENTIFIER (animalAction | flies);

terrestrialAction:
    THE TERRESTRIAL IDENTIFIER (animalAction | runs | walks);

creatureAction:
    THE CREATURE IDENTIFIER (animalAction | walks | swims | runs | flies);

humanAction:
    THE HUMAN IDENTIFIER (animalAction | walks | runs | swims);

robotAction:
    THE ROBOT IDENTIFIER (animalAction | walks | runs | swims);

// scene actions
sceneAction:
    THE SCENE IDENTIFIER (objectAction | enters | leaves);

// environment  actions
environmentAction:
    objectAction;

artAction:
    THE ART IDENTIFIER (environmentAction | setAuthor | setPrice | setYear);

buildingAction:
    THE BUILDING IDENTIFIER (environmentAction);

exteriorAction:
    THE EXTERIOR IDENTIFIER (environmentAction);

fornitureAction:
    THE FORNITURE IDENTIFIER (environmentAction | setPrice | setColor | setDimension);

skyAction:
    THE SKY IDENTIFIER (environmentAction);

vegetationAction:
    THE VEGETATION IDENTIFIER (environmentAction);

terrainAction:
    THE TERRAIN IDENTIFIER (environmentAction);

// props actions
propsActions:
    objectAction |
    setPrice;

clothingAction :
    (THE CLOTHING IDENTIFIER (propsActions | setBrand | setSize | setColor )) |
    wears | unwears;

electronicAction :
    THE ELECTRONIC IDENTIFIER (propsActions | setBrand | setModel | turns);

foodAction:
    (THE FOOD IDENTIFIER (propsActions | setWeight | setExpiration | setDescription)) |
    eats;

weaponAction:
    propsActions | setPower;

bulletAction:
    THE BULLET IDENTIFIER (weaponAction | setSpeed);

edgedAction:
    THE EDGED IDENTIFIER (weaponAction | stabs | slices);

firearmAction:
    THE FIREARM IDENTIFIER (weaponAction | recharges | fires | aims);

shieldAction:
    THE SHIELD IDENTIFIER (weaponAction | blocks);

// vehicle actions
vehicleAction:
    objectAction |
    setSpeed | starts | stops | accelerates | slowsDown;

airVehicleAction:
    THE AIRVEHICLE IDENTIFIER (vehicleAction | takesOff | lands);

landVehicleAction:
    THE LANDVEHICLE IDENTIFIER (vehicleAction);

seaVehicleAction:
    THE SEAVEHICLE IDENTIFIER (vehicleAction);

spaceVehicleAction:
    THE SPACEVEHICLE IDENTIFIER (vehicleAction | takesOff | lands | setGravity | setOxygen);

// interaction actions
interactionActions:
    objectAction;

boundsAction:
    (THE character IDENTIFIER (userRotates | userScales | userMoves) )|
    (THE BOUNDS IDENTIFIER (interactionActions ));

buttonAction:
    (THE character IDENTIFIER (userPushes) ) |
    (THE BUTTON IDENTIFIER (objectAction));

cameraAction:
    THE CAMERA IDENTIFIER (interactionActions | setPointOfView | setZoom | zoomsIn | zoomsOut | setPlaying);

imageAction:
    THE IMAGE IDENTIFIER (interactionActions | setSource | setHeight| setWidth);

lightAction:
    THE LIGHT IDENTIFIER (interactionActions | turns | setIntensity | setMaxIntensity | setColor );

textAction:
    THE TEXT IDENTIFIER (interactionActions | setContent | appends | deletes );

videoAction:
    THE VIDEO IDENTIFIER (interactionActions | plays | pauses
    | stops | ends | setSource | setVolume | setMaxVolume | setCurrentTime | setDuration);

// behaviour actions
containerAction:
    insertsObject | removes | empties |
    THE object IDENTIFIER (setCapacity | setObjectNumber);

collectableAction:
    collects;

counterAction:
    THE object IDENTIFIER setCount;

highlightAction:
    THE object IDENTIFIER (setHighlight | setHighlightColor);

keypadAction:
    THE object IDENTIFIER (inserts | resets | setKeyCode);

lockAction:
    THE object IDENTIFIER (opens | closes | setLocked);

particleAction:
    THE object IDENTIFIER turns;

soundAction:
    THE object IDENTIFIER
        (setSoundSource | setSoundVolume | setSoundMaxVolume | setSoundCurrentTime
         setSoundDuration | playSound |pauseSound | stopSound  );

placeholderAction:
    ;

switchAction:
    THE object IDENTIFIER (turns );

transitionAction:
    THE character IDENTIFIER teleports |
    THE object IDENTIFIER (setTargetScene);

triggerAction:
    THE object IDENTIFIER (triggers);

timerAction:
    THE object IDENTIFIER (setTimer | setDuration | startTimer | stopTimer | pauseTimer | elapseTimer | resets);

// verb list
accelerates:
    ACCELERATES (BY floatLiteral);

aims:
    AIMS reference;

appends:
    APPENDS STRING_LITERAL;

blocks:
    BLOCKS THE weapon IDENTIFIER;

closes:
    CLOSES;

deletes:
    DELETES;

elapseTimer:
    TIMERELAPSE;

ends:
    ENDS;

fires:
    FIRES reference;

flies:
    FLIES ((TO position) | (ON path));

interacts:
    INTERACTS WITH reference;

inserts:
    INSERTS STRING_LITERAL;

jumps:
    JUMPS ((TO position) | (ON path))? ;

lands:
    LANDS (IN position)?;

looksAt:
    LOOKS AT reference;

moves:
    MOVES ((TO position) | (ON path));

opens:
    OPENS;

pauses:
    PAUSES;

pauseSound:
    SOUNDPAUSE;

pauseTimer:
    TIMERPAUSE;

plays:
    PLAYS;

playSound:
    SOUNDPLAY;

recharges:
    RECHARGES (BY DECIMAL_LITERAL)?;

resets:
    RESETS;

rotates:
    ROTATES OF angle;

runs:
    RUNS ((TO position) | (ON path));

slices:
    SLICES reference;

slowsDown:
    SLOWS (BY floatLiteral)?;

speaks:
    SPEAKS STRING_LITERAL;

stabs:
    STABS reference;

startsAnimation:
    STARTSANIMATION STRING_LITERAL;

starts:
    STARTS;

startTimer:
    TIMERSTART;

stops:
    STOPS;

stopSound:
    SOUNDSTOP;

stopTimer:
    TIMERSTOP;

swims :
    SWIMS ((TO position) | (ON path));

teleports:
    TELEPORTS TO SCENE IDENTIFIER;

takesOff:
    TAKESOFF (FROM position)?;

triggers:
    TRIGGERS;

turns:
    TURNS (ON | OFF);

walks:
    WALKS ((TO position) | (ON path));

zoomsIn:
    ZOOMSIN;
zoomsOut:
    ZOOMSOUT;

// setters
setActivable:
    CHANGES ACTIVABLE TO BOOL_YES_NO;

setAuthor:
    CHANGES AUTHOR TO STRING_LITERAL;

setBrand:
    CHANGES BRAND TO STRING_LITERAL;

setCapacity:
    (INCREASES | DECREASES) CAPACITY (BY DECIMAL_LITERAL)? |
    CHANGES CAPACITY TO DECIMAL_LITERAL;

setColor:
    CHANGES COLOR TO (color | COLOR_LITERAL);

setContent:
    CHANGES CONTENT TO STRING_LITERAL;

setCount:
    (INCREASES | DECREASES) COUNT (BY DECIMAL_LITERAL)? |
        CHANGES COUNT TO DECIMAL_LITERAL;

setCurrentTime:
    CHANGES CURRENTTIME TO TIME_LITERAL;

setDescription:
    CHANGES DESCRIPTION TO STRING_LITERAL;

setDimension:
    (INCREASES | DECREASES) DIMENSION (BY floatLiteral)? |
    CHANGES DIMENSION TO floatLiteral;

setDuration:
    CHANGES DURATION TO TIME_LITERAL;

setHighlight:
    SWITCHES HIGHLIGHT TO (ON | OFF);

setHighlightColor:
    CHANGES HIGHLIGHTCOLOR TO color;

setKeyCode:
    CHANGES KEYCODE TO STRING_LITERAL;

setLocked:
    CHANGES LOCKED TO BOOL_LITERAL;

setExpiration:
    EXPIRES IN TIME_LITERAL;

setGravity:
    (INCREASES | DECREASES) GRAVITY (BY floatLiteral)? |
    CHANGES GRAVITY TO floatLiteral;

setHeight:
     (INCREASES | DECREASES) HEIGHT (BY floatLiteral PX)? |
     CHANGES HEIGHT TO floatLiteral PX;

setIntensity:
    (INCREASES | DECREASES) INTENSITY (BY floatLiteral)? |
     CHANGES INTENSITY TO floatLiteral;

setLife:
    (INCREASES | DECREASES) LIFE (BY floatLiteral)? |
    CHANGES LIFE TO floatLiteral;

setMaxIntensity:
    (INCREASES | DECREASES) MAXINTENSITY (BY floatLiteral)? |
     CHANGES MAXINTENSITY TO floatLiteral;

setMaxVolume:
    (INCREASES | DECREASES) MAXVOLUME (BY floatLiteral)? |
         CHANGES MAXVOLUME TO floatLiteral;

setModel:
    CHANGES MODEL TO STRING_LITERAL;

setObjectNumber:
    (INCREASES | DECREASES) OBJECTSCOUNT (BY DECIMAL_LITERAL)? |
        CHANGES OXYGEN TO DECIMAL_LITERAL;

setOxygen:
    (INCREASES | DECREASES) OXYGEN (BY floatLiteral)? |
    CHANGES OXYGEN TO floatLiteral;

setSize:
    CHANGES SIZE TO STRING_LITERAL;

setSource:
    CHANGES SOURCE TO STRING_LITERAL;

setPlaying:
    CHANGES PLAYING TO BOOL_YES_NO;

setPower:
    (INCREASES | DECREASES) POWER (BY floatLiteral)? |
    CHANGES POWER TO floatLiteral;

setPrice:
    (INCREASES | DECREASES) PRICE (BY floatLiteral)? |
    CHANGES PRICE TO floatLiteral;

setPointOfView:
    CHANGES POV TO POV_LITERAL;

setSpeed:
    (INCREASES | DECREASES) SPEED (BY floatLiteral KMH)? |
    CHANGES SPEED TO floatLiteral KMH;

setSoundDuration:
     CHANGES SOUNDDURATION TO TIME_LITERAL;

setSoundSource:
    CHANGES SOUNDSOURCE TO STRING_LITERAL;

setSoundVolume:
    (INCREASES | DECREASES) SOUNDVOLUME (BY floatLiteral)? |
        CHANGES SOUNDVOLUME TO floatLiteral;

setSoundMaxVolume:
    (INCREASES | DECREASES) SOUNDMAXVOLUME (BY floatLiteral)? |
        CHANGES SOUNDMAXVOLUME TO floatLiteral;

setSoundCurrentTime:
    CHANGES SOUNDCURRENTTIME TO TIME_LITERAL;

setTargetScene:
    CHANGES TARGET TO THE SCENE IDENTIFIER;

setTimer:
    CHANGES TIMER TO TIME_LITERAL;

setVisible:
    CHANGES VISIBLE TO BOOL_YES_NO;

setVolume:
    (INCREASES | DECREASES) VOLUME (BY floatLiteral)? |
        CHANGES VOLUME TO floatLiteral;

setWidth:
    (INCREASES | DECREASES) WIDTH (BY floatLiteral PX)? |
    CHANGES WIDTH TO floatLiteral PX;

setWeight:
    (INCREASES | DECREASES) WEIGHT (BY floatLiteral KILOS)? |
    CHANGES WEIGHT TO floatLiteral KILOS;

setYear:
    (INCREASES | DECREASES) YEAR (BY DECIMAL_LITERAL)? |
    CHANGES YEAR TO DECIMAL_LITERAL;

setZoom:
    (INCREASES | DECREASES) ZOOM (BY floatLiteral)? |
    CHANGES ZOOM TO floatLiteral;

// passive actions
collects:
    THE character IDENTIFIER COLLECTS reference;

insertsObject:
    THE object IDENTIFIER GETS reference;

removes:
    THE object IDENTIFIER REMOVES reference;

enters:
    THE character IDENTIFIER ENTERS THE SCENE IDENTIFIER;

empties:
    THE object IDENTIFIER EMPTIES;

eats:
    THE character IDENTIFIER EATS THE FOOD IDENTIFIER;

leaves:
    THE character IDENTIFIER LEAVES THE SCENE IDENTIFIER;

wears:
    THE character IDENTIFIER WEARS THE CLOTHING IDENTIFIER;

unwears:
    THE character IDENTIFIER UNWEARS THE CLOTHING IDENTIFIER;

userMoves:
    MOVES  ((TO position| ON path))?;

userPushes:
    PUSHES THE BUTTON IDENTIFIER;

userRotates:
    ROTATES THE BOUNDS IDENTIFIER (OF angle)?;

userScales:
    SCALES THE BOUNDS IDENTIFIER (TO floatLiteral);



condition :
    baseCondition |
    NOT condition |
    condition (AND | OR) condition |
    LPAREN condition RPAREN;

baseCondition:
        aquaticCondition | flyingCondition | terrestrialCondition |
        creatureCondition | humanCondition | robotCondition |
        sceneCondition |
        artCondition | buildingCondition | exteriorCondition | fornitureCondition |
        skyCondition | vegetationCondition | terrainCondition |
        clothingCondition | electronicCondition | foodCondition |
        bulletCondition | edgedCondition | firearmCondition | shieldCondition
        airVehicleCondition | landVehicleCondition | seaVehicleCondition | spaceVehicleCondition
        boundsCondition | buttonCondition | cameraCondition | imageCondition |
        lightCondition | textCondition | videoCondition |
        containerCondition | collectableCondition | counterCondition | highlightCondition
        keypadCondition | lockCondition | particleCondition | soundCondition | placeholderCondition
        switchCondition | transitionCondition | triggerCondition | timerCondition
    ;

numberOp: EQUAL | GT | LT | GE | LE | NOTEQUAL;

// object properties
objectCondition :
    VISIBLE IS BOOL_YES_NO |
    ACTIVABLE IS BOOL_YES_NO |
    POSITION IS positionLiteral |
    ROTATION IS angle |
    LOOKS AT reference;

// character properties
characterCondition :
    objectCondition |
    LIFE numberOp floatLiteral |
    PLAYING IS BOOL_YES_NO;

aquaticCondition :
    THE AQUATIC IDENTIFIER
    characterCondition;

flyingCondition:
    THE FLYING IDENTIFIER
    characterCondition;

terrestrialCondition:
    THE TERRESTRIAL IDENTIFIER
    characterCondition;

humanCondition:
    THE HUMAN IDENTIFIER
    characterCondition;

creatureCondition:
    THE CREATURE IDENTIFIER
    characterCondition;

robotCondition:
    THE ROBOT IDENTIFIER
    characterCondition;

// scene properties
sceneCondition:
    (THE SCENE IDENTIFIER objectCondition) |
    (THE character IDENTIFIER IS INSIDE THE SCENE IDENTIFIER);

// environment properties
environmentCondition:
    objectCondition;

artCondition:
    THE ART IDENTIFIER
    (environmentCondition |
    AUTHOR IS STRING_LITERAL |
    YEAR  IS DECIMAL_LITERAL |
    PRICE numberOp floatLiteral);

buildingCondition:
    THE BUILDING IDENTIFIER
    environmentCondition;

exteriorCondition:
    THE EXTERIOR IDENTIFIER
    environmentCondition;

fornitureCondition:
    THE FORNITURE IDENTIFIER (
    environmentCondition |
    PRICE numberOp floatLiteral |
    COLOR IS color |
    DIMENSION numberOp floatLiteral
    );

skyCondition:
    THE SKY IDENTIFIER (environmentCondition);

vegetationCondition:
    THE VEGETATION IDENTIFIER (environmentCondition);

terrainCondition:
    THE TERRAIN IDENTIFIER (environmentCondition);

propsCondition:
    objectCondition |
    PRICE numberOp floatLiteral;

clothingCondition:
    (THE CLOTHING IDENTIFIER
        (propsCondition |
        BRAND IS STRING_LITERAL |
        COLOR IS color |
        SIZE IS STRING_LITERAL |
        IS WEARED)) |
     THE character IDENTIFIER WEARS THE CLOTHING IDENTIFIER|
     THE character IDENTIFIER UNWEARS THE CLOTHING IDENTIFIER;

electronicCondition:
    THE ELECTRONIC IDENTIFIER
    (propsCondition |
    BRAND IS STRING_LITERAL |
    MODEL IS STRING_LITERAL
    IS ON | IS OFF);

foodCondition:
    (THE FOOD IDENTIFIER
        (propsCondition |
        WEIGHT numberOp floatLiteral) )|
    (THE character IDENTIFIER HASEATEN THE FOOD IDENTIFIER);

weaponCondition:
    propsCondition |
    POWER numberOp floatLiteral;

bulletCondition:
    THE BULLET IDENTIFIER
        (weaponCondition |
         SPEED numberOp floatLiteral);

edgedCondition:
    THE EDGED IDENTIFIER (weaponCondition);

firearmCondition:
    THE FIREARM IDENTIFIER
        (weaponCondition |
        CHARGE numberOp DECIMAL_LITERAL);

shieldCondition:
    THE SHIELD IDENTIFIER (weaponCondition);

// vehicle conditions
vehicleCondition:
    objectCondition |
    SPEED numberOp floatLiteral;

airVehicleCondition:
    THE AIRVEHICLE IDENTIFIER (vehicleCondition);

landVehicleCondition:
    THE LANDVEHICLE IDENTIFIER (vehicleCondition);

seaVehicleCondition:
    THE SEAVEHICLE IDENTIFIER (vehicleCondition);

spaceVehicleCondition:
    THE SPACEVEHICLE IDENTIFIER
        (vehicleCondition
        OXYGEN numberOp floatLiteral
        GRAVITY numberOp floatLiteral);

// interaction condition
interactionCondition:
    objectCondition;

boundsCondition:
    THE BOUNDS IDENTIFIER
        (interactionCondition |
        SCALE numberOp floatLiteral);

buttonCondition:
    THE BUTTON IDENTIFIER (interactionCondition);

cameraCondition:
    THE CAMERA IDENTIFIER
        (interactionCondition |
        POV IS POV_LITERAL |
        ZOOM numberOp floatLiteral |
        PLAYING IS BOOL_YES_NO);

imageCondition:
    THE IMAGE IDENTIFIER
        (interactionCondition |
        SOURCE IS STRING_LITERAL |
        HEIGHT numberOp floatLiteral |
        WIDTH numberOp  floatLiteral );

lightCondition:
    THE LIGHT IDENTIFIER
    (interactionCondition |
    INTENSITY numberOp floatLiteral |
    MAXINTENSITY numberOp floatLiteral |
    COLOR IS color |
    IS (ON | OFF));

textCondition:
    THE TEXT IDENTIFIER
    (interactionCondition |
    CONTENT IS STRING_LITERAL);

videoCondition:
    THE VIDEO IDENTIFIER
    (interactionCondition |
    SOURCE IS STRING_LITERAL |
    VOLUME numberOp floatLiteral |
    MAXVOLUME numberOp floatLiteral |
    DURATION numberOp TIME_LITERAL |
    CURRENTTIME numberOp TIME_LITERAL |
    IS (PLAYING | STOPPED | PAUSED | ENDED));

containerCondition:
    THE object IDENTIFIER  (
        CONTAINS reference |
        OBJECTSCOUNT numberOp DECIMAL_LITERAL |
        CAPACITY numberOp DECIMAL_LITERAL);

collectableCondition:
    THE character IDENTIFIER COLLECTED reference;

counterCondition:
    THE object IDENTIFIER COUNT numberOp floatLiteral;

highlightCondition:
    THE object IDENTIFIER
        (HIGHLIGHT IS (ON | OFF) |
         HIGHLIGHTCOLOR IS color);

keypadCondition:
    THE object IDENTIFIER
        (INPUT IS STRING_LITERAL |
         KEYCODE IS STRING_LITERAL);

lockCondition:
    THE object IDENTIFIER LOCKED IS BOOL_YES_NO;

placeholderCondition:;

particleCondition:
    THE object IDENTIFIER  IS (ON | OFF);

soundCondition:
    THE object IDENTIFIER
    (SOUNDSOURCE IS STRING_LITERAL |
    SOUNDVOLUME numberOp floatLiteral |
    SOUNDMAXVOLUME numberOp floatLiteral |
    SOUNDDURATION numberOp TIME_LITERAL
    SOUNDCURRENTTIME numberOp TIME_LITERAL |
    IS (PLAYING | STOPPED | PAUSED | ENDED));


switchCondition:
    THE object IDENTIFIER IS (ON |OFF);

transitionCondition:
    THE object IDENTIFIER TARGET IS THE SCENE IDENTIFIER;

triggerCondition:;

timerCondition:
    THE object IDENTIFIER (TIMER | DURATION) numberOp TIME_LITERAL;






