using Microsoft.Win32;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WechatClear.ViewModels;

namespace WechatClear
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }

        private void TreeViewItem_Selected(object sender, RoutedEventArgs e)
        {
            var item = sender as TreeViewItem;
            if (item != null)
            {
                item.BringIntoView();
                e.Handled = true;
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnSelectFolder_Click(object sender, RoutedEventArgs e)
        {
            // 创建文件夹选择对话框
            var folderDialog = new OpenFolderDialog();

            // 可选：设置对话框标题
            folderDialog.Title = "请选择目标文件夹";

            // 可选：设置初始目录（比如当前文本框已有的路径）
            if (!string.IsNullOrEmpty(txtFolderPath.Text))
            {
                folderDialog.InitialDirectory = txtFolderPath.Text;
            }
            else
            {
                // 默认打开桌面
                folderDialog.InitialDirectory = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
            }

            // 显示对话框并判断是否确认选择
            if (folderDialog.ShowDialog() == true)
            {
                // 将选中的路径赋值给文本框显示
                txtFolderPath.Text = folderDialog.FolderName;

            }
        }
    }
}