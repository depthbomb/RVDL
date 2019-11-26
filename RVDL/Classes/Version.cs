﻿namespace RVDL
{
    public static class Version
    {
        public static System.Version AsDotNetVersion() => new System.Version(Major, Minor, Patch, Hotfix);
        private static int Major => 2;
        private static int Minor => 0;
        private static int Patch => 0;
        private static int Hotfix => 0;
        public static ReleaseType ReleaseType => ReleaseType.PreRelease;
        public static string FullVersion => $"{Major}.{Minor}.{Patch}.{Hotfix}-{ReleaseType}";
    }

    public enum ReleaseType
    {
        Development,
        PreRelease,
        Release
    }
}
