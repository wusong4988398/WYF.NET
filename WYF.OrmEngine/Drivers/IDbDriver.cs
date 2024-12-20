﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.DataEntity.Metadata.database;
using WYF.Form.DataEntity;
using WYF.OrmEngine.dataManager;

namespace WYF.OrmEngine.Drivers
{
    public interface IDbDriver
    {

        IOrmTransaction BeginTransaction(IDbTransaction dbTransaction = null);
        void Select(SelectCallback callback, IEnumerable<DbMetadataTable> tablesSchema, DbMetadataTable rootTable, ReadWhere where, OperateOption option = null);
        void UpdateMetadata(DbMetadataDatabase dbMetadata, OperateOption option = null);
        string ConnectionString { get; set; }
    }
}
