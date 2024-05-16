using System.Reflection;

namespace Application;

public static class Application
{
    public static Assembly Assembly => typeof(Application).Assembly;
}