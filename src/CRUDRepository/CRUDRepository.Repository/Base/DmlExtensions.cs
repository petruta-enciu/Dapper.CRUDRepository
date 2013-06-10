using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using Dapper;
using Dapper.Contrib.Extensions;

namespace Mood.Weather.Domain.Repository.Base
{
  
    public static class DmlExtensions 
    {       
        private static IEnumerable<PropertyInfo> GetAllProperties(object entity)
        {
            if (entity == null) entity = new {};
            return entity.GetType().GetProperties();
        }

        private static IEnumerable<PropertyInfo> GetScaffoldableProperties(object entity)
        {
            var props = entity.GetType().GetProperties().Where(p => p.GetCustomAttributes(true).Any(attr => attr.GetType().Name == "WriteAttribute" && !IsWriteable(p)) == false);
            return props.Where(p => p.PropertyType.IsPrimitiveType() || IsWriteable(p));
        }

        public static bool IsWriteable(PropertyInfo pi)
        {
            object[] attributes = pi.GetCustomAttributes(typeof(WriteAttribute), false);
            if (attributes.Length == 1)
            {
                var write = (WriteAttribute)attributes[0];
                return write.Write;
            }
            return true; // default is false ????
        }

        private static IEnumerable<PropertyInfo> GetNonIdProperties(object entity)
        {
            return GetScaffoldableProperties(entity).Where(p => p.Name != "Id" && p.GetCustomAttributes(true).Any(attr => attr.GetType().Name == "KeyAttribute") == false);
        }

        private static IEnumerable<PropertyInfo> GetIdProperties(object entity)
        {
            var type = entity.GetType();
            return GetIdProperties(type);
        }

        private static IEnumerable<PropertyInfo> GetIdProperties(Type type)
        {
            return type.GetProperties().Where(p => p.Name == "Id" || p.GetCustomAttributes(true).Any(attr => attr.GetType().Name == "KeyAttribute"));
        }

        private static string GetTableName(object entity)
        {
            var type = entity.GetType();
            return GetTableName(type);
        }

        private static string GetTableName(Type type)
        {
            var tableName = type.Name;

            var tableattr = type.GetCustomAttributes(true).SingleOrDefault(attr => attr.GetType().Name == "TableAttribute") as dynamic;
            if (tableattr != null)
                tableName = tableattr.Name;

            return tableName;
        }


        //build update statement based on list on an entity
        private static void BuildUpdateSet(object entityToUpdate, StringBuilder sb)
        {
            var nonIdProps = GetNonIdProperties(entityToUpdate).ToArray();


            for (var i = 0; i < nonIdProps.Length; i++)
            {
                var property = nonIdProps[i];


                sb.AppendFormat("{0} = @{1}", property.Name, property.Name);
                if (i < nonIdProps.Length - 1)
                    sb.AppendFormat(", ");
            }
        }

        //build where clause based on list of properties
        private static void BuildWhere(StringBuilder sb, IEnumerable<PropertyInfo> idProps)
        {
            var propertyInfos = idProps.ToArray();
            for (var i = 0; i < propertyInfos.Count(); i++)
            {
                sb.AppendFormat("[{0}] = @{1}", propertyInfos.ElementAt(i).Name, propertyInfos.ElementAt(i).Name);
                if (i < propertyInfos.Count() - 1)
                    sb.AppendFormat(" and ");
            }
        }

        //build where clause based on list of properties
        private static void BuildWhere(StringBuilder sb, List<KeyValuePair<string, dynamic>> propertyInfos)
        {
            for (var i = 0; i < propertyInfos.Count(); i++)
            {
                sb.AppendFormat("[{0}] = @{1}", propertyInfos[i].Key, propertyInfos[i].Key);
                if (i < propertyInfos.Count() - 1)
                    sb.AppendFormat(" and ");
            }
        }

        //build insert values which include all properties in the class that are not marked with the Editable(false) attribute,
        //are not marked with the [Key] attribute, and are not named Id
        private static void BuildInsertValues(object entityToInsert, StringBuilder sb)
        {
            var props = GetScaffoldableProperties(entityToInsert).ToArray();
            for (var i = 0; i < props.Count(); i++)
            {
                var property = props.ElementAt(i);
                if (property.GetCustomAttributes(true).Any(attr => attr.GetType().Name == "KeyAttribute")) continue;
                if (property.Name == "Id") continue;
                sb.AppendFormat("@{0}", property.Name);
                if (i < props.Count() - 1)
                    sb.Append(", ");
            }
            if (sb.ToString().EndsWith(", "))
                sb.Remove(sb.Length - 2, 2);

        }

        //build insert parameters which include all properties in the class that are not marked with the Editable(false) attribute,
        //are not marked with the [Key] attribute, and are not named Id
        private static void BuildInsertParameters(object entityToInsert, StringBuilder sb)
        {
            var props = GetScaffoldableProperties(entityToInsert).ToArray();


            for (var i = 0; i < props.Count(); i++)
            {
                var property = props.ElementAt(i);
                if (property.GetCustomAttributes(true).Any(attr => attr.GetType().Name == "KeyAttribute")) continue;
                if (property.Name == "Id") continue;
                sb.Append(property.Name);
                if (i < props.Count() - 1)
                    sb.Append(", ");
            }
            if (sb.ToString().EndsWith(", "))
                sb.Remove(sb.Length - 2, 2);
        }


        //GetBy - single primary key
        /// <returns>Returns a single entity by a single id from table T.</returns>
        public static T GetById<T>(this IDbConnection connection, dynamic id)
        {
            var currenttype = typeof(T);
            var idProps = GetIdProperties(currenttype).ToList();

            if (!idProps.Any())
                throw new ArgumentException("Get<T> only supports an entity with a [Key] or Id property");
            if (idProps.Count() > 1)
                throw new ArgumentException("Get<T> only supports an entity with a single [Key] or Id property");

            var onlyKey = idProps.First();
            var name = GetTableName(currenttype);

            var sb = new StringBuilder();
            sb.AppendFormat("Select * from [{0}]", name);
            sb.Append(" where " + onlyKey.Name + " = @Id");

            var dynParms = new DynamicParameters();
            dynParms.Add("@id", id);

            return connection.Query<T>(sb.ToString(), dynParms).FirstOrDefault();
        }

        //GetBy - multiple primary key
        /// <returns>Returns a single entity by a single id from table T.</returns>
        public static T Get<T>(this IDbConnection connection, object multipleKeys)
        {
            var currenttype = typeof(T);
            var idProps = GetIdProperties(currenttype).ToList();

            if (!idProps.Any())
                throw new ArgumentException("Entity must have at least one [Key] property");

            var name = GetTableName(currenttype);

            var sb = new StringBuilder();
            var whereprops = GetAllProperties(multipleKeys).ToArray();
            sb.AppendFormat("Select * from [{0}]", name);

            if (whereprops.Any())
            {
                sb.Append(" where ");
                BuildWhere(sb, whereprops);
            }

            return connection.Query<T>(sb.ToString(), multipleKeys).FirstOrDefault();
        }

        //GetBy - multiple primary key
        /// <returns>Returns a single entity by a single id from table T.</returns>
        public static T Get<T>(this IDbConnection connection, List<KeyValuePair<string, dynamic>> multipleKeys)
        {
            var currenttype = typeof(T);
            var idProps = GetIdProperties(currenttype).ToList();

            if (!idProps.Any())
                throw new ArgumentException("Entity must have at least one [Key] property");

            var name = GetTableName(currenttype);
            
            var sb = new StringBuilder();
            sb.AppendFormat("Select * from [{0}]", name);

            var dynParms = new DynamicParameters();

            for (var i = 0; i < multipleKeys.Count(); i++)
            {
                sb.AppendFormat("[{0}] = @{1}", multipleKeys[i].Key, multipleKeys[i].Key);
                dynParms.Add("@" + multipleKeys[i].Key, multipleKeys[i].Value);

                if (i < multipleKeys.Count() - 1)
                    sb.AppendFormat(" and ");
            }

            return connection.Query<T>(sb.ToString(), dynParms).FirstOrDefault();
        }
  
        /// <returns>Gets a list of entities with optional exact match where conditions</returns>
        public static IEnumerable<T> GetList<T>(this IDbConnection connection, object whereConditions)
        {
            var currenttype = typeof(T);
            var idProps = GetIdProperties(currenttype).ToList();

            if (!idProps.Any())
                throw new ArgumentException("Entity must have at least one [Key] property");

            var name = GetTableName(currenttype);

            var sb = new StringBuilder();
            var whereprops = GetAllProperties(whereConditions).ToArray();
            sb.AppendFormat("Select * from [{0}]", name);

            if (whereprops.Any())
            {
                sb.Append(" where ");
                BuildWhere(sb, whereprops);
            }

            return connection.Query<T>(sb.ToString(), whereConditions);
        }

        /// <returns>Gets a list of all entities</returns>
        public static IEnumerable<T> GetList<T>(this IDbConnection connection)
        {
            return connection.GetList<T>(new { });
        }

        /// <returns>The ID (primary key) of the newly inserted record</returns>
        public static int Insert(this IDbConnection connection, object entityToInsert, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var name = GetTableName(entityToInsert);

            var sb = new StringBuilder();
            sb.AppendFormat("insert into [{0}]", name);
            sb.Append(" (");
            BuildInsertParameters(entityToInsert, sb);
            sb.Append(") values (");
            BuildInsertValues(entityToInsert, sb);
            sb.Append(")");

            return connection.Execute(sb.ToString(), entityToInsert, transaction, commandTimeout);

           // var r = connection.Query("select @@IDENTITY id", null, transaction, true, commandTimeout);
           // return (int)r.First().id;
        }

        /// <returns>The number of effected records</returns>
        public static int Update(this IDbConnection connection, object entityToUpdate, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var idProps = GetIdProperties(entityToUpdate).ToList();

            if (!idProps.Any())
                throw new ArgumentException("Entity must have at least one [Key] or Id property");

            var name = GetTableName(entityToUpdate);

            var sb = new StringBuilder();
            sb.AppendFormat("update [{0}]", name);

            sb.AppendFormat(" set ");
            BuildUpdateSet(entityToUpdate, sb);
            sb.Append(" where ");
            BuildWhere(sb, idProps);

            return connection.Execute(sb.ToString(), entityToUpdate, transaction, commandTimeout);
        }

        /// <returns>The number of records effected</returns>
        public static int Delete<T>(this IDbConnection connection, T entityToDelete, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var idProps = GetIdProperties(entityToDelete).ToList();

            if (!idProps.Any())
                throw new ArgumentException("Entity must have at least one [Key] or Id property");

            var name = GetTableName(entityToDelete);

            var sb = new StringBuilder();
            sb.AppendFormat("delete from [{0}]", name);

            sb.Append(" where ");
            BuildWhere(sb, idProps);

            return connection.Execute(sb.ToString(), entityToDelete, transaction, commandTimeout);
        }

        // delete using primary key 
        /// <returns>The number of records effected</returns>
        public static int DeleteBy<T>(this IDbConnection connection, dynamic id, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var currenttype = typeof(T);
            var idProps = GetIdProperties(currenttype).ToList();

            if (!idProps.Any())
                throw new ArgumentException("Delete<T> only supports an entity with a [Key] or Id property");
            if (idProps.Count() > 1)
                throw new ArgumentException("Delete<T> only supports an entity with a single [Key] or Id property");

            var onlyKey = idProps.First();
            var name = GetTableName(currenttype);

            var sb = new StringBuilder();
            sb.AppendFormat("Delete from [{0}]", name);
            sb.Append(" where " + onlyKey.Name + " = @Id");

            var dynParms = new DynamicParameters();
            dynParms.Add("@id", id);

            return connection.Execute(sb.ToString(), dynParms, transaction, commandTimeout);
        }

        // delete using primary key - multiple key
        /// <returns>The number of records effected</returns>
        public static int Delete<T>(this IDbConnection connection, object whereConditions, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var currenttype = typeof(T);
            var idProps = GetIdProperties(currenttype).ToList();

            if (!idProps.Any())
                throw new ArgumentException("Entity must have at least one [Key] property");

            var name = GetTableName(currenttype);

            var sb = new StringBuilder();
            var whereprops = GetAllProperties(whereConditions).ToArray();
            sb.AppendFormat("delete from [{0}]", name);

            if (whereprops.Any())
            {
                sb.Append(" where ");
                BuildWhere(sb, whereprops);
            }
            return connection.Execute(sb.ToString(), whereprops, transaction, commandTimeout);

        }


    }











    
}
