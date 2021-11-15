namespace ProjectUtils.Console
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            // WfSlnNormalizer.Normalize(); // 1
            // SysSlnNormalizer.Normalize(); // 2
            // BnNormalizer.Normalize();
            DmDllRefNormalizer.Normalize();
            // CCNormalizer.Normalize();
        }
    }
}