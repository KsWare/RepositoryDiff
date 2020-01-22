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
            ResultsListView3.SizeChanged+=ResultsListView3_SizeChanged;
            Loaded += (s, e) =>
            {
                ResultsListView_SizeChanged(null, null);

                var gv = (GridView) ResultsListView3.View;
                gv.Columns[0].Width = 430;
                gv.Columns[1].Width = 40;
                gv.Columns[2].Width = 75;
                gv.Columns[3].Width = 70;
                gv.Columns[4].Width = 75;

            };
        }

        private void ResultsListView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var gv = (GridView) ResultsListView.View;
            gv.Columns[0].Width = (ResultsListView.ActualWidth - gv.Columns[1].ActualWidth) / 2;
            gv.Columns[2].Width = gv.Columns[0].Width;
        }

        private void ResultsListView3_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // var gv = (GridView) ResultsListView3.View;
            // gv.Columns[0].Width = (ResultsListView3.ActualWidth - gv.Columns[1].ActualWidth) / 2.5;
            // gv.Columns[2].Width = gv.Columns[0].Width/2;
            // gv.Columns[3].Width = gv.Columns[0].Width/2;
            // gv.Columns[4].Width = gv.Columns[0].Width/2;
        }
    }

}
