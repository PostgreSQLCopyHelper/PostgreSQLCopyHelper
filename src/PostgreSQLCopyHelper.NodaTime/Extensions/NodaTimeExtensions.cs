using System;
using NodaTime;
using NpgsqlTypes;

namespace PostgreSQLCopyHelper.NodaTime
{
    public static class NodaTimeExtensions
    {
        public static PostgreSQLCopyHelper<TEntity> MapTimestampTz<TEntity>(this PostgreSQLCopyHelper<TEntity> helper, string columnName, Func<TEntity, ZonedDateTime> propertyGetter)
        {
            return helper.Map(columnName, propertyGetter, NpgsqlDbType.TimestampTz); 
        }

        public static PostgreSQLCopyHelper<TEntity> MapTimestampTz<TEntity>(this PostgreSQLCopyHelper<TEntity> helper, string columnName, Func<TEntity, OffsetDateTime> propertyGetter)
        {
            return helper.Map(columnName, propertyGetter, NpgsqlDbType.TimestampTz);
        }

        public static PostgreSQLCopyHelper<TEntity> MapTimestamp<TEntity>(this PostgreSQLCopyHelper<TEntity> helper, string columnName, Func<TEntity, Instant> propertyGetter)
        {
            return helper.Map(columnName, propertyGetter, NpgsqlDbType.Timestamp);
        }

        public static PostgreSQLCopyHelper<TEntity> MapTimestamp<TEntity>(this PostgreSQLCopyHelper<TEntity> helper, string columnName, Func<TEntity, LocalDateTime> propertyGetter)
        {
            return helper.Map(columnName, propertyGetter, NpgsqlDbType.Timestamp);
        }

        public static PostgreSQLCopyHelper<TEntity> MapDate<TEntity>(this PostgreSQLCopyHelper<TEntity> helper, string columnName, Func<TEntity, LocalDate> propertyGetter)
        {
            return helper.Map(columnName, propertyGetter, NpgsqlDbType.Date);
        }

        public static PostgreSQLCopyHelper<TEntity> MapDate<TEntity>(this PostgreSQLCopyHelper<TEntity> helper, string columnName, Func<TEntity, NpgsqlDate> propertyGetter)
        {
            return helper.Map(columnName, propertyGetter, NpgsqlDbType.Date);
        }

        public static PostgreSQLCopyHelper<TEntity> MapTime<TEntity>(this PostgreSQLCopyHelper<TEntity> helper, string columnName, Func<TEntity, LocalTime> propertyGetter)
        {
            return helper.Map(columnName, propertyGetter, NpgsqlDbType.Time);
        }

        public static PostgreSQLCopyHelper<TEntity> MapTimeTz<TEntity>(this PostgreSQLCopyHelper<TEntity> helper, string columnName, Func<TEntity, OffsetTime> propertyGetter)
        {
            return helper.Map(columnName, propertyGetter, NpgsqlDbType.TimeTz);
        }


        public static PostgreSQLCopyHelper<TEntity> MapInterval<TEntity>(this PostgreSQLCopyHelper<TEntity> helper, string columnName, Func<TEntity, Interval> propertyGetter)
        {
            return helper.Map(columnName, propertyGetter, NpgsqlDbType.Interval);
        }
    }
}
