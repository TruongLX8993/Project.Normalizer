using System.Diagnostics;

namespace ProjectUtils
{
    public class MsBuildProcess
    {
        public static void Start(string sln)
        {
            var p = new Process();
            p.StartInfo = new ProcessStartInfo(@"C:\Program Files (x86)\MSBuild\14.0\Bin\msbuild.exe")
            {
                Arguments = string.Format(sln)
            };
            p.Start();
        }
    }
}