using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.OrmEngine.Query.Crud
{
    public static class EntityConst
    {
        public const string MultiLangTableNameSuffix = "_L";
        public const string MultiLangTableLocaleField = "FLocaleId";
        public const string MultiLangTableLocaleField_lower = "flocaleid";
        public const string MultiLangPKField = "FPkId";
        public const string MultiLangPropertyName = "multilanguagetext";
        public const string MultiLangPKId = "pkid";
        public const string MultiLangTableLocaleId = "localeid";
        public const string Ref_object_appended_fk_suffix = "_id";
        public const string String_pk_default_value = " ";
        public const int Number_pk_default_value = 0;
        public const double Number_pk_default_value_1 = 0.0d;
        public const string Encrypt_property_field_suffix = "_enp";
        public const string Privacy_property_field_suffix = "_pr";
        public const string Multi_basedata_rel_fk_id = "fbasedataid";
    }
}
