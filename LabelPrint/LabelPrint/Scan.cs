using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LabelPrint.Util;
namespace LabelPrint
{
    public partial class Scan : Form
    {
        BardCodeHooK BarCodeHook = new BardCodeHooK();
        public Scan()
        {
            InitializeComponent();
            BarCodeHook.BarCodeEvent += new BardCodeHooK.BardCodeDeletegate(BarCode_BarCodeEvent);
        }

        private void Scan_Load(object sender, EventArgs e)
        {
            BarCodeHook.Start();
        }

        private void Scan_FormClosed(object sender, FormClosedEventArgs e)
        {
            BarCodeHook.Stop();
        }


        delegate void AsynBarCode_BarCodeEvent(BardCodeHooK.BarCodes barCode);
        void BarCode_BarCodeEvent(BardCodeHooK.BarCodes barCode)
        {
            Log.d("HH", "Final Barcode=" + barCode.BarCode);
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new AsynBarCode_BarCodeEvent(BarCode_BarCodeEvent), new object[] { barCode });
            }
            else
            {
                tb_barcode.Text = barCode.BarCode;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }

            //WidthP.Text = barCode.BarCode;
        }

        private void bt_Ok_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        public String GetBarCodeValue()
        {
            this.DialogResult = DialogResult.Cancel;
            return tb_barcode.Text;
        }
    }

}
