using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Service.Metadata
{
    public class MetadataService : IMetadataService
    {

        private static volatile MetadataServiceReader _reader;

        //private static SqlsugarHelper Db = SqlsugarHelper.Init();

        private static MetadataServiceReader GetInstance()
        {
            if (_reader == null)
                _reader = new MetadataServiceReader();
            return _reader;
        }
        public MetadataService()
        {
            _reader = GetInstance();
        }
        /// <summary>
        /// 加载实体元数据
        /// </summary>
        /// <param name="entityName"></param>
        /// <returns></returns>
        public string LoadEntityMeta(string entityName)
        {
            string entityMeta = _reader.LoadEntityMeta(entityName);
            return entityMeta;
        }

        //private readonly ISqlSugarClient _db;
        //private readonly ISqlSugarRepository<SysRole> _repository;

        //public MetadataService(ISqlSugarClient db, ISqlSugarRepository<SysRole> repository)
        //{   
        //    this._db = db;
        //    this._repository = repository;
        //}


        public string LoadFormConfig(string formId)
        {

            //string sdata = _repository.Context.Ado.GetString($"select FDATA from T_META_FORM where FNUMBER = '{formId}' and FKEY = '{formId}' and FTYPE = {(int)RuntimeMetaType.Config}");
            // string sdata = DB.Instance.GetClient().Ado.GetString($"select FDATA from T_META_FORM where FNUMBER = '{formId}' and FKEY = '{formId}' and FTYPE = {(int)RuntimeMetaType.Config}");
            // string sdata = DB.Instance.QuerySqlScalarSync($"select FDATA from T_META_FORM where FNUMBER = '{formId}' and FKEY = '{formId}' and FTYPE = {(int)RuntimeMetaType.Config}").ToNullString();
            // return sdata;
            return "";


        }
        /// <summary>
        /// 加载表单运行时元数据
        /// </summary>
        /// <param name="formId"></param>
        /// <param name="formMetaType"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public string LoadFormRuntimeMeta(string formId, int formMetaType, string key)
        {
            return _reader.LoadFormRuntimeMeta(formId, formMetaType, key);
        }
        /// <summary>
        ///加载前端表单元数据
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public string LoadClientFormMeta(string number)
        {
            string str = _reader.LoadFormRuntimeMeta(number, (int)RuntimeMetaType.Client, number);

            return str;
        }

        public string GetRuntimeMetadataVersion(string number)
        {
            throw new NotImplementedException();
        }
    }

}
