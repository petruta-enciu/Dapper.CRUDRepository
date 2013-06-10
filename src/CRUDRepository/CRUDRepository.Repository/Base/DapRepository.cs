using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Reflection.Emit;

using Dapper;
using DapperExtensions;
using DapperExtensions.Mapper;
using DapperExtensions.Sql; 

namespace CRUDRepository.Repository.Base
{
    public class DapRepository<T> : IRepository<T> where T : class
    {
        public const string no_insert_msg = "Insert not performed";
        public const string no_update_msg = "Update not performed";
        public const string no_delete_msg = "Delete not performed";

        private readonly IDatabase _db;
        private string _tableName;
        private string _tableAlias;

        public DapRepository(DapDatabaseFactory dbcontextFactory, string tbAlias = null)
        {
            dbcontextFactory.ThrowIfArgumentIsNull("dbcontextFactory");

            _db = dbcontextFactory.Create();
            _db.ThrowIfArgumentIsNull("database");

            _tableName = DetermineTableName() ;
            _tableAlias = tbAlias != null ? tbAlias : _tableName;
        }

        private string DetermineTableName()
        {
            var tableMapping = _db.GetMap<T>();
            var name = tableMapping == null ? typeof(T).Name : tableMapping.TableName;
            return name;
        }

        public IDatabase Database
        {
            get
            {
                return _db;
            }
        }
        public String TableName
        {
            get
            {
                return _tableName ?? DetermineTableName();
            }
        }

        public String TableNameAlias
        {
            get
            {
                return _tableAlias;
            }
            set
            {
                _tableAlias = value ?? _tableName;
            }
        }

       public void Dispose()
       {
            if (_db != null)
            {       
                if (_db.HasActiveTransaction)
                {
                    _db.Rollback();
                }
                _db.Connection.Close();
            }
        }

        public void Insert(T value)
        {
            var success = _db.RunInTransaction<dynamic>(() => _db.Insert<T>(value));
            if (success == null)
            {
                throw new Exception(no_insert_msg);
            }
        }

        public void Insert(dynamic value)
        {
            var success = _db.RunInTransaction<dynamic>(() => _db.Insert<T>(value));
            if (success == null)
            {
                throw new Exception(no_insert_msg);
            }            
        }

        public void Insert(IEnumerable<T> values)
        {
            try
            {
                _db.BeginTransaction();
                _db.Insert<T>(entities: values);
                _db.Commit();
            }
            catch 
            {         
                _db.Rollback();
                throw;
            }
        }

        public void Update(T value)
        {
            var success = _db.RunInTransaction<bool>(() => _db.Update<T>(value));
            if (!success)
            {
                throw new Exception(no_update_msg);
            }
        }
        
        public void Update(dynamic value)
        {
            var success = _db.RunInTransaction<bool>(() => _db.Update<T>(value));
            if (!success)
            {
                throw new Exception(no_update_msg);
            }
        }

        public void Delete(T value)
        {
            var success = _db.RunInTransaction<bool>(() => _db.Delete<T>(value));
            if (!success)
            {
                throw new Exception(no_delete_msg);
            }
        }

        public void Delete(dynamic where)
        {
            var success = _db.RunInTransaction<bool>(() => _db.Delete<T>(where));
            if (!success)
            {
                throw new Exception(no_delete_msg);
            }
        }

        public void Delete(IEnumerable<T> values)
        {
            try
            {
                _db.BeginTransaction();
                _db.Delete<T>(values);
                _db.Commit();
            }
            catch
            {
                _db.Rollback();
                throw;
            }
        }

        public T GetBy(dynamic id)
        {
            return _db.Get<T>(id, null);
        }

        public IEnumerable<T> Get(IEnumerable<dynamic> predicates)
        {
            return _db.GetList<T>(predicates);
        }

        public IEnumerable<T> GetAll()
        {
            return _db.GetList<T>();
        }
 
    }
}
