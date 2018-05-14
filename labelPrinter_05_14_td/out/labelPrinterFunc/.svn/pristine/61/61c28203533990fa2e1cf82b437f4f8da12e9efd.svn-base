using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabelPrint.Receipt
{
    class ReceiptCreator
    {
        public enum RECEIPT_TYPE{
            RECEIPT1,
            RECEIPT2,

        }
        public  static ReceiptPrintPattern CreateReceipt()
        {
            SystemSetting SysSetting;
            SysSetting = GlobalConfig.Setting;
            String label = SysSetting.CurSettingInfo.Label;
            if (String.Compare(label, "Label1")==0)
            {
                return new Receipt1();
            }
            else if (String.Compare(label, "DefOutBounding") == 0)
            {

            }
            else if (String.Compare(label, "DefLiuYan") == 0)
            {

            }
            else if (String.Compare(label, "DefCut") == 0)
            {

            }
            else if (String.Compare(label, "DefPrint") == 0)
            {

            }
            else if (String.Compare(label, "DefQA") == 0)
            {

            }
            else if (String.Compare(label,"DefPack")==0)
            {

            }
            else if (String.Compare(label, "DefRcv") == 0)
            {

            }
                return null;
        }
    }
}
