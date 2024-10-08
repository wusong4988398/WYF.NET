using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.Algo.dataset;
using static IronPython.Modules._ast;


namespace WYF.Algo
{
    public class ResultSetStreamSource : StreamSource
    {

        private RowMeta rowMeta;

        private IDataReader dataReader;

        private InnerRowIterator innerRowIterator;

        public ResultSetStreamSource(IDataReader dataReader)
        {
            this.dataReader = dataReader;
            this.Type = "ResultSet";
            this.rowMeta = InitRowMeta(dataReader);
        }

        public ResultSetStreamSource(IDataReader rs, RowMeta rowMeta)
        {
            this.dataReader = rs;
            this.rowMeta = rowMeta;
        }

        private RowMeta InitRowMeta(IDataReader rs)
        {
            return RowMeta.FromResultSet(rs);
        }


        public override void Close()
        {
            if (this.dataReader != null)
            {
                try
                {
                    this.dataReader.Close();
                }
                catch (Exception ex)
                {
                    throw;
                    
                }
            }
        }



        public override RowMeta RowMeta { get { return rowMeta; } }
        

        public override InnerRowIterator RowIterator()
        {
            if (this.innerRowIterator == null)
                this.innerRowIterator = ConvertIterator();
            return this.innerRowIterator;
        }

        public InnerRowIterator ConvertIterator()
        {
            return new ConcreteInnerRowIterator(this.dataReader, this.rowMeta);
        }


        private class ConcreteInnerRowIterator : InnerRowIterator
        {
            private bool nexted = false;
            private bool hasNext = false;
            private RowMeta rowMeta;
            private IDataReader dataReader;

            public ConcreteInnerRowIterator(IDataReader dataReader,RowMeta rowMeta)
            {
                this.rowMeta = rowMeta;
                this.dataReader = dataReader;
            }

            protected override bool _HasNext()
            {
                if (!this.nexted)
                    DoNext();
                return this.hasNext;
            }

            private void DoNext()
            {
                this.nexted = true;
                try
                {
                    this.hasNext = this.dataReader.Read();
                }
                catch (Exception e)
                {
                    throw new AlgoException(e.Message);
                }
            }

            protected override IRow _Next()
            {
                try
                {
                    if (!this.nexted)
                        DoNext();
                    if (!this.hasNext)
                        throw new AlgoException("EOF");
                    return RowFactory.CreateRow(this.rowMeta, this.dataReader);
                }
                finally
                {
                    this.nexted = false;
                }
            }

           // protected override void _Remove() { }
        }
    }
}
