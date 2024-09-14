using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.OrmEngine.Drivers
{
    public abstract class SqlTask : IDatabaseTask, IDisposable
    {
        private IDbCommand _cmd;
        private int _expectedAffectedCount;
        private int _tableLevel;
        internal StringBuilder SqlBuilder;

        protected SqlTask()
        {
            this.SqlBuilder = new StringBuilder(0x200);
            this._expectedAffectedCount = -1;
        }

        public SqlTask(IDbCommand cmd)
        {
            this.SqlBuilder = new StringBuilder(0x200);
            this._expectedAffectedCount = -1;
            this._cmd = cmd;
        }

        public virtual object AddParamter(string name, DbType dbType, object value, out string paramterName)
        {
            if (this._cmd == null)
            {
                throw new NotImplementedException();
            }
            IDbDataParameter parameter = this._cmd.CreateParameter();
            paramterName = this.BuildParamterName(name);
            parameter.ParameterName = paramterName;
            value = value ?? DBNull.Value;
            parameter.Value = value;
            parameter.DbType = dbType;
            this._cmd.Parameters.Add(parameter);
            return parameter;
        }

        public virtual void AddParamters(object[] paramters)
        {
            if (this._cmd == null)
            {
                throw new NotImplementedException();
            }
            if (paramters == null)
            {
                throw new ArgumentNullException("paramters");
            }
            foreach (IDbDataParameter parameter in paramters)
            {
                IDbDataParameter parameter2 = this._cmd.CreateParameter();
                parameter2.ParameterName = parameter.ParameterName;
                parameter2.DbType = parameter.DbType;
                parameter2.Value = parameter.Value;
                parameter2.Direction = parameter.Direction;
                parameter2.Size = parameter.Size;
                parameter2.Scale = parameter.Scale;
                this._cmd.Parameters.Add(parameter2);
            }
        }

        public virtual object AddUdtParamter(string name, DbType dbType, object value, out string paramterName)
        {
            if (this._cmd == null)
            {
                throw new NotImplementedException();
            }
            IDbDataParameter parameter = this._cmd.CreateParameter();
            paramterName = this.BuildParamterName(name);
            parameter.ParameterName = paramterName;
            value = value ?? DBNull.Value;
            parameter.Value = value;
            parameter.DbType = dbType;
            this._cmd.Parameters.Add(parameter);
            return parameter;
        }

        protected virtual string BuildParamterName(string name)
        {
            return ("@" + name);
        }

        public void Dispose()
        {
            if (this._cmd != null)
            {
                this._cmd.Dispose();
            }
        }

        public virtual void Execute(IDbConnection con, IDbTransaction tran)
        {
            IDbCommand command = this.Command;
            if (command == null)
            {
                throw new NotImplementedException();
            }
            command.CommandText = this.SQL;
            command.Connection = con;
            if (tran != null)
            {
                command.Transaction = tran;
            }
            command.ExecuteNonQuery();
        }
        public IDbCommand Command
        {
            get
            {
                return this._cmd;
            }
        }
        public int ExpectedAffectedCount
        {
            get
            {
                return this._expectedAffectedCount;
            }
            protected internal set
            {
                this._expectedAffectedCount = value;
            }
        }

        public int Level
        {
            get
            {
                return this._tableLevel;
            }
            internal set
            {
                this._tableLevel = value;
            }
        }

        public bool ShouldCheckAffectedCount
        {
            get
            {
                return (this.ExpectedAffectedCount > 0);
            }
        }

        public string SQL
        {
            get
            {
                return this.SqlBuilder.ToString();
            }
        }
    }
}
