using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.DataEntity.Entity;
using WYF.DataEntity.Metadata;
using WYF.DataEntity.Metadata.Dynamicobject;
using WYF.Entity.DataModel;
using WYF.Entity.Validate;

namespace WYF.Entity.Property
{
    public class MulBasedataProp : DynamicCollectionProperty, IValidatorHanlder, IBasedataField, ICompareTypeConfig
    {
        private IDataEntityProperty refIdProp;
        private string baseEntityName;

       // public CompareTypeConfig CompareTypeConfig { get; set; }
        /// <summary>
        /// 是否必填
        /// </summary>
        [SimpleProperty(Name = "MustInput")]
        public bool IsMustInput {  get; set; }
        /// <summary>
        /// 多选基础资料对应使用组织的属性名
        /// </summary>
        [SimpleProperty]
        public string OrgProp {  get; set; }
        /// <summary>
        /// 组织业务职能编码
        /// </summary>
        [SimpleProperty]
        public string OrgFunc {  get; set; }

        public string BaseEntityId 
        { 
          get => this.baseEntityName; 
          
          set => this.baseEntityName=value; 
        }
        public bool IsSysField {  get; set; }=false;

        public int DbType => ((DynamicSimpleProperty)this.RefIdProp).DbType;

        public object? DefValue { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        /// <summary>
        /// 多选基础资料字段主键实体属性
        /// </summary>
        public IDataEntityProperty RefIdProp
        {
            get
            {  //??= 是空合并赋值运算符
               //如果 refIdProp 当前是 null，则将 ItemType.Properties["fbasedataid_id"] 的值赋给 refIdProp。
               //如果 refIdProp 不是 null，则不做任何操作，保持 refIdProp 的当前值不变。
                refIdProp ??= ItemType.Properties["fbasedataid_id"];
                return refIdProp;
            }
        }

        public void ApplyDefaultValue(IDataModel model, DynamicObject dataEntity, int rowIndex)
        {
            throw new NotImplementedException();
        }

        public void SetFieldValue(IDataModel model, object dataEntity, object Value)
        {
            throw new NotImplementedException();
        }
    }
}
