using WYF.DataEntity.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.DataEntity.Metadata.database
{
    public class DbMetadataColumn: DbMetadataBase
    {
        private Type _clrType;
        private DbMetadataTable _table;

        public DbMetadataColumn() { }

        public DbMetadataColumn(string name):base(name) 
        {
            
        }

        public DbMetadataTable Table
        {
            get
            {
                return this._table;
            }
            internal set
            {
                this._table = value;
            }
        }


        public Type ClrType
        {
            get
            {
                return this._clrType;
            }
            set
            {
                this._clrType = value;
                this.SetDefaultValue(this._clrType);
            }
        }
        public bool Encrypt { get; set; }
        public AutoSync AutoSync { get; set; }
        public object DefaultValue { get; set; }
        public bool IsEncrypt { get; set; }
        public bool IsEnableNull { get; set; }
        public int Size { get; set; }
        public byte Scale { get; set; }

        public int DbType { get; set; }

        public bool IsNullable { get; set; }

        public string TableGroup { get; set; }

        public int ColumnIndex { get; set; }

        private void SetDefaultValue(Type clrType)
        {
            if (clrType.Equals(typeof(string)))
            {
                this.DefaultValue = " ";
            }
            else if (clrType.Equals(typeof(byte[])))
            {
                this.DefaultValue = null;
            }
            else
            {
                this.DefaultValue = Activator.CreateInstance(clrType);
            }
        }


        public DbMetadataColumn Clone(string newName)
        {
            return new DbMetadataColumn(newName) { ClrType = this.ClrType, Size = this.Size, Scale = this.Scale, DbType = this.DbType, IsNullable = this.IsNullable, AutoSync = this.AutoSync, TableGroup = this.TableGroup };
        }

    }
}
