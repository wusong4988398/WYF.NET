using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Common
{
    public static class SequentialGuid
    {

        private static bool _isok = true;


        public static Guid NewGuid()
        {
            Guid guid;
            if (!_isok)
            {
                return Guid.NewGuid();
            }
            if (UuidCreateSequential(out guid) != 0)
            {
                _isok = false;
                return Guid.NewGuid();
            }
            byte[] buffer = guid.ToByteArray();
            byte[] array = new byte[0x10];
            buffer.CopyTo(array, 0);
            array[0] = buffer[13];
            array[1] = buffer[12];
            array[2] = buffer[11];
            array[3] = buffer[10];
            array[4] = buffer[15];
            array[5] = buffer[14];
            array[6] = buffer[9];
            array[7] = buffer[8];
            array[8] = buffer[7];
            array[9] = buffer[6];
            array[10] = buffer[5];
            array[11] = buffer[4];
            array[12] = buffer[3];
            array[13] = buffer[2];
            array[14] = buffer[1];
            array[15] = buffer[0];
            return new Guid(array);
        }

        public static Guid NewNativeGuid()
        {
            Guid guid;
            if (!_isok)
            {
                return Guid.NewGuid();
            }
            if (UuidCreateSequential(out guid) != 0)
            {
                _isok = false;
                guid = Guid.NewGuid();
            }
            return guid;
        }

        [DllImport("rpcrt4.dll", SetLastError = true)]
        private static extern int UuidCreateSequential(out Guid guid);
    }
}
