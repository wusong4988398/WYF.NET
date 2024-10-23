using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.DataEntity.Metadata.database;
using WYF.DbEngine;
using WYF.OrmEngine.dataManager;

namespace WYF.OrmEngine.Drivers.Sql
{
    internal class KSql4SQLDataReader : OrmDataReaderBase
    {

        private Context _ctx;
        private IEnumerator<DbMetadataTable> _tablesSchema;
        public KSql4SQLDataReader(IList<StringBuilder> selectSqls, Context ctx, ReadWhere where, IEnumerable<DbMetadataTable> tablesSchema, DbMetadataTable rootTable)
        {
            StringBuilder builder = new StringBuilder(selectSqls.Sum<StringBuilder>((Func<StringBuilder, int>)(p => p.Length)));
            foreach (StringBuilder builder2 in selectSqls)
            {
                builder.Append(builder2);
                builder.AppendLine();
            }
            List<SqlParam> paramList = new List<SqlParam>();
            if (where.IsSingleValue)
            {
                paramList.Add(new SqlParam("@PKValue", (KDbType)rootTable.PrimaryKey.DbType, where.ReadOids[0]));
            }
            else if ((where.ReadOids != null) && builder.ToString().Contains("@PKValue"))
            {
                object[] objArray = where.ReadOids.Distinct<object>().ToArray<object>();
                object obj2 = OrmDataReaderBase.ToArray((DbType)rootTable.PrimaryKey.DbType, objArray);
                paramList.Add(SqlParam.CreateUdtParamter("@PKValue", (KDbType)rootTable.PrimaryKey.DbType, obj2));
            }
            else if (where.SqlParams != null)
            {
                paramList.AddRange(where.SqlParams.Cast<SqlParam>());
            }
            this.initi(DBUtils.ExecuteReader(ctx, builder.ToString(), paramList), tablesSchema, ctx);
        }

        protected override Func<object, object> GetConverter(DbMetadataColumn col)
        {
            switch ((DbType)col.DbType)
            {
                case DbType.Int16:
                    if (!(col.ClrType == typeof(bool)))
                    {
                        return new Func<object, object>(OrmDataReaderBase.ToInt16);
                    }
                    return new Func<object, object>(OLEDbDriver.CharToBool);

                case DbType.Int32:
                    if (!(col.ClrType == typeof(bool)))
                    {
                        return new Func<object, object>(OrmDataReaderBase.ToInt32);
                    }
                    return new Func<object, object>(OLEDbDriver.CharToBool);

                case DbType.Int64:
                    return new Func<object, object>(OrmDataReaderBase.ToInt64);

                case DbType.StringFixedLength:
                    if ((col.ClrType == typeof(char)) || (Nullable.GetUnderlyingType(col.ClrType) == typeof(char)))
                    {
                        return new Func<object, object>(OrmDataReaderBase.ToChar);
                    }
                    if (col.ClrType == typeof(bool))
                    {
                        return new Func<object, object>(OLEDbDriver.CharToBool);
                    }
                    if (Nullable.GetUnderlyingType(col.ClrType) == typeof(bool))
                    {
                        return new Func<object, object>(OLEDbDriver.CharToNullBool);
                    }
                    return base.GetConverter(col);
            }
            return base.GetConverter(col);
        }

        private void initi(IDataReader sqlDataReader, IEnumerable<DbMetadataTable> tablesSchema, Context ctx)
        {
            this._ctx = ctx;
            this._tablesSchema = tablesSchema.GetEnumerator();
            this._tablesSchema.MoveNext();
            DbMetadataTable current = this._tablesSchema.Current;
            base.ChangeParent(sqlDataReader, current);
        }

        public override bool NextResult()
        {
            if (base.NextResult())
            {
                this._tablesSchema.MoveNext();
                DbMetadataTable current = this._tablesSchema.Current;
                base.ChangeParent(base.Parent, current);
                return true;
            }
            return false;
        }
    }
}
