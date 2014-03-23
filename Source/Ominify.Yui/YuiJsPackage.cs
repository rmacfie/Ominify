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

        protected override string Minify(string rawFileContent)
        {
            return jsCompressor.Compress(rawFileContent);
        }

        public override string GetContentType()
        {
            return jsCompressor.ContentType;
        }

        public override string GetHtmlElement(string url)
        {
            return string.Format("<script src=\"{0}\"></script>", url);
        }
    }
}