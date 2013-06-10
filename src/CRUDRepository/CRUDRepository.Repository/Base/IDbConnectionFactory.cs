using System;
using System.Collections.Generic;
using System.Data;

namespace CRUDRepository.Repository.Base
{
    /// <summary>
    /// Generic interface used to build a connection
    /// </summary>
    public interface IDbConnectionFactory 
    {
        IDbConnection Create();
    }
}
