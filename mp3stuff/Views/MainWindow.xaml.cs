using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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

namespace Mp3Stuff
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string path = @"F:\Music\music";
        List<Track> tracks = new List<Track>();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void b1_Click(object sender, RoutedEventArgs e)
        {
            tracks.Clear();
            dgTracks.ItemsSource = null;
            dgTracks.Items.Clear();
            dgTracks.Items.Refresh();
            DirectoryInfo di = new DirectoryInfo(path);
            FileInfo[] files = di.GetFiles("*.mp3", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                var tags = TagLib.File.Create(file.FullName);
                tracks.Add(new Track(file.Name, tags.Tag.FirstPerformer, tags.Tag.Title, tags.Tag.Album));
            }
//            lb1.Content = $"Файлов: {files.Length}";
            
            dgTracks.ItemsSource = tracks;
        }

        private void b2_Click(object sender, RoutedEventArgs e)
        {
            ICollectionView cvs = CollectionViewSource.GetDefaultView(dgTracks.ItemsSource);
            if (cvs != null && cvs.CanGroup == true)
            {
                cvs.GroupDescriptions.Clear();
                cvs.GroupDescriptions.Add(new PropertyGroupDescription("Artist"));
            }
        }
    }
}
