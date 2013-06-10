using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

using Dapper;
using DapperExtensions;
using DapperExtensions.Mapper;
using DapperExtensions.Sql; 

namespace CRUDRepository.Repository.Base
{
    public class DapDatabaseFactory : IDatabaseFactory<IDatabase>
    {
        private readonly IDbConnection _dbConnection ;
        private readonly ISqlDialect _sqlDialect ;

        public DapDatabaseFactory(IDbConnectionFactory connFactory, ISqlDialectFactory dialectFactory)
        {
            connFactory.ThrowIfArgumentIsNull("connFactory");
            dialectFactory.ThrowIfArgumentIsNull("dialectFactory");

            _dbConnection = connFactory.Create();
            _sqlDialect = dialectFactory.Create(); 
        }

        public IDatabase Create()
        {
            //var config = new DapperExtensionsConfiguration(typeof(AutoClassMapper<>), new List<Assembly>(), new SqlServerDialect());

            var config = new DapperExtensionsConfiguration(typeof(AutoClassMapper<>), new List<Assembly>(), _sqlDialect);
            config.ThrowIfArgumentIsNull("dapperConfiguration");

            var sqlGenerator = new SqlGeneratorImpl(config);
            var database = new Database(_dbConnection, sqlGenerator);
            database.ThrowIfArgumentIsNull("Database");

            return database;
        }


    }
}
