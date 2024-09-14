using System;
using System.IO;

namespace WYF.SqlParser
{
 

    public abstract class IntegralType : NumericType
    {
        private const long SerialVersionUID = 1402034741679856504L;

        protected IntegralType(int ordinal, string name) : base(ordinal, name)
        {
        }

        public static IntegralType ComputeCompatibleUp(NumericType left, NumericType right)
        {
            NumericType type = left.GetCompatibleLevel() >= right.GetCompatibleLevel() ? right : left;
            if (type.GetCompatibleLevel() > DataType.LongType.GetCompatibleLevel())
            {
                return DataType.LongType;
            }
            return (IntegralType)type;
        }

        public static IntegralType ComputeCompatibleDown(NumericType left, NumericType right)
        {
            NumericType type = left.GetCompatibleLevel() >= right.GetCompatibleLevel() ? left : right;
            if (type.GetCompatibleLevel() > DataType.LongType.GetCompatibleLevel())
            {
                return DataType.LongType;
            }
            return (IntegralType)type;
        }
    }
}