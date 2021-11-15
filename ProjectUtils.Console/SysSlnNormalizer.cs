using System.Xml;

namespace ProjectUtils.Console
{
    public static class SysSlnNormalizer
    {
        private class CustomSysNormalizer : ProjectDllCustomizer
        {
            public override CustomizerAction NormalizeDllRef(
                ProjectRefNormalizerContext context, XmlNode refNode, XmlNode hintNode)
            {
                return CustomizerAction.NORMAL;
            }
        }

        public static void Normalize()
        {
            const string slnPath = @"E:\Newfolder\onemes3.codes\ONEMES3.SYS\ONEMES3.SYS.sln";
            const string originSlnPath = @"E:\ONEMES3.SYS\ONEMES3.SYS.sln";
            const string libPath = @"E:\Newfolder\onemes3.codes\Libs";
            const string corePath = @"E:\Newfolder\onemes3.codes\ONEMES3.HT\CoreDlls";
            SlnRecovery.Reco(originSlnPath, slnPath);
            var slnNormalizer =
                new SlnNormalizer(slnPath, corePath, libPath, "onemes3", Constance.OutPutMap,
                    new CustomSysNormalizer());
            slnNormalizer.Normalize();
        }
    }
}