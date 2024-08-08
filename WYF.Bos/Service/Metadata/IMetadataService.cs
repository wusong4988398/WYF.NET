using WYF.Bos.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Service.Metadata;

public interface IMetadataService
{
    /// <summary>
    /// 加载实体元数据
    /// </summary>
    /// <param name="entityName"></param>
    /// <returns></returns>
    string LoadEntityMeta(string entityName);

    string LoadFormConfig(string formId);
    /// <summary>
    /// 加载表单运行云数据
    /// </summary>
    /// <param name="formId"></param>
    /// <param name="formMetaType"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    string LoadFormRuntimeMeta(string formId, RuntimeMetaType formMetaType, string key);

    /// <summary>
    /// 加载前端表单元数据
    /// </summary>
    /// <param name="formId"></param>
    /// <returns></returns>
    string LoadClientFormMeta(String formId);
}
