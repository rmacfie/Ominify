namespace Ominify
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Ominifier
    {
        static OminifyOptions options;
        static IDictionary<string, OminifyPackage> packages;

        readonly Func<IDictionary<string, object>, Task> next;

        public Ominifier(Func<IDictionary<string, object>, Task> next)
        {
            this.next = next;
        }

        public async Task Invoke(IDictionary<string, object> environment)
        {
            var path = (string)environment["owin.RequestPath"];

            OminifyPackage package;
            if (!packages.TryGetValue(path, out package))
            {
                await next(environment);
                return;
            }

            var responseHeaders = (IDictionary<string, string[]>)environment["owin.ResponseHeaders"];
            var responseBody = (Stream)environment["owin.ResponseBody"];

            responseHeaders["Content-Type"] = new[] { package.GetContentType() };
            responseHeaders["Access-Control-Allow-Origin"] = new[] { "*" };
            responseHeaders["Cache-Control"] = new[] { TimeSpan.FromDays(1).TotalSeconds.ToString(CultureInfo.InvariantCulture) };
            responseHeaders["Expires"] = new[] { DateTime.UtcNow.AddYears(1).ToString("R") };
            responseHeaders["Last-Modified"] = new[] { package.GetLastModifiedUtc(options).ToString("R") };

            using (var writer = new StreamWriter(responseBody))
            {
                await writer.WriteAsync(package.GetContent(options));
            }
        }

        public static void Initialize(OminifyOptions minifierOptions)
        {
            if (options != null)
                throw new InvalidOperationException("The Ominifier can only be initialized once.");

            options = minifierOptions;
            packages = options.Packages.ToDictionary(x => x.PackagePath, x => x);
        }

        public static string GetElement(string packagePath)
        {
            var package = GetPackageOrThrow(packagePath);

            if (options.EnableBundling)
            {
                var url = GetUrl(package);
                return package.GetHtmlElement(url);
            }
            var sb = new StringBuilder();

            foreach (var filePath in package.GetFilePaths())
            {
                var htmlElement = package.GetHtmlElement(filePath);
                sb.AppendLine(htmlElement);
            }

            return sb.ToString();
        }

        public static string GetUrl(string packagePath)
        {
            var package = GetPackageOrThrow(packagePath);
            return GetUrl(package);
        }

        static string GetUrl(OminifyPackage package)
        {
            return string.Format("{0}?t={1}", package.PackagePath, package.GetLastModifiedUtc(options).Ticks);
        }

        static OminifyPackage GetPackageOrThrow(string packagePath)
        {
            OminifyPackage package;
            if (!packages.TryGetValue(packagePath, out package))
            {
                throw new ArgumentOutOfRangeException("packagePath", string.Format("Package not found (path: '{0}').", packagePath));
            }
            return package;
        }
    }
}