using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bob.Abp.AppGen.Models
{
    /// <summary>
    /// Model to provide all information about the entity to generate CRUD codes.
    /// </summary>
    public class AhEntity
    {
        /// <summary>
        /// Name of the Entity, for example: public class ClaimType: AggregateRoot<Guid>,
        /// then the EntityName is ClaimType.
        /// </summary>
        [JsonProperty(Order = 3)]
        public string Name { get; set; }

        /// <summary>
        /// Relative path of the Entity file's folder.
        /// For example: \Identity\ClaimTypes\ClaimType.cs, then RelativePath = "Identity\ClaimTypes
        /// </summary>
        [JsonProperty(Order = 2)]
        public string RelativePath { get; set; }

        /// <summary>
        /// Namespace which the Entity belong to.
        /// </summary>
        [JsonProperty(Order = 1)]
        public string Namespace { get; set; }

        /// <summary>
        /// About the entity's character.
        /// </summary>
        [JsonProperty(Order = 4)]
        public EntityKinds EntityKind { get; set; }
        [JsonProperty(Order = 5)]
        public string EntityKindsRemark = "0-None, 1-Entity, 2-Hierarcy, 4-Extensible, 8-MultiTent";

        /// <summary>
        /// Provide batch delete function?
        /// </summary>
        [JsonProperty(Order = 6)]
        public bool AllowBatchDelete { get; set; } = false;

        /// <summary>
        /// Extra usings should be included.
        /// </summary>
        [JsonProperty(Order = 7)]
        public List<string> ExtraUsings { get; set; }

        /// <summary>
        /// All Entity Properties
        /// </summary>
        [JsonProperty(Order = 8)]
        public List<EntityProperty> Properties { get; set; }

        /// <summary>
        /// Used in index list.
        /// </summary>
        [JsonProperty(Order = 8)]
        public string ListFields { get; set; } = String.Empty;

        /// <summary>
        /// Used in request dto.
        /// </summary>
        [JsonProperty(Order = 8)]
        public string RequestFields { get; set; } = String.Empty;

        /// <summary>
        /// Used while create entity.
        /// </summary>
        [JsonProperty(Order = 8)]
        public string CreateFields { get; set; } = String.Empty;

        /// <summary>
        /// Used while update entity.
        /// </summary>
        [JsonProperty(Order = 8)]
        public string UpdateFields { get; set; } = String.Empty;

        /// <summary>
        /// Used while update entity but hidden.
        /// </summary>
        [JsonProperty(Order = 8)]
        public string HiddenFields { get; set; } = String.Empty;

        /// <summary>
        /// Used with update entity but readonly.
        /// </summary>
        [JsonProperty(Order = 8)]
        public string ReadOnlyFields { get; set; } = String.Empty;

        /// <summary>
        /// Entity's properties divide into Tabs
        /// </summary>
        [JsonProperty(Order = 9)]
        public List<PropertyGroup> PropertyGroups { get; set; }

        /// <summary>
        /// Language source for the Entity
        /// </summary>
        [JsonProperty(Order = 10)]
        public List<LanguageResource> Languages { get; set; }

        /// <summary>
        /// Settings controlling whether skip the file generating?
        /// </summary>
        [JsonProperty(Order = 11)]
        public Dictionary<string, bool> SkipSettings { get; set; }

        /// <summary>
        /// Solution which the entity belongs to.
        /// </summary>
        [JsonIgnore]
        public AhSolution Solution { get; set; }

        [JsonIgnore]
        public string FullFileName { get; set; }

        [JsonIgnore]
        public AhEntity SavedEntity { get; set; }

        public AhEntity()
        {
        }

        public AhEntity(EntityKinds entityKind, string prjFullPath, string relativePath, string name, string ns)
        {
            Namespace = ns;
            Name = name;
            RelativePath = relativePath;
            EntityKind = entityKind;
            FullFileName = Path.Combine(prjFullPath, relativePath, name + ".cs.json");
            SavedEntity = null;

            ExtraUsings = new List<string>();
            Properties = new List<EntityProperty>();
            PropertyGroups = new List<PropertyGroup> { new PropertyGroup(null, PropertyGroupType.BasicTab) };

            Languages = new List<LanguageResource>();
        }

        public void AddProperty(EntityProperty property)
        {
            Properties.Add(property);
            string name = property.PropertyName + ",";
            if (!property.IsList)
            {
                ListFields += name;
                var group = PropertyGroups.FirstOrDefault(t => t.TabType == PropertyGroupType.BasicTab);
                group.Properties += name;
            }
            CreateFields += name;
            UpdateFields += name;
        }

        /// <summary>
        /// Initialize SkipSettings dictionary.
        /// </summary>
        public void SetDefaultSkipSettings()
        {
            SkipSettings = new Dictionary<string, bool>
            {
                { AbpMiscFile.Shared_Localization.ToString(), false }
            };
            foreach (AbpMainFile amFile in Enum.GetValues(typeof(AbpMainFile)))
            {
                bool skip = EntityKind.HasFlag(EntityKinds.Extensible) && amFile == AbpMainFile.Web_Page_ExtraJs
                    || amFile == AbpMainFile.HttpApi_Controller
                    || amFile == AbpMainFile.MongoDB_Repository;
                SkipSettings.Add(amFile.ToString(), skip);
            }          
        }

        /// <summary>
        /// Create language resource according basic information about the entity, if the language resource exists, it will be recreated.
        /// </summary>
        /// <param name="newPatten">"New {0}" or "New Entity" for the culture, Default is "New {0}"</param>
        public LanguageResource SetDefaultLanguageResource(string culture, string newPatten = null)
        {
            if (string.IsNullOrEmpty(newPatten)) newPatten = "New {0}";
            newPatten = string.Format(newPatten, Name);
            LanguageResource language = new LanguageResource
            {
                Culture = culture,
                DisplayName = Name,
                NewEntityCmd = newPatten,
                Properties = new Dictionary<string, string>(),
                Enums = new Dictionary<string, Dictionary<string, string>>()
            };
            foreach (var property in Properties)
            {
                language.Properties[property.PropertyName] = property.PropertyName;
                if (property.IsEnum)
                {
                    language.Enums[property.PropertyType] = CreateDefaultEnumLanguageResource(property);
                }
            }

            var lang = Languages.FirstOrDefault(t => t.Culture == culture);
            if (lang != null) Languages.Remove(lang);
            Languages.Add(language);

            return language;
        }

        private Dictionary<string, string> CreateDefaultEnumLanguageResource(EntityProperty property)
        {
            var enumDict = new Dictionary<string, string>();
            foreach (var member in property.Members)
            {
                enumDict[$"Enum:{property.PropertyType}.{member}"] = member;
            }
            return enumDict;
        }

        private void MergeLanguageResources(List<LanguageResource> languages)
        {
            foreach (LanguageResource srcLang in languages)
            {
                //if dest language resource doesn't exist then create a default one
                LanguageResource destLang = Languages.FirstOrDefault(t => t.Culture == srcLang.Culture);
                if (destLang == null)
                {
                    //make sure dest language exists.
                    destLang = SetDefaultLanguageResource(srcLang.Culture, srcLang.NewEntityCmd);
                }
                //use src language to update dest language
                destLang.DisplayName = srcLang.DisplayName;
                destLang.NewEntityCmd = srcLang.NewEntityCmd;
                foreach (var property in Properties)
                {
                    destLang.Properties.TryUpdate(property.PropertyName, srcLang.Properties);

                    if (property.IsEnum && srcLang.Enums.ContainsKey(property.PropertyType))
                    {
                        destLang.Enums[property.PropertyType].UpdateDictionary(srcLang.Enums[property.PropertyType]);
                    }
                }
            }
        }

        public void MergeSavedEntity()
        {
            if (SavedEntity != null)
            {
                SkipSettings.UpdateDictionary(SavedEntity.SkipSettings);
                MergeLanguageResources(SavedEntity.Languages);
            }
        }

        /// <summary>
        /// Save to file.
        /// </summary>
        /// <param name="autoBackup">backup old file.</param>
        /// <returns>Self for chain calling.</returns>
        public AhEntity SaveEntityModel(bool autoBackup = false)
        {
            //try to get old extra information
            if (File.Exists(FullFileName))
            {
                if (autoBackup)
                {
                    Utils.BackupFile(FullFileName); //backup old one
                }
                File.Delete(FullFileName);   //delete old one
            }
            //save it
            string content = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(FullFileName, content);

            return this;
        }
    }
}
