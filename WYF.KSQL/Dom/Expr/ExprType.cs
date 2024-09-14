using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.KSQL.Dom.Expr
{
    public static class ExprType
    {
       
        public static string typename(int type)
        {
            switch (type)
            {
                case -1:
                    return "Default";
                case 0:
                    return "BinaryOp";
                case 1:
                    return "Int";
                case 2:
                    return "Double";
                case 3:
                    return "Variant";
                case 4:
                    return "Identifier";
                case 5:
                    return "Char";
                case 6:
                    return "NChar";
                case 7:
                    return "FunctionInvoke";
                case 8:
                    return "AllColumn";
                case 9:
                    return "Unary";
                case 10:
                    return "MethodInvoke";
                case 11:
                    return "Aggregate";
                default:
                    switch (type)
                    {
                        case 27:
                            return "Long";
                        case 28:
                            return "Identity";
                        case 30:
                            return "XmlType";
                    }
                    return "";
            }
        }


        public const int BinaryOp = 0;


        public const int Int = 1;

   
        public const int Double = 2;

   
        public const int Variant = 3;

      
        public const int Identifier = 4;

      
        public const int Char = 5;

   
        public const int NChar = 6;

      
        public const int FunctionInvoke = 7;

       
        public const int AllColumn = 8;

   
        public const int Unary = 9;

    
        public const int MethodInvoke = 10;

     
        public const int Aggregate = 11;

     
        public const int Case = 12;

       
        public const int InSubQuery = 13;

  
        public const int InList = 14;

   
        public const int Exists = 15;

   
        public const int All = 16;

      
        public const int Between = 17;

    
        public const int Any = 18;

       
        public const int Some = 19;

     
        public const int Null = 20;

   
        public const int DateTime = 21;

     
        public const int DeclareVariant = 22;

   
        public const int ObjectCreate = 23;

     
        public const int Subquery = 24;


        public const int PriorIdentifier = 25;

    
        public const int JavaObjectValue = 26;

 
        public const int Long = 27;

     
        public const int Identity = 28;

    
        public const int Empty = 29;

  
        public const int XmlType = 30;

       
        public const int Default = -1;
    }
}
