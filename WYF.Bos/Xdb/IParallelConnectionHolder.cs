using System;
using System.Data;
using WYF.Bos.xdb.datasource;

namespace WYF.Bos.xdb
{
    /// <summary>
    /// Interface for managing parallel database connections.
    /// </summary>
    public interface IParallelConnectionHolder
    {
        /// <summary>
        /// Requires a database connection for use in parallel operations.
        /// </summary>
        /// <param name="forManager">Indicates whether the connection is required for a manager operation.</param>
        /// <param name="canUseNoneMainConnection">Indicates whether non-main connections can be used.</param>
        /// <param name="querySQL">The SQL query that will be executed with the connection.</param>
        /// <returns>The database connection.</returns>
        IDbConnection RequireConnection(bool forManager, bool canUseNoneMainConnection, string querySQL);

        /// <summary>
        /// Releases a connection for sharing among parallel operations.
        /// </summary>
        /// <param name="connection">The database connection to release.</param>
        void ReleaseForSharing(IDbConnection connection);

        /// <summary>
        /// Closes a database connection and optionally rolls back any pending transactions.
        /// </summary>
        /// <param name="connection">The database connection to close.</param>
        /// <param name="rollback">Indicates whether to rollback any pending transactions.</param>
        void CloseConnection(IDbConnection connection, bool rollback);

        /// <summary>
        /// Determines whether a given connection is the main connection.
        /// </summary>
        /// <param name="connection">The database connection to check.</param>
        /// <returns>True if the connection is the main connection; otherwise, false.</returns>
        bool IsMainConnection(IDbConnection connection);

        /// <summary>
        /// Gets the type of the database being used.
        /// </summary>
        /// <returns>The database type.</returns>
        DBType GetDBType();

        /// <summary>
        /// Gets the number of currently held connections.
        /// </summary>
        /// <returns>The number of connections.</returns>
        int GetHoldingConnections();
    }
}
