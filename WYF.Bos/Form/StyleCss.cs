using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Form
{

    /// <summary>
    /// 窗体风格层叠样式(HTML Only)
    /// </summary>
    [Serializable]
    public class StyleCss
    {
      
        public string Width { get; set; }
        public string Height { get; set; }
        public string MarginLeft { get; set; }
        public string MarginRight { get; set; }
    }
}
