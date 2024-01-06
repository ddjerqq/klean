using System.Reflection;

namespace Infrastructure;

/// <summary>
/// The <see cref="InfrastructureAssembly" /> class.
/// </summary>
public static class InfrastructureAssembly
{
    /// <summary>
    /// Gets the assembly.
    /// </summary>
    public static Assembly Assembly => typeof(InfrastructureAssembly).Assembly;
}