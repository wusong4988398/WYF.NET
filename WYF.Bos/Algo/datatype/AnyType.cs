
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace WYF.Bos.algo.datatype
{
    public class AnyType : DataType
    {
        public static  AnyType Instance = new AnyType();

        private AnyType():base(100, "Any")
        {
         
        }

        public override bool AcceptsType(DataType paramDataType)
        {
            return true;
        }

        public override Type GetCsharpType()
        {
            return this.GetType();
        }

        public override int GetFixedSize()
        {
            return -1;
        }

        public override int GetSqlType()
        {
            return 1111;
        }

        public override object Read(BinaryReader input)
        {
            return input.ReadString();


        }

        //public override void Write(object value, BinaryWriter output)
        //{
        //    BinaryFormatter formatter = new BinaryFormatter();
        //    MemoryStream stream = new MemoryStream();
        //    formatter.Serialize(stream, value);
        //    byte[] bytes = stream.ToArray();
        //    output.Write(bytes, 0, bytes.Length);
        //}


        public override void Write(object value, BinaryWriter output)
        {
            try
            {
                // 使用默认的 Json 选项
                var jsonOptions = new JsonSerializerOptions
                {
                    WriteIndented = false, // 不需要缩进，提高序列化效率
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping // 不对特殊字符进行转义
                };
                // 写入 JSON 长度
                using (var ms = new MemoryStream())
                {
                    JsonSerializer.Serialize(ms, value, value.GetType(), jsonOptions);
                    ms.Position = 0; // 回到流的开始
                    var jsonLength = (int)ms.Length;
                    output.Write(jsonLength);

                    // 将序列化的 JSON 数据写入输出流
                    ms.CopyTo(output.BaseStream);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("序列化失败", ex);
            }
        }

    }
}
