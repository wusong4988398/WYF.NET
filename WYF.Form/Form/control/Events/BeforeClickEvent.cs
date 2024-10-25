namespace WYF.Form.control.Events
{
    public class BeforeClickEvent : ClickEvent
    {
        public bool IsCancel { get; set; } = false;

        public BeforeClickEvent(object source) : base(source)
        {
        }
        public BeforeClickEvent(Object source, Dictionary<String, Object> paramsDic) : base(source, paramsDic)
        {
        }


    }
}