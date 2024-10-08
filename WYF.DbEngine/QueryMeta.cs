using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.Algo;
using static IronPython.Modules.PythonCsvModule;

namespace WYF.DbEngine
{
    public class QueryMeta
    {
        public RowMeta RowMeta {  get; set; }

        internal static QueryMeta CreateOrFixQueryMeta(QueryMeta qm, IDataReader reader, DatabaseType databaseType)
        {
            if (qm == null)
            {
                int col = reader.FieldCount;
                Field[] fs = new Field[col];
                for (int i = 0; i < col; i++)
                {
                    string typeName = reader.GetFieldType(i).FullName;
                    Type typeCls = Type.GetType(typeName);
                    fs[i] = new Field(reader.GetName(i), DataSetDataType.GetDataType(typeCls));
                }
                qm = new QueryMeta();
                qm.RowMeta = new RowMeta(fs);
            }
            else
            {
                int i = 0;
                foreach (Field f in qm.RowMeta.Fields)
                {
                    i++;
                    if (f.DataType == DataType.UnknownType)
                        f.DataType = DataSetDataType.GetDataType(Type.GetType(reader.GetFieldType(i - 1).FullName));
                }
            }

            return qm;
        }
    }
}
