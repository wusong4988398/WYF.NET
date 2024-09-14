grammar G;

// 关键字和符号
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
SLASH: '/';
PERCENT: '%';
QUESTION: '?';
CONCAT: '||';
COMMA: ',';
LPAREN: '(';
RPAREN: ')';
DOT: '.';

// 数值和字符串字面量
STRING:'N'? '\'' (~'\'' | '\'\'')* '\'';
BIGINT_LITERAL: DIGIT+;
SMALLINT_LITERAL: DIGIT+;
TINYINT_LITERAL: DIGIT+;
BYTELENGTH_LITERAL: DIGIT+;
INTEGER_VALUE: DIGIT+;
DECIMAL_VALUE: DIGIT+ ('.' DIGIT*)?;
SCIENTIFIC_DECIMAL_VALUE: DIGIT+ ('.' DIGIT*)? EXPONENT;
DOUBLE_LITERAL: DIGIT+ '.' DIGIT+;
IDENTIFIER: LETTER (LETTER | DIGIT)*;
BACKQUOTED_IDENTIFIER: '`' ~('`')* '`';

// 注释和空白字符
SIMPLE_COMMENT: '--' ~('\n'|'\r')* ('\n'|'\r');
BRACKETED_COMMENT: '/*' .* '*/';
WS: [ \t\r\n]+ -> skip;
UNRECOGNIZED: .;

// 字符集定义
EXPONENT: 'e' | 'E' ('+' | '-')? DIGIT+;
DIGIT: '0'..'9';
LETTER: 'a'..'z' | 'A'..'Z';


singleExpression
    : query EOF
    ;

query
    : queryNoWith queryOrganization?
    ;

queryNoWith
    : queryTerm (UNION queryTerm)*
    ;

queryOrganization
    : ORDER BY sortSet
    | LIMIT INTEGER_VALUE
    ;

queryTerm
    : queryPrimary
    | queryTerm UNION queryPrimary
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
    : SELECT setQuantifier? namedExpressionSeq fromClause? whereClause? groupByClause? havingClause?
    ;

fromClause
    : FROM relation (',' relation)*
    ;


whereClause
    : WHERE booleanExpression
    ;

groupByClause
    : GROUP BY sortSet
    ;

havingClause
    : HAVING booleanExpression
    ;

aggregation
    : expression
    ;

setQuantifier
    : DISTINCT
    ;

relation
    : relationPrimary joinType? joinCriteria?
    ;

joinType
    : INNER JOIN
    | LEFT JOIN
    | RIGHT JOIN
    | FULL JOIN
    | CROSS JOIN
    ;

joinCriteria
    : ON booleanExpression
    ;

relationPrimary
    : tableIdentifier
    ;

tableIdentifier
    : IDENTIFIER
    ;

namedExpression
    : expression (AS IDENTIFIER)?
    ;

namedExpressionSeq
    : namedExpression (',' namedExpression)*
    ;

expression
    : booleanExpression
    ;

booleanExpression
    : booleanExpression OR booleanExpression
    | booleanExpression AND booleanExpression
    | NOT booleanExpression
    | predicated
    ;

predicated
    : predicate
    ;

predicate
    : valueExpression comparisonOperator valueExpression
    | valueExpression IS NULL
    | valueExpression IS NOT NULL
    | valueExpression BETWEEN valueExpression AND valueExpression
    | valueExpression IN '(' valueExpression (',' valueExpression)* ')'
    | valueExpression LIKE valueExpression
    ;

valueExpression
    : primaryExpression
    ;

primaryExpression
    : constant
    | identifier
    ;

constant
    : STRING
    | number
    | booleanValue
    ;

comparisonOperator
    : EQ
    | NEQ
    | NEQJ
    | LT
    | LTE
    | GT
    | GTE
    ;

booleanValue
    : TRUE
    | FALSE
    ;

dataType
    : IDENTIFIER
    ;

whenClause
    : WHEN booleanExpression THEN expression
    ;

qualifiedName
    : identifier ('.' identifier)*
    ;

identifier
    : strictIdentifier
    ;

strictIdentifier
    : IDENTIFIER
    | BACKQUOTED_IDENTIFIER
    ;

quotedIdentifier
    : BACKQUOTED_IDENTIFIER
    ;

number
    : BIGINT_LITERAL
    | SMALLINT_LITERAL
    | TINYINT_LITERAL
    | INTEGER_VALUE
    | DECIMAL_VALUE
    | DOUBLE_LITERAL
    ;

nonReserved
    : NULL
    | ASC
    | DESC
    | LIMIT
    | AS
    | BETWEEN
    | BY
    | FALSE
    | GROUP
    | IN
    | IS
    | LIKE
    | ORDER
    | OUTER
    | TRUE
    | AND
    | CASE
    | CAST
    | DISTINCT
    | ELSE
    | END
    | OR
    | THEN
    | WHEN
    | SELECT
    | FROM
    | WHERE
    | HAVING
    | NOT
    ;