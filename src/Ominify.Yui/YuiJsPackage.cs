namespace Ominify.Yui
{
    using Yahoo.Yui.Compressor;

    public class YuiJsPackage : OminifyPackage
    {
        readonly JavaScriptCompressor jsCompressor = new JavaScriptCompressor
        {
            CompressionType = CompressionType.Standard,
        };

        public YuiJsPackage(string packagePath) : base(packagePath)
        {
        }

        public override string GetContentType()
        {
            return jsCompressor.ContentType;
        }

        public override string GetHtmlElement(string url)
        {
            return string.Format("<script src=\"{0}\"></script>", url);
        }

        protected override string ReadFileContent(string fileSystemPath, bool minify)
        {
            var rawFileContent = base.ReadFileContent(fileSystemPath, false);

            if (minify)
                return jsCompressor.Compress(rawFileContent);
            else
                return rawFileContent;
        }
    }
}