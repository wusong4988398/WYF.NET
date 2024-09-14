using System;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Dom.Stmt
{
    public class StmtType
    {
        // Fields
        public const int AlterDatabase = 30;
        public const int AlterFunction = 0x1f;
        public const int AlterProcedure = 0x20;
        public const int AlterTable = 0x21;
        public const int AlterTrigger = 0x22;
        public const int AlterView = 0x23;
        public const int Call = 0x30;
        public const int CreateDatabase = 20;
        public const int CreateFunction = 0x15;
        public const int CreateIndex = 0x16;
        public const int CreateProcedure = 0x17;
        public const int CreateTalble = 0x18;
        public const int CreateTrigger = 0x19;
        public const int CreateView = 0x1a;
        public const int DeclareCursor = 5;
        public const int DeclareVariant = 4;
        public const int Delete = 1;
        public const int Do = 8;
        public const int DropDatabase = 40;
        public const int DropFunction = 0x29;
        public const int DropIndex = 0x2e;
        public const int DropProcedure = 0x2a;
        public const int DropTable = 0x2b;
        public const int DropTrigger = 0x2c;
        public const int DropView = 0x2d;
        public const int Exec = 6;
        public const int If = 9;
        public const int Insert = 2;
        public const int Merge = 0x31;
        public const int Select = 0;
        public const int Set = 10;
        public const int ShowColumns = 0x2e;
        public const int ShowTables = 0x2f;
        public const int TruncateTable = 12;
        public const int Unknown = 100;
        public const int Update = 3;
        public const int While = 7;

        // Methods
        public static string typename(int type)
        {
            switch (type)
            {
                case 0:
                    return "Select";

                case 1:
                    return "Delete";

                case 2:
                    return "Insert";

                case 3:
                    return "Update";

                case 4:
                    return "DeclareVairnt";

                case 5:
                    return "DeclareCursor";

                case 6:
                    return "Exec";

                case 7:
                    return "While";

                case 8:
                    return "Do";

                case 9:
                    return "If";

                case 10:
                    return "Set";

                case 0x31:
                    return "Merge";
            }
            return "Unkonwn";
        }
    }


   



}
