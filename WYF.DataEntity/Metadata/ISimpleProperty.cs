using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WYF.Form.DataEntity;

namespace WYF.DataEntity.Metadata
{
    /// <summary>
    /// 描述了简单属性 简单属性是最基本的属性描述符，此属性具有不可拆解。例如"员工"实体类型具有Age属性。
    /// </summary>
    public interface ISimpleProperty: IDataEntityProperty
    {
        /// <summary>
        /// 给定一个实体，重置此属性在此实体的值
        /// </summary>
        /// <param name="dataEntity">实体对象</param>
        void ResetValue(object dataEntity);


        long PrivacyType { get; set; }

        object GetSaveValue(object dataEntity, OperateOption option, RowOperateType type);
        bool ShouldSerializeValue(object dataEntity);
        bool IsLocalizable { get; }
        /// <summary>
        /// 是否为主键
        /// </summary>
        /// <returns></returns>
        bool IsPrimaryKey { get; }
        /// <summary>
        /// 数据类型
        /// </summary>
        /// <returns></returns>
        int DbType { get; set; }
        /// <summary>
        /// 是否需要加密
        /// </summary>
        /// <returns></returns>
        bool IsEncrypt { get; }
        /// <summary>
        /// 设置实体全部为脏或不脏
        /// </summary>
        /// <param name="dataEntity">要设置的实体对象</param>
        /// <param name="newValue">true:全置脏，false：全清脏</param>
        void SetDirty(Object dataEntity, bool newValue);
    }
}
