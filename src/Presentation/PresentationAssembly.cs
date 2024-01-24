using System.Reflection;

namespace Presentation;

/// <summary>
/// The <see cref="PresentationAssembly" /> class.
/// </summary>
public static class PresentationAssembly
{
    /// <summary>
    /// Gets the assembly.
    /// </summary>
    public static Assembly Assembly => typeof(PresentationAssembly).Assembly;
}