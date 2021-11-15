using System.Xml;

namespace ProjectUtils.Console
{
    public static class WfSlnNormalizer
    {
        private class WfCus : ProjectDllCustomizer
        {
            public override CustomizerAction NormalizeDllRef(
                ProjectRefNormalizerContext context, XmlNode refNode, XmlNode hintNode)
            {
                var fileName = GetFileName(hintNode);
                if (context.GetProjectFileName()
                        .ContainIgnoreCase("WF.WorkflowUtility") &&
                    fileName.ContainIgnoreCase("WF.ObjectWorkflow.Utility"))
                {
                    return CustomizerAction.GET_FROM_CORE;
                }

                if (context.GetProjectFileName()
                        .ContainIgnoreCase("WF.core.processwebservice") &&
                    fileName.ContainIgnoreCase("WF.webserviceutility"))
                {
                    return CustomizerAction.GET_FROM_CORE;
                }


                return CustomizerAction.NORMAL;
            }
        }


        public static void Normalize()
        {
            const string slnPath = @"E:\Newfolder\onemes3.codes\ONEMES3.WF\ONEMES3.WF.sln";
            const string originSlnPath = @"E:\ONEMES3.WF\ONEMES3.WF.sln";
            const string libPath = @"E:\Newfolder\onemes3.codes\Libs";
            const string corePath = @"E:\Newfolder\onemes3.codes\ONEMES3.WF\CoreDlls";

            SlnRecovery.Reco(originSlnPath, slnPath);
            var slnNormalizer =
                new SlnNormalizer(slnPath, corePath, libPath, "onemes3", Constance.OutPutMap, new WfCus());
            slnNormalizer.Normalize();
            MsBuildProcess.Start(slnPath);
        }
    }
}