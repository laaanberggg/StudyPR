using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace StudyPR
{
    public partial class Product
    {
        public string image
        {
            get
            {
                if(Image != null) 
                {
                    return Image;
                }
                else 
                {
                    return "Images/picture.png";
                }
            }
        }
        public SolidColorBrush MinColor
        {
            get
            {
                SolidColorBrush br = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 153, 153));
                SolidColorBrush br2 = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
                if (MinCostForAgent > 2000)
                {
                    return br;
                }
                else
                {
                    return br2;
                }
            }
        }
    }
}
