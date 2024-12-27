using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Persistence.ValueConverters;

internal sealed class UlidToStringConverter() : ValueConverter<Ulid, string>(
    v => v.ToString(),
    v => Ulid.Parse(v),
    new ConverterMappingHints(26));