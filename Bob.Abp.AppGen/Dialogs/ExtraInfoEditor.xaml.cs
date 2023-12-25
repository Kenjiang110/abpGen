using Bob.Abp.AppGen.Dialogs.ViewModel;
using Bob.Abp.AppGen.Models;
using Microsoft.VisualStudio.PlatformUI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Bob.Abp.AppGen.Dialogs
{
    /// <summary>
    /// ExtraInfoEditor.xaml 的交互逻辑
    /// </summary>
    public partial class ExtraInfoEditor : DialogWindow
    {
        public AhEntity EntityModel { get; set; }

        private EntityViewModel ViewModel { get; set; }

        public ExtraInfoEditor(AhEntity entityModel)
        {
            EntityModel = entityModel;
            ViewModel = JsonConvert.DeserializeObject<EntityViewModel>(JsonConvert.SerializeObject(entityModel));
            ViewModel.SetProperties(entityModel.FullFileName);

            InitializeComponent();
            DataContext = ViewModel;

            cbTabType.ItemsSource = new KeyValue<PropertyGroupType, string>[] {
                new KeyValue<PropertyGroupType, string> {Key = PropertyGroupType.BasicTab, Value = PropertyGroupType.BasicTab.ToString() },
                new KeyValue<PropertyGroupType, string> {Key = PropertyGroupType.MultiSelect, Value = PropertyGroupType.MultiSelect.ToString() },
                new KeyValue<PropertyGroupType, string> {Key = PropertyGroupType.SimpleList, Value = PropertyGroupType.SimpleList.ToString() }
            };
            cbTabType.SelectedIndex = 0;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.CollectProperties();
            EntityModel = JsonConvert.DeserializeObject<AhEntity>(JsonConvert.SerializeObject(ViewModel));
            EntityModel.FullFileName = ViewModel.FullFileName;

            this.DialogResult = true;
        }

        #region Extra Using CUD

        private void AddExtraUsing_Click(object sender, RoutedEventArgs e)
        {
            var text = tbExtraUsing.Text.Trim();
            if (!string.IsNullOrEmpty(text))
            {
                if (lxExtraUsings.Items.IndexOf(text) > -1)
                {
                    MessageBox.Show($"{text} already exists!");
                }
                else
                {
                    ViewModel.ExtraUsings.Add(text);
                }
            }
        }

        private void UpdateExtraUsing_Click(object sender, RoutedEventArgs e)
        {
            var text = tbExtraUsing.Text.Trim();
            if (!string.IsNullOrEmpty(text))
            {
                if (lxExtraUsings.SelectedItem == null)
                {
                    MessageBox.Show("Please select the item to be updated.");
                }
                else
                {
                    ViewModel.ExtraUsings[lxExtraUsings.SelectedIndex] = text;
                }
            }
        }

        private void RemoveExtraUsing_Click(object sender, RoutedEventArgs e)
        {
            if (lxExtraUsings.SelectedItem != null)
            {
                ViewModel.ExtraUsings.RemoveAt(lxExtraUsings.SelectedIndex);
            }
            else
            {
                MessageBox.Show("Please select the item to be deleted.");
            }
        }

        #endregion

        #region Property Members CUD

        private void AddMember_Click(object sender, RoutedEventArgs e)
        {
            var text = tbMember.Text.Trim();
            if (!string.IsNullOrEmpty(text))
            {
                if (ViewModel.CurrentMembers.IndexOf(text) > -1)
                {
                    MessageBox.Show($"{text} already exists!");
                }
                else
                {
                    ViewModel.CurrentMembers.Add(text);
                }
            }

        }

        private void UpdateMember_Click(object sender, RoutedEventArgs e)
        {
            var text = tbMember.Text.Trim();
            if (!string.IsNullOrEmpty(text) && lxMembers.SelectedItem != null)
            {
                if (lxMembers.SelectedItem == null)
                {
                    MessageBox.Show("Please select the member to be updated.");
                }
                else
                {
                    ViewModel.CurrentMembers[lxMembers.SelectedIndex] = text;
                }
            }
        }

        private void RemoveMember_Click(object sender, RoutedEventArgs e)
        {
            if (lxMembers.SelectedItem != null)
            {
                ViewModel.CurrentMembers.RemoveAt(lxMembers.SelectedIndex);
            }
            else
            {
                MessageBox.Show("Please select the member to be deleted.");
            }
        }

        #endregion

        #region Property Group CUD

        private void AddGroup_Click(object sender, RoutedEventArgs e)
        {
            var title = tbTitle.Text.Trim();
            var tabType = (PropertyGroupType) cbTabType.SelectedValue;
            if (tabType == PropertyGroupType.BasicTab && string.IsNullOrEmpty(title))
            {
                title = "BasicInfo";
            }
            if (!string.IsNullOrEmpty(title))
            {
                var group = new PropertyGroupViewModel { Title = title, TabType = tabType };
                var properties = ViewModel.Properties.Select(t => new KeyValue<string, string> { Key = t.PropertyType, Value = t.PropertyName }).ToList();
                group.SetProperties(properties);
                ViewModel.PropertyGroups.Add(group);
            }
            else
            {
                MessageBox.Show("Please input group's title and type.");
            }
        }

        private void UpdateGroup_Click(object sender, RoutedEventArgs e)
        {
            var title = tbTitle.Text.Trim();
            var tabType = (PropertyGroupType)cbTabType.SelectedValue;
            if (tabType == PropertyGroupType.BasicTab && string.IsNullOrEmpty(title))
            {
                title = "BasicInfo";
            }
            if (!string.IsNullOrEmpty(title))
            {
                if (ViewModel.CurrentGroup != null)
                {
                    var group = ViewModel.CurrentGroup;
                    group.Title = title;
                    group.TabType = tabType;
                }
                else
                {
                    MessageBox.Show("Please select the group to be updated.");
                }
            }
            else
            {
                MessageBox.Show("Please input group's title and type.");
            }
        }

        private void RemoveGroup_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.CurrentGroup != null)
            {
                var group = ViewModel.CurrentGroup;
                if (ViewModel.PropertyGroups.Count == 1)
                {
                    MessageBox.Show("At least left one Tab.");
                }
                else
                {
                    ViewModel.PropertyGroups.Remove(group);
                }
            }
            else
            {
                MessageBox.Show("Please select the group to be deleted.");
            }
        }

        private void DeselectProperty_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lxNames.SelectedIndex > -1)
            {
                var item = lxNames.SelectedItem as KeyValue<string, string>;
                ViewModel.CurrentNames.RemoveAt(lxNames.SelectedIndex);
                ViewModel.CurrentAvalibles.Add(item);
            }
        }

        private void SelectProperty_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lxAvalibles.SelectedIndex > -1)
            {
                var item = lxAvalibles.SelectedItem as KeyValue<string, string>;
                ViewModel.CurrentAvalibles.RemoveAt(lxAvalibles.SelectedIndex);
                ViewModel.CurrentNames.Add(item);
            }
        }

        #endregion
    }
}
