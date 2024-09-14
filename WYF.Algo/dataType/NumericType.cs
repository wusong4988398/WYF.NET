using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Algo.dataType
{
    public class NumericType : DataType
    {
        public static readonly NumericType Instance = new NumericType(200, "Numeric");
        private const long SerialVersionUID = -1352757987136279685L;

        protected NumericType(int ordinal, string name) : base(ordinal, name)
        {
        }

        public override int GetFixedSize()
        {
            return 0;
        }

        public override bool AcceptsType(DataType other)
        {
            if (other is NullType || other is AnyType || other is UnknownType)
            {
                return true;
            }
            return other is NumericType;
        }

        public static NumericType ComputeCompatibleDown(NumericType left, NumericType right)
        {
            if (left == null)
            {
                return right;
            }
            if (right == null)
            {
                return left;
            }
            return left.GetCompatibleLevel() >= right.GetCompatibleLevel() ? left : right;
        }

        public override Type GetJavaType()
        {
            return typeof(IConvertible);
        }

        public override bool IsAbstract()
        {
            return true;
        }

        public virtual int GetCompatibleLevel()
        {
            return 0;
        }

        public override void Write(object value, BinaryWriter output)
        {
            throw new NotSupportedException();
        }

        public override object Read(BinaryReader input)
        {
            throw new NotSupportedException();
        }

        public override int GetSqlType()
        {
            return 2;
        }


    }
}
