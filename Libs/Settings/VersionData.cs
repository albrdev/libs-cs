using System;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace Libs.Settings
{
    [Serializable, XmlType("Version")]
    public struct VersionData
    {
        public enum RevisionType
        {
            a = 0,
            b = 1,
            rc = 2,
            r = 3
        }

        [XmlAttribute("Major")]
        public int Major { get; private set; }

        [XmlAttribute("Minor")]
        public int Minor { get; private set; }

        [XmlAttribute("Maintenance")]
        public int Maintenance { get; private set; }

        [XmlAttribute("Revision")]
        public RevisionType Revision { get; private set; }

        [XmlAttribute("Build")]
        public int Build { get; private set; }

        public override string ToString()
        {
            return ToString("%M.%m.%i%R");
        }

        public string ToString(string format)
        {
            return Regex.Replace(format, @"%(?<flag>.?)", MatchEvaluator);
        }

        private string MatchEvaluator(Match match)
        {
            string value = match.Groups["flag"].Value;
            if(value == string.Empty)
                return string.Empty;

            char flag = value[0];
            switch(flag)
            {
                case '%':
                    return "%";
                case 'M':
                    return Major.ToString();
                case 'm':
                    return Minor.ToString();
                case 'i':
                    return Maintenance.ToString();
                case 'r':
                    return Revision.ToString();
                case 'R':
                    return Revision != RevisionType.r ? Revision.ToString() : string.Empty;
                case 'n':
                    return ((int)Revision).ToString();
                case 'N':
                    return Revision != RevisionType.r ? ((int)Revision).ToString() : string.Empty;
                case 'b':
                    return Build != 0 ? Build.ToString() : string.Empty;
                default:
                    throw new System.FormatException($@"Unknown format flag: '{flag}'");
            }
        }

        public VersionData(VersionData other) : this()
        {
            Major = other.Major;
            Minor = other.Minor;
            Maintenance = other.Maintenance;
            Revision = other.Revision;
            Build = other.Build;
        }

        public VersionData(int major, int minor, int maintenance, RevisionType revision = RevisionType.r, int build = 0) : this()
        {
            Major = major;
            Minor = minor;
            Maintenance = maintenance;
            Revision = revision;
            Build = build;
        }
    }
}
