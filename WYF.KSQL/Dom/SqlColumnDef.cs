using System;
using System.Collections.Generic;
using System.Text;
using WYF.KSQL.Dom.Expr;

namespace WYF.KSQL.Dom
{
    [Serializable]
    public class SqlColumnDef : SqlObject
    {
        // Fields
        public bool allowNull = true;
        public bool autoIncrement;
        public SqlExpr checkExpr;
        private string checkWord;
        public bool clustered;
        private string clusteredWord;
        public string collateName;
        private string collateWord;
        public SqlExpr ComputedColumnExpr;
        public bool ComputedColumnisPersisted;
        private string constraintWord;
        public string containtName;
        public string dataType;
        public SqlExpr defaultValueExpr;
        private string defaultWord;
        private string foreignWord;
        public bool IsComputedColumn;
        public bool isPrimaryKey;
        public bool isUnique;
        public int length;
        public string name;
        private string nullWord;
        private string orgDataTypeWord;
        public int precision;
        private string primaryWord;
        public string refColumnName;
        private string referencesWord;
        public string refTableName;
        public int scale;
        private string uniqueWord;

        // Methods
        public override object Clone()
        {
            SqlColumnDef def = new SqlColumnDef
            {
                name = this.name,
                dataType = this.dataType,
                length = this.length,
                precision = this.precision,
                scale = this.scale,
                allowNull = this.allowNull
            };
            if (this.defaultValueExpr != null)
            {
                def.defaultValueExpr = (SqlExpr)this.defaultValueExpr.Clone();
            }
            def.containtName = this.containtName;
            def.isPrimaryKey = this.isPrimaryKey;
            def.isUnique = this.isUnique;
            def.clustered = this.clustered;
            def.refTableName = this.refTableName;
            def.refColumnName = this.refColumnName;
            if (this.checkExpr != null)
            {
                def.checkExpr = (SqlExpr)this.checkExpr.Clone();
            }
            def.collateName = this.collateName;
            def.autoIncrement = this.autoIncrement;
            def.setOrgDataTypeWord(this.getOrgDataTypeWord());
            def.setCollateWord(this.getCollateWord());
            def.setNullWord(this.getNullWord());
            def.setDefaultWord(this.getDefaultWord());
            def.setConstraintWord(this.getConstraintWord());
            def.setPrimaryWord(this.getPrimaryWord());
            def.setClusteredWord(this.getClusteredWord());
            def.setForeignWord(this.getForeignWord());
            def.setCheckWord(this.getCheckWord());
            return def;
        }

        public string getCheckWord()
        {
            if ((this.checkWord != null) && (this.checkWord.Trim().Length != 0))
            {
                return this.checkWord;
            }
            return "CHECK";
        }

        public string getClusteredWord()
        {
            if ((this.clusteredWord != null) && (this.clusteredWord.Trim().Length != 0))
            {
                return this.clusteredWord;
            }
            if (this.clustered)
            {
                return "CLUSTERED";
            }
            return "NONCLUSTERED";
        }

        public string getCollateWord()
        {
            if ((this.collateWord == null) && (this.collateWord.Trim().Length == 0))
            {
                return "COLLATE";
            }
            return this.collateWord;
        }

        public string getConstraintWord()
        {
            if ((this.constraintWord != null) && (this.constraintWord.Trim().Length != 0))
            {
                return this.constraintWord;
            }
            return "CONSTRAINT";
        }

        public string getDefaultWord()
        {
            if ((this.defaultWord != null) && (this.defaultWord.Trim().Length != 0))
            {
                return this.defaultWord;
            }
            return "DEFAULT";
        }

        public string getForeignWord()
        {
            if ((this.foreignWord != null) && (this.foreignWord.Trim().Length != 0))
            {
                return this.foreignWord;
            }
            return "FOREIGN KEY";
        }

        public string getNullWord()
        {
            if (this.nullWord == null)
            {
                return "";
            }
            return this.nullWord;
        }

        public string getOrgDataTypeWord()
        {
            if ((this.orgDataTypeWord != null) && (this.orgDataTypeWord.Length != 0))
            {
                return this.orgDataTypeWord;
            }
            return this.dataType;
        }

        public string getPrimaryWord()
        {
            if ((this.primaryWord == null) && (this.primaryWord.Trim().Length == 0))
            {
                return "PRIMARY KEY";
            }
            return this.primaryWord;
        }

        public string getReferencesWord()
        {
            if ((this.referencesWord != null) && (this.referencesWord.Trim().Length != 0))
            {
                return this.referencesWord;
            }
            return "REFERENCES";
        }

        public string getUniqueWord()
        {
            if ((this.uniqueWord != null) && (this.uniqueWord.Trim().Length != 0))
            {
                return this.uniqueWord;
            }
            return "UNIQUE";
        }

        public void setCheckWord(string checkWord)
        {
            this.checkWord = checkWord;
        }

        public void setClusteredWord(string clusteredWord)
        {
            this.clusteredWord = clusteredWord;
        }

        public void setCollateWord(string collateWord)
        {
            this.collateWord = collateWord;
        }

        public void setConstraintWord(string constraintWord)
        {
            this.constraintWord = constraintWord;
        }

        public void setDefaultWord(string defaultWord)
        {
            this.defaultWord = defaultWord;
        }

        public void setForeignWord(string foreignWord)
        {
            this.foreignWord = foreignWord;
        }

        public void setNullWord(string nullWord)
        {
            this.nullWord = nullWord;
        }

        public void setOrgDataTypeWord(string orgDataTypeWord)
        {
            this.orgDataTypeWord = orgDataTypeWord;
        }

        public void setPrimaryWord(string primaryWord)
        {
            this.primaryWord = primaryWord;
        }

        public void setReferencesWord(string referencesWord)
        {
            this.referencesWord = referencesWord;
        }

        public void setUniqueWord(string uniqueWord)
        {
            this.uniqueWord = uniqueWord;
        }

        public string toString()
        {
            return this.name;
        }
    }


   



}
