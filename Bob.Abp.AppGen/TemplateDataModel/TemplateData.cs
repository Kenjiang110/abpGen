using Bob.Abp.AppGen.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Bob.Abp.AppGen.Templates
{
    public class TemplateData : TemplateCoreData
    {
        public bool IsHierarcy { get; set; }

        public bool IsMultiTenant { get; set; }

        public bool IsExtensible { get; set; }

        public bool HasRequestField { get; set; }

        public bool HasStringRequestField { get; set; }

        public bool HasNotStringRequestField { get; set; }

        /// <summary>
        /// Provide batch delete function?
        /// </summary>
        public bool AllowBatchDelete { get; set; }

        public List<string> ExtraUsings { get; set; }

        /// <summary>
        /// Dashed and lowered entity name
        /// </summary>
        public string DashedName { get; set; }

        /// <summary>
        /// Entity namespace
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        /// First letter lowered names.
        /// </summary>
        public TemplateCoreData Lower { get; set; }

        /// <summary>
        /// Properties of the entity used in create.
        /// </summary>
        public List<PropertyInfo> CreateProperties { get; private set; }

        /// <summary>
        /// Properties of the entity used in update.
        /// </summary>
        public List<PropertyInfo> UpdateProperties { get; private set; }

        /// <summary>
        /// Properties of the entity used in create but update.
        /// </summary>
        public List<PropertyInfo> PureCreateProperties { get; private set; }

        /// <summary>
        /// Properties of the entity used in update but create.
        /// </summary>
        public List<PropertyInfo> PureUpdateProperties { get; private set; }

        /// <summary>
        /// Properties of the entity used in both update and create.
        /// </summary>
        public List<PropertyInfo> BasicProperties { get; private set; }

        /// <summary>
        /// Properties of the entity used in request.
        /// </summary>
        public List<PropertyInfo> RequestProperties { get; private set; }

        /// <summary>
        /// Not string properties of the entity used in request.
        /// </summary>
        public List<PropertyInfo> RequestNotStringProperties { get; private set; }

        /// <summary>
        /// String properties of the entity used in request.
        /// </summary>
        public List<PropertyInfo> RequestStringProperties { get; private set; }

        /// <summary>
        /// Properties of the entity used in list.
        /// </summary>
        public List<PropertyInfo> ListProperties { get; private set; }

        /// <summary>
        /// All properties of the entity.
        /// </summary>
        public List<PropertyInfo> AllProperties { get; private set; }

        /// <summary>
        /// Ui Tabs.
        /// </summary>
        public List<TabInfo> UiTabs { get; set; }

        /// <summary>
        /// First basic Ui Tab
        /// </summary>
        public TabInfo BasicUiTab => UiTabs?.FirstOrDefault(t => t.IsBasic);

        /// <summary>
        /// true to use Tab Ui, or use Simple Ui?
        /// </summary>
        public bool UseTabUi { get; set; }

        /// <summary>
        /// All tabs are basic/none tab
        /// </summary>
        public bool OnlyBasicTabUi { get; set; }

        /// <summary>
        /// Variable chain for variable element type.
        /// </summary>
        public List<SimpleChainNode> VariableChain { get; set; }

        public TemplateData(AhEntity entity) : base(
            entity.Solution.RootNamespace,
            entity.Solution.ModuleName,
            entity.Name,
            entity.RelativePath.Split(Path.DirectorySeparatorChar)
            )
        {
            IsExtensible = entity.EntityKind.HasFlag(EntityKinds.Extensible);
            IsHierarcy = entity.EntityKind.HasFlag(EntityKinds.Hierarcy);
            IsMultiTenant = entity.EntityKind.HasFlag(EntityKinds.MultiTenant);
            AllowBatchDelete = entity.AllowBatchDelete;

            Namespace = entity.Namespace;
            DashedName = EntityName.ToDashed();

            var properties = entity.Properties;
            AllProperties = GetProperties(entity);

            RequestProperties = GetProperties(t => t.IsRequestField);
            RequestNotStringProperties = GetProperties(t => t.IsRequestField && !t.IsString);
            RequestStringProperties = GetProperties(t => t.IsRequestField && t.IsString);
            HasRequestField = RequestProperties.Any();
            HasNotStringRequestField = RequestNotStringProperties.Any();
            HasStringRequestField = RequestStringProperties.Any();

            CreateProperties = GetProperties(t => t.IsCreateField);
            UpdateProperties = GetProperties(t => t.IsUpdateField);
            BasicProperties = GetProperties(t => t.IsUpdateField && t.IsCreateField);
            PureCreateProperties = GetProperties(t => t.IsCreateField && !t.IsUpdateField);
            PureUpdateProperties = GetProperties(t => t.IsUpdateField && !t.IsCreateField);

            ListProperties = GetProperties(t => t.IsListField && !t.IsList);

            UiTabs = GetTabs(entity.PropertyGroups, AllProperties);
            UseTabUi = UiTabs.Any(t => t.IsMultiSelect || t.IsSimpleList) || UiTabs.Count > 1; //have special tab or more than 2 tabs
            OnlyBasicTabUi = UiTabs.All(t => t.IsBasic);  //all are basic tabs, so we don't need edit/create js file.

            ExtraUsings = new List<string>(entity.ExtraUsings);

            Lower = new TemplateCoreData(
                RootNamespace.ToCamel(),
                ModuleName.ToCamel(),
                EntityName.ToCamel(),
                EntityGroups.ToCamel()
            );
        }

        protected List<PropertyInfo> GetProperties(AhEntity entity)
        {
            var createFields = entity.CreateFields.ToList(',');
            var requestFields = entity.RequestFields.ToList(',');
            var updateFields = entity.UpdateFields.ToList(',');
            var hiddenFields = entity.HiddenFields.ToList(',');
            var listFields = entity.ListFields.ToList(',');
            var readOnlyFields = entity.ReadOnlyFields.ToList(',');

            var piList = entity.Properties.Select(t => new PropertyInfo
            {
                PropertyType = t.PropertyType,
                PropertyName = t.PropertyName,
                CamelName = t.PropertyName.ToCamel(),
                IsBoolean = string.Compare(t.PropertyType, "bool", true) == 0,
                IsDateTime = string.Compare(t.PropertyType, "DateTime", true) == 0,
                IsEnum = t.IsEnum,
                Nullable = t.Nullable,
                PublicSetter = t.PublicSetter,
                IsDecimal = string.Compare(t.PropertyType, "Decimal", true) == 0,
                IsString = string.Compare(t.PropertyType, "String", true) == 0,
                MaxLengthOrPercise = t.MaxLengthOrPercise,
                MinLengthOrScale = t.MinLengthOrScale,
                Required = t.Required,
                Members = t.Members,
                IsList = t.IsList,
                IsCreateField = createFields.Contains(t.PropertyName),
                IsRequestField = requestFields.Contains(t.PropertyName),
                IsUpdateField = updateFields.Contains(t.PropertyName),
                IsHiddenField = hiddenFields.Contains(t.PropertyName),
                IsListField = listFields.Contains(t.PropertyName),
                ReadOnlyWhenUpdate = readOnlyFields.Contains(t.PropertyName),
                IsLast = false,
                IsFirst = false,
            }).ToList();

            if (piList.Count > 0)
            {
                piList[0].IsFirst = true;
                piList[piList.Count - 1].IsLast = true;
            }
            return piList;
        }

        protected List<PropertyInfo> GetProperties(Func<PropertyInfo, bool> predicate)
        {
            var piList = AllProperties.Where(predicate)
                .Select(t => t.Clone() as PropertyInfo)
                .OrderByDescending(t => t.IsString)  //strings go first
                .ToList();

            piList.ForEach(t => { t.IsLast = false; t.IsFirst = false; });
            if (piList.Count > 0)
            {
                piList[0].IsFirst = true;
                piList[piList.Count - 1].IsLast = true;
            }
            return piList;
        }

        protected List<TabInfo> GetTabs(List<PropertyGroup> propertyGroups, List<PropertyInfo> properties)
        {
            var utList = new List<TabInfo>();
            foreach (var propertyGroup in propertyGroups)
            {
                var ut = new TabInfo
                {
                    Title = propertyGroup.Title,
                    IsBasic = propertyGroup.TabType == PropertyGroupType.BasicTab,
                    IsMultiSelect = propertyGroup.TabType == PropertyGroupType.MultiSelect,
                    IsSimpleList = propertyGroup.TabType == PropertyGroupType.SimpleList,
                };

                var tabProperties = new List<PropertyInfo>();
                var tabPropertyNames = propertyGroup.Properties.ToList(',');
                foreach (var name in tabPropertyNames)
                {
                    var property = properties.FirstOrDefault(t => t.PropertyName == name);
                    if (property != null) tabProperties.Add(property);
                }
                if (tabProperties.Count < tabPropertyNames.Count)
                {
                    throw new ApplicationException($"Can't find all properties '{propertyGroup.Properties}' for Tab {ut.Title}");
                }
                ut.Properties = tabProperties;

                ut.IsUpdateTab = ut.IsBasic || tabProperties.All(t => t.IsUpdateField);  //show in edit modal page
                ut.IsCreateTab = ut.IsBasic || tabProperties.All(t => t.IsCreateField);  //show in create modal page
                utList.Add(ut);
            }

            return utList;
        }
    }
}
