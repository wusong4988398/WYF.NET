using WYF.DataEntity.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Orm.query.multi
{
    public class PropertyField
    {

        private static readonly int MaxLength = 100;
        public string FullObjectName { get; set; }

        public string Name { get; set; }

        public string Field { get; set; }

        public string Alias { get; set; }

        public PropertySegExpress PropertySegExpress { get; set; }

        public bool MultiLangProperty { get; set; }

        public string EntityAlias { get; set; }

        public IDataEntityType EntityType { get; set; }

        public IDataEntityProperty PeropertyType { get; set; }

        public EntityItemProperty PropertyItem { get; set; }

        public bool IsExpress
        {
            get {



                return (this.PropertySegExpress != null && this.PropertySegExpress.IsExpress());

            }
        }


        public bool IsInnerField
        {
            get
            {
                return (this.Alias != null && this.Alias.StartsWith("INNER_"));
            }
        }



        public PropertyField(string peropertyExpress)
        {
            peropertyExpress = peropertyExpress.Trim();
            int blank = peropertyExpress.IndexOf(' ');
            if (blank == -1)
            {
                this.Name = peropertyExpress;
            }
            else
            {
                this.Name = peropertyExpress.Substring(0, blank);
                this.Alias = peropertyExpress.Substring(blank + 1).Trim();
                if (this.Alias.ToLower().StartsWith("as "))
                    this.Alias = this.Alias.Substring(3).Trim();
            }
            int dot = this.Name.LastIndexOf('.');
            if (dot != -1)
            {
                this.FullObjectName = this.Name.Substring(0, dot).ToLower();
                this.Name = this.Name.Substring(dot + 1);
            }
            Init();
        }
        public PropertyField(string fullObjectName, string name, string alias)
        {
            this.FullObjectName = (string.IsNullOrEmpty(fullObjectName)) ? "" : fullObjectName.ToLower();
            this.Name = name;
            this.Alias = alias;
            Init();
        }
        public bool IsSameWith(PropertyField sf)
        {
            if (sf.Name==this.Name&&sf.FullObjectName==this.FullObjectName)
                return true;
            return false;
        
        }

        public string ToSelectField(bool withAlias, QContext ctx)
        {
            if ("__NULL_FIELD".Equals(this.Field))
            {
                if (withAlias)
                    return "NULL " + GenShortAlias(this.Name);
                return "NULL";
            }
            StringBuilder selectField = new StringBuilder(64);
            if (IsExpress)
            {
                String rootObjName;
                int dot = this.FullObjectName.IndexOf('.');
                if (dot == -1)
                {
                    rootObjName = this.FullObjectName;
                }
                else
                {
                    rootObjName = this.FullObjectName.Substring(0, dot);
                }
                selectField.Append(this.PropertySegExpress.ToString(rootObjName, ctx));
            }
            else
            {
                selectField.Append(ctx.GetSimpleEntityAlias(this.EntityAlias)).Append('.').Append(this.Field);
                if (this.PeropertyType is ISimpleProperty && ((ISimpleProperty)this.PeropertyType).IsEncrypt)
                selectField.Append("_enp");
            }
            if (this.MultiLangProperty)
            {
                IDataEntityType mainDT = this.PeropertyType.Parent.Parent;
                if (mainDT != null)
                {
                    ISimpleProperty mainProperty = (ISimpleProperty)mainDT.Properties[this.Name];
                    if (mainProperty != null && !mainProperty.IsDbIgnore && !this.PeropertyType.IsDbIgnore)
                    {
                        String parentEntityAlias = this.FullObjectName;
                        String glField = ctx.GetSimpleEntityAlias(parentEntityAlias) + '.' + this.PeropertyType.Alias;
                        StringBuilder tempSb = new StringBuilder();
                        tempSb.Append("CASE WHEN ").Append(selectField).Append(" IS NULL THEN ").Append(glField)
                          .Append(" WHEN ").Append(selectField).Append(" = '' THEN ").Append(glField).Append(" WHEN ")
                          .Append(selectField).Append(" = ' ' THEN ").Append(glField).Append(" ELSE ")
                          .Append(selectField).Append(" END");
                        selectField = tempSb;
                    }
                }
            }
            if (withAlias)
                selectField.Append(GenShortAlias(selectField.ToString()));
            return selectField.ToString();
        }


        private string GenShortAlias(string selectField)
        {
            if (this.Alias.Length <= MaxLength)
                return " \"" + this.Alias + '"';
            if (selectField.Length < MaxLength)
                return " \"" + selectField + '"';
            int h = selectField.GetHashCode();
            if (h == int.MinValue)
            {
                h = 0;
            }
            else if (h < 0)
            {
                h = -h;
            }
            return " \"_F_" + h + '"';
        }

        public string GetFullName()
        {
            if (!string.IsNullOrEmpty(this.FullObjectName))
                return (new StringBuilder(this.FullObjectName.Length + this.Name.Length + 1)).Append(this.FullObjectName).Append('.')
                  .Append(this.Name).ToString();
            return this.Name;
        }
        private void Init()
        {
            if (string.IsNullOrEmpty(this.FullObjectName))
                this.FullObjectName = "";
            if (string.IsNullOrEmpty(this.Alias))
            {
                this.Alias = this.Name;
                if (!string.IsNullOrEmpty(this.FullObjectName))
                {
                    this.Alias = this.FullObjectName + '.' + this.Alias;
                }
             
                   
            }
        }

    }
}
