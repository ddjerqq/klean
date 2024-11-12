using System.ComponentModel.DataAnnotations;

namespace Domain.ValueObjects;

[Flags]
public enum Role
{
    [Display(Name = "user")]
    User = 0b00001,

    [Display(Name = "staff")]
    Staff = 0b00010,

    [Display(Name = "administrator")]
    Admin = 0b00100,
}

public static class RoleExt
{
    public const string User = "1";
    public const string Staff = "2";
    public const string Admin = "4";
}