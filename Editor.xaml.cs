using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Логика взаимодействия для Editor.xaml
    /// </summary>
    public partial class Editor : Window
    {
        string path;
        bool IsCreate = false;
        List<ProductMaterial> MS = Base.DB.ProductMaterial.ToList();
        public Editor()
        {
            InitializeComponent();
            CbMaterialType.ItemsSource = Base.DB.MaterialType.ToList();
            CbMaterialType.SelectedValuePath = "ID";
            CbMaterialType.DisplayMemberPath = "Title";

            IsCreate = true;
            LbSupliers.SelectedValuePath = "ID";
            LbSupliers.DisplayMemberPath = "Title";
        }
        Product ProductRed = new Product();
        public Editor(Product editImport)
        {
            InitializeComponent();
            ProductRed = editImport;

            CbMaterialType.ItemsSource = Base.DB.MaterialType.ToList();
            CbMaterialType.SelectedValuePath = "ID";
            CbMaterialType.DisplayMemberPath = "Title";
            CbMaterialType.SelectedIndex = ProductRed.ProductTypeID - 1;

            TbTitle.Text = ProductRed.Title;
            TbCountInStock.Text = ProductRed.ArticleNumber.ToString();
            TbCountInPack.Text = ProductRed.ProductionPersonCount.ToString();
            TbCost.Text = ProductRed.MinCostForAgent.ToString();
            TbMinCount.Text = ProductRed.ProductionPersonCount.ToString();

            if (ProductRed.Image != null)
            {
                BitmapImage BI = new BitmapImage(new Uri(ProductRed.Image, UriKind.RelativeOrAbsolute));
                MaterialImage.Source = BI;
            }

            LbSupliers.SelectedValuePath = "ID";
            LbSupliers.DisplayMemberPath = "Title";
        }


        private void ButtEditImage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog OFD = new OpenFileDialog();
                OFD.ShowDialog();
                path = OFD.FileName;
                int n = path.IndexOf("materials");
                path = path.Substring(n);
                BitmapImage img = new BitmapImage(new Uri(path, UriKind.RelativeOrAbsolute));
                MaterialImage.Source = img;
            }
            catch
            {
                MessageBox.Show("Картинка не выбрана", "Редактирование", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ButtUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ProductRed.Title = TbTitle.Text;
                ProductRed.ProductTypeID = CbMaterialType.SelectedIndex + 1;
                ProductRed.ArticleNumber = TbCountInStock.Text;
                ProductRed.ProductionPersonCount = Convert.ToInt32(TbCountInPack.Text);
                ProductRed.ProductionWorkshopNumber = Convert.ToInt32(TbMinCount.Text);
                ProductRed.MinCostForAgent = Convert.ToInt32(TbCost.Text);
                ProductRed.Description = TbDescription.Text;
                ProductRed.Image = path;
                if (IsCreate == true)
                {
                    Base.DB.Product.Add(ProductRed);
                }
                Base.DB.SaveChanges();
                List<ProductMaterial> materialSuppliersOld = MS.Where(x => x.ProductID == ProductRed.ID).ToList();
                if (materialSuppliersOld.Count != 0)
                {
                    foreach (ProductMaterial ms in materialSuppliersOld)
                    {
                        Base.DB.ProductMaterial.Remove(ms);
                    }
                }
                foreach (Supplier t in LbSupliers.Items)
                {
                    ProductMaterial ms = new ProductMaterial();
                    ms.ProductID = ProductRed.ID;
                    ms.MaterialID = t.ID;
                    Base.DB.ProductMaterial.Add(ms);
                }
                Base.DB.SaveChanges();
                MessageBox.Show("Данные записаны", "Редактирование", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                this.Close();
            }
            catch
            {
                MessageBox.Show("Не удалось записать данные, повторите попытку", "Редактирование", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ButtDelete_Click(object sender, RoutedEventArgs e)
        {
            if (IsCreate == true)
            {
                MessageBox.Show("Невозможно удалить запись, так как она еще не существует", "Редактирование", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBoxResult.Yes == MessageBox.Show("Вы уверены, что хотите удалить эту запись?", "Редактирование", MessageBoxButton.YesNo, MessageBoxImage.Question))
            {
                Base.DB.Product.Remove(ProductRed);
                Base.DB.SaveChanges();
                this.Close();
            }
            else
            {
                return;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Regex regex = new Regex("^[0-9]{8}$");  // регулярное выражение для определения верного числого кода
            if (regex.IsMatch(tbCode.Text))
            {
                tbCode.IsEnabled = false;  // блокируем TextBox

                string binaryCode = "101";  // левый защитный шаблон
                                            // отрисовка левого защитного шаблона
                for (int i = 0; i < binaryCode.Length; i++)
                {
                    if (binaryCode[i] == '1')
                    {
                        Rectangle r = new Rectangle() // отрисовка черного прямоугольника
                        {
                            Stroke = Brushes.Black,
                            StrokeThickness = 2,
                            SnapsToDevicePixels = true,
                            Height = 100
                        };
                        spBarCode.Children.Add(r);
                    }
                    else
                    {
                        Rectangle r = new Rectangle()  // отрисовка белого прямоугольника
                        {
                            Stroke = Brushes.White,
                            StrokeThickness = 2,
                            SnapsToDevicePixels = true,
                            Height = 100
                        };
                        spBarCode.Children.Add(r);  // добавление прямоугольника в нужную StackPanel
                    }
                }

                binaryCode = "";
                // определение двоичного кода левой части
                for (int i = 0; i < 4; i++)
                {
                    binaryCode += Barcode.GetLeftL(tbCode.Text[i]);
                }

                StackPanel spLeftCodeAndNumber = new StackPanel() // создание StackPanel для хранения левой части штрих-кода и его числового значения
                {
                    Orientation = Orientation.Vertical,
                    Height = 100
                };

                StackPanel spLeftCode = new StackPanel()  // создание StackPanel для хранения левой части штрих-кода
                {
                    Orientation = Orientation.Horizontal,
                };
                // отрисовка левой части штрих-кода
                for (int i = 0; i < binaryCode.Length; i++)
                {
                    if (binaryCode[i] == '1')
                    {
                        Rectangle r = new Rectangle() // отрисовка черного прямоугольника
                        {
                            Stroke = Brushes.Black,
                            StrokeThickness = 2,
                            SnapsToDevicePixels = true,
                            Height = 80
                        };
                        spLeftCode.Children.Add(r);
                    }
                    else
                    {
                        Rectangle r = new Rectangle()  // отрисовка белого прямоугольника
                        {
                            Stroke = Brushes.White,
                            StrokeThickness = 2,
                            SnapsToDevicePixels = true,
                            Height = 80
                        };
                        spLeftCode.Children.Add(r);  // добавление прямоугольника в нужную StackPanel
                    }
                }

                string leftNumber = " "; // строка для хранения числового значения левой части штрих-кода
                for (int i = 0; i < 4; i++)
                {
                    leftNumber += tbCode.Text[i] + " ";

                }
                // отрисовка TextBlock, который будет хранить числовое значение левой чатси штрих-кода
                TextBlock tbLeftNumber = new TextBlock()
                {
                    Text = leftNumber,
                    FontSize = 16,
                    TextAlignment = TextAlignment.Center
                };
                spLeftCodeAndNumber.Children.Add(spLeftCode); // добавление в StackPanel левой части штрих-кода
                spLeftCodeAndNumber.Children.Add(tbLeftNumber);  // добавление в StackPanel числового значения левой части штрих-кода
                spBarCode.Children.Add(spLeftCodeAndNumber);  // добавление StackPanel для хранения левой части штрих-кода и его числового значения в общую StackPanel

                binaryCode = "01010"; // код среднего защитного шаблона
                                      // отрисовка среднего защитного шаблона
                for (int i = 0; i < binaryCode.Length; i++)
                {
                    if (binaryCode[i] == '1')
                    {
                        Rectangle r = new Rectangle() // отрисовка черного прямоугольника
                        {
                            Stroke = Brushes.Black,
                            StrokeThickness = 2,
                            SnapsToDevicePixels = true,
                            Height = 100
                        };
                        spBarCode.Children.Add(r);
                    }
                    else
                    {
                        Rectangle r = new Rectangle()  // отрисовка белого прямоугольника
                        {
                            Stroke = Brushes.White,
                            StrokeThickness = 2,
                            SnapsToDevicePixels = true,
                            Height = 100
                        };
                        spBarCode.Children.Add(r);  // добавление прямоугольника в нужную StackPanel
                    }
                }

                binaryCode = "";
                // определение двоичного кода правой части
                for (int i = 4; i < 8; i++)
                {
                    binaryCode += Barcode.GetRightR(tbCode.Text[i]);
                }

                StackPanel spRightCodeAndNumber = new StackPanel() // создание StackPanel для хранения правой части штрих-кода и его числового значения
                {
                    Orientation = Orientation.Vertical,
                    Height = 100
                };

                StackPanel spRightCode = new StackPanel()  // создание StackPanel для хранения правой части штрих-кода
                {
                    Orientation = Orientation.Horizontal,
                };
                // отрисовка правой части штрих-кода
                for (int i = 0; i < binaryCode.Length; i++)
                {
                    if (binaryCode[i] == '1')
                    {
                        Rectangle r = new Rectangle() // отрисовка черного прямоугольника
                        {
                            Stroke = Brushes.Black,
                            StrokeThickness = 2,
                            SnapsToDevicePixels = true,
                            Height = 80
                        };
                        spRightCode.Children.Add(r);
                    }
                    else
                    {
                        Rectangle r = new Rectangle()  // отрисовка белого прямоугольника
                        {
                            Stroke = Brushes.White,
                            StrokeThickness = 2,
                            SnapsToDevicePixels = true,
                            Height = 80
                        };
                        spRightCode.Children.Add(r);  // добавление прямоугольника в нужную StackPanel
                    }
                }

                string rightNumber = " "; // строка для хранения числового значения правой части штрих-кода
                for (int i = 4; i < 8; i++)
                {
                    rightNumber += tbCode.Text[i] + " ";

                }
                // отрисовка TextBlock, который будет хранить числовое значение правой части штрих-кода
                TextBlock tbRightNumber = new TextBlock()
                {
                    Text = rightNumber,
                    FontSize = 16,
                    TextAlignment = TextAlignment.Center
                };
                spRightCodeAndNumber.Children.Add(spRightCode); // добавление в StackPanel правой части штрих-кода
                spRightCodeAndNumber.Children.Add(tbRightNumber);  // добавление в StackPanel числового значения правой части штрих-кода
                spBarCode.Children.Add(spRightCodeAndNumber);  // добавление StackPanel для хранения правой части штрих-кода и его числового значения в общую StackPanel

                binaryCode = "101";  // правый защитный шаблон
                                     // отрисовка правого защитного шаблона
                for (int i = 0; i < binaryCode.Length; i++)
                {
                    if (binaryCode[i] == '1')
                    {
                        Rectangle r = new Rectangle() // отрисовка черного прямоугольника
                        {
                            Stroke = Brushes.Black,
                            StrokeThickness = 2,
                            SnapsToDevicePixels = true,
                            Height = 100
                        };
                        spBarCode.Children.Add(r);
                    }
                    else
                    {
                        Rectangle r = new Rectangle()  // отрисовка белого прямоугольника
                        {
                            Stroke = Brushes.White,
                            StrokeThickness = 2,
                            SnapsToDevicePixels = true,
                            Height = 100
                        };
                        spBarCode.Children.Add(r);  // добавление прямоугольника в нужную StackPanel
                    }
                }
            }
            else
            {
                MessageBox.Show("Неверный числовой код");
            }

        }
    }
}
