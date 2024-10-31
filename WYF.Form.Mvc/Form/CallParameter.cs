namespace WYF.Form.Mvc.Form
{
    [Serializable]
    public class CallParameter
    {
        private String Key { get; set; }

        private String Methodname { get; set; }

        private Object[] Args { get; set; }
    }
}