using System;

namespace oris0310.Framework
{
    [AttributeUsage(AttributeTargets.Method)]
    public class PostAttribute : Attribute
    {
        public string Route { get; }
        public PostAttribute(string route) => Route = route;
    }
}
