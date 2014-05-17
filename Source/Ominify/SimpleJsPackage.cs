namespace Ominify
{
    public class SimpleJsPackage : OminifyPackage
    {
        public SimpleJsPackage(string packagePath) : base(packagePath)
        {
        }

        protected override string Minify(string rawFileContent)
        {
            return rawFileContent;
        }

        public override string GetContentType()
        {
            return "application/javascript";
        }

        public override string GetHtmlElement(string url)
        {
            return string.Format("<script src=\"{0}\"></script>", url);
        }
    }
}