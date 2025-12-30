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
using WechatClear.Core;

namespace WechatClear.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        public MainViewModel()
        {
            Elements = new ObservableCollection<ElementViewModel>();
            ImageZoom = 20;
        }
        public ObservableCollection<ElementViewModel> Elements { get; private set; }
        public ObservableCollection<DetailViewModel> SelectedItemDetails => SelectedItemInTree?.ItemDetails;

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
