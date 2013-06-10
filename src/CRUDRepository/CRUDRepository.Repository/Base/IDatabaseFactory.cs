using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace CRUDRepository.Repository.Base
{
    public interface IDatabaseFactory<TDatabase> where TDatabase : class
     {
        TDatabase Create();
     }
}
