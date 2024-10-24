// See https://aka.ms/new-console-template for more information

using ConsoleApp2;
using WYF;
using WYF.Algo.Sql.parser;
using WYF.Algo.Sql.Tree;
using WYF.OrmEngine.Query.Multi;
using WYF.OrmEngine.Query.OQL.G.expr;

//Console.WriteLine("Hello, World!");
////Expr expr= new SqlParser().ParseExpr("id,number,name");
//SelectFields selectFields= SelectFields.ParseFrom("id,number,name");
//List<PropertyField> propertyFields= selectFields.CreatePropertyFields("bos_user");


//Console.WriteLine("3323");
RequestContext rc = RequestContext.Create();
rc.AccountId = "ws";
rc.UserId = "10001";
Test1.TestMethod1();
//Test1.TestCache();//测试缓存
Console.WriteLine("3323");
