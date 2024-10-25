namespace WYF.Form.control.Events
{
    public class ClickEvent : EventObject
    {
        private Dictionary<String, Object> _paramsMap;
        public ClickEvent(object source) : base(source)
        {
            SetSource(source);
        }
        public ClickEvent(Object source, Dictionary<String, Object> paramsDic) : base(source)
        {

            this._paramsMap = paramsDic;
            SetSource(source);
        }

        public Dictionary<String, Object> ParamsDic
        {
            get { return _paramsMap; }
            set { _paramsMap = value; }
        }

        public void SetSource(Object source)
        {
            this.source = source;
        }
    }
}