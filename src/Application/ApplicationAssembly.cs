using System.Reflection;
using Domain;

namespace Application;

/// <summary>
/// The <see cref="ApplicationAssembly" /> class.
/// </summary>
public static class ApplicationAssembly
{
    /// <summary>
    /// Gets the assembly.
    /// </summary>
    public static Assembly Assembly => typeof(DomainAssembly).Assembly;
}