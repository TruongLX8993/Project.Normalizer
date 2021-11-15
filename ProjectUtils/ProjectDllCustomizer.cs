using System.Xml;

namespace ProjectUtils
{
    public enum CustomizerAction
    {
        APPENDED,
        GET_FROM_CORE,
        GET_FROM_CORE_SLN,
        GET_FROM_LIB,
        NO_ACTION,
        NORMAL,
        REMOVE,
    }


    public abstract class ProjectRefCustomizer
    {
        public abstract CustomizerAction Normalize(
            ProjectRefNormalizerContext context,
            XmlNode refNode,
            string refToProject);

       
    }

    public abstract class ProjectDllCustomizer
    {
        protected string CurrentFolder;


        public abstract CustomizerAction NormalizeDllRef(
            ProjectRefNormalizerContext context,
            XmlNode refNode,
            XmlNode hintNode);


        public string GetFileName(XmlNode hintNode)
        {
            return PathUtil.GetFileName(hintNode.InnerText);
        }
    }
}