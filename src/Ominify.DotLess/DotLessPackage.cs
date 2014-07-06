using dotless.Core;
using dotless.Core.Importers;
using dotless.Core.Input;
using dotless.Core.Parser;
using System.IO;

namespace Ominify.DotLess
{
    public class DotLessPackage : OminifyPackage
    {
        public DotLessPackage(string packagePath) : base(packagePath)
        {
        }

        public override string GetContentType()
        {
            return "text/css";
        }

        public override string GetHtmlElement(string url)
        {
            return string.Format("<link rel=\"stylesheet\" href=\"{0}\" />", url);
        }

        protected override string ReadFileContent(string fileSystemPath, bool minify)
        {
            var directory = Path.GetDirectoryName(fileSystemPath);
            var pathResolver = new AbsolutePathResolver(directory);
            var fileReader = new FileReader { PathResolver = pathResolver };
            var importer = new Importer { FileReader = fileReader };
            var parser = new Parser { Importer = importer };
            var lessEngine = new LessEngine { Parser = parser };

            var rawFileContent = base.ReadFileContent(fileSystemPath, false);

            return lessEngine.TransformToCss(rawFileContent, fileSystemPath);
        }
    }
}