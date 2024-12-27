using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Persistence.ValueConverters;

internal sealed class MoneyToStringConverter() : ValueConverter<Money, string>(
    v => v.ToString(),
    v => Money.Parse(v));