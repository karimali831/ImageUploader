using System.Reflection;

namespace ImageUploader.Helper
{
    public static class DapperHelper
    {
        public static string[] DapperFields(this Type type)
        {
            return
                type
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
                    .Where(x => !x.CustomAttributes.Any(a => a.AttributeType == typeof(DbIgnoreAttribute)))
                    .Select(x => x.Name).ToArray();
        }

        public static string SELECT(string table, string[] fields, int? top = null, string alias = null)
        {
            var selectFields = alias == null ? fields : fields.Select(f => $"{alias}.{f}");
            string sql = "";
            if (top.HasValue)
                sql = $"SELECT TOP {top} {string.Join(", ", selectFields)} FROM {table} ";
            else
                sql = $"SELECT {string.Join(", ", selectFields)} FROM {table} ";

            if (alias != null)
            {
                sql += $"{alias} ";
            }

            return sql;
        }

        public static string INSERT(string table, string[] fields)
        {
            return $"INSERT INTO {table} ({string.Join(", ", fields)}) VALUES ({string.Join(", ", fields.Select(f => $"@{f}"))})";
        }
    }

    public class DbIgnoreAttribute : Attribute
    {
    }
}
