namespace Ominify
{
    using System.Collections.Generic;

    public class OminifyOptions
    {
        readonly List<OminifyPackage> packages = new List<OminifyPackage>();

        public IEnumerable<OminifyPackage> Packages
        {
            get { return packages.AsReadOnly(); }
        }

        public bool AutoRefreshOnFileChanges { get; set; }

        public bool EnableBundling { get; set; }

        public bool MinifyBundles { get; set; }

        public void AddPackage(OminifyPackage package)
        {
            packages.Add(package);
        }
    }
}