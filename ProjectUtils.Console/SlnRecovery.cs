using System.IO;
using System.Linq;

namespace ProjectUtils.Console
{
    public static class SlnRecovery
    {
        public static void Reco(string slnSrc, string slnDes)
        {
            var slnReader = new SlnReader(slnSrc);
            var csProjectFilePaths = slnReader.GetCsProjPaths();
            var slnDesKeyMap = new SlnReader(slnDes).GetCsProjPaths()
                .ToDictionary(Path.GetFileName, path => path);
            foreach (var csProjectFilePath in csProjectFilePaths)
            {
                File.Copy(csProjectFilePath, slnDesKeyMap[Path.GetFileName(csProjectFilePath)], true);
            }
        }
    }
}