using WYF.Bos.DataEntity.Entity;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Entity.property
{
    public  class BillStatusProp: ComboProp
    {

        [CollectionProperty(CollectionItemPropertyType = typeof(StatusItemPro))]
        public List<StatusItemPro> StatusItems {  get; set; }=new List<StatusItemPro>();

        public new bool IsEmptyItems=>StatusItems.Count==0;


        public StatusItemPro? GetStatusItem(string statusKey)
        {
            foreach (var item in this.StatusItems)
            {
                if (item.StatusKey==statusKey) return item;
               
            }
            return null;
        }

        [Serializable]
        public  class StatusItemPro
        {
            [SimpleProperty(IsPrimaryKey =true)]
            public string StatusKey { get; set; }
            [SimpleProperty]
            public string StatusName {  get; set; }
            [SimpleProperty]
            [DefaultValue("")]
            public string OperationerKey { get; set; } = "";
            [SimpleProperty]
            [DefaultValue("")]
            public string OperationDateKey { get; set; } = "";
            [SimpleProperty]
            public bool ClearOperationKey { get; set; }
        }


    }
}
