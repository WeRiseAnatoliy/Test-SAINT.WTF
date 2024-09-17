using System;

namespace TestTask.UI
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ResourcePathAttribute : Attribute
    {
        public string Path;

        public ResourcePathAttribute(string path)
        {
            Path = path;
        }
    }
}