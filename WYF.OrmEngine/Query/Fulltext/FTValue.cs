namespace WYF.OrmEngine.Query.Fulltext
{
    public class FTValue
    {
        private FTDataType dataType;

        private Lang lang;
        private object[] values;

        private bool allAnd = true;

        private bool isPinyin = false;

        private float boost = 1.0F;

        private FTValue(Lang lang, FTDataType dataType)
        {
            this.lang = lang;
            this.dataType = dataType;
        }
    }
}