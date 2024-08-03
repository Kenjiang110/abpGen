using EnvDTE;
using Bob.Abp.AppGen.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;

namespace Bob.Abp.AppGen
{
    public static class Utils
    {
        public const int OLEMS_YES = 6;
        public const int OLEMS_NO = 7;
        public const int OLEMS_CANCEL = 2;

        private static readonly PluralizationService service = PluralizationService.CreateService(CultureInfo.GetCultureInfo("en-us"));

        #region Convert

        public static string ToCamel(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }
            else
            {
                return $"{str[0].ToString().ToLower()}{str.Substring(1)}";
            }
        }

        public static string[] ToCamel(this string[] strs)
        {
            if (strs == null || strs.Length == 0)
            {
                return strs;
            }
            else
            {
                return strs.Select(str => str.ToCamel()).ToArray();
            }
        }

        public static List<string> ToCamel(this List<string> strs)
        {
            if (strs == null || strs.Count == 0)
            {
                return strs;
            }
            else
            {
                return strs.Select(str => str.ToCamel()).ToList();
            }
        }

        public static string[] ToLower(this string[] strs)
        {
            if (strs == null || strs.Length == 0)
            {
                return strs;
            }
            else
            {
                return strs.Select(str => str.ToLower()).ToArray();
            }
        }

        /// <summary>
        /// Convert "CamelPascal" like to "camel-pascal" like.
        /// </summary>
        public static string ToDashed(this string str)
        {
            if (str == null || str.Length == 0)
            {
                return str;
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(char.ToLower(str[0]));
                for (int i = 1; i < str.Length; i++)
                {
                    if (char.IsUpper(str[i]))
                    {
                        sb.Append('-').Append(char.ToLower(str[i]));
                    }
                    else
                    {
                        sb.Append(str[i]);
                    }
                }
                return sb.ToString();
            }
        }

        /// <summary>
        /// Get path of the fullFileName relative to folderPath if fullFileName is in the folder.
        /// </summary>
        /// <returns>fullFileName's full path if not in the folderPath,</returns>
        public static string GetRelativePath(string fullFileName, string folderPath)
        {
            string filePath = Path.GetDirectoryName(fullFileName);
            if (filePath.StartsWith(folderPath))
            {
                return filePath.Remove(0, folderPath.Length).TrimStart(Path.DirectorySeparatorChar);
            }
            else
            {
                return filePath;
            }
        }

        /// <summary>
        /// If typeName is List then return underly type of the List.
        /// </summary>
        public static string GetUnderlyType(this string typeName)
        {
            if (typeName == null || !typeName.IsList())
            {
                return typeName;
            }
            else
            {
                while (typeName.IsList())
                {
                    var sIdx = typeName.IndexOf("<");
                    typeName = typeName.Substring(sIdx + 1);
                }
                var eIdx = typeName.IndexOf(">");
                return typeName.Remove(eIdx);
            }
        }

        /// <summary>
        /// Try to remove end string from target string.
        /// </summary>
        public static string TrimEnd(this string target, string end)
        {
            if (!string.IsNullOrEmpty(target))
            {
                var idx = target.LastIndexOf(end);
                if (idx != -1)
                {
                    return target.Remove(idx);
                }
            }
            return target;
        }

        /// <summary>
        /// Extract all namespace prefix from fullTypeName or property define source code.
        /// </summary>
        /// <param name="fullTypeName">Full type name or source code</param>
        /// <param name="typeName">typeName without namespace prefix</param>
        /// <returns>all namespace prefixies</returns>
        public static List<string> ExtractNamespace(this string fullTypeName, out string typeName)
        {
            List<string> nsList = new List<string>();
            typeName = fullTypeName;

            if (!string.IsNullOrWhiteSpace(fullTypeName))
            {
                //get typename from source code
                var segments = fullTypeName.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                var setgetIdx = Array.FindIndex(segments, s => s.Trim().StartsWith("{"));
                if (setgetIdx > 0)
                {
                    fullTypeName = segments[setgetIdx - 1];
                }
                //extract namespaces
                segments = fullTypeName.Split(new char[] { '<', '>', ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var seg in segments)
                {
                    var lastDotIndex = seg.LastIndexOf('.');
                    if (lastDotIndex > -1)
                    {
                        nsList.Add(seg.Substring(0, lastDotIndex));
                    }
                }
            }

            foreach (var ns in nsList)
            {
                typeName = typeName.Replace(ns + ".", string.Empty);
            }

            return nsList;
        }

        /// <summary>
        /// Splited by Environment.NewLine.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string[] ToLines(this string str, StringSplitOptions options)
        {
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }
            else
            {
                return str.Split(new string[] { Environment.NewLine }, options);
            }
        }

        /// <summary>
        /// Splited by seperator and remove spaces at start and end of every item.
        /// </summary>
        public static List<string> ToList(this string str, char seperator)
        {
            if (string.IsNullOrEmpty(str))
            {
                return new List<string>();
            }
            else
            {
                var result = str.Split(new char[] { seperator }, StringSplitOptions.RemoveEmptyEntries);
                return result.Select(t => t.Trim()).ToList();
            }
        }

        public static string ToSingle(this string str)
        {
            return service.Singularize(str);
        }

        public static string ToPlural(this string str)
        {
            return service.Pluralize(str);
        }

        #endregion

        #region Assert

        /// <summary>
        /// Return if an array is null or it's length is less than miniLength.
        /// </summary>
        public static bool IsEmpty(this vsCMElement[] anyArray, int miniLength = 1)
        {
            return anyArray == null || anyArray.Length < miniLength;
        }

        /// <summary>
        /// Return if an array is null or it's length is less than miniLength.
        /// </summary>
        public static bool IsEmptyOrLessThan(this string[] anyArray, int miniLength = 1)
        {
            return anyArray == null || anyArray.Length < miniLength;
        }

        /// <summary>
        /// Whether the element kind has body?
        /// </summary>
        public static bool HasBody(this vsCMElement kind)
        {
            return kind == vsCMElement.vsCMElementClass
                || kind == vsCMElement.vsCMElementInterface
                || kind == vsCMElement.vsCMElementNamespace
                || kind == vsCMElement.vsCMElementFunction;
        }

        public static bool ToSkip(this Dictionary<string, bool> skipSettings, AbpMainFile abpFile)
        {
            return skipSettings[abpFile.ToString()] == true;
        }

        public static bool ToSkip(this Dictionary<string, bool> skipSettings, AbpMiscFile abpFile)
        {
            return skipSettings[abpFile.ToString()] == true;
        }

        public static bool IsList(this string typeName)
        {
            var setTypes = new string[] { "List<", "IList<", "ICollection<", "Collection<" };
            return setTypes.Any(t => typeName != null && typeName.StartsWith(t));
        }

        public static bool IsSimpleType(this string typeName)
        {
            var simpleTypes = new string[] { "sbyte", "short", "int", "long", "byte", "ushort", "uint", "ulong", "float", "double", "bool", "char",
                "string", "Guid", "DateTime" };
            return simpleTypes.Any(t => typeName != null && typeName.StartsWith(t));
        }

        #endregion

        #region Dictionary

        /// <summary>
        /// Update destDict by srcDict
        /// </summary>
        /// <returns>destDict</returns>
        public static Dictionary<TKey, TValue> UpdateDictionary<TKey, TValue>(this Dictionary<TKey, TValue> destDict, Dictionary<TKey, TValue> srcDict)
        {
            var keys = destDict.Keys.ToArray();
            foreach (var destKey in keys)
            {
                destDict.TryUpdate(destKey, srcDict);
            }

            return destDict;
        }

        /// <summary>
        /// Try to update destDict[destKey] to srcDict[destKey] if destKey exists in destDict and srcDict.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="destDict"></param>
        /// <param name="destKey"></param>
        /// <param name="srcDict"></param>
        public static void TryUpdate<TKey, TValue>(this Dictionary<TKey, TValue> destDict, TKey destKey, Dictionary<TKey, TValue> srcDict)
        {
            if (destDict.ContainsKey(destKey) && srcDict.ContainsKey(destKey))
            {
                destDict[destKey] = srcDict[destKey];
            }
        }

        #endregion

        #region File

        /// <summary>
        /// Try to backup the file if exists.
        /// </summary>
        /// <param name="fullFileName"></param>
        public static void BackupFile(string fullFileName)
        {
            if (!string.IsNullOrEmpty(fullFileName) && File.Exists(fullFileName))
            {
                int i = 0;
                string bakFileName = $"{fullFileName}.bak";
                while (File.Exists(bakFileName))
                {
                    i++;
                    bakFileName = $"{fullFileName}.bak.{i}";
                }

                File.Copy(fullFileName, bakFileName, true);
            }
        }

        /// <summary>
        /// Create temprary file in temprary folder.
        /// </summary>
        /// <returns>full file name of the temprary file.</returns>
        public static string CreateTempFile(string fileName, string content)
        {
            string tempPath = Path.GetTempPath();
            Directory.CreateDirectory(tempPath);
            string fullFileName = Path.Combine(tempPath, fileName);
            File.WriteAllText(fullFileName, content, Encoding.UTF8);
            return fullFileName;
        }

        private static bool CheckIfExists(string fullFolderName, bool throwExceptionIfYes)
        {
            bool exists = Directory.Exists(fullFolderName);
            if (throwExceptionIfYes && exists)
            {
                throw new ApplicationException($"{fullFolderName} already exists but isn't included in the project.");
            }

            return exists;
        }

        /// <summary>
        /// Check if folder who's name is folderName exists in the same folder as file sameFolderFullFileName.
        /// </summary>
        public static bool ExistsInSameFolder(this string folderName, string sameFolderFullFileName, bool throwExceptionIfYes)
        {
            string fullName = Path.Combine(Path.GetDirectoryName(sameFolderFullFileName), folderName);
            return CheckIfExists(fullName, throwExceptionIfYes);
        }

        /// <summary>
        /// Check if file who's name is folderName exists in the fullFolderName folder.
        /// </summary>
        public static bool ExistsInTheFolder(this string folderName, string fullFolderName, bool throwExceptionIfYes)
        {
            string fullName = Path.Combine(fullFolderName, folderName);
            return CheckIfExists(fullName, throwExceptionIfYes);
        }

        public static string GetUpFolder(this string path, bool keepAtLeastOne = true)
        {
            if (string.IsNullOrEmpty(path))
            {
                return path;
            }

            if (path.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                path = path.TrimEnd(Path.DirectorySeparatorChar);
            }

            if (path.StartsWith(Path.DirectorySeparatorChar.ToString()))
            {
                path = path.TrimStart(Path.DirectorySeparatorChar);
            }

            int idx = path.IndexOf(Path.DirectorySeparatorChar);
            if (idx > -1 || !keepAtLeastOne)
            {
                path = path.Substring(0, idx);
            }

            return path;
        }

        #endregion
    }
}
