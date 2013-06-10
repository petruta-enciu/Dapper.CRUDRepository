using System;

namespace CRUDRepository.Repository
{
    /// <summary>
    /// Enumeration of database server types
    /// </summary>
    public enum DbServerName
    {
        None = 0,
        Sql,
        SqlCe,
        MySql,
        Oracle
    }

    /// <summary>
    /// Enumeration tools extension
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Method used to get the name of DbServerName enumeration value
        /// </summary>
        /// <param name="value">
        /// Is of type DbServerName 
        /// </param>
        /// <returns>
        /// Return the name of DbServerName enumeration value
        /// </returns>
        public static string GetName(this DbServerName value)
        {
            return Enum.GetName(value.GetType(), value);
        }

        /// <summary>
        ///  Method used to get the string user in SQL statements as parameter prefix
        /// </summary>
        /// <param name="value">
        /// Is of type DbServerName 
        /// </param>
        /// <returns>
        /// Returns ":" if enumeration value is Oracle otherwise "@" ; Default value is "@"
        /// </returns>
        public static string GetSqlParameterTag(this DbServerName value)
        {
            return Enum.GetName(value.GetType(), value) == "Oracle" ? ":" : "@";
        }
    }

}
