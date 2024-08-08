// MIT License
// 开源地址：https://gitee.com/co1024/AbcMvc
// Copyright (c) 2021-2023 1024
// Abc.Mvc=Furion+EF+SqlSugar+Pear layui admin.

using WYF.Bos.Form;

namespace JNPF.Form;
public class FormMetadataCache
{
    //private readonly IFormMetadataProvide _formMetadataProvide;
    private static IFormMetadataProvide _provide;

    public static IFormMetadataProvide GetFormMetadataProvide()
    {
        if (_provide == null)
            _provide = new FormMetadataProvide();
        return _provide;
    }

    //public FormMetadataCache(IFormMetadataProvide formMetadataProvide)
    //{
    //    this._formMetadataProvide= formMetadataProvide;
    //}

    public static FormConfig GetFormConfig(string formId)
    {
       return GetFormMetadataProvide().GetFormConfig(formId);
    }


    public static FormRoot GetRootControl(string formId)
    {
        return GetFormMetadataProvide().GetRootControl(formId.ToLower());
    }
}
