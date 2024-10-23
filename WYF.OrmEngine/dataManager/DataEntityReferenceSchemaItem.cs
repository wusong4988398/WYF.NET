using WYF.DataEntity.Metadata;

namespace WYF.OrmEngine.dataManager
{
    /// <summary>
    /// 表示数据实体之间的引用关系。它封装了与引用相关的各种属性，这些属性描述了从一个实体到另一个实体的引用方式
    /// </summary>
    public class DataEntityReferenceSchemaItem
    {
        private string privatePropertyPath;
        private ISimpleProperty privateReferenceOidProperty;
        private string privateReferenceTo;
        private IComplexProperty privateReferenceObjectProperty;

        /// <summary>
        /// 存储属性路径（property path），这是引用属性在数据实体中的路径。
        /// 例如，如果有一个嵌套的对象结构，这个路径可以用来表示从根对象到引用属性的完整路径。
        /// </summary>
        public string PropertyPath
        {
            get { return this.privatePropertyPath; }
            set { this.privatePropertyPath = value; }
        }

        /// <summary>
        /// 这个属性通常是一个简单属性（如整数或字符串），用于存储被引用实体的唯一标识符（例如主键）。
        /// </summary>
        public ISimpleProperty ReferenceOidProperty
        {
            get { return this.privateReferenceOidProperty; }
            set { this.privateReferenceOidProperty = value; }
        }

        /// <summary>
        /// 存储引用目标（reference to），即被引用的数据实体的名称或类型。这通常是一个字符串，表示被引用实体的名称
        /// </summary>
        public string ReferenceTo
        {
            get { return this.privateReferenceTo; }
            set { this.privateReferenceTo = value; }
        }

        /// <summary>
        /// 存储引用对象属性（reference object property）。这是一个复杂属性，可能包含多个子属性，用于表示被引用实体的详细信息。
        /// 这个属性通常是可选的，不是所有引用都需要完整的对象信息。
        /// </summary>
        public IComplexProperty ReferenceObjectProperty
        {
            get { return this.privateReferenceObjectProperty; }
            set { this.privateReferenceObjectProperty = value; }
        }
    }
}