using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.DataEntity.Entity
{
    public class ComplexPropertyAttribute : Attribute
    {
        //属性的名称
        public string Name { get; set; } = "";

        public string RefIdPropertyName { get; set; } = "";
    }
}
