using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.DataEntity.Serialization
{
    internal sealed class DcAction
    {


        private readonly int _actionId;
        private readonly string _actionName;



        public static readonly DcAction PropertyAction_SetValue = new DcAction(1, "setvalue");


        public static readonly DcAction PropertyAction_Reset = new DcAction(2, "reset");


        public static readonly DcAction PropertyAction_SetNull = new DcAction(3, "setnull");

        public static readonly DcAction ListAction_Add = new DcAction(4, "add");

        public static readonly DcAction ListAction_Edit = new DcAction(5, "edit");


        public static readonly DcAction ListAction_Remove = new DcAction(6, "remove");


        public static readonly DcAction ListAction_Clear = new DcAction(7, "clear");

        private DcAction(int actionId, string actionName)
        {
            this._actionId = actionId;
            this._actionName = actionName;
        }


        public string ActionName
        {
            get
            {
                return this._actionName;
            }
        }

        public override bool Equals(object obj)
        {
            DcAction dcJsonAction = obj as DcAction;
            return dcJsonAction != null && this._actionId == dcJsonAction._actionId;
        }


        public bool Equals(DcAction obj)
        {
            return obj != null && this._actionId == obj._actionId;
        }

        public static bool Equals(DcAction a, DcAction b)
        {
            return (a == null && b == null) || (a != null && b != null && a._actionId == b._actionId);
        }


        public override int GetHashCode()
        {
            return this._actionId;
        }

        public override string ToString()
        {
            return this._actionName;
        }

       
    }
}
