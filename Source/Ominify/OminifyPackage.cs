namespace Ominify
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Caching;
    using System.Text;

    public abstract class OminifyPackage
    {
        static readonly MemoryCache cache = new MemoryCache("OMinifierPackageCache");
        static readonly string rootFileSystemPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
        static readonly object syncLock = new object();

        readonly List<string> filePaths = new List<string>();
        readonly List<string> fileSystemPaths = new List<string>();
        readonly string packagePath;

        bool isLocked;

        protected OminifyPackage(string packagePath)
        {
            this.packagePath = packagePath;
        }

        public string PackagePath
        {
            get { return packagePath; }
        }

        protected abstract string Minify(string rawFileContent);

        public abstract string GetContentType();

        public abstract string GetHtmlElement(string url);

        public void AddFilePaths(IEnumerable<string> paths)
        {
            foreach (var path in paths)
            {
                AddFilePath(path);
            }
        }

        public void AddFilePath(string path)
        {
            if (isLocked)
                throw new InvalidOperationException("The package is locked for further additions.");

            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("The path must not be null or empty.", "path");

            if (!path.StartsWith("/"))
                throw new ArgumentException("The path must start with a forward slash ('/').", "path");

            if (filePaths.Contains(path))
                throw new ArgumentException("The same path can't be added more than once.", "path");

            var fileSystemPath = GetFileSystemPath(path);

            filePaths.Add(path);
            fileSystemPaths.Add(fileSystemPath);
        }

        public string GetContent(OminifyOptions options)
        {
            var contentItem = GetOrLoadContentItem(options);
            return contentItem.Content;
        }

        public DateTime GetLastModifiedUtc(OminifyOptions options)
        {
            var contentItem = GetOrLoadContentItem(options);
            return contentItem.LastModifiedUtc;
        }

        public IEnumerable<string> GetFilePaths()
        {
            return filePaths.AsReadOnly();
        }

        PackageContentItem GetOrLoadContentItem(OminifyOptions options)
        {
            isLocked = true;

            var content = cache.Get(packagePath) as PackageContentItem;

            if (content == null)
            {
                lock (syncLock)
                {
                    content = CreateContent(options);

                    var cacheItemPolicy = new CacheItemPolicy();

                    if (options.AutoRefreshOnFileChanges)
                    {
                        cacheItemPolicy.ChangeMonitors.Add(new HostFileChangeMonitor(fileSystemPaths));
                    }

                    cache.Set(packagePath, content, cacheItemPolicy);
                }
            }

            return content;
        }

        PackageContentItem CreateContent(OminifyOptions options)
        {
            var topLastModified = DateTime.MinValue;
            var sb = new StringBuilder();

            foreach (var fileSystemPath in fileSystemPaths)
            {
                var content = File.ReadAllText(fileSystemPath);

                if (options.MinifyBundles)
                {
                    content = Minify(content);
                }

                sb.AppendLine(content);

                var lastModified = File.GetLastWriteTimeUtc(fileSystemPath);
                if (topLastModified < lastModified)
                {
                    topLastModified = lastModified;
                }
            }

            return new PackageContentItem(sb.ToString(), topLastModified);
        }

        static string GetFileSystemPath(string path)
        {
            var normalizedPath = path.TrimStart('/').Replace('/', '\\');
            var fileSystemPath = Path.Combine(rootFileSystemPath, normalizedPath);

            return fileSystemPath;
        }

        public class PackageContentItem
        {
            public PackageContentItem(string content, DateTime lastModifiedUtc)
            {
                Content = content;
                LastModifiedUtc = lastModifiedUtc;
            }

            public string Content { get; private set; }

            public DateTime LastModifiedUtc { get; private set; }
        }
    }
}