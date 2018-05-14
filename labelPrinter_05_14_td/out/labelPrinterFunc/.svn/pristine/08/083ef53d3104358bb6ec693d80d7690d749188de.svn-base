using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using LabelPrint.Receipt;
namespace LabelPrint
{
    public partial class SystemSettingForm : Form
    {
        private SettingInfo Info;
        String[] baudRate = {
                "110",
                "300",
                "600",
                "1200",
                "2400",
                "4800",
                "9600",
                "14400",
                "19200",
                "38400",
                "57600",
                "115200",
                "230400",
                "380400",
                "460800",
                "921600"
            };
        String[] parity = { "None", "Odd", "Even", "Mark", "Space" };
        String[] stopbit = { "1", "1.5", "2" };

        ManufacturePhaseType MPhase = ManufacturePhaseType.PRINT;
        int mMachineNo = 1;

        String folderpath;
        PrintHelper helper = new PrintHelper();
        Receipt1 recp = new Receipt1();
        LabelPattern pattern;
        String[] GetJsonFileUnderPath(String path)
        {

            int i = 0;
            DirectoryInfo root = new DirectoryInfo(path);
            int len = root.GetFiles().Length;
            String[] strs = new String[len];
            foreach (FileInfo f in root.GetFiles())
            {
                if (f.Name.IndexOf(".json") != -1)
                {
                    strs[i] = f.Name;
                    i++;
                }
            }
            return strs;
        }
        public SystemSettingForm()
        {
            InitializeComponent();
        }

        void DisableAllTabPages()
        {
           
            tabpage_Label.Parent = null;
            tabpage_serial.Parent = null;
            tabpage_communication.Parent = null;
            tabpage_printer.Parent = null;
            tabpage_manage.Parent = null;
        }
        void EnablePageByPhase(ManufacturePhaseType type)
        {
            switch(type)
            {

                case ManufacturePhaseType.LIUYAN:
                    tabpage_Label.Parent = tab_Setting;
                    break;
                case ManufacturePhaseType.CUT:
                    tabpage_serial.Parent = tab_Setting;
                    break;
                case ManufacturePhaseType.PRINT:
                    tabpage_communication.Parent = tab_Setting;
                    break;
                case ManufacturePhaseType.QUALITYCHECK:
                    tabpage_printer.Parent = tab_Setting;
                    break;


                default:
                    break;
            }
            tabpage_manage.Parent = tab_Setting;
        }


        void InitPrinterSetting()
        {
            int thermalprinterIndex = -1;
            int printerIndex = -1;
            int Index = 0;
            foreach (string printer in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
            {

                if (Info.ThermalPrinter != null)
                {
                    if (String.Compare(printer, Info.ThermalPrinter) == 0)
                    {
                        thermalprinterIndex = Index;
                    }
                }
                if (Info.Printer != null)
                {
                    if (String.Compare(printer, Info.Printer) == 0)
                    {
                        printerIndex = Index;
                    }
                }

                cb_thermalPrinter.Items.Add(printer);
                cb_Printer.Items.Add(printer);
                Index++; ;
            }
            if (thermalprinterIndex != -1)
                cb_thermalPrinter.SelectedIndex = thermalprinterIndex;
            if (printerIndex != -1)
                cb_Printer.SelectedIndex = printerIndex;
        }

        void InitLabelSetting()
        {

            folderpath = GlobalConfig.Setting.GetCurrentSubFolder();

            string[] labels = GetJsonFileUnderPath(folderpath);
            if (labels == null)
                return;
            int index = -1;
            for (int i = 0; i < labels.Length; i++)
            {
                if ((labels[i] != null) && (labels[i] != ""))
                {
                    String labelstr = labels[i].ToLower();

                    this.lb_label.Items.Add(labelstr.Substring(0, labelstr.IndexOf(".json")));
                    if (String.Compare(GlobalConfig.Setting.CurSettingInfo.LabelFileName, labelstr) == 0)
                    {
                        index = i;
                    }
                }
            }
            this.lb_label.SelectedIndex = index;
        }

        private void SystemSettingForm_Load(object sender, EventArgs e)
        {
            Info = GlobalConfig.Setting.CurSettingInfo;
            Init_SerialPortComBoxList();

            tb_ServerIP.Text = Info.ServerIP;
            Boolean SiteConfigured = Info.SiteConfigured;
            MPhase = (ManufacturePhaseType)Info.ManufacturePhase;

            DisableAllTabPages();

          //  if (SiteConfigured== false)
            {
                tabpage_manage.Parent = tab_Setting;
            }
                tabpage_serial.Parent = tab_Setting;
                tabpage_communication.Parent = tab_Setting;
                tabpage_printer.Parent = tab_Setting;
                tabpage_Label.Parent = tab_Setting;




            InitLabelSetting();

            InitPrinterSetting();
        }

        private void SystemSettingForm_Leave(object sender, EventArgs e)
        {

        }


        private void SystemSettingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //SaveSetting();
        }

        #region Cut
        Boolean IsIpAddressValid(String ipStr)
        {
            System.Net.IPAddress ip;
            if (System.Net.IPAddress.TryParse(ipStr, out ip))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        void clear_SerialPortComboxList()
        {
            cb_SerialPort.Items.Clear();
            cb_BoudRate.Items.Clear();
            cb_DataBits.Items.Clear();
            cb_Parity.Items.Clear();
            cb_StopBits.Items.Clear();
        }

        void SetInitSelSerialPortComboList()
        {
            cb_SerialPort.SelectedIndex = 0;
            cb_BoudRate.SelectedIndex = 9;
            cb_DataBits.SelectedIndex = 3;
            cb_Parity.SelectedIndex = 0;
            cb_StopBits.SelectedIndex = 0;
        }
        void SetCurSelSerialPortComboList()
        {
            cb_SerialPort.Text = Info.ScaleSerialPort;
            cb_BoudRate.Text = Info.ScaleBaudRate;
            cb_DataBits.Text = Info.ScaleDataBit;
            cb_Parity.Text = Info.ScaleSerialParity;
            cb_StopBits.Text = Info.ScaleStopBit;
        }
        void GetCurSelSerialPortComboList()
        {

        }

        void Init_SerialPortComBoxList()
        {
            clear_SerialPortComboxList();

            for (int i = 1; i < 100; i++)
            {
                cb_SerialPort.Items.Add("COM" + i);
            }
            cb_BoudRate.Items.AddRange(baudRate);
            for (int i = 5; i <= 8; i++)
            {
                cb_DataBits.Items.Add(i);
            }

            cb_Parity.Items.AddRange(parity);
        
            cb_StopBits.Items.AddRange(stopbit);

            if (Info.ScaleBaudRate == null)
                SetInitSelSerialPortComboList();
            else
                SetCurSelSerialPortComboList();


        }



        Boolean IsServerIPChanged()
        {
            tb_ServerIP.Text = tb_ServerIP.Text.Trim();
            if (String.Compare(Info.ServerIP, tb_ServerIP.Text) != 0)
                return true;
            else
                return false;
        }

        int GetComPortIndex(String port)
        {
            for (int i = 1; i < 100; i++)
            {
                String str = "COM" + i;
                if (String.Compare(port, str) == 0)
                {
                    return i-1;
                }
            }
            return -1;

        }

        int GetBaudRateIndex(String Rate)
        {
            
            for (int i = 0; i<baudRate.Length; i++)
            {
                if (String.Compare(Rate,baudRate[i])==0)
                {
                    return i;
                }
            }
            return -1;
        }

        int GetStopBitIndex(String stopBit)
        {
            for (int i = 0; i < 2; i++)
            {
                if (String.Compare(stopBit, stopbit[i]) == 0)
                {
                    return i;
                }
            }
            return -1;
        }
        int GetParityIndex(String parityStr)
        {
            for (int i = 0; i< parity.Length; i++ )
            {
                if (String.Compare(parityStr, parity[i]) == 0)
                {
                    return i;
                }
            }
            return -1;
        }

        int GetDataBitIndex(String dataBitStr)
        {
            for (int i = 0; i < 4; i++)
            {
                int a = 5 + i;
                String str = ""+a;
                if (String.Compare(dataBitStr, str) == 0)
                {
                    return i;
                }
            }
            return -1;
        }

        Boolean IsScaleComPortChanged()
        {

            if (cb_SerialPort.SelectedIndex != -1)
            {
                int index = GetComPortIndex(Info.ScaleSerialPort);
                if (index != cb_SerialPort.SelectedIndex)
                    return true;
                else
                    return false;
            }
            else
            {
                if (String.Compare(Info.ScaleSerialPort, cb_SerialPort.SelectedItem.ToString()) != 0)
                    return true;
                else
                    return false;
            }
            
        }
        Boolean IsScaleComBaudRateChanged()
        {
            if (cb_BoudRate.SelectedIndex != -1)
            {
                int index = GetBaudRateIndex(Info.ScaleBaudRate);
                if (index != cb_BoudRate.SelectedIndex)
                    return true;
                else
                    return false;
            }
            else
            {
                if (String.Compare(Info.ScaleBaudRate, cb_BoudRate.SelectedItem.ToString()) != 0)
                    return true;
                else
                    return false;
            }
        }
        Boolean IsScaleComDataBitChanged()
        {
            int index = GetDataBitIndex(Info.ScaleDataBit);
            if (index != cb_DataBits.SelectedIndex)
                return true;
            else
                return false;
        }
        Boolean IsScaleComParityChanged()
        {
            int index = GetParityIndex(Info.ScaleSerialParity);
            if (index != cb_Parity.SelectedIndex)
                return true;
            else
                return false;
        }
        Boolean IsScaleComStopBitChanged()
        {
            int index = GetStopBitIndex(Info.ScaleStopBit);
            if (index != cb_StopBits.SelectedIndex)
                return true;
            else
                return false;
        }


        Boolean IsScaleComSettingChanged()
        {
            if (IsScaleComPortChanged()
                || IsScaleComBaudRateChanged()
                || IsScaleComParityChanged()
                || IsScaleComDataBitChanged()
                || IsScaleComStopBitChanged())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        Boolean IsLabelChange()
        {
            return false;
        }
        Boolean IsManufacturePhaseChanged()
        {
            if ((int)MPhase != GlobalConfig.Setting.CurSettingInfo.ManufacturePhase)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private Boolean IsSettingChanged()
        {
#if false
                      
            if ( IsServerIPChanged()
                || IsScaleComSettingChanged()
                || IsLabelChange()
                || IsManufacturePhaseChanged()
                )
                return true;
            else
                return false;
#else
            return true;
#endif 
        }

        private Boolean IsUserInputValid()
        {
            if (IsIpAddressValid(tb_ServerIP.Text))
                return true;
            else {
                MessageBox.Show("服务器IP地址不合法");
                return false;
            }
        }


#endregion

 


        /*
         *These function is used to save the user input to Settings. 
        */
        private Boolean SaveSetting()
        {
            Boolean NeedSave = true;

 

            //}
            if (Info.LabelFileName==null || Info.LabelFileName=="")
                Info.LabelFileName = "default.json";

            //            public String ScaleSerialPort;
            //public String ScaleBaudRate;
            //public String ScaleDataBit;
            //public String ScaleSerialParity;
            //public String ScaleStopBit;

            //public String PrinterSerialPort;
            //public String PrinterBaudRate;
            //public String Label;
            //public int ManufacturePhase;
            //public String LabelFileName;
            Info.SiteConfigured = true;

            Info.MachineNo = mMachineNo.ToString();
            Info.ManufacturePhase = (int)MPhase;

            Info.ScaleSerialPort = cb_SerialPort.SelectedItem.ToString();
            Info.ScaleBaudRate = cb_BoudRate.SelectedItem.ToString();
            Info.ScaleDataBit = cb_DataBits.SelectedItem.ToString();
            Info.ScaleSerialParity = cb_Parity.SelectedItem.ToString();
            Info.ScaleStopBit = cb_StopBits.SelectedItem.ToString();
                           Info.ManufacturePhase = (int)MPhase;
            Info.ServerIP = tb_ServerIP.Text.Trim();
            selectLabel();



            GlobalConfig.Setting.SaveSystemSettingsToJsonFile(Info);
            MessageBox.Show("设定保存!");
            return true;
        }

        private void bt_ok_Click(object sender, EventArgs e)
        {

        }

        private void bt_cancel_Click(object sender, EventArgs e)
        {

        }

        private void bt_ok_Click_1(object sender, EventArgs e)
        {
            if (!IsUserInputValid())
                return;
            SaveSetting();
            this.Close();
        }

        private void bt_cancel_Click_1(object sender, EventArgs e)
        {
            Console.WriteLine("Port" + cb_SerialPort.SelectedIndex);
            this.Close();
        }

        private void SelRadioBoxByMPhase(ManufacturePhaseType type)
        {
            switch (type)
            {
                case ManufacturePhaseType.OUTBOUNING:
                    rb_OutBounding.Checked = true;
                    break;
                case ManufacturePhaseType.LIUYAN:
                    rb_LiuYan1.Checked = true;
                    break;
                case ManufacturePhaseType.CUT:
                    rb_Cut1.Checked = true;
                    break;
                case ManufacturePhaseType.PRINT:
                    rb_Print1.Checked = true;
                    break;
                case ManufacturePhaseType.QUALITYCHECK:
                    rb_QualityCheck.Checked = true;
                    break;
                case ManufacturePhaseType.PACK:
                    rb_pack1.Checked = true;
                    break;
                case ManufacturePhaseType.RECOVERY:
                    rb_Recovery1.Checked = true;
                    break;
                default:
                    break;
            }
        }
        private void rb_OutBounding_CheckedChanged(object sender, EventArgs e)
        {
            
            if (rb_OutBounding.Checked == true)
            {
                mMachineNo = 1;
                MPhase = ManufacturePhaseType.OUTBOUNING;
            }
            //Liu Yan
            else if (rb_LiuYan1.Checked == true)
            {
                mMachineNo = 1;
                MPhase = ManufacturePhaseType.LIUYAN;
            }
            else if (rb_LiuYan2.Checked == true)
            {
                mMachineNo = 2;
                MPhase = ManufacturePhaseType.LIUYAN;
            }
            else if (rb_LiuYan3.Checked == true)
            {
                mMachineNo = 3;
                MPhase = ManufacturePhaseType.LIUYAN;
            }
            else if (rb_LiuYan4.Checked == true)
            {
                mMachineNo = 4;
                MPhase = ManufacturePhaseType.LIUYAN;
            }
            else if (rb_LiuYan5.Checked == true)
            {
                mMachineNo = 5;
                MPhase = ManufacturePhaseType.LIUYAN;
            }
            else if (rb_LiuYan6.Checked == true)
            {
                mMachineNo = 6;
                MPhase = ManufacturePhaseType.LIUYAN;
            }
            else if (rb_LiuYan7.Checked == true)
            {
                mMachineNo = 7;
                MPhase = ManufacturePhaseType.LIUYAN;
            }

            //Cut
            else if (rb_Cut1.Checked == true)
            {
                mMachineNo = 1;
                MPhase = ManufacturePhaseType.CUT;
            }
            else if (rb_Cut2.Checked == true)
            {
                mMachineNo = 2;
                MPhase = ManufacturePhaseType.CUT;
            }
            else if (rb_Cut3.Checked == true)
            {
                mMachineNo = 3;
                MPhase = ManufacturePhaseType.CUT;
            }
            else if (rb_Cut4.Checked == true)
            {
                mMachineNo = 4;
                MPhase = ManufacturePhaseType.CUT;
            }
            else if (rb_Cut5.Checked == true)
            {
                mMachineNo = 5;
                MPhase = ManufacturePhaseType.CUT;
            }
            //Print
            else if (rb_Print1.Checked == true)
            {
                mMachineNo = 1;
                MPhase = ManufacturePhaseType.PRINT;
            }
            else if (rb_Print2.Checked == true)
            {
                mMachineNo = 2;
                MPhase = ManufacturePhaseType.PRINT;
            }
            else if (rb_Print3.Checked == true)
            {
                mMachineNo = 3;
                MPhase = ManufacturePhaseType.PRINT;
            }
            else if (rb_Print4.Checked == true)
            {
                mMachineNo = 4;
                MPhase = ManufacturePhaseType.PRINT;
            }
            else if (rb_Print5.Checked == true)
            {
                mMachineNo = 5;
                MPhase = ManufacturePhaseType.PRINT;
            }

            //QA
            else if (rb_QualityCheck.Checked == true)
            {
                mMachineNo = 1;
                MPhase = ManufacturePhaseType.QUALITYCHECK;
            }
            //Recovery
            else if (rb_Recovery1.Checked == true)
            {
                mMachineNo = 1;
                MPhase = ManufacturePhaseType.RECOVERY;
            }
            else if (rb_Recovery2.Checked == true)
            {
                mMachineNo = 2;
                MPhase = ManufacturePhaseType.RECOVERY;
            }
            else if (rb_Recovery3.Checked == true)
            {
                mMachineNo = 3;
                MPhase = ManufacturePhaseType.RECOVERY;
            }
            //Pack
            else if (rb_pack1.Checked == true)
            {
                mMachineNo = 1;
                MPhase = ManufacturePhaseType.PACK;
            }
            else if (rb_pack2.Checked == true)
            {
                mMachineNo = 2;
                MPhase = ManufacturePhaseType.PACK;
            }

        }

        private void lb_label_SelectedIndexChanged(object sender, EventArgs e)
        {
            String str = this.lb_label.SelectedItem.ToString();
            //string fp = System.Windows.Forms.Application.StartupPath + "\\label\\FilmPrint\\FilmPrint1.json";
            // String subfolder = "FilmPrint";

            string fp = folderpath;
            //string fp = System.Windows.Forms.Application.StartupPath + "\\label\\" + subfolder + "\\";
            //string[] labels = GetJsonFileUnderPath(fp);
            //if (labels == null)
            //  return;
            //fp = fp + labels[0];
            fp = fp + str + ".json";
            if (!File.Exists(fp))  // 判断是否已有相同文件 
            {
                FileStream fs1 = new FileStream(fp, FileMode.Create, FileAccess.ReadWrite);
                fs1.Close();
            }
            try { 
            pattern = JsonHelper.FromJson<LabelPattern>(File.ReadAllText(fp));
            }catch(Exception e1)
            {
                Util.Log.e("Setting",e1.Message);
            }
            //PrintInfo = p.receiptInfos;
            if (pattern != null) { 
                recp.PrintInfo = pattern.receiptInfos;
                }
            else
            {
                recp.PrintInfo = null;
            }
            panel1.Invalidate();
        }

        void selectLabel()
        {
            String labelName = null;
            try {
                labelName = this.lb_label.SelectedItem.ToString();
            }catch (Exception e)
            {
                Info.LabelFileName = "default.json";
            }
            if ((labelName != null) && (labelName != ""))
            {

                SystemSetting SysSetting;
                SysSetting = GlobalConfig.Setting;
                Info.LabelFileName = labelName + ".json";
                
               // SysSetting.SaveSystemSettingsToJsonFile();
                //save to file.
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.PageScale = 1;
            e.Graphics.PageUnit = GraphicsUnit.Millimeter;//单位

            recp.printok_image(e.Graphics);
        }

        private void cb_thermalPrinter_SelectedIndexChanged(object sender, EventArgs e)
        {
            Info.ThermalPrinter = cb_thermalPrinter.SelectedItem.ToString();
        }

        private void cb_Printer_SelectedIndexChanged(object sender, EventArgs e)
        {
            Info.Printer = cb_Printer.SelectedItem.ToString();
        }
    }
}
