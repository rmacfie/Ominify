using dotless.Core.Input;
using System.IO;

namespace Ominify.DotLess
{
    public class AbsolutePathResolver : IPathResolver
    {
        readonly string directoryPath;

        public AbsolutePathResolver(string directoryPath)
        {
            this.directoryPath = directoryPath;
        }

        public string GetFullPath(string path)
        {
            return Path.Combine(directoryPath, path);
        }
    }
}