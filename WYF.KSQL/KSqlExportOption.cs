using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace WYF.KSQL
{
    [StructLayout(LayoutKind.Sequential)]
    public struct KSqlExportOption
    {
        public int SourceDBType;
        public int TargetDBType;
        public bool CheckTable;
        public bool CheckUnique;
        public bool CheckPrimaryKey;
        public bool CheckForeignKey;
        public bool CheckCheckRule;
        public bool CheckIndex;
        public bool CheckData;
        public bool ExpTableSql;
        public bool ExpUniqueSql;
        public bool ExpPrimaryKeySql;
        public bool ExpForeignKeySql;
        public bool ExpCheckSql;
        public bool ExpIndexSql;
        public bool ExpDataSql;
        public string ExpTableFileName;
        public string ExpConstraintFileName;
        public string ExpDataFileName;
        public KSqlExportOption(int srcDBType, int destDBType = 0)
        {
            this.SourceDBType = srcDBType;
            if (destDBType == 0)
            {
                this.TargetDBType = this.SourceDBType;
            }
            else
            {
                this.TargetDBType = destDBType;
            }
            this.CheckTable = true;
            this.CheckUnique = true;
            this.CheckPrimaryKey = true;
            this.CheckForeignKey = true;
            this.CheckCheckRule = true;
            this.CheckIndex = true;
            this.CheckData = true;
            this.ExpTableSql = true;
            this.ExpUniqueSql = true;
            this.ExpPrimaryKeySql = true;
            this.ExpForeignKeySql = true;
            this.ExpCheckSql = true;
            this.ExpIndexSql = true;
            this.ExpDataSql = true;
            this.ExpTableFileName = "";
            this.ExpConstraintFileName = "";
            this.ExpDataFileName = "";
        }
    }


   



}
