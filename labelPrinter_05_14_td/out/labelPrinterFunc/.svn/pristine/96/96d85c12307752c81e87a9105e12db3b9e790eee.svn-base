using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace LabelPrint.Util
{
    public class ControlHelper
    {
        public static void LimitToDigitOnly(KeyPressEventArgs e)
        {
            if (e.KeyChar != 8 && !Char.IsDigit(e.KeyChar))//如果不是输入数字就不让输入
            {
                e.Handled = true;
            }
        }
        public static void LimitToDigitAndDotOnly(KeyPressEventArgs e)
        {
            if (e.KeyChar != 8 && !Char.IsDigit(e.KeyChar) && e.KeyChar != '.')//如果不是输入数字就不让输入
            {
                e.Handled = true;
            }
        }
    }
}
