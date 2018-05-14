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
    public partial class LabelDesignForm : Form
    {

        LabelPattern pattern;
        String folderpath;
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

        public LabelDesignForm()
        {
            InitializeComponent();
        }
        

        private void LabelDesignForm_Load(object sender, EventArgs e)
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
                    if (String.Compare(GlobalConfig.Setting.CurSettingInfo.LabelFileName, labelstr)==0)
                    {
                        index = i;
                    }
                }
            }
            this.lb_label.SelectedIndex = index;
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
            fp = fp + str+ ".json";
            if (!File.Exists(fp))  // 判断是否已有相同文件 
            {
                FileStream fs1 = new FileStream(fp, FileMode.Create, FileAccess.ReadWrite);
                fs1.Close();
            } 

            pattern = JsonHelper.FromJson<LabelPattern>(File.ReadAllText(fp));
            //PrintInfo = p.receiptInfos;
            recp.PrintInfo = pattern.receiptInfos;
            panel1.Invalidate();
        }

        PrintHelper helper = new PrintHelper();
        Receipt1 recp = new Receipt1();
        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.PageScale = 1;
            e.Graphics.PageUnit = GraphicsUnit.Millimeter;//单位
            
            recp.printok_image(e.Graphics);
        }
        void selectLabel()
        {
            String labelName = this.lb_label.SelectedItem.ToString();
            if ((labelName != null) && (labelName != ""))
            {

                SystemSetting SysSetting;
                SysSetting = GlobalConfig.Setting;
                SysSetting.CurSettingInfo.LabelFileName = labelName + ".json";
                
                SysSetting.SaveSystemSettingsToJsonFile();
                //save to file.
            }
        }
        private void bt_ok_Click(object sender, EventArgs e)
        {
            selectLabel();
        }
    }
}
