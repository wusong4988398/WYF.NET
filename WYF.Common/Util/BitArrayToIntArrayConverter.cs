using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF
{
    public class BitArrayToIntArrayConverter
    {
        public static int[] ConvertBitArrayToIntArray(BitArray bitArray)
        {
            // 计算需要多少个int来存储这些位
            int lengthInBytes = (bitArray.Length + 7) / 8; // 向上取整到最近的字节
            byte[] byteArray = new byte[lengthInBytes];
            bitArray.CopyTo(byteArray, 0);

            // 将字节数组转换为int数组
            List<int> intList = new List<int>();
            for (int i = 0; i < byteArray.Length; i++)
            {
                intList.Add(byteArray[i]);
            }

            return intList.ToArray();
        }
    }
}
    
