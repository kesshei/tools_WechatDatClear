using System.IO;
using System.Windows;
using WechatClear.Core;

namespace WechatClear.ViewModels
{
    public class ElementViewModel : ObservableObject
    {
        public event Action<ElementViewModel> SelectionChanged;

        public ElementViewModel(string root, string dir)
        {
            this.Root = root;
            this.Dir = dir;
            Children = new ExtendedObservableCollection<ElementViewModel>();
            ItemDetails = new ExtendedObservableCollection<DetailViewModel>();
        }

        public string Root { get; }
        public string Dir { get; }

        public bool IsSelected
        {
            get { return GetProperty<bool>(); }
            set
            {
                try
                {
                    if (value)
                    {
                        LoadDetailsAsync();
                    }

                    SetProperty(value);
                }
                catch (Exception ex)
                {
                    Console.Write(ex.ToString());
                }
            }
        }

        /// <summary>
        /// 异步加载详情数据（核心优化点）
        /// </summary>
        /// <returns></returns>
        private async Task LoadDetailsAsync()
        {
            try
            {
                var details = await Task.Run(() => LoadDetails()); // 避免阻塞UI线程
                ItemDetails.Reset(details);
                SelectionChanged?.Invoke(this); // 确保事件在详情加载完成后触发
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError($"加载详情失败: {ex}");
                ItemDetails.Reset(null); // 异常时清空详情，保证状态干净
            }
        }
        public bool IsExpanded
        {
            get { return GetProperty<bool>(); }
            set
            {
                SetProperty(value);
                if (value)
                {
                    LoadChildren(true);
                }
            }
        }
        public ExtendedObservableCollection<ElementViewModel> Children { get; set; }

        public ExtendedObservableCollection<DetailViewModel> ItemDetails { get; set; }

        public void LoadChildren(bool loadInnerChildren)
        {
            foreach (var child in Children)
            {
                child.SelectionChanged -= SelectionChanged;
            }

            var childrenViewModels = new List<ElementViewModel>();
            try
            {
                var newRoot = Path.Combine(Root, Dir);
                foreach (var child in Directory.GetDirectories(newRoot))
                {
                    var dir2Info = new DirectoryInfo(child);
                    var childViewModel = new ElementViewModel(dir2Info.Parent?.FullName, dir2Info.Name);
                    childViewModel.SelectionChanged += SelectionChanged;
                    childrenViewModels.Add(childViewModel);

                    if (loadInnerChildren)
                    {
                        childViewModel.LoadChildren(false);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }

            Children.Reset(childrenViewModels);
        }

        private List<DetailViewModel> LoadDetails()
        {
            var detailGroups = new List<DetailViewModel>();
            foreach (var item in Directory.GetFiles(Path.Combine(Root, Dir)))
            {
                detailGroups.Add(new DetailViewModel(item));
            }
            return detailGroups;
        }
    }
}
