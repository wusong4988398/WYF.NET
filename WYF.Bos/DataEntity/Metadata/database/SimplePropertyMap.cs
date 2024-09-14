using WYF.DataEntity.Entity;
using WYF.Bos.Orm.dataentity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.DataEntity.Metadata.database
{
    public class SimplePropertyMap : PropertyMap<ISimpleProperty>
    {
        public DbMetadataColumn DbColumn { get; set; }

        public bool IsPrimaryKey => _source.IsPrimaryKey;

        public bool IsVersionProperty { get; set; } = false;

        public int DbType => _source.DbType;

        public bool IsEncrypt => _source.IsEncrypt;

        public bool IsEnableNull => _source.IsEnableNull;

        public string TableGroup => _source.TableGroup;

        public AutoSync AutoSync => AutoSync.Never;
    }
}
