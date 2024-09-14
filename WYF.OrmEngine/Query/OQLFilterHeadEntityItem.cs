namespace WYF.OrmEngine.Query
{
    [Serializable]
    public class OQLFilterHeadEntityItem : OQLFilterItem
    {
        /// <summary>
        /// 主实体过滤条件
        /// </summary>

        public override string EntityKey
        {
            get
            {
                return "FBillHead";
            }
            set
            {
                base.EntityKey = "FBillHead";
            }
        }
    }
}