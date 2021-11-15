// using System.Collections.Generic;
//
// namespace ProjectUtils
// {
//     public class SlnNormalizerParameter
//     {
//         private readonly SlnReader _slnReader;
//         private readonly DirectoryProxy _coreDirProxy;
//         private readonly DirectoryProxy _libDirProxy;
//         private readonly string _sysPrefix;
//         private readonly IDictionary<string, string> _outMap;
//         private readonly ProjectDllCustomizer _customizer;
//
//         public class SlnNormalizerParameterBuilder
//         {
//             private string _slnPath;
//             private string _prefix;
//             private SlnNormalizerParameter _parameter;
//             
//             public SlnNormalizerParameterBuilder(string slnPath, string sysPrefix)
//             {
//                 _slnPath = slnPath;
//                 _prefix = sysPrefix;
//             }
//
//             public SlnNormalizerParameterBuilder SetCoreDll(string coreDllDir)
//             {
//                 
//             }
//
//             public SlnNormalizerParameterBuilder SetSlnCore(string slnCoreDllDir)
//             {
//             }
//
//             public SlnNormalizerParameterBuilder OutMap(IDictionary<string, string> outMap)
//             {
//             }
//
//             public SlnNormalizerParameterBuilder ProjectRefHandler()
//             {
//             }
//
//             public SlnNormalizerParameterBuilder DllRefHandler()
//             {
//             }
//         }
//     }
// }