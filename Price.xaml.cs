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
using System.Windows.Shapes;

namespace StudyPR
{
    /// <summary>
    /// Логика взаимодействия для Price.xaml
    /// </summary>
    public partial class Price : Window
    {
        public Price(double max)
        {
            InitializeComponent();
            TbValue.Text = max.ToString();
        }

        double newPrice = 0;

        private void ButtOk_Click(object sender, RoutedEventArgs e)
        {
            newPrice = Convert.ToDouble(TbValue.Text);
            this.Close();
        }

        public double NewPrice
        {
            get
            {
                return newPrice;
            }
        }
    }
}