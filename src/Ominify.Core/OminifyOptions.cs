using System;
using System.Collections.Generic;
using System.Globalization;

namespace Ominify
{
    public class OminifyOptions
    {
        static readonly Func<VersionAlgorithmContext, string> versionAlgorithmDefault = v => v.LastModifiedUtc.Ticks.ToString(CultureInfo.InvariantCulture);

        readonly List<OminifyPackage> packages = new List<OminifyPackage>();

        public OminifyOptions()
        {
            VersionAlgorithm = VersionAlgorithmDefault;
        }

        public Func<VersionAlgorithmContext, string> VersionAlgorithmDefault
        {
            get { return versionAlgorithmDefault; }
        }

        public IEnumerable<OminifyPackage> Packages
        {
            get { return packages.AsReadOnly(); }
        }

        public bool AutoRefreshOnFileChanges { get; set; }

        public bool EnableBundling { get; set; }

        public bool MinifyBundles { get; set; }

        public Func<VersionAlgorithmContext, string> VersionAlgorithm { get; set; }

        public void AddPackage(OminifyPackage package)
        {
            packages.Add(package);
        }

        public class VersionAlgorithmContext
        {
            public DateTime LastModifiedUtc { get; set; }
        }
    }
}