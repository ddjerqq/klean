using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Persistence.ValueConverters;

public sealed class UlidToStringConverter() : ValueConverter<Ulid, string>(
    v => v.ToString(),
    v => Ulid.Parse(v));