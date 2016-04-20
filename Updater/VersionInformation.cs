using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Updater
{
    public class VersionsInformation
    {
        public bool HasNewVersion { get; set; }
        public Version LatestVersion { get; set; }
        public Dictionary<Version,string> ChangesByVersions { get; set; }
        public void Add(Version version, string info)
        {
            if (ChangesByVersions == null)
            {
                ChangesByVersions = new Dictionary<Version, string>();
            }
            ChangesByVersions.Add(version, info);
        }
        public void SetLatestVersionIfEmpty(Version version)
        {
            if (LatestVersion == null)
            {
                LatestVersion = version;
                HasNewVersion = true;
            }
        }
    }
}
