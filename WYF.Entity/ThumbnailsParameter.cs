using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.DataEntity.Entity;

namespace WYF.Entity
{
    public  class ThumbnailsParameter
    {
        [SimpleProperty(Name = "thumbnailsTag")]
        public string ThumbnailsTag {  get; set; }
        [SimpleProperty(Name = "scale")]
        public string Scale {  get; set; }
        [SimpleProperty(IsPrimaryKey =true)]
        public string Id {  get; set; }
    }
}
