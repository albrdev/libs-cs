using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace Libs.Settings
{
    [Serializable, XmlType("Version")]
    public struct VersionData
    {
        [XmlAttribute("Major")]
        public int Major { get; private set; }

        [XmlAttribute("Minor")]
        public int Minor { get; private set; }

        [XmlAttribute("Patch")]
        public int Patch { get; private set; }

        [XmlAttribute("Tag")]
        public string Tag { get; private set; }

        [XmlAttribute("Build")]
        public string Build { get; private set; }

        public static bool operator ==(VersionData lhs, VersionData rhs)
        {
            return lhs.Major == rhs.Major && lhs.Minor == rhs.Minor && lhs.Patch == rhs.Patch;
        }

        public static bool operator !=(VersionData lhs, VersionData rhs)
        {
            return lhs.Major != rhs.Major || lhs.Minor != rhs.Minor || lhs.Patch != rhs.Patch;
        }

        public static bool operator >(VersionData lhs, VersionData rhs)
        {
            return lhs.Major > rhs.Major || lhs.Minor > rhs.Minor || lhs.Patch > rhs.Patch;
        }

        public static bool operator <(VersionData lhs, VersionData rhs)
        {
            return lhs.Major < rhs.Major || lhs.Minor < rhs.Minor || lhs.Patch < rhs.Patch;
        }

        public static VersionData operator +(VersionData lhs, VersionData rhs)
        {
            (int Major, int Minor, int Patch) result = lhs;
            if(rhs.Major != 0)
            {
                result.Major += rhs.Major;
                result.Patch = result.Minor = 0;
            }
            else if(rhs.Minor != 0)
            {
                result.Minor += rhs.Minor;
                result.Patch = 0;
            }
            else
            {
                result.Patch += rhs.Patch;
            }

            return result;
        }

        public static VersionData operator -(VersionData lhs, VersionData rhs)
        {
            (int Major, int Minor, int Patch) result = lhs;
            if(rhs.Major != 0)
            {
                result.Major -= rhs.Major;
                result.Patch = result.Minor = 0;
            }
            else if(rhs.Minor != 0)
            {
                result.Minor -= rhs.Minor;
                result.Patch = 0;
            }
            else
            {
                result.Patch -= rhs.Patch;
            }

            return result;
        }

        public static implicit operator VersionData((int Major, int Minor, int Patch) rhs)
        {
            return new VersionData(rhs.Major, rhs.Minor, rhs.Patch);
        }

        public static implicit operator (int Major, int Minor, int Patch)(VersionData rhs)
        {
            return (rhs.Major, rhs.Minor, rhs.Patch);
        }

        public override string ToString()
        {
            return ToString("%F");
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
                case 'p':
                    return Patch.ToString();
                case 't':
                    return Tag;
                case 'b':
                    return Build;

                case 'F':
                    return $"{Major}.{Minor}.{Patch}{(Tag != null ? $"-{Tag}" : null)}{(Build != null ? $"+{Build}" : null)}";
                case 'f':
                    return $"{Major}.{Minor}.{Patch}";
                case 'T':
                    return $"{Major}.{Minor}.{Patch}{(Tag != null ? $"-{Tag}" : null)}";
                case 'B':
                    return $"{Major}.{Minor}.{Patch}{(Build != null ? $"+{Build}" : null)}";

                default:
                    throw new System.FormatException($@"Unknown format flag: '{flag}'");
            }
        }

        public override bool Equals(object obj)
        {
            return obj is VersionData data && Major == data.Major && Minor == data.Minor && Patch == data.Patch && Tag == data.Tag && Build == data.Build;
        }

        public override int GetHashCode()
        {
            var result = 1429805799;
            result = result * -1521134295 + Major.GetHashCode();
            result = result * -1521134295 + Minor.GetHashCode();
            result = result * -1521134295 + Patch.GetHashCode();
            result = result * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Tag);
            result = result * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Build);
            return result;
        }

        public VersionData(VersionData other) : this()
        {
            Major = other.Major;
            Minor = other.Minor;
            Patch = other.Patch;
            Tag = other.Tag;
            Build = other.Build;
        }

        public VersionData(int major, int minor, int patch, string tag = null, string build = null) : this()
        {
            Major = major;
            Minor = minor;
            Patch = patch;
            Tag = tag;
            Build = build;
        }
    }
}
