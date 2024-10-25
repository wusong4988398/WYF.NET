namespace WYF.Form.control.Events
{
    public class BeforeItemClickEvent : ItemClickEvent
    {
        public bool IsCancel { get; set; }

        public BeforeItemClickEvent(object source, Dictionary<string, object> paramsMap) : base(source, paramsMap)
        {
        }

        public BeforeItemClickEvent(object source, string itemKey, string operationKey) : base(source, itemKey, operationKey)
        {
        }
    }
}