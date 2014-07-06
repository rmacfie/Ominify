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

        public override string GetContentType()
        {
            return cssCompressor.ContentType;
        }

        public override string GetHtmlElement(string url)
        {
            return string.Format("<link rel=\"stylesheet\" href=\"{0}\" />", url);
        }

        protected override string ReadFileContent(string fileSystemPath, bool minify)
        {
            var rawFileContent = base.ReadFileContent(fileSystemPath, false);

            if (minify)
                return cssCompressor.Compress(rawFileContent);
            else
                return rawFileContent;
        }
    }
}