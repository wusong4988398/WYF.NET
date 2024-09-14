grammar G;

singleExpression 
: namedExpressionSeq
;
    //: namedExpressionSeq  
make_token:'9';

query
    : queryNoWith
    ;

queryNoWith
    : queryTerm queryOrganization
    ;

queryOrganization
    : (ORDER BY sortItem (',' sortItem)*)?
      (LIMIT INTEGER_VALUE (',' INTEGER_VALUE)*)?
    ;

queryTerm
    : queryPrimary
    | queryTerm UNION setQuantifier? queryTerm
    ;

queryPrimary
    : querySpecification
    ;

sortSet
    : sortItem (',' sortItem)*
    ;

sortItem
    : expression (ASC | DESC)?
    ;

querySpecification
    : SELECT setQuantifier? namedExpressionSeq
      fromClause?
      (WHERE booleanExpression)?
      (GROUP BY expression (',' expression)*)?
      (HAVING booleanExpression)?
    ;

fromClause
    : FROM relation (',' relation)*
    ;

aggregation
    : GROUP BY expression (',' expression)*
    ;

setQuantifier
    : DISTINCT
    ;

relation
    : relationPrimary
    | relation joinType JOIN relation joinCriteria?
    | relation CROSS JOIN relation
    ;

joinType
    : INNER?
    | LEFT OUTER?
    | RIGHT OUTER?
    | FULL OUTER?
    ;

joinCriteria
    : ON booleanExpression
    ;

relationPrimary
    : tableIdentifier (AS? identifier)?
    ;

tableIdentifier
    : (identifier '.')? identifier
    ;

   namedExpression
    : expression ( AS? ( qualifiedName | identifier ) )?
    ;
 

namedExpressionSeq
    : namedExpression (',' namedExpression)*
    ;

expression
    : booleanExpression
    ;

booleanExpression
    : booleanDefault 
    //| logicalNot
    //| booleanExpression AND booleanExpression | booleanExpression OR booleanExpression
    ;

logicalNot
    : NOT booleanExpression
    ;

booleanDefault
    : predicated
    ;

logicalBinary
    : booleanExpression AND booleanExpression | booleanExpression OR booleanExpression
    
    ;

predicated
    : valueExpression
    //| predicate
    ;

predicate
    : NOT? IN '(' expression (',' expression)* ')'
    | NOT? IN expression
    | NOT? LIKE valueExpression
    | IS NOT? NULL
    ;

valueExpression
    : valueExpressionDefault
    //| arithmeticUnaryOperator valueExpression
    //| valueExpression arithmeticBinaryOperator valueExpression
    //| valueExpression STRING_ADD valueExpression
    //| valueExpression comparisonOperator valueExpression
    ;

 valueExpressionDefault
    : primaryExpression
    ;

arithmeticUnary
    : arithmeticUnaryOperator valueExpression
    ;

arithmeticBinary
    : valueExpression arithmeticBinaryOperator valueExpression
    ;

arithmeticUnaryOperator
    : PLUS
    | MINUS
    ;

arithmeticBinaryOperator
    : ASTERISK
    | DIVIDE
    | PERCENT
    | PLUS
    | MINUS
    ;

stringAdd
    : valueExpression STRING_ADD valueExpression
    ;

comparison
    : valueExpression comparisonOperator valueExpression
    ;



primaryExpression
    : columnReference 
    //| constantDefault
    //| searchedCase
    //| cast
    //| simpleCase
    //| tuple
    //| parenthesizedExpression
    //| star
    //| question
    //| primaryExpression DOT identifier
    //| functionCall
    ;

    dereference
    : primaryExpression DOT identifier
    ;

constantDefault
    : constant
    ;

star
    : ASTERISK
    ;

qualifiedName
    : ID (DOT ID)*
    ;

question
    : QUESTION
    ;

tuple
    : LPAREN expression (COMMA expression)* RPAREN
    ;

functionCall
    : qualifiedName (LPAREN)? (expression (COMMA expression)*)? (RPAREN)?
    ;

simpleCase
    : CASE valueExpression (whenClause)+ (ELSE expression)? END
    ;

searchedCase
    : CASE (whenClause)+ (ELSE expression)? END
    ;

cast
    : CAST LPAREN expression AS dataType RPAREN
    ;

columnReference
    : identifier
    ;

parenthesizedExpression
    : LPAREN expression RPAREN
    ;





constant
    : NULL
    | identifier STRING
    | number
    | booleanValue
    | STRING+
    ;

comparisonOperator
    : '=' | '!=' | '<>' | '<' | '<=' | '>' | '>='
    ;

booleanValue
    : TRUE | FALSE
    ;

dataType
    : identifier ('(' INTEGER_VALUE (',' INTEGER_VALUE)* ')')?
    ;

whenClause
    : WHEN expression THEN expression
    ;



identifier
    : strictIdentifier
    //| FULL | INNER | LEFT | RIGHT | JOIN | CROSS | ON | UNION
    ;

strictIdentifier
    :IDENTIFIER
    //| quotedIdentifier
    //| nonReserved
    ;

IDENTIFIER : [a-zA-Z_] [a-zA-Z_0-9]* ;

quotedIdentifier
    : BACKQUOTED_IDENTIFIER
    ;

number
    : DECIMAL_VALUE
    | SCIENTIFIC_DECIMAL_VALUE  
    | INTEGER_VALUE
    | BIGINT_LITERAL
    | SMALLINT_LITERAL
    | TINYINT_LITERAL
    | DOUBLE_LITERAL
    ;

nonReserved
    : SELECT | FROM | AS | WHERE | GROUP | BY | ORDER | HAVING | LIMIT
    | CASE | WHEN | THEN | ELSE | END | CAST | UNION
    ;
STRING_LITERAL
 : '\'' ( ~'\'' | '\'\'' )* '\''
 ;
SELECT: 'SELECT';
FROM: 'FROM';
AS: 'AS';
DISTINCT: 'DISTINCT';
WHERE: 'WHERE';
GROUP: 'GROUP';
BY: 'BY';
ORDER: 'ORDER';
HAVING: 'HAVING';
LIMIT: 'LIMIT';
OR: 'OR';
AND: 'AND';
IN: 'IN';
NOT: 'NOT';
BETWEEN: 'BETWEEN';
LIKE: 'LIKE';
IS: 'IS';
NULL: 'NULL';
TRUE: 'TRUE';
FALSE: 'FALSE';
ASC: 'ASC';
DESC: 'DESC';
CASE: 'CASE';
WHEN: 'WHEN';
THEN: 'THEN';
ELSE: 'ELSE';
END: 'END';
JOIN: 'JOIN';
CROSS: 'CROSS';
OUTER: 'OUTER';
INNER: 'INNER';
LEFT: 'LEFT';
RIGHT: 'RIGHT';
FULL: 'FULL';
ON: 'ON';
CAST: 'CAST';
UNION: 'UNION';

EQ: '=';
NSEQ: '<=>';
NEQ: '<>';
NEQJ: '!=';
LT: '<';
LTE: '<=';
GT: '>';
GTE: '>=';
PLUS: '+';
MINUS: '-';
ASTERISK: '*';
DIVIDE: '/';
PERCENT: '%';
QUESTION: '?';
CONCAT: '||';
LPAREN: '(';
RPAREN: ')';
COMMA: ',';
DOT: '.';
STRING_ADD : '+';
ID : [a-zA-Z_][a-zA-Z_0-9]*;
STRING: '\'' (~'\'' | '\'\'')* '\'';
SMALLINT_LITERAL: DIGIT+ 'S';
TINYINT_LITERAL: DIGIT+ 'Y';
INTEGER_VALUE: DIGIT+;
DECIMAL_VALUE: DIGIT+ '.' DIGIT* | '.' DIGIT+;
SCIENTIFIC_DECIMAL_VALUE: DECIMAL_VALUE 'E' [+-]? DIGIT+;
DOUBLE_LITERAL: DIGIT+ ('.' DIGIT*)? EXPONENT? 'D';

BACKQUOTED_IDENTIFIER: '`' ~'`'* '`';

SIMPLE_COMMENT: '--' ~[\r\n]* -> channel(HIDDEN);
BRACKETED_COMMENT: '/*' .*? '*/' -> channel(HIDDEN);
WS: [ \t\r\n]+ -> skip; // Skip whitespace

fragment DIGIT: [0-9];
fragment EXPONENT: 'E' [+-]? DIGIT+;