using System.Reflection;

namespace Application;

/// <summary>
/// The <see cref="ApplicationAssembly" /> class.
/// </summary>
public static class ApplicationAssembly
{
    /// <summary>
    /// Gets the assembly.
    /// </summary>
    public static Assembly Assembly => typeof(ApplicationAssembly).Assembly;
}