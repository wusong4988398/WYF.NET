using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using WYF.DataEntity;

namespace WYF.Bos.DataEntity
{
    public class SerializationException:OrmException
    {
        
        private SerializationExceptionData _exData;

        
        protected SerializationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this._exData = (SerializationExceptionData)info.GetValue("ExceptionData", typeof(SerializationExceptionData));
        }

        public SerializationException(string code, string message, SerializationExceptionData data) : this(code, message, data, null)
        {
        }

        public SerializationException(string code, string message, SerializationExceptionData data, Exception inner) : base(code, message, inner)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            data.SetIsReadonly();
            this._exData = data;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("CoExceptionDatade", this._exData, typeof(SerializationExceptionData));
        }

        // Properties
        public SerializationExceptionData ExceptionData
        {
            get
            {
                return this._exData;
            }
        }

        public override string Message
        {
            get
            {
                StringBuilder builder = new StringBuilder(base.Message);
                builder.AppendLine();
                builder.AppendFormat("时机：{0}\n", this._exData.OnReading ? "反序列化" : "序列化");
                builder.AppendFormat("位置：行({0}),列({1})\n", this._exData.LineNumber, this._exData.LinePosition);
                builder.AppendFormat("级别：{0}\n", this._exData.CanIgnore ? "警告" : "错误");
                return builder.ToString();
            }
        }


        [Serializable]
        public sealed class SerializationExceptionData
        {
            // Fields
            private bool _canIgnore;
            private bool _isReadonly;
            private int _lineNumber;
            private int _linePosition;
            private bool _onReading;

            // Methods
            internal void SetIsReadonly()
            {
                this._isReadonly = true;
            }

            // Properties
            public bool CanIgnore
            {
                get
                {
                    return this._canIgnore;
                }
                set
                {
                    if (this._isReadonly)
                    {
                        throw new ReadOnlyException();
                    }
                    this._canIgnore = value;
                }
            }

            public int LineNumber
            {
                get
                {
                    return this._lineNumber;
                }
                set
                {
                    if (this._isReadonly)
                    {
                        throw new ReadOnlyException();
                    }
                    this._lineNumber = value;
                }
            }

            public int LinePosition
            {
                get
                {
                    return this._linePosition;
                }
                set
                {
                    if (this._isReadonly)
                    {
                        throw new ReadOnlyException();
                    }
                    this._linePosition = value;
                }
            }

            public bool OnReading
            {
                get
                {
                    return this._onReading;
                }
                set
                {
                    if (this._isReadonly)
                    {
                        throw new ReadOnlyException();
                    }
                    this._onReading = value;
                }
            }
        }
    }


}
