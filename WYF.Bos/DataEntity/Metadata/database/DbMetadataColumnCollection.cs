using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.DataEntity.Metadata.database
{
    public class DbMetadataColumnCollection : DbMetadataCollection<DbMetadataColumn>
    {
        private DbMetadataTable _tableSchema;

        internal DbMetadataColumnCollection(DbMetadataTable tableSchema)
        {
            if (tableSchema == null)
            {
                throw new ArgumentNullException("tableSchema");
            }
            this._tableSchema = tableSchema;
        }
        protected override void ClearItems()
        {
            foreach (DbMetadataColumn column in this)
            {
                column.Table = null;
            }
            base.ClearItems();
        }
        protected override void InsertItem(int index, DbMetadataColumn item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            if (item.Table != null)
            {
                if (item.Table == this._tableSchema)
                {
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "列{0}已经添加到当前表{1}，不要重复添加。", new object[] { item.Name, this._tableSchema.Name }));
                }
                item.Table.Columns.Remove(item);

            }
            base.InsertItem(index, item);
            item.Table = this._tableSchema;
            this.ResetColumnIndex(index);


        }

        private void ResetColumnIndex(int start)
        {
            if (start < 0)
            {
                start = 0;
            }
            for (int i = start; i < base.Count; i++)
            {
                base[i].ColumnIndex = i;
            }
        }

        protected override void RemoveItem(int index)
        {
            DbMetadataColumn column = base[index];
            base.RemoveItem(index);
            column.Table = null;
            this.ResetColumnIndex(index);
        }
        protected override void SetItem(int index, DbMetadataColumn item)
        {
            if (item.Table != null)
            {
                if (item.Table == this._tableSchema)
                {
                    return;
                }
                item.Table.Columns.Remove(item);
            }
            DbMetadataColumn column = base[index];
            column.Table = null;
            base.SetItem(index, item);
            item.Table = this._tableSchema;
            item.ColumnIndex = index;
        }




    }
}
