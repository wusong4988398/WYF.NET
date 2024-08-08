
using WYF.Bos.DataEntity.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.DataEntity.Metadata.Clr
{
    public sealed class ComplexProperty : DataEntityProperty, IComplexProperty
    {
        private IDataEntityType _complexType;
        private Type _propertyType;
        private string _refIdPropertyName;

        public IDataEntityType ComplexType { get => this._complexType; }
        public string RefIdPropName { get => this._refIdPropertyName; }

        public ComplexProperty(PropertyInfo propertyInfo, int ordinal) : base(propertyInfo, ordinal)
        {
            ComplexPropertyAttribute cpAtt = propertyInfo.GetCustomAttribute<ComplexPropertyAttribute>();
            this.name = !string.IsNullOrEmpty(cpAtt.Name) ? cpAtt.Name : propertyInfo.Name;
            this._propertyType = propertyInfo.PropertyType;
            this._refIdPropertyName = cpAtt.RefIdPropertyName;
            this.Init();

        }
        private void Init()
        {
            this._complexType = DataEntityType.GetDataEntityType(this._propertyType);
        }
    
        
    }

}

