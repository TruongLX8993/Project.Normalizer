using System.Xml;

namespace ProjectUtils.Console
{
    public class DmProjectRefNormalizer : ProjectRefCustomizer
    {
        public override CustomizerAction Normalize(
            ProjectRefNormalizerContext context, XmlNode refNode, string refToProject)
        {
            
            // return CustomizerAction.GET_FROM_CORE;
            
            System.Console.WriteLine($"{nameof(DmProjectRefNormalizer)}:refToProject");
            if (context.IsProjectCurrent("dm.bussiness.sql"))
            {
                if (refToProject.ContainIgnoreCase("dm.gui.webparts"))
                    return CustomizerAction.GET_FROM_CORE;
                if (refToProject.ContainIgnoreCase("dm.call.bussiness.template"))
                    return CustomizerAction.GET_FROM_CORE;
            }
            
            if (context.IsProjectCurrent("dm.bussiness.utility"))
            {
                if (refToProject.ContainIgnoreCase("dm.call.bussiness.sql"))
                    return CustomizerAction.GET_FROM_CORE;
            }
            if (context.IsProjectCurrent("dm.gui.importutility"))
            {
                if (refToProject.ContainIgnoreCase("dm.webparts"))
                    return CustomizerAction.GET_FROM_CORE;
            }

            return CustomizerAction.NORMAL;
        }
    }

    public class DmDllRefNormalizer
    {
        private class CustomNormalizer : ProjectDllCustomizer
        {
            public override CustomizerAction NormalizeDllRef(
                ProjectRefNormalizerContext context, XmlNode refNode, XmlNode hintNode)
            {
                // return CustomizerAction.GET_FROM_CORE;

                var fileName = GetFileName(hintNode);
                System.Console.WriteLine(fileName);
                var currentProject = context.GetProjectFileName();

                if (currentProject.ContainIgnoreCase("dm.bussiness.sql"))
                {
                    if (fileName.ContainIgnoreCase("dm.call.bussiness.sql"))
                        return CustomizerAction.GET_FROM_CORE;
                    if (fileName.ContainIgnoreCase("dm.call.bussiness.template"))
                        return CustomizerAction.GET_FROM_CORE;
                    if (fileName.ContainIgnoreCase("dm.bussiness.utility"))
                        return CustomizerAction.GET_FROM_CORE;
                    if (fileName.ContainIgnoreCase("dm.gui.webparts"))
                        return CustomizerAction.REMOVE;
                }

                if (context.IsProjectCurrent("dm.bussiness.utility"))
                {
                    if (fileName.ContainIgnoreCase("dm.call.bussiness.sql"))
                        return CustomizerAction.GET_FROM_CORE;
                }
                
                if (context.IsProjectCurrent("dm.gui.importutility"))
                {
                    if (fileName.ContainIgnoreCase("dm.webparts"))
                        return CustomizerAction.GET_FROM_CORE;
                }

                return CustomizerAction.NORMAL;
            }
        }

        public static void Normalize()
        {
            const string slnPath = @"E:\Newfolder\onemes3.codes\ONEMES3.DM\ONEMES3.DM.sln";
            const string originSlnPath = @"E:\ONEMES3.DM\ONEMES3.DM.sln";
            const string libPath = @"E:\Newfolder\onemes3.codes\Libs";
            const string corePath = @"E:\Newfolder\onemes3.codes\ONEMES3.HT\CoreDlls";
            SlnRecovery.Reco(originSlnPath, slnPath);
            var slnNormalizer =
                new SlnNormalizer(slnPath, corePath, libPath, "onemes3", Constance.OutPutMap,
                    new CustomNormalizer());
            slnNormalizer.ProjectRefCustomizer = new DmProjectRefNormalizer();
            slnNormalizer.Normalize();
        }
    }
}