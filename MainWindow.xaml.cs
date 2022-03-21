using System;
using System.Collections.Generic;
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


namespace StudyPR
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static bool adTumbler = false;
        public MainWindow()
        {
            InitializeComponent();
            Base.DB = new FaceEntities();
            FrameCl.mainFrame = MainFrame;
            FrameCl.mainFrame.Navigate(new Products());
        }
    }
}
