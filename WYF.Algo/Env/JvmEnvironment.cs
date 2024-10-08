using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using WYF.Algo.dataset;

namespace WYF.Algo.Env
{
    public class JvmEnvironment : Environment
    {
        private HashSet<AbstractDataSet> dataSets = new HashSet<AbstractDataSet>();
        public JvmEnvironment(AlgoConfig config) : base(config)
        {
        }

        public override void Close()
        {
            AutoRelease();
        }

        private void AutoRelease()
        {
            throw new NotImplementedException();
        }

        public override IDataSet CreateDataSet(IDataReader rs, RowMeta rowMeta)
        {
            return CreateDataSet((StreamSource)new ResultSetStreamSource(rs, rowMeta));
        }
        public IDataSet CreateDataSet(StreamSource streamSource)
        {
            StreamSourceDataSet dataSet = new StreamSourceDataSet(this, streamSource);
            return (IDataSet)dataSet;
        }

        public override void RegisterDataSet(IDataSet dataSet)
        {
            this.dataSets.Add((AbstractDataSet)dataSet);
        }
    }
}
