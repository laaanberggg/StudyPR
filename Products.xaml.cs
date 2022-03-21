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
    /// Логика взаимодействия для Products.xaml
    /// </summary>
    public partial class Products : Page
    {
        List<Product> ServiceStart = Base.DB.Product.ToList();
        Pagination pag = new Pagination();
        public Products()
        {
            InitializeComponent();
            LVUslugi.ItemsSource = ServiceStart;
            CbFilt.Items.Add("Все типы");
            List<Product> prd = Base.DB.Product.ToList();
            for (int i = 0; i < prd.Count; i++)
            {
                CbFilt.Items.Add(prd[i].Title);
            }
            CbFilt.SelectedIndex = 0;
            TbCount.Text = "Записей: " + ServiceStart.Count().ToString() + " из " + ServiceStart.Count().ToString();
        }
        private void TbMaterial_Loaded(object sender, RoutedEventArgs e)
        {
            TextBlock tb = (TextBlock)sender;
            int index = Convert.ToInt32(tb.Uid);
            List<ProductMaterial> mtList = Base.DB.ProductMaterial.Where(x => x.ProductID == index).ToList();
            string str = "";
            foreach (ProductMaterial item in mtList)
            {
                str += item.Material.Title + ", ";
            }
            if (mtList.Count == 0)
            {
                tb.Text = "Материалы: отсутствуют";
            }
            else
            {
                tb.Text = "Материалы: " + str.Substring(0, str.Length - 2);
            }
        }
        List<Product> ServiceFilter = new List<Product>();

        List<Product> ServiceSearch = new List<Product>();

        private void TbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TbSearch.Text != String.Empty)
            {
                ServiceSearch = ServiceStart.Where(x => x.Title.Contains(TbSearch.Text)).ToList();
                FliterSort();
            }
            else
            {
                FliterSort();
            }
        }
        private void CbFilt_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FliterSort();
        }
        private void CbSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FliterSort();
        }
        private void FliterSort()
        {
            int filterIndex = CbFilt.SelectedIndex;

            if (TbSearch.Text != String.Empty)
            {
                if (filterIndex != 0)
                {
                    ServiceFilter = ServiceSearch.Where(x => x.ProductTypeID == filterIndex).ToList();
                }
                else
                {
                    ServiceFilter = ServiceSearch;
                }
            }
            else
            {
                if (filterIndex != 0)
                {
                    ServiceFilter = ServiceStart.Where(x => x.ProductTypeID == filterIndex).ToList();
                }
                else
                {
                    ServiceFilter = ServiceStart;
                }
            }

            switch (CbSort.SelectedIndex)
            {
                case 0:
                    ServiceFilter.Sort((x, y) => x.Title.CompareTo(y.Title));
                    break;
                case 1:
                    ServiceFilter.Sort((x, y) => x.Title.CompareTo(y.Title));
                    ServiceFilter.Reverse();
                    break;
                case 2:
                    ServiceFilter.Sort((x, y) => x.MinCostForAgent.CompareTo(y.MinCostForAgent));
                    break;
                case 3:
                    ServiceFilter.Sort((x, y) => x.MinCostForAgent.CompareTo(y.MinCostForAgent));
                    ServiceFilter.Reverse();
                    break;
                case 4:
                    ServiceFilter.Sort((x, y) => x.ProductionWorkshopNumber.CompareTo(y.ProductionWorkshopNumber));
                    break;
                case 5:
                    ServiceFilter.Sort((x, y) => x.ProductionWorkshopNumber.CompareTo(y.ProductionWorkshopNumber));
                    ServiceFilter.Reverse();
                    break;
            }

            LVUslugi.ItemsSource = ServiceFilter;
            LVUslugi.Items.Refresh();
            TbCount.Text = "Записей: " + ServiceFilter.Count().ToString() + " из " + ServiceStart.Count().ToString();
        }
        private void LVUslugi_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LVUslugi.SelectedIndex != -1)
            {
                ButtEditPrice.Visibility = Visibility.Visible;
            }
            else
            {
                ButtEditPrice.Visibility = Visibility.Hidden;
            }
        }

        private void ButtEditPrice_Click(object sender, RoutedEventArgs e)
        {
            var selectedList = LVUslugi.SelectedItems;
            double maxMc = 0;
            foreach (Product mC in selectedList)
            {
                if (mC.MinCostForAgent > maxMc)
                {
                    maxMc = mC.MinCostForAgent;
                }
            }
            Price mCWin = new Price(maxMc);
            mCWin.ShowDialog();
            if (mCWin.NewPrice > 0)
            {
                foreach (Product mC in selectedList)
                {
                    mC.MinCostForAgent = mCWin.NewPrice;
                }
                LVUslugi.Items.Refresh();
            }

        }
        private void ButtEdit_Click(object sender, RoutedEventArgs e)
        {
            Button B = (Button)sender;
            int id = Convert.ToInt32(B.Uid);
            Product ProductRed = Base.DB.Product.FirstOrDefault(y => y.ID == id);
            Editor editWindow = new Editor(ProductRed);
            editWindow.ShowDialog();
            LVUslugi.Items.Refresh();
        }

        private void ButtNew_Click(object sender, RoutedEventArgs e)
        {
            Editor editWindow = new Editor();
            editWindow.ShowDialog();
            LVUslugi.Items.Refresh();
        }
        private void GoPage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            TextBlock tb = (TextBlock)sender;
            switch (tb.Uid)
            {
                case "prev":
                    pag.CurrentPage--;
                    break;
                case "next":
                    pag.CurrentPage++;
                    break;
                default:
                    pag.CurrentPage = Convert.ToInt32(tb.Text);
                    break;
            }
            LVUslugi.ItemsSource = ServiceFilter.Skip(pag.CurrentPage * pag.CountPage - pag.CountPage).Take(pag.CountPage).ToList();
        }
    }

}

