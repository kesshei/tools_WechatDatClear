using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;
using System.Windows.Input;
using WechatClear.Core;

namespace WechatClear.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        public MainViewModel()
        {
            Elements = new ObservableCollection<ElementViewModel>();
            ImageZoom = 20;
            Action_ImagesDelete = new RelayCommand(o =>
            {
                if (SelectedItemDetails != null)
                {
                    var toDelete = SelectedItemDetails.Where(d => d.IsSelected).ToList();
                    foreach (var detail in toDelete)
                    {
                        DeleteSelectedItemInTree(detail);
                    }
                }
            });
            Action_DeleteAllItems = new RelayCommand(o =>
            {
                if (SelectedItemDetails != null)
                {
                    foreach (var detail in SelectedItemDetails.ToList())
                    {
                        DeleteSelectedItemInTree(detail);
                    }
                }
            });
        }
        public void DeleteSelectedItemInTree(DetailViewModel detail)
        {
            try
            {
                detail.IsDeleted = true;

                if (File.Exists(detail.Path))
                {
                    FileAttributes attributes = File.GetAttributes(detail.Path);
                    if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                    {
                        attributes &= ~FileAttributes.ReadOnly;
                        File.SetAttributes(detail.Path, attributes);
                    }
                    using (detail)
                    {
                        File.Delete(detail.Path);
                    }
                }
                SelectedItemInTree.ItemDetails.Remove(detail);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error deleting file {detail.Path}: {ex.Message}");
            }
        }
        public ObservableCollection<ElementViewModel> Elements { get; private set; }
        public ObservableCollection<DetailViewModel> SelectedItemDetails => SelectedItemInTree?.ItemDetails;
        public ICommand Action_ImagesDelete { get; private set; }
        public ICommand Action_DeleteAllItems { get; private set; }
        public ElementViewModel SelectedItemInTree
        {
            get { return GetProperty<ElementViewModel>(); }
            private set { SetProperty(value); }
        }
        public double ImageZoom
        {
            get { return GetProperty<double>(); }
            private set { SetProperty(value); }
        }
        public void Initialize(string dir)
        {
            if (Directory.Exists(dir))
            {
                var dirinfo = new DirectoryInfo(dir);
                var desktopViewModel = new ElementViewModel(dirinfo.Parent?.FullName, dirinfo.Name);
                desktopViewModel.SelectionChanged += DesktopViewModel_SelectionChanged;
                desktopViewModel.LoadChildren(false);
                Elements.Clear();
                Elements.Add(desktopViewModel);
                Elements[0].IsExpanded = true;
            }
        }
        private void DesktopViewModel_SelectionChanged(ElementViewModel obj)
        {
            SelectedItemInTree = obj;
            OnPropertyChanged(() => SelectedItemDetails);
        }
    }
}
