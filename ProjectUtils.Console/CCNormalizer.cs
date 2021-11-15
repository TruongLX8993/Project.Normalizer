using System.Xml;

namespace ProjectUtils.Console
{
    public class CCNormalizer
    {
        private class CustomNormalizer : ProjectDllCustomizer
        {
            public override CustomizerAction NormalizeDllRef(
                ProjectRefNormalizerContext context, XmlNode refNode, XmlNode hintNode)
            {
                return CustomizerAction.NORMAL;
            }
        }

        public static void Normalize()
        {
            const string slnPath = @"E:\Newfolder\onemes3.codes\ONEMES3.CC\ONEMES3.BN.sln";
            const string originSlnPath = @"E:\ONEMES3.CC\ONEMES3.CC.sln";
            const string libPath = @"E:\Newfolder\onemes3.codes\Libs";
            const string corePath = @"E:\Newfolder\onemes3.codes\ONEMES3.KCB\CoreDlls";
            SlnRecovery.Reco(originSlnPath, slnPath);
            var slnNormalizer =
                new SlnNormalizer(slnPath, corePath, libPath, "onemes3", Constance.OutPutMap,
                    new CustomNormalizer());
            slnNormalizer.Normalize();
        }
    }
}