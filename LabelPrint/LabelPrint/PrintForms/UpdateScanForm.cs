using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms;
using LabelPrint.Util;
using LabelPrint.Data;
namespace LabelPrint.PrintForms
{
    public partial class UpdateScanForm : Form
    {
        BardCodeHooK BarCodeHook = new BardCodeHooK();
        List<String> BarcodeList = new List<string>();
        ProcessData UserInput = null;
        public UpdateScanForm()
        {
            InitializeComponent();
        }

        private void UpdateScanForm_Load(object sender, EventArgs e)
        {
            BarCodeHook.BarCodeEvent += new BardCodeHooK.BardCodeDeletegate(BarCode_BarCodeEvent);
            BarCodeHook.Start();
            tb_BarCode.Text = "";
            tb_BarCode.Enabled = false;

#if true
            BarcodeList.Add("S17110906L302Y21803250005050");
#endif
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
                tb_BarCode.Text = barCode.BarCode;
                LstB_Barcode.Focus();
                if (!BarcodeList.Contains(barCode.BarCode))
                {
                    BarcodeList.Add(barCode.BarCode);
                    this.LstB_Barcode.Items.Add(barCode.BarCode);
                }
                //this.DialogResult = DialogResult.OK;
                //this.Close();
            }

            //WidthP.Text = barCode.BarCode;
        }

        public void SetUserInput(ProcessData UserInput)
        {
            this.UserInput = UserInput;
        }

        private void bt_Cancel_Click(object sender, EventArgs e)
        {
            BarcodeList.Clear();
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void bt_Ok_Click(object sender, EventArgs e)
        {
            for(int i = 0; i<BarcodeList.Count; i++)
            {
                if (UserInput!=null)
                {
                    UserInput.UpdateMStateInDB(BarcodeList[i]);
                }
            }
            BarcodeList.Clear();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void UpdateScanForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            BarCodeHook.Stop();
            UserInput = null;
        }

        private void UpdateScanForm_Activated(object sender, EventArgs e)
        {
            LstB_Barcode.Focus();
        }
    }
}
