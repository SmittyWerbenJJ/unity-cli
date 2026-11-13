using System;
using UnityEngine;

namespace Unity.Pipeline.Editor.Compat
{
    /// <summary>
    /// Shared Unity runtime version parser and comparator for version-specific API adapters
    /// inside the single precompiled Unity.Pipeline.Editor assembly.
    /// </summary>
    public readonly struct UnityRuntimeVersion : IComparable<UnityRuntimeVersion>
    {
        public int Major { get; }
        public int Minor { get; }
        public int Patch { get; }
        public string Raw { get; }

        public UnityRuntimeVersion(int major, int minor, int patch, string raw = null)
        {
            Major = major;
            Minor = minor;
            Patch = patch;
            Raw = raw ?? $"{major}.{minor}.{patch}";
        }

        public static UnityRuntimeVersion Current => Parse(Application.unityVersion);

        public static UnityRuntimeVersion Parse(string unityVersion)
        {
            if (string.IsNullOrWhiteSpace(unityVersion))
            {
                return new UnityRuntimeVersion(0, 0, 0, unityVersion ?? string.Empty);
            }

            string[] parts = unityVersion.Trim().Split('.');
            int major = parts.Length > 0 ? ParseLeadingInt(parts[0]) : 0;
            int minor = parts.Length > 1 ? ParseLeadingInt(parts[1]) : 0;
            int patch = parts.Length > 2 ? ParseLeadingInt(parts[2]) : 0;
            return new UnityRuntimeVersion(major, minor, patch, unityVersion);
        }

        public bool IsAtLeast(int major, int minor = 0, int patch = 0) =>
            CompareTo(new UnityRuntimeVersion(major, minor, patch)) >= 0;

        public bool IsBelow(int major, int minor = 0, int patch = 0) =>
            CompareTo(new UnityRuntimeVersion(major, minor, patch)) < 0;

        public int CompareTo(UnityRuntimeVersion other)
        {
            int c = Major.CompareTo(other.Major);
            if (c != 0) return c;
            c = Minor.CompareTo(other.Minor);
            if (c != 0) return c;
            return Patch.CompareTo(other.Patch);
        }

        private static int ParseLeadingInt(string segment)
        {
            if (string.IsNullOrEmpty(segment)) return 0;
            int len = 0;
            while (len < segment.Length && char.IsDigit(segment[len])) len++;
            return len > 0 && int.TryParse(segment.Substring(0, len), out int val) ? val : 0;
        }
    }
}
