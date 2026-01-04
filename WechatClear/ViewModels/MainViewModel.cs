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
                        try
                        {
                            detail.IsDeleted = true;
                            //if (File.Exists(detail.Path))
                            //{
                            //    File.Delete(detail.Path);
                            //}
                            SelectedItemInTree.ItemDetails.Remove(detail);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Error deleting file {detail.Path}: {ex.Message}");
                        }
                    }
                }
            });
        }
        public ObservableCollection<ElementViewModel> Elements { get; private set; }
        public ObservableCollection<DetailViewModel> SelectedItemDetails => SelectedItemInTree?.ItemDetails;
        public ICommand Action_ImagesDelete { get; private set; }   
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
