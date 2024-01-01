using System.Reflection;

namespace Domain;

/// <summary>
/// The <see cref="DomainAssembly" /> class.
/// </summary>
public static class DomainAssembly
{
    /// <summary>
    /// Gets the assembly.
    /// </summary>
    public static Assembly Assembly => typeof(DomainAssembly).Assembly;
}