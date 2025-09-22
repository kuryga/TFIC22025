using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
// REMOVE: using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class ColumnAttribute : Attribute
{
    public ColumnAttribute(string name) { Name = name; }
    public string Name { get; }
}

public static class DbMapper
{
    private static readonly ConcurrentDictionary<Tuple<Type, string>, Delegate> _cache =
        new ConcurrentDictionary<Tuple<Type, string>, Delegate>();

    public static List<T> MapToList<T>(IDataReader reader) where T : new()
    {
        var list = new List<T>();
        var map = GetRowMapper<T>(reader);
        while (reader.Read())
            list.Add(map(reader));
        return list;
    }

    public static T MapSingleOrDefault<T>(IDataReader reader) where T : new()
    {
        var map = GetRowMapper<T>(reader);

        if (!reader.Read())
            return default(T); // para clases = null

        var item = map(reader);

        if (reader.Read())
            throw new InvalidOperationException("Se esperaba 0 o 1 fila, pero se obtuvo más de una.");

        return item;
    }

    public static Func<IDataRecord, T> GetRowMapper<T>(IDataReader reader) where T : new()
    {
        var key = Tuple.Create(typeof(T), GetSchemaKey(reader));
        Delegate del;
        if (_cache.TryGetValue(key, out del))
            return (Func<IDataRecord, T>)del;

        var mapper = BuildMapper<T>(reader);
        _cache[key] = mapper;
        return mapper;
    }

    private static string GetSchemaKey(IDataReader reader)
    {
        var cols = Enumerable.Range(0, reader.FieldCount)
            .Select(i => reader.GetName(i).ToLowerInvariant());
        return string.Join("|", cols);
    }

    private static Func<IDataRecord, T> BuildMapper<T>(IDataReader reader) where T : new()
    {
        var recordParam = Expression.Parameter(typeof(IDataRecord), "r");
        var resultVar = Expression.Variable(typeof(T), "item");
        var assignNew = Expression.Assign(resultVar, Expression.New(typeof(T)));

        var bodyExpressions = new List<Expression> { assignNew };

        var ordinals = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        for (int i = 0; i < reader.FieldCount; i++)
            ordinals[reader.GetName(i)] = i;

        var getValueMethod = typeof(IDataRecord).GetMethod("GetValue");
        var isDbNullMethod = typeof(IDataRecord).GetMethod("IsDBNull");
        if (getValueMethod == null || isDbNullMethod == null)
            throw new InvalidOperationException("IDataRecord methods not found.");

        foreach (var prop in typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            if (!prop.CanWrite) continue;

            // Only our custom [Column] attribute
            var colAttr = prop.GetCustomAttribute(typeof(ColumnAttribute)) as ColumnAttribute;
            var targetName = colAttr != null ? colAttr.Name : prop.Name;

            int ordinal;
            if (!ordinals.TryGetValue(targetName, out ordinal))
                continue;

            var isDbNull = Expression.Call(recordParam, isDbNullMethod, Expression.Constant(ordinal));
            var getValue = Expression.Call(recordParam, getValueMethod, Expression.Constant(ordinal));

            var converted = BuildConversion(getValue, prop.PropertyType);

            var assignProp = Expression.IfThen(
                Expression.Not(isDbNull),
                Expression.Assign(Expression.Property(resultVar, prop), converted)
            );

            bodyExpressions.Add(assignProp);
        }

        bodyExpressions.Add(resultVar);

        var body = Expression.Block(new[] { resultVar }, bodyExpressions);
        var lambda = Expression.Lambda<Func<IDataRecord, T>>(body, recordParam);
        return lambda.Compile();
    }

    private static Expression BuildConversion(Expression valueExpr, Type targetType)
    {
        var nonNullableType = Nullable.GetUnderlyingType(targetType) ?? targetType;

        if (nonNullableType == typeof(string))
        {
            var cast = Expression.Convert(valueExpr, typeof(object));
            var toString = typeof(Convert).GetMethod("ToString", new[] { typeof(object) });
            var str = Expression.Call(toString, cast);
            return targetType == nonNullableType ? (Expression)str : Expression.Convert(str, targetType);
        }

        if (nonNullableType.IsEnum)
        {
            var toStringCall = Expression.Call(valueExpr, typeof(object).GetMethod("ToString"));
            var parse = typeof(Enum).GetMethod("Parse", new[] { typeof(Type), typeof(string), typeof(bool) });
            var parseCall = Expression.Call(parse, Expression.Constant(nonNullableType), toStringCall, Expression.Constant(true));
            var enumConverted = Expression.Convert(parseCall, nonNullableType);
            return targetType == nonNullableType ? (Expression)enumConverted : Expression.Convert(enumConverted, targetType);
        }

        var changeType = typeof(Convert).GetMethod("ChangeType", new[] { typeof(object), typeof(Type) });
        var changed = Expression.Call(changeType, valueExpr, Expression.Constant(nonNullableType, typeof(Type)));
        var strong = Expression.Convert(changed, nonNullableType);

        return targetType == nonNullableType ? (Expression)strong : Expression.Convert(strong, targetType);
    }
}
