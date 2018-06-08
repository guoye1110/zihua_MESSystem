using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using LabelPrint.Data;
namespace LabelPrint
{





    public class SettingInfo
    {
        public Boolean SiteConfigured;
        public String MachineNo;

        public String ServerIP;

        public String ScaleSerialPort;
        public String ScannerSerialPort;
        public String ScaleBaudRate;
        public String ScaleDataBit;
        public String ScaleSerialParity;
        public String ScaleStopBit;

        public String PrinterSerialPort;
        public String PrinterBaudRate;
        public String ThermalPrinter;
        public String Printer;
        public String Label;
        public int ManufacturePhase;
        public String LabelFileName;
    };
    
    public class SystemSetting
    {
        SettingInfo _CurSettingInfo;
        BaseProductInfo _CurBasicProductInfo;
        DynamicPrintLabelData _DynPrinData;
        public int recoveryBigrollIndex = 0;
        static String[] foldernames =
        {
            "outbounding",
            "liuyan",
            "cut",
            "FilmPrint",
            "QA",
            "Recovery",
            "Pack"
        };
        public String GetCurrentSubFolder()
        {
            String  subFolder = foldernames[CurSettingInfo.ManufacturePhase];
            String folderPath = System.Windows.Forms.Application.StartupPath + "\\label\\" + subFolder + "\\";
            return folderPath;
        }

        public String GetCurrentLabelFullName()
        {
            String folderPath = GetCurrentSubFolder();
            return folderPath + _CurSettingInfo.LabelFileName;
        }
        public DynamicPrintLabelData DynPrintData
        {
            get { return _DynPrinData; }
            set { _DynPrinData = value; }
        }
        //String FilePath;
        //String _CurWorker;

        //CutProductItem[]
        //private static readonly SystemSetting instance = new SystemSetting();
        //private SystemSetting() { }
        //public static SystemSetting Instance
        //{
        //    get
        //    {
        //        return instance;
        //    }
        //}
        //public String CurWorker
        //{
        //    get { return _CurWorker; }
        //    set { _CurWorker = value; }
        //}
        public SettingInfo CurSettingInfo
        {
            get { return _CurSettingInfo; }
            set { _CurSettingInfo = value; }
        }
        public SettingInfo GetDefaultSetting()
        {
            SettingInfo info = new SettingInfo();
            info.MachineNo = "";
            info.ServerIP = "";

            info.ScaleSerialPort = "COM1";
            info.ScannerSerialPort = "COM2";
            info.ScaleBaudRate = "38400";
            info.ScaleDataBit = "8";
            info.ScaleSerialParity = "None";
            info.ScaleStopBit = "1";
            info.Label = "Label1";
            info.ManufacturePhase = (int)ManufacturePhaseType.PRINT;
            return info;
        }

        public BaseProductInfo CurBasicProductInfo
        {
            get { return _CurBasicProductInfo; }
            set { _CurBasicProductInfo = value;}
        }

        public BaseProductInfo GetBasicProductInfoFromServer()
        {
            return null;
        }

        public void SaveSettingToFile(SettingInfo Info)
        {


        }
        public void RestoreDefaultLabel()
        {
            if ( _CurSettingInfo != null){
                _CurSettingInfo.LabelFileName = "default.json";
            }
        }
        public Boolean SaveSystemSettingsToJsonFile()
        {
            return SaveSystemSettingsToJsonFile(CurSettingInfo);
        }
        public Boolean SaveSystemSettingsToJsonFile(SettingInfo Info)
        {
            string fp = System.Windows.Forms.Application.StartupPath + "\\Setting.json";
            if (!File.Exists(fp))  // 判断是否已有相同文件 
            {
                FileStream fs1 = new FileStream(fp, FileMode.Create, FileAccess.ReadWrite);
                fs1.Close();
            }

            File.WriteAllText(fp, JsonHelper.ToJson(Info));
            return true;
        }

        public SettingInfo RestoreSystemSettingsFromJsonFile()
        {
            string fp = System.Windows.Forms.Application.StartupPath + "\\Setting.json";
            if (!File.Exists(fp))  // 判断是否已有相同文件 
            {
                FileStream fs1 = new FileStream(fp, FileMode.Create, FileAccess.ReadWrite);
                fs1.Close();
            }
            SettingInfo p = JsonHelper.FromJson<SettingInfo>(File.ReadAllText(fp));
            return p;
        }
    }
}
