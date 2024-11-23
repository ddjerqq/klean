using Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

internal class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasIndex(x => x.FullName)
            .IsUnique();

        // builder.Property<CultureInfo>()
        //     .HasConversion(culture => culture.Name, name => new CultureInfo(name))
        //     .HasMaxLength(6);

        // builder.Property<IEdition>()
        //     .HasConversion(e => EditionToString(e), s => StringToEdition(s))
        //     .HasMaxLength(11);

    }

    // private static string EditionToString(IEdition edition) => edition switch
    // {
    //     OrdinalEdition ordinal => $"{ordinal.Number}",
    //     SeasonalEdition seasonal => $"{Enum.GetName(seasonal.Season)} {seasonal.Year}",
    //     _ => throw new ArgumentException("Edition type not supported yet"),
    // };
    //
    // private static IEdition StringToEdition(string edition) => edition.Split(' ') switch
    // {
    //     [var season, var year] => new SeasonalEdition((Season)Enum.Parse(typeof(Season), season), int.Parse(year)),
    //     [var number] => new OrdinalEdition(int.Parse(number)),
    //     _ => throw new ArgumentException("Edition type not supported yet"),
    // };
}

// public abstract record PublicationDate;
//
// public sealed record FullDate(DateOnly Date) : PublicationDate;
// public sealed record YearMonth(int Year, int Month) : PublicationDate;
// public sealed record Year(int Number) : PublicationDate;
//
// public static class PublicationDateExt
// {
//     public static TResult Map<TResult>(this PublicationDate publicationDate,
//         Func<FullDate, TResult> fullDate,
//         Func<YearMonth, TResult> yearMonth,
//         Func<Year, TResult> year) => publicationDate switch
//     {
//         FullDate p => fullDate(p),
//         YearMonth p => yearMonth(p),
//         Year p => year(p),
//         _ => throw new ArgumentException("Publication date type not supported yet"),
//     };
// }
//
// public abstract record PublicationInfo;
//
// public sealed record Published(DateTime PublishedOn) : PublicationInfo;
//
// public sealed record Planned(DateTime PlannedFor) : PublicationInfo;
//
// public sealed record NotPlanned : PublicationInfo;
//
// public static class PublicationInfoExt
// {
//     public static TResult Map<TResult>(this PublicationInfo publication,
//         Func<Published, TResult> published,
//         Func<Planned, TResult> planned,
//         Func<NotPlanned, TResult> notPlanned) => publication switch
//     {
//         Published p => published(p),
//         Planned p => planned(p),
//         NotPlanned p => notPlanned(p),
//         _ => throw new ArgumentException("Publication type not supported yet"),
//     };
// }