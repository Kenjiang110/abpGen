using Bob.Abp.AppGen.Models;
using Newtonsoft.Json;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bob.Abp.AppGen.Dialogs.ViewModel
{
    /// <summary>
    /// Model to provide all information about the entity to generate CRUD codes.
    /// </summary>
    public class EntityViewModel : NotifyClassBase
    {
        #region Core properties

        /// <summary>
        /// Name of the Entity, for example: public class ClaimType: AggregateRoot<Guid>,
        /// then the EntityName is ClaimType.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Relative path of the Entity file's folder.
        /// For example: \Identity\ClaimTypes\ClaimType.cs, then RelativePath = "Identity\ClaimTypes
        /// </summary>
        public string RelativePath { get; set; }

        /// <summary>
        /// Namespace which the Entity belong to.
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        /// About the entity's character.
        /// </summary>
        public EntityKinds EntityKind { get; set; }

        /// <summary>
        /// Provide batch delete function?
        /// </summary>
        public bool AllowBatchDelete { get; set; } = false;

        /// <summary>
        /// Extra usings should be included.
        /// </summary>
        public ObservableCollection<string> ExtraUsings { get; set; }

        /// <summary>
        /// All Entity Properties
        /// </summary>
        public ObservableCollection<PropertyViewModel> Properties { get; set; }

        /// <summary>
        /// Used in index list.
        /// </summary>
        public string ListFields { get; set; }

        /// <summary>
        /// Used in request dto.
        /// </summary>
        public string RequestFields { get; set; }

        /// <summary>
        /// Used while create entity.
        /// </summary>
        public string CreateFields { get; set; }

        /// <summary>
        /// Used while update entity.
        /// </summary>
        public string UpdateFields { get; set; }

        /// <summary>
        /// Used while update entity but hidden.
        /// </summary>
        public string HiddenFields { get; set; }

        /// <summary>
        /// Used with update entity but readonly.
        /// </summary>
        public string ReadOnlyFields { get; set; }

        /// <summary>
        /// Entity's properties divide into Tabs
        /// </summary>
        public ObservableCollection<PropertyGroupViewModel> PropertyGroups { get; set; }

        /// <summary>
        /// Language source for the Entity
        /// </summary>
        public ObservableCollection<LanguageResourceViewModel> Languages { get; set; }

        /// <summary>
        /// Settings controlling whether skip the file generating?
        /// </summary>
        public Dictionary<string, bool> SkipSettings { get; set; }

        #endregion

        /// <summary>
        /// Temperary save this information.
        /// </summary>
        public string FullFileName { get; private set; }


        #region Core 2 UI

        /// <summary>
        /// Prepare pure UI model
        /// </summary>
        /// <param name="fullFileName"></param>
        public void SetProperties(string fullFileName)
        {
            //full file name
            FullFileName = fullFileName;
            //properties
            var createFields = CreateFields.ToList(',');
            var requestFields = RequestFields.ToList(',');
            var updateFields = UpdateFields.ToList(',');
            var hiddenFields = HiddenFields.ToList(',');
            var listFields = ListFields.ToList(',');
            var readOnlyFields = ReadOnlyFields.ToList(',');

            foreach (var property in Properties)
            {
                property.IsCreateField = createFields.Contains(property.PropertyName);
                property.IsRequestField = requestFields.Contains(property.PropertyName);
                property.IsUpdateField = updateFields.Contains(property.PropertyName);
                property.IsHiddenField = hiddenFields.Contains(property.PropertyName);
                property.IsListField = listFields.Contains(property.PropertyName);
                property.ReadOnlyWhenUpdate = readOnlyFields.Contains(property.PropertyName);
            }
            //groups
            List<KeyValue<string, string>> names = Properties.Select(t => new KeyValue<string, string> { Key = t.PropertyType, Value = t.PropertyName }).ToList();
            foreach (var group in PropertyGroups)
            {
                group.SetProperties(names);
            }
        }

        public PropertyViewModel CurrentProperty { get; set; }

        public ObservableCollection<string> CurrentMembers
        {
            get
            {
                if (CurrentProperty != null)
                {
                    if (CurrentProperty.Members == null)
                    {
                        CurrentProperty.Members = new ObservableCollection<string>();
                    }
                    return CurrentProperty.Members;
                }
                else
                {
                    return null;
                }
            }
        }

        public PropertyGroupViewModel CurrentGroup { get; set; }

        public ObservableCollection<KeyValue<string, string>> CurrentNames
        {
            get
            {
                if (CurrentGroup != null)
                {
                    return CurrentGroup.Names;
                }
                else
                {
                    return null;
                }
            }
        }

        public ObservableCollection<KeyValue<string, string>> CurrentAvalibles
        {
            get
            {
                if (CurrentGroup != null)
                {
                    return CurrentGroup.Avalibles;
                }
                else
                {
                    return null;
                }
            }
        }

        #endregion

        #region Ui to Core

        public void CollectProperties()
        {
            //collect skipSettings
            foreach (var setting in SkipCollection)
            {
                SkipSettings[setting.Key] = !setting.Value;
            }
            //collect fields
            CreateFields = string.Join(",", Properties.Where(t => t.IsCreateField).Select(t => t.PropertyName));
            RequestFields = string.Join(",", Properties.Where(t => t.IsRequestField).Select(t => t.PropertyName));
            UpdateFields = string.Join(",", Properties.Where(t => t.IsUpdateField).Select(t => t.PropertyName));
            HiddenFields = string.Join(",", Properties.Where(t => t.IsHiddenField).Select(t => t.PropertyName));
            ListFields = string.Join(",", Properties.Where(t => t.IsListField).Select(t => t.PropertyName));
            ReadOnlyFields = string.Join(",", Properties.Where(t => t.ReadOnlyWhenUpdate).Select(t => t.PropertyName));
            //PropertyGroups
            foreach (var group in PropertyGroups)
            {
                group.CollectProperties();
            }
            //Languages
            //foreach (var lang in Languages)
            //{
            //    lang.CollectProperties();
            //}
        }

        public ObservableCollection<KeyValue<string, bool>> SkipCollection
        {
            get
            {
                if (_skipCollection == null)
                {
                    _skipCollection = new ObservableCollection<KeyValue<string, bool>>();
                    foreach (var settingKey in SkipSettings.Keys.OrderBy(t => t))
                    {
                        _skipCollection.Add(new KeyValue<string, bool> { Key = settingKey, Value = !SkipSettings[settingKey] });
                    }
                }
                return _skipCollection;
            }
        }
        private ObservableCollection<KeyValue<string, bool>> _skipCollection = null;

        #endregion
    }
}
