using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.DataEntity.Entity;

namespace WYF.Entity.Property
{
    public class PictureProp : FieldProp
    {


        public PictureProp()
        {
            this.CompareGroupID = "2";
        }

        public override string ClientType => "picture";
        public override int DbType => -9;
        public override Type PropertyType => typeof(string);
        public Dictionary<string, object> CreateEntityTreeNode(EntityTreeNode parentEntityTreeNode)
        {
            var col = base.CreateEntityTreeNode(parentEntityTreeNode);
            if (this.Parent?.Name == "bos_user")
            {
                col["Type"] = "AvatarListColumnAp";
                col["ShowStyle"] = 1;
            }
            else
            {
                col["Type"] = "PictureListColumnAp";
                col["ShowStyle"] = 1;
            }
            return col;
        }

        [CollectionPropertyAttribute(Name = "thumbnailsParams", CollectionItemPropertyType = typeof(ThumbnailsParameter))]
        public List<ThumbnailsParameter> ThumbnailsParams { get; set; } = new List<ThumbnailsParameter>();



  
    }
}

