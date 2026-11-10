#if UNITY_EDITOR
using System;
using UnityEditor;

namespace Unity.Pipeline.PlayerSupport
{
    internal struct NamedBuildTarget : IEquatable<NamedBuildTarget>
    {
        public BuildTargetGroup TargetGroup { get; }
        public string TargetName => TargetGroup.ToString();

        public NamedBuildTarget(BuildTargetGroup group) => TargetGroup = group;

        public static NamedBuildTarget FromBuildTargetGroup(BuildTargetGroup group) => new NamedBuildTarget(group);
        public static NamedBuildTarget Server => new NamedBuildTarget(BuildTargetGroup.Standalone);
        public static NamedBuildTarget Standalone => new NamedBuildTarget(BuildTargetGroup.Standalone);
        public static NamedBuildTarget Unknown => new NamedBuildTarget(BuildTargetGroup.Unknown);

        public bool Equals(NamedBuildTarget other) => TargetGroup == other.TargetGroup;
        public override bool Equals(object obj) => obj is NamedBuildTarget other && Equals(other);
        public override int GetHashCode() => (int)TargetGroup;
        public static bool operator ==(NamedBuildTarget left, NamedBuildTarget right) => left.Equals(right);
        public static bool operator !=(NamedBuildTarget left, NamedBuildTarget right) => !left.Equals(right);
        public override string ToString() => TargetName;
    }

    internal enum StandaloneBuildSubtarget
    {
        Default = 0,
        Server = 1
    }
}
#endif
