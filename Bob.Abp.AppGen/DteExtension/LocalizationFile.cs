using EnvDTE;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bob.Abp.AppGen.DteExtension
{
    public class LocalizationFile
    {
        private readonly string _localizationFileName;

        public bool IsDirty { get; set; }

        private bool _forceUpdate;

        private JsonLocalizationFile ResourceFile { get; set; }

        private Dictionary<string, string> _newResource;

        private readonly string spaceKey = "Bob.Abp.AppGen.SpaceKey";

        /// <summary>
        /// Contructor.
        /// </summary>
        /// <param name="localizationPath">The Localization file's folder full path name.</param>
        /// <param name="culture">Culture name, for example : en, zh-Hans...</param>
        /// <param name="forceUpdate">If the localization string exists, then force to update it.</param>
        public LocalizationFile(string localizationPath, string culture, bool forceUpdate = false)
        {
            if (localizationPath != null)
            {
                _localizationFileName = Path.Combine(localizationPath, $"{culture}.json");
                if (!File.Exists(_localizationFileName))
                {
                    ResourceFile = new JsonLocalizationFile { Culture = culture, Texts = new Dictionary<string, string>() };
                    IsDirty = true;
                }
                else
                {
                    ParseJsonFile();
                    IsDirty = false;
                }
            }
            _forceUpdate = forceUpdate;
            _newResource = new Dictionary<string, string>();
        }

        private int spaceLineCnt = 0;

        public void ParseJsonFile()
        {
            string[] rawFile = File.ReadAllLines(_localizationFileName, Encoding.UTF8);
            int lineState = 0; //0-initial line; 1-content line without ",";  2-content line with ","; 3-this is an empty line
            for (int i = 0; i < rawFile.Length; i++)
            {
                var line = rawFile[i].Trim(); //trimed both side spaces.

                #region get new line state according to this line and last line state

                if (line.EndsWith("{") || line.EndsWith("}"))
                {
                    lineState = 0;
                }
                else if (string.IsNullOrEmpty(line))  //is a true empty line
                {
                    //if lineState == 0, last line is an initial line, ingore this empty line just after initial line
                    //if lineState == 1, last line is content and not ended, ingore this empty line
                    //if lineState == 3, this is a repeated empty line, ingore.
                    if (lineState == 2) //this is an empty line after a content line
                    {
                        lineState = 3;  //set line state to empty.
                    }
                }
                else if (line == ",")  //is ","
                {
                    //if lineState == 0 or 2 or 3, last line is an initial line or conent line or empty, this "," is illegal but to ingore.
                    //1-前面内容没有终结，这个终结同时作为空行
                    if (lineState == 1) //last lien is a content line without "," 
                    {
                        lineState = 3;  //complete last line and also treated as an empty line.
                    }
                }
                else if (line.EndsWith(","))  //completed content line
                {
                    lineState = 2;  //a normal content line
                }
                else  //content without "," is not a completed line.
                {
                    lineState = 1;
                }

                #endregion

                if (lineState == 3)  //tag the empty line using {spaceKey}.{n} = "", 
                {
                    rawFile[i] = $"{line}    \"{spaceKey}.{++spaceLineCnt}\": \"\",";  //use {line} to keep the "," if it is a ",".
                }
            }

            ResourceFile = BuildFromJsonString(string.Join(string.Empty, rawFile));
        }

        /// <summary>
        /// Add a localization string.
        /// </summary>
        /// <param name="name">string's name</param>
        /// <param name="value">string's value</param>
        /// <param name="addSpace">if true and name exists then add space line before the name.</param>
        /// <returns>This object for chain call or null if somethign goes wrong.</returns>
        public LocalizationFile Add(string name, string value, bool addSpace = false)
        {
            if (ResourceFile == null) return null;

            if (ResourceFile.Texts.ContainsKey(name)) //if exists move to tail and use 
            {
                if (!_forceUpdate)
                {
                    value = ResourceFile.Texts[name];
                }
                ResourceFile.Texts.Remove(name); //move to tail by rmove it first
                if (addSpace)
                {
                    _newResource.Add($"{spaceKey}.{++spaceLineCnt}", "");
                }
            }

            _newResource.Add(name, value); //add it
            this.IsDirty = true;

            return this;
        }

        public LocalizationFile Add(Dictionary<string, string> propertyResources)
        {
            foreach (var property in propertyResources)
            {
                Add($"DisplayName:{property.Key}", property.Value);
            }
            return this;
        }

        public LocalizationFile Add(Dictionary<string, Dictionary<string, string>> enumResources)
        {
            foreach (var enumName in enumResources.Keys)
            {
                foreach (var enumLanguage in enumResources[enumName])
                {
                    Add(enumLanguage.Key, enumLanguage.Value);
                }
            }

            return this;
        }

        public void Save(bool autoBackup)
        {
            if (IsDirty)
            {
                ResourceFile.Texts = Combine(ResourceFile.Texts, _newResource);
                if (autoBackup) Utils.BackupFile(_localizationFileName);

                List<string> newLines = new List<string>();
                string formattedJsonContent = JsonConvert.SerializeObject(ResourceFile, Formatting.Indented);
                string[] lines = formattedJsonContent.ToLines(StringSplitOptions.None);

                string spacePattern = $"\"{spaceKey}";
                bool lastIsSapce = false;
                for (int i = 0; i < lines.Length; i++)
                {
                    var line = lines[i].Trim();
                    if (line.StartsWith(spacePattern))
                    {
                        if (!lastIsSapce)
                        {
                            newLines.Add(string.Empty);
                        }
                        lastIsSapce = true;
                    }
                    else
                    {
                        newLines.Add(lines[i]);
                        lastIsSapce = false;
                    }
                }
                formattedJsonContent = string.Join(Environment.NewLine, newLines);

                File.WriteAllText(this._localizationFileName, formattedJsonContent, Encoding.UTF8);
            }
        }

        private Dictionary<string, string> Combine(Dictionary<string, string> dict1, Dictionary<string, string> dict2)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (var kv in dict1)
            {
                dict.Add(kv.Key, kv.Value);
            }
            foreach (var kv in dict2)
            {
                dict.Add(kv.Key, kv.Value);
            }

            return dict;
        }

        private JsonLocalizationFile BuildFromJsonString(string jsonString)
        {
            JsonLocalizationFile jsonFile;
            jsonFile = JsonConvert.DeserializeObject<JsonLocalizationFile>(jsonString);

            return jsonFile;
        }

        private class JsonLocalizationFile
        {
            /// <summary>
            /// Culture name; eg : en , en-us, zh-CN
            /// </summary>
            public string Culture { get; set; }

            public Dictionary<string, string> Texts { get; set; }

            public JsonLocalizationFile()
            {
                Texts = new Dictionary<string, string>();
            }
        }
    }


}
