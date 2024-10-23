using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.DataEntity.Metadata
{
    public interface ILocaleProperty
    {
        void SetGlangValue(object dataEntity, object value);

        IDataEntityProperty GetLocaleCollectionProperty();


        IDataEntityProperty GetLangProperty();
    
    }
}
