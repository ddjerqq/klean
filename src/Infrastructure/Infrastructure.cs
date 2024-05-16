using System.Reflection;

namespace Infrastructure;

public static class Infrastructure
{
    public static Assembly Assembly => typeof(Infrastructure).Assembly;
}