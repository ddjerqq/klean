using System.Reflection;

namespace Presentation;

public static class PresentationAssembly
{
    public static Assembly Assembly => typeof(PresentationAssembly).Assembly;
}