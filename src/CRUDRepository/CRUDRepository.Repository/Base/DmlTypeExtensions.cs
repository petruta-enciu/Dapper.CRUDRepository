using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mood.Weather.Domain.Repository.Base
{
    static class DmlTypeExtensions
    {
        public static bool IsPrimitiveType(this Type type)
        {
            var primitiveTypes = new List<Type>
                    {
                        typeof (byte),
                        typeof (sbyte),
                        typeof (short),
                        typeof (ushort),
                        typeof (int),
                        typeof (uint),
                        typeof (long),
                        typeof (ulong),
                        typeof (float),
                        typeof (double),
                        typeof (decimal),
                        typeof (bool),
                        typeof (string),
                        typeof (char),
                        typeof (Guid),
                        typeof (DateTime),
                        typeof (DateTimeOffset),
                        typeof (byte[]),
                        typeof (DateTime?),
                        typeof (byte?),
                        typeof (sbyte?),
                        typeof (short?),
                        typeof (ushort?),
                        typeof (int?),
                        typeof (uint?),
                        typeof (long?),
                        typeof (ulong?),
                        typeof (float?),
                        typeof (double?),
                        typeof (decimal?),
                        typeof (bool?),
                        typeof (char?),
                        typeof (Guid?),
                        typeof (DateTime?),
                        typeof (DateTimeOffset?),
                        typeof (byte?[])
                    };
            return primitiveTypes.Contains(type);
        }
    }

}
