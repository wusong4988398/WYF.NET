using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Environment = WYF.Algo.Env.Environment;
namespace WYF.Algo.dataset
{
    public class StreamSourceDataSet : AbstractDataSet
    {
        private StreamSource streamSource;

        public StreamSourceDataSet(Environment environment, StreamSource streamSource)
            : base(streamSource.Type, environment, new List<AbstractDataSet>())
        {
            this.streamSource = streamSource;
            this.rowMeta = CreateTargetRowMeta();
        }

        protected override RowMeta CreateTargetRowMeta()
        {
            return this.streamSource.RowMeta;
        }

        protected override InnerRowIterator CreateIterator()
        {
            CheckClosed();
            return this.streamSource.RowIterator();
        }

        public override void RealClose()
        {
            this.streamSource.Close();
        }

    
    }
}
