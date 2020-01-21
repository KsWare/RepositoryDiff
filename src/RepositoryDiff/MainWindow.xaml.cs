using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace KsWare.RepositoryDiff
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        public MainWindow()
        {
            InitializeComponent();
            ResultsListView.SizeChanged+=ResultsListView_SizeChanged;

        }

        private void ResultsListView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var gv = (GridView) ResultsListView.View;
            gv.Columns[0].Width = (ResultsListView.ActualWidth - gv.Columns[1].ActualWidth) / 2;
            gv.Columns[2].Width = gv.Columns[0].Width;
        }
    }

}
