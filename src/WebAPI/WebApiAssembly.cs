using System.Reflection;

namespace WebAPI;

/// <summary>
/// The <see cref="WebApiAssembly" /> class.
/// </summary>
public static class WebApiAssembly
{
    /// <summary>
    /// Gets the assembly.
    /// </summary>
    public static Assembly Assembly => typeof(WebApiAssembly).Assembly;
}