namespace Ominify.Yui
{
    using Yahoo.Yui.Compressor;

    public class YuiCssPackage : OminifyPackage
    {
        readonly CssCompressor cssCompressor = new CssCompressor
        {
            CompressionType = CompressionType.Standard,
        };

        public YuiCssPackage(string packagePath) : base(packagePath)
        {
        }

        protected override string Minify(string rawFileContent)
        {
            return cssCompressor.Compress(rawFileContent);
        }

        public override string GetContentType()
        {
            return cssCompressor.ContentType;
        }

        public override string GetHtmlElement(string url)
        {
            return string.Format("<link rel=\"stylesheet\" href=\"{0}\" />", url);
        }
    }
}