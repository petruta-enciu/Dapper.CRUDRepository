using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper;
using DapperExtensions;
using DapperExtensions.Sql;


namespace CRUDRepository.Repository.Base
{
    public class SqlDialectFactory : ISqlDialectFactory
    {
        public ISqlDialect Create()
        {
            return new SqlServerDialect();
        }
    }
}
