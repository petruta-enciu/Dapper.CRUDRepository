using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace CRUDRepository.Repository.Base
{
    public interface IRepository<T> : IDisposable where T : class
    {
        String TableName { get; }
        String TableNameAlias { get; set; }
        
        void Insert(T value);
        void Insert(dynamic value);
        void Insert(IEnumerable<T> values);

        void Update(T value);
        void Update(dynamic value);
      
        void Delete(T value);
        void Delete(dynamic where);
        void Delete(IEnumerable<T> values);
                
        T GetBy(dynamic key);
        IEnumerable<T> Get(IEnumerable<dynamic> where);
        IEnumerable<T> GetAll();
    }
}
