using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace CRUDRepository.Repository.Base
{
    /// <summary>
    /// Class used to inject SqlConnection object
    /// </summary>
    public class SqlDbConnectionFactory :  IDbConnectionFactory
    {
        private readonly string _connectionString;

        /// <summary>
        /// Class constructor need to receive a valid connection string
        /// </summary>
        /// <param name="connectionString"></param>
        public SqlDbConnectionFactory(string connectionString)
        {
            connectionString.ThrowIfArgumentIsNull("connectionString");
            _connectionString = connectionString;
        }

        /// <summary>
        /// Creates a SqlConnection according connectionString used to create this class
        /// </summary>
        /// <returns>
        /// SqlConnection
        /// </returns>
        public IDbConnection Create()
        {            
            return new SqlConnection(_connectionString);
        }
    }
}
