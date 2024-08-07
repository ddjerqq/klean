using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Persistence.ValueConverters;

public sealed class DateTimeUtcValueConverter()
    : ValueConverter<DateTime, DateTime>(
        v => v.ToUniversalTime(),
        v => DateTime.SpecifyKind(v, DateTimeKind.Utc));