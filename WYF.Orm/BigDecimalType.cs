using System.IO;
using System;
using System.Numerics;

namespace WYF.SqlParser
{
    [Serializable]
    public class BigDecimalType : FractionalType
    {
        private static readonly long serialVersionUID = 4626196577863060845L;
        private int precision;
        private int scale;

        public BigDecimalType() : base(5, "Decimal")
        {
            this.precision = 23;
            this.scale = 10;
        }

        public override int GetFixedSize()
        {
            return 0; // BigDecimal does not have a fixed binary size
        }

        public override int GuessHeapSize(object value)
        {
            if (value == null)
                return 4;
            return 50; // Arbitrary large size for heap estimation
        }

        public int GetPrecision() => this.precision;
        public int GetScale() => this.scale;

        public override Type GetJavaType()
        {
            return typeof(decimal); // Closest equivalent in C# to BigDecimal
        }

        public override int GetCompatibleLevel()
        {
            return 4;
        }

        public override void Write(object value, BinaryWriter target)
        {
            if (value == null)
            {
                WriteBigInteger(null, target);
                return;
            }

            decimal dec = (decimal)value;
            int[] bits = decimal.GetBits(dec);
            BigInteger unscaledValue = new BigInteger(bits[0] + (bits[1] << 32) + ((long)bits[2] << 64));
            if (dec == decimal.Zero)
            {
                WriteBigInteger(BigInteger.Zero, target);
                target.Write(0);
                return;
            }
            WriteBigInteger(unscaledValue, target);
            target.Write(bits[3] >> 16); // scale
        }

        public override object Read(BinaryReader source)
        {
            BigInteger unscaledValue = ReadBigInteger(source);
            if (unscaledValue == null)
            {
                return null;
            }
            int scale = source.ReadInt32();
            bool isNegative = unscaledValue.Sign < 0;
            unscaledValue = BigInteger.Abs(unscaledValue);

            // Extract the low, mid, and high parts of the BigInteger for the decimal constructor
            byte[] bytes = unscaledValue.ToByteArray();
            if (bytes.Length > 13) // Decimal can only handle up to 96 bits, which are 12 bytes, plus sign
                throw new OverflowException("BigInteger too large to fit a decimal");

            int lo = 0, mid = 0, hi = 0;
            for (int i = 0; i < bytes.Length; i++)
            {
                if (i < 4) lo |= bytes[i] << (8 * i);
                else if (i < 8) mid |= bytes[i] << (8 * (i - 4));
                else if (i < 12) hi |= bytes[i] << (8 * (i - 8));
            }

            return new decimal(lo, mid, hi, isNegative, (byte)scale);
        }

        public static void WriteBigInteger(BigInteger? value, BinaryWriter target)
        {
            if (value == null)
            {
                target.Write(0);
                return;
            }
            byte[] bytes = value.Value.ToByteArray();
            target.Write(bytes.Length);
            target.Write(bytes);
        }

        public static BigInteger ReadBigInteger(BinaryReader source)
        {
            int len = source.ReadInt32();
            if (len == 0)
                return BigInteger.Zero;
            byte[] bytes = new byte[len];
            source.Read(bytes, 0, len);
            return new BigInteger(bytes);
        }
    }
}