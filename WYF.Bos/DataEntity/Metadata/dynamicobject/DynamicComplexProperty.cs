using WYF.DataEntity.Entity;
using WYF.DataEntity.Metadata.Dynamicobject;
using WYF.Bos.Orm.Exceptions;


namespace WYF.DataEntity.Metadata.dynamicobject
{
    [Serializable]
    public class DynamicComplexProperty : DynamicProperty, IComplexProperty
    {
  



        [SimpleProperty]
        public string TableGroup { get; set; }
        [ComplexProperty]
        public IDataEntityType ComplexType {  get; set; }
        [SimpleProperty]
        public string RefIdPropName { get; set; }
        public DynamicComplexProperty() { }

        public DynamicComplexProperty(string name, string refIdPropertyName, DynamicObjectType dynamicPropertyType, bool isReadonly)
           : base(name, typeof(DynamicObject), null, isReadonly)
        {
            if (dynamicPropertyType == null)
                throw new ORMArgInvalidException("???????", "构造动态实体复杂属性DynamicComplexProperty失败，构造参数：动态实体属性类型dynamicPropertyType不能为空！");
            this._propertyType = dynamicPropertyType.ClrType;
            this.ComplexType = dynamicPropertyType;
            this.RefIdPropName = refIdPropertyName;
        }
    }
}
