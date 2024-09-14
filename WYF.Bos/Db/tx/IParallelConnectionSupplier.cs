using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Db.tx
{
    /// <summary>
    /// Represents a supplier of database connections that can be used in parallel.
    /// </summary>
    public interface IParallelConnectionSupplier
    {
        /// <summary>
        /// Gets a connection from the supplier.
        /// </summary>
        /// <returns>A database connection.</returns>
        IDbConnection GetConnection();
    }
}
