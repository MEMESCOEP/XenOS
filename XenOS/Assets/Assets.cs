using IL2CPU.API.Attribs;

namespace XenOS
{
    public static class Assets
    {
        // Include The Files At Compile Time
        public const string Base = "XenOS.Assets.";
        [ManifestResourceStream(ResourceName = Base + "Default.btf")] public readonly static byte[] Font1B;
    }
}