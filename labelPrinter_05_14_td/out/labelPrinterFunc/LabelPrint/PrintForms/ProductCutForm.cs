using LabelPrint.Label;
using System;
using System.IO.Ports;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.Drawing.Imaging;
using LabelPrinting;
using LabelPrint.Receipt;
using LabelPrint.Util;
using LabelPrint.Data;
using LabelPrint.PrintForms;
using LabelPrint.excelOuput;
using LabelPrint.NetWork;

namespace LabelPrint
{
    public partial class ProductCutForm : Form
    {
		//分切工序
		private const int COMMUNICATION_TYPE_SLIT_PROCESS_START = 0xBF;
		private const int COMMUNICATION_TYPE_SLIT_PROCESS_MATERIAL_BARCODE_UPLOAD = 0xC0;
		private const int COMMUNICATION_TYPE_SLIT_PROCESS_PRODUCT_BARCODE_UPLOAD = 0xC1;
		private const int COMMUNICATION_TYPE_SLIT_PROCESS_PACKAGE_BARCODE_UPLOAD = 0xC2;
		private const int COMMUNICATION_TYPE_SLIT_PROCESS_END = 0xC3;
		private FilmSocket m_FilmSocket;
		FilmSocket.networkstatehandler m_networkstatehandler;
		private string m_dispatchCode;

        String[] ProductStateStr = { "合格品", "不合格品","待处理","边角料","废料" };
        String[] ProductQualityStr = { "A", "B", "C", "D", "DC", "E", "W" };
        String[] ProductQualityStrForComBoList = { "A-晶点孔洞", "B-厚薄暴筋", "C-皱折", "D-端面错位(毛糙)", "DC-待处理", "E-油污", "W-蚊虫" };
        String[] YesNoStr = { "是", "否" };
#pragma warning disable CS0649 // Field 'ProductCutForm.ProdItem' is never assigned to, and will always have its default value null
        CutProductItem ProdItem;
#pragma warning restore CS0649 // Field 'ProductCutForm.ProdItem' is never assigned to, and will always have its default value null

        CutUserinputData UserInput;

        SerialPort serialPort1;

        //String CurProductState;// = GetProductState();
        //String CurProductQuality;// = GetProductQuality();
        //String CurShowRealWeight;// = GetShowRealWeight();
        //String CurDesc;
        //String CurBigRollNo;
        //String CurLittleRollNo;
        int BigRollNo = 1;
        int LittleRoleNo = 1;
        int TotalRoll = 0;
        //PlateInfo CurPlatInfo;
        //   String CurWorkNo;
        const int ProductTypeCount = 3;
        TextBox[] tb_CustomerNames = new TextBox[ProductTypeCount];
        TextBox[] tb_BatchNos = new TextBox[ProductTypeCount];
        TextBox[] tb_Recipes = new TextBox[ProductTypeCount];
        TextBox[] tb_PlateNos = new TextBox[ProductTypeCount];
        TextBox[] tb_PlateRollPerLays = new TextBox[ProductTypeCount];
        TextBox[] tb_PlateLayers = new TextBox[ProductTypeCount];
        TextBox[] tb_PlateRollNums = new TextBox[ProductTypeCount];
        TextBox[] tb_RawMaterialCodes = new TextBox[ProductTypeCount];

        Byte[] serialDataBuf = new Byte[128];

        int MaxLittleRoll = 14;

        const int MaxMiscLittleRoll = 14;
        int MiscRollIndexInOneRound = 0;
        int productTypeIndex = 0;
        int MaxSupportRollOneTime;
       // MiscCutData[] MiscDataList = new MiscCutData[MaxMiscLittleRoll];
        MiscCutProductData[] MiscProductList = new MiscCutProductData[3];
        int TotalProductInMisc = 0;
        List<String> ProductStrList = new List<String>();
        ComboBox[] cb_ProductCodes = new ComboBox[MaxMiscLittleRoll];
        TextBox[] tb_Widths = new TextBox[MaxMiscLittleRoll];

        BardCodeHooK BarCodeHook = new BardCodeHooK();

        //void InitMiscDataList()
        //{
        //    for (int i = 0; i < MiscDataList.Length; i++)
        //    {
        //        MiscDataList[i] = new MiscCutData();
        //        MiscDataList[i].ProductCode = null;
        //        MiscDataList[i].Width = null;
        //    }
        //}
        public ProductCutForm(FilmSocket filmsocket)
        {
          //  InitMiscDataList();
            InitializeComponent();
			m_FilmSocket = filmsocket;
        }
		~ProductCutForm()
        {
            m_FilmSocket.network_state_event -= m_networkstatehandler;
		}

		public void network_status_change(bool status)
        {
        	Console.WriteLine("network changed to {0}", status);
		}

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        void InitProductCodeComboBox(ComboBox productCode, String[] strs)
        {
            productCode.Items.AddRange(strs);
        }


        void InitProductStateComboBox(ComboBox productStates)
        {
            productStates.Items.AddRange(ProcessData.ProductStateStr);

        }

        void InitProductQualityComboBox(ComboBox productQulity)
        {
            productQulity.Items.AddRange(ProcessData.ProductQualityStrForComBoList);

        }

        void InitShowRealWeight(ComboBox showRealWeight)
        {
            showRealWeight.Items.AddRange(YesNoStr);
        }


        void SetPlateRollComponent1(PlateInfo plateInfo)
        {
            tb_PlateRollPerLay1.Text = plateInfo.LittleRollPerlayer.ToString();
            tb_PlateLayer1.Text = plateInfo.Layer.ToString();
            tb_PlateRollNum1.Text = plateInfo.getMaxLittleRoll().ToString();
        }

        void SetPlateRollComponent2(PlateInfo plateInfo)
        {
            tb_PlateRollPerLay2.Text = plateInfo.LittleRollPerlayer.ToString();
            tb_PlateLayer2.Text = plateInfo.Layer.ToString();
            tb_PlateRollNum2.Text = plateInfo.getMaxLittleRoll().ToString();
        }

        void SetPlateRollComponent3(PlateInfo plateInfo)
        {
            tb_PlateRollPerLay3.Text = plateInfo.LittleRollPerlayer.ToString();
            tb_PlateLayer3.Text = plateInfo.Layer.ToString();
            tb_PlateRollNum3.Text = plateInfo.getMaxLittleRoll().ToString();
        }

        void SetPlateNo1(PlateInfo plateInfo)
        {
            tb_PlateNo1.Text = plateInfo.PLateNo.ToString();
        }
        void SetPlateNo2(PlateInfo plateInfo)
        {
            tb_PlateNo2.Text = plateInfo.PLateNo.ToString();
        }
        void SetPlateNo3(PlateInfo plateInfo)
        {
            tb_PlateNo3.Text = plateInfo.PLateNo.ToString();
        }
        void LimitPlateTextBox(int a, int b)
        {
            tb_PlateRollPerLay1.MaxLength = a;
            tb_PlateRollPerLay1.MaxLength = a;
            tb_PlateRollPerLay1.MaxLength = a;
            tb_PlateLayer1.MaxLength = b;
            tb_PlateLayer2.MaxLength = b;
            tb_PlateLayer3.MaxLength = b;
            tb_PlateRollNum1.MaxLength = a + b;//1x1 max=2 1x2 max=3 2x2 max=4 2x3 max=5
            tb_PlateRollNum2.MaxLength = a + b;
            tb_PlateRollNum3.MaxLength = a + b;
        }

        void LimitPlateTextBox(int a)
        {
            LimitPlateTextBox(a, a);
        }

        void InitialPlateInfo(PlateInfo plateInfo, ManufactureType type)
        {
            LimitPlateTextBox(2);
            if (type == ManufactureType.M_MULTIPLE)
            {

                SetPlateRollComponent1(plateInfo);
                SetPlateRollComponent2(plateInfo);
                SetPlateRollComponent3(plateInfo);
                SetPlateNo1(plateInfo);
                SetPlateNo2(plateInfo);
                SetPlateNo3(plateInfo);
            }
            else
            {
                SetPlateRollComponent1(plateInfo);
                SetPlateNo1(plateInfo);
            }
        }

        void InitialSettingDataForForm()
        {
            SystemSetting SysSetting;
            SysSetting = GlobalConfig.Setting;

            tx_ScaleSerialPort.Text = SysSetting.CurSettingInfo.ScaleSerialPort;
            tb_CutMachineNo.Text = SysSetting.CurSettingInfo.MachineNo;
            tb_worker.Text = gVariable.userAccount;
            tb_CutMachineNo.Enabled = false;

            tb_worker.Text = gVariable.userAccount;
            tb_worker.Enabled = false;



        }

        Boolean InitialDataForForm(CutProductItem ProdItem)
        {
            CutSampleData SysData;
            SystemSetting SysSetting;
            SysSetting = GlobalConfig.Setting;
            //CutProductItem ProdItem;
            SysData = CutSampleData.Instance;
            //WorkerNumberTextBox.Text = ProdItem.WorkerNumber;
            tb_worker.Text = SysData.CreateNormalLabelString(null);

            InitProductStateComboBox(cb_ProductState);
            InitProductQualityComboBox(cb_ProductQuality);
            InitShowRealWeight(cb_ShowRealWight);

            cb_ProductState.SelectedIndex = 0;
            cb_ProductQuality.SelectedIndex = 0;
            cb_ShowRealWight.SelectedIndex = 1;

            CutProductItem Item = null; //= SysData.GetCurProductItem();
            if (Item != null)
            {
                cb_ProductCode1.Text = Item.ProductCode[0];
                tb_RecipeCode1.Text = Item.RawMaterialCode[0];
                tb_RecipeCode2.Text = Item.RawMaterialCode[1];
                tb_RecipeCode3.Text = Item.RawMaterialCode[2];

                //tb_MaterialName1.Text = Item.MaterialName[0];
                //tb_MaterialName2.Text = Item.MaterialName[1];
                //tb_MaterialName3.Text = Item.MaterialName[2];

                tb_CustomerName1.Text = Item.CustomerName[0];
                tb_CustomerName2.Text = Item.CustomerName[1];
                tb_CustomerName3.Text = Item.CustomerName[2];

                tb_BatchNo1.Text = Item.BatchNo[0];
                tb_BatchNo2.Text = Item.BatchNo[1];
                tb_BatchNo3.Text = Item.BatchNo[2];

                tb_WorkNo.Text = Item.OrderNo;
                tb_BigRollNo.Text = Item.BigRollNo;
                tb_LittleRollWeight.Text = Item.Weight;
            }
            tb_ManHour.Text = "0";
            tb_JointCount.Text = "0";

            // String bigRoll = "086";
            //  BigRollNo = 080;
            // LittleRoleNo = 0;
            //UpdateBigRollNo(BigRollNo);
            tb_BigRollNo.Text = CommonFormHelper.UpdateBigRollNo(BigRollNo);
            //         UpdateLittleRollNo(BigRollNo, LittleRoleNo);
            tb_LittleRollNo.Text = CommonFormHelper.UpdateLittleRollNo(BigRollNo, LittleRoleNo);
            //tb_LittleRollNo.Text = Item.LittleRoleNo;


            tb_PlateRollNum1.Enabled = false;
            tb_PlateRollNum2.Enabled = false;
            tb_PlateRollNum3.Enabled = false;



            //tb_CutMachineNo.Text = SysSetting.CurSettingInfo.MachineNo;
            return true;
        }



        private void SetComponentsByManufactureType(ManufactureType type)
        {
            if (type == ManufactureType.M_MULTIPLE)
            {
           //     cb_ProductCodes[MiscRollIndexInOneRound].BackColor = Color.Yellow;
             //   tb_Widths[MiscRollIndexInOneRound].BackColor = Color.Yellow;
                //for (int i = 1; i < MaxMiscLittleRoll; i++)
                    for (int i = 1; i < 1; i++)
                    {
                    cb_ProductCodes[i].Enabled = true;
                    tb_Widths[i].Enabled = true;
                }


                tb_RecipeCode2.Enabled = true;
                tb_RecipeCode3.Enabled = true;


                //tb_MaterialName2.Enabled = true;
                //tb_MaterialName3.Enabled = true;

                tb_CustomerName2.Enabled = true;
                tb_CustomerName3.Enabled = true;

                tb_BatchNo2.Enabled = true;
                tb_BatchNo3.Enabled = true;

                tb_PlateLayer2.Enabled = true;
                tb_PlateLayer3.Enabled = true;
                tb_PlateRollNum2.Enabled = true;
                tb_PlateRollNum3.Enabled = true;
                tb_PlateRollPerLay2.Enabled = true;
                tb_PlateRollPerLay3.Enabled = true;

                tb_PlateNo2.Enabled = true;
                tb_PlateNo3.Enabled = true;
                tb_RawMaterialCode2.Enabled = true;
                tb_RawMaterialCode3.Enabled = true;
            }
            else
            {

                //Restore the set work color and set the single work color.
                cb_ProductCodes[MiscRollIndexInOneRound].BackColor = Color.White;
                tb_Widths[MiscRollIndexInOneRound].BackColor = Color.White;

                cb_ProductCodes[0].BackColor = Color.White;
                tb_Widths[0].BackColor = Color.White;
                tb_CustomerNames[0].BackColor = Color.White;
                tb_BatchNos[0].BackColor = Color.White;
                tb_Recipes[0].BackColor = Color.White;
                tb_PlateNos[0].BackColor = Color.White;
                tb_RawMaterialCodes[0].BackColor = Color.White;

                tb_PlateLayers[0].BackColor = Color.White;
                tb_PlateRollPerLays[0].BackColor = Color.White;
                tb_PlateRollNums[0].BackColor = Color.White;


                for (int i = 1; i < MaxMiscLittleRoll; i++)
                {
                    cb_ProductCodes[i].Enabled = false;
                    tb_Widths[i].Enabled = false;
                }



                tb_RecipeCode2.Enabled = false;
                tb_RecipeCode3.Enabled = false;


                //tb_MaterialName2.Enabled = false;
                //tb_MaterialName3.Enabled = false;

                tb_CustomerName2.Enabled = false;
                tb_CustomerName3.Enabled = false;


                tb_BatchNo2.Enabled = false;
                tb_BatchNo3.Enabled = false;

                tb_PlateLayer2.Enabled = false;
                tb_PlateLayer3.Enabled = false;
                tb_PlateRollNum2.Enabled = false;
                tb_PlateRollNum3.Enabled = false;
                tb_PlateRollPerLay2.Enabled = false;
                tb_PlateRollPerLay3.Enabled = false;

                tb_PlateNo2.Enabled = false;
                tb_PlateNo3.Enabled = false;
                tb_RawMaterialCode2.Enabled = false;
                tb_RawMaterialCode3.Enabled = false;


            }
        }
        //object [] GetComboStrsForProductCode()
        //{
        //    DataTable dt = null;
        //    dt = mySQLClass.queryDataTableAction("sampledatabase", "select productCode from productlist", null);

        //    if (dt == null)
        //        return null;
        //    object[] productCodes = new object[dt.Rows.Count];
        //    for (int i = 0; i < dt.Rows.Count; i++)
        //        productCodes[i] = dt.Rows[i][0];
        //    return productCodes;
        //}
        void InitProductCodeArray()
        {
            cb_ProductCodes[0] = cb_ProductCode1;
            cb_ProductCodes[1] = cb_ProductCode2;
            cb_ProductCodes[2] = cb_ProductCode3;
            cb_ProductCodes[3] = cb_ProductCode4;
            cb_ProductCodes[4] = cb_ProductCode5;
            cb_ProductCodes[5] = cb_ProductCode6;
            cb_ProductCodes[6] = cb_ProductCode7;
            cb_ProductCodes[7] = cb_ProductCode8;
            cb_ProductCodes[8] = cb_ProductCode9;
            cb_ProductCodes[9] = cb_ProductCode10;
            cb_ProductCodes[10] = cb_ProductCode11;
            cb_ProductCodes[11] = cb_ProductCode12;
            cb_ProductCodes[12] = cb_ProductCode13;
            cb_ProductCodes[13] = cb_ProductCode14;
        }
        void InitWidthsArray()
        {
            tb_Widths[0] = tb_Width1;
            tb_Widths[1] = tb_Width2;
            tb_Widths[2] = tb_Width3;
            tb_Widths[3] = tb_Width4;
            tb_Widths[4] = tb_Width5;
            tb_Widths[5] = tb_Width6;
            tb_Widths[6] = tb_Width7;
            tb_Widths[7] = tb_Width8;
            tb_Widths[8] = tb_Width9;
            tb_Widths[9] = tb_Width10;
            tb_Widths[10] = tb_Width11;
            tb_Widths[11] = tb_Width12;
            tb_Widths[12] = tb_Width13;
            tb_Widths[13] = tb_Width14;

        }

        void InitCustomerNameArray()
        {
            tb_CustomerNames[0] = tb_CustomerName1;
            tb_CustomerNames[1] = tb_CustomerName2;
            tb_CustomerNames[2] = tb_CustomerName3;
        }
        void InitBatchNoArray()
        {
            tb_BatchNos[0] = tb_BatchNo1;
            tb_BatchNos[1] = tb_BatchNo2;
            tb_BatchNos[2] = tb_BatchNo3;
        }
        void InitRecipeArray()
        {
            tb_Recipes[0] = tb_RecipeCode1;
            tb_Recipes[1] = tb_RecipeCode2;
            tb_Recipes[2] = tb_RecipeCode3;

        }
        void InitPlateNoArray()
        {
            tb_PlateNos[0] = tb_PlateNo1;
            tb_PlateNos[1] = tb_PlateNo2;
            tb_PlateNos[2] = tb_PlateNo3;
        }

        void InitPlatoMatrixInfoArrays()
        {
            tb_PlateRollPerLays[0] = tb_PlateRollPerLay1;
            tb_PlateRollPerLays[1] = tb_PlateRollPerLay2;
            tb_PlateRollPerLays[2] = tb_PlateRollPerLay3;
            tb_PlateLayers[0] = tb_PlateLayer1;
            tb_PlateLayers[1] = tb_PlateLayer2;
            tb_PlateLayers[2] = tb_PlateLayer3;
            tb_PlateRollNums[0] = tb_PlateRollNum1;
            tb_PlateRollNums[1] = tb_PlateRollNum2;
            tb_PlateRollNums[2] = tb_PlateRollNum3;
        }

        void InitRawMaterialCodeArrays()
        {
            tb_RawMaterialCodes[0] = tb_RawMaterialCode1;
            tb_RawMaterialCodes[1] = tb_RawMaterialCode2;
            tb_RawMaterialCodes[2] = tb_RawMaterialCode3;
        }

        void InitComponentArray()
        {
            InitProductCodeArray();
            InitWidthsArray();
            InitCustomerNameArray();
            InitBatchNoArray();
            InitRecipeArray();
            InitPlateNoArray();
            InitPlatoMatrixInfoArrays();
            InitRawMaterialCodeArrays();
        }
        //TextBox[] tb_CustomerNames = new TextBox[ProductTypeCount];
        //TextBox[] tb_BatchNos = new TextBox[ProductTypeCount];
        //TextBox[] tb_Recipes = new TextBox[ProductTypeCount];
        //TextBox[] tb_PlateNos = new TextBox[ProductTypeCount];
        private void ProductCutForm_Load(object sender, EventArgs e)
        {
            CutSampleData SysData;
            //CurPlatInfo = new PlateInfo(6, 9);

            UserInput = new CutUserinputData();
            UserInput.CreateDataTable();
            UserInput.CurPlatInfo = new PlateInfo(6, 9);
            SysData = CutSampleData.Instance;
            DataTable dt = new DataTable();
            InitComponentArray();
            this.Icon = new Icon("..\\..\\resources\\zihualogo_48X48.ico");

            BarCodeHook.BarCodeEvent += new BardCodeHooK.BardCodeDeletegate(BarCode_BarCodeEvent);
            BarCodeHook.Start();

            object[] productCodes = UserInput.GetComboStrsForProductCode();
            for (int i = 0; i < MaxMiscLittleRoll; i++)
            {
                cb_ProductCodes[i].Items.AddRange(productCodes);
            }

            //InitProductCodeComboBox( dt);
            //ProdItem = SysData.GetCurProductItem();
            InitialDataForForm(ProdItem);


            InitialSettingDataForForm();


            SystemSetting SysSetting;
            SysSetting = GlobalConfig.Setting;


            //    SysSetting.DynPrintData = new DynamicPrintLabelData();
            //    SysSetting.DynPrintData.setWorkProcess("Cut");

            SetWorkClassType(WorkClassType.BING);

            //SetWorkClassTypeUnChangeAble(WorkClassType.CLASS_BING);

            SetWorkTimeType(WorkTimeType.DAYWORK);

            //SetWorkTimeUnChangeable(WorkTimeType.TIME_DAY);

            ManufactureType Mtype = ManufactureType.M_SINGLE;
            SetManufactureType(Mtype);

            SetComponentsByManufactureType(Mtype);
            InitialPlateInfo(UserInput.CurPlatInfo, Mtype);
            //ProdItem.

            //rb_NoonWork.Visible = false;

            initSerialPort();

			m_networkstatehandler = new FilmSocket.networkstatehandler(network_status_change);
			m_FilmSocket.network_state_event += m_networkstatehandler;
        }

        private void ProductCutForm_FormClosing(object sender, EventArgs e)
        {
            serialPort1.Close();
        }
        
        void initSerialPort()
        {
            SystemSetting SysSetting;
            SysSetting = GlobalConfig.Setting;

            try
            {
                serialPort1 = new SerialPort(SysSetting.CurSettingInfo.ScaleSerialPort, 9600, Parity.None, 8, StopBits.One);
                serialPort1.Open();
                serialPort1.DataReceived += new SerialDataReceivedEventHandler(serialDataReceived);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Open serial port failed!" + ex);
            }
        }

        void serialDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int num;
            string weightStr;

            try
            {
                num = 0;
                weightStr = "";

                Thread.Sleep(1000);

                while (weightStr.Length < 4)
                {
                    serialPort1.Read(serialDataBuf, 0, serialPort1.BytesToRead);
                    weightStr += getSerialPortOutput(serialDataBuf);
                    if (weightStr.Length == 0)
                        break;

                    num++;
                    if (num > 2)
                        break;
                }

                if (weightStr.Length > 1)
                {
                    this.Invoke((EventHandler)(delegate
                    {
                        tb_LittleRollWeight.Text = weightStr;
                        printOneLabel();
                    }));
                }
            }
            catch (TimeoutException ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        string getSerialPortOutput(byte[] serialDataBuf)
        {
            int i;
            int start, end;
            int flag;
            string str; 

            flag = 0;
            start = 0;
            end = 0;

            str = System.Text.Encoding.ASCII.GetString(serialDataBuf);
            //Console.WriteLine(DateTime.Now.ToString("yy-MM-dd HH:mm:ss") + ": " + str);

            for (i = 0; i < 20; i++)
            {
                Console.Write(serialDataBuf[i].ToString() + ", ");
                if (serialDataBuf[i] == 0x0d || serialDataBuf[i] == 0x0a || serialDataBuf[i] == 0)
                {
                    end = i;
                    break;
                }

                //we got the data header
                if(flag == 0 && serialDataBuf[i] == 0x20 && serialDataBuf[i + 1] == 0x20 && serialDataBuf[i + 2] == 0x20 && serialDataBuf[i + 3] == 0x20)
                {
                    flag = 1;
                }
                else if (flag == 1 && serialDataBuf[i] != 0x20)  //data start position
                {
                    start = i;
                    flag = 2;
                }
            }

            Console.WriteLine(" ");
            
            if (i > 20)
                return null;

            return ( System.Text.Encoding.ASCII.GetString(serialDataBuf, start, end - start));
        }


        private void SetManufactureType(ManufactureType type)
        {
            switch (type)
            {
                case ManufactureType.M_MULTIPLE:
                    rb_SetWork.Checked = true;
                    break;
                case ManufactureType.M_SINGLE:
                    rb_SingleWork.Checked = true;
                    break;
            }
        }
        private ManufactureType GetManufactureType()
        {
            if (rb_SetWork.Checked)
                return ManufactureType.M_MULTIPLE;
            else
                return ManufactureType.M_SINGLE;
        }


        private void SetWorkClassType(WorkClassType type)
        {
            switch (type)
            {
                case WorkClassType.JIA:
                    rb_Jia.Checked = true;
                    break;
                case WorkClassType.YI:
                    rb_Yi.Checked = true;
                    break;
                case WorkClassType.BING:
                    rb_Bing.Checked = true;
                    break;
                    //case WorkClassType.DING:
                    //    rb_Ding.Checked = true;
                    //    break;
            }
        }
        private void SetWorkClassTypeUnChangeAble(WorkClassType type)
        {
            rb_Jia.Enabled = false;
            rb_Yi.Enabled = false;
            rb_Bing.Enabled = false;
            //rb_Ding.Enabled = false;
            switch (type)
            {
                case WorkClassType.JIA:
                    rb_Jia.Enabled = true;
                    break;
                case WorkClassType.YI:
                    rb_Yi.Enabled = true;
                    break;
                case WorkClassType.BING:
                    rb_Bing.Enabled = true;
                    break;
                    //case WorkClassType.DING:
                    //    rb_Ding.Enabled = true;
                    //    break;
            }
        }

        private WorkClassType GetWorkClassType()
        {
            if (rb_Jia.Checked)
            {
                return WorkClassType.JIA;
            }
            else if (rb_Yi.Checked)
            {
                return WorkClassType.YI;
            }
            else if (rb_Bing.Checked)
            {
                return WorkClassType.BING;
            }
            //else if (rb_Ding.Checked)
            //{
            //    return WorkClassType.DING;
            //}
            return WorkClassType.JIA;
        }

        private void SetWorkTimeType(WorkTimeType type)
        {
            switch (type)
            {
                case WorkTimeType.DAYWORK:
                    rb_DayWork.Checked = true;
                    break;
                //case WorkTimeType.MIDDLEWORK:
                //    rb_NoonWork.Checked = true;
                //    break;
                case WorkTimeType.NIGHTWORK:
                    rb_NightWork.Checked = true;
                    break;
            }
        }
        private void SetWorkTimeUnChangeable(WorkTimeType type)
        {
            rb_DayWork.Enabled = false;
            //rb_NoonWork.Enabled = false;
            rb_NightWork.Enabled = false;
            switch (type)
            {
                case WorkTimeType.DAYWORK:
                    rb_DayWork.Enabled = true;
                    break;
                //case WorkTimeType.MIDDLEWORK:
                //    rb_NoonWork.Enabled = true;
                //    break;
                case WorkTimeType.NIGHTWORK:
                    rb_NightWork.Enabled = true;
                    break;
            }
        }
        private WorkTimeType GetWorkTimeType()
        {
            if (rb_DayWork.Checked)
            {
                return WorkTimeType.DAYWORK;
            }
            else if (rb_NightWork.Checked)
            {
                return WorkTimeType.NIGHTWORK;
            }
            //else if (rb_NoonWork.Checked)
            //{
            //    return WorkTimeType.MIDDLEWORK;
            //}
            return WorkTimeType.DAYWORK;
        }

        private void rb_DayWork_Click(object sender, EventArgs e)
        {
            if (rb_DayWork.Checked)
            {
                //    label26.Text = "day";
            }
            else if (rb_NightWork.Checked)
            {
                //    label26.Text = "Night";
            }
            //else if (rb_NoonWork.Checked)
            //{
            //    label26.Text = "noon";
            //}
        }

        private void rb_Jia_Click(object sender, EventArgs e)
        {
            if (rb_Jia.Checked)
            {
                //   label26.Text = "甲";
            }
            else if (rb_Yi.Checked)
            {
                //  label26.Text = "乙";
            }
            else if (rb_Bing.Checked)
            {
                //  label26.Text = "丙";
            }

        }

        private void rb_SingleWork_Click(object sender, EventArgs e)
        {
            if (rb_SingleWork.Checked)
            {
            }
            else if (rb_SetWork.Checked)
            {
            }
        }

        #region Update the Data input by worker and check
        /// <summary>
        /// Following is the data cache
        /// </summary>


        Boolean IsExistInList(List<String> list, String str)
        {
            foreach (String var in list)
            {
                if (String.Compare(str, var) == 0)
                    return true;
            }
            return false;
        }


        int GetIndexInList(List<String> list, String str)
        {
            int i = 0;
            foreach (String var in list)
            {
                if (String.Compare(str, var) == 0)
                    return i;
                i++;
            }
            return -1;
        }

        void UpdateProductTypes()
        {

            //for (int i = 0; i< MiscDataList.Length; i++)
            //{
            //    if (MiscDataList[i].ProductCode!=null)
            //    {
            //        if (!IsExistInList(ProductStrList, MiscDataList[i].ProductCode))
            //            ProductStrList.Add(MiscDataList[i].ProductCode);
            //    }
            //}
        }

        void UpdateMiscProcductCodeAndWidth()
        {
            //if (UserInput.MType == ManufactureType.M_SINGLE) {
            //    MiscDataList[0].ProductCode = cb_ProductCode1.Text;
            //    MiscDataList[0].Width = tb_Width1.Text;
            //    return;
            //}

            //for (int i = 0; i< MiscDataList.Length; i++)
            //{
            //    if (cb_ProductCodes[i].Text !=null)
            //    {
            //        MiscDataList[i].ProductCode = cb_ProductCodes[i].Text;
            //        MiscDataList[i].Width = tb_Widths[i].Text;
            //    }

            //}

        }

        //String GetProductWidth2()
        //{
        //    String str = null;
        //    if (UserInput.MType == ManufactureType.M_SINGLE)
        //    {
        //        str = tb_Width1.Text;
        //    }
        //    else
        //    {
        //        str = MiscDataList[MiscRollIndexInOneRoll].Width;
        //    }
        //    return str;
        //}

        //String GetProductCode2()
        //{
        //    String str = null;
        //    if (UserInput.MType == ManufactureType.M_SINGLE)
        //    {
        //        str = cb_ProductCode1.Text;
        //    }
        //    else
        //    {
        //        str = MiscDataList[MiscRollIndexInOneRoll].ProductCode;
        //    }
        //    return str;
        //}

        String GetProductWidth()
        {
            String str = null;
            if (UserInput.MType == ManufactureType.M_SINGLE)
            {
                str = tb_Width1.Text;
            }
            else
            {
                str = tb_Widths[MiscRollIndexInOneRound].Text;

            }
            return str;
        }

        String GetProductCode()
        {
            String str = null;
            if (UserInput.MType == ManufactureType.M_SINGLE)
            {
                str = cb_ProductCode1.Text;
            }
            else
            {
                str = cb_ProductCodes[MiscRollIndexInOneRound].Text;
            }
            return str;
        }

        private void UpdateUserInputByIndex(int idx)
        {
            UserInput.CustomerName = tb_CustomerNames[idx].Text;
            UserInput.BatchNo = tb_BatchNos[idx].Text;
            UserInput.RecipeCode = tb_Recipes[idx].Text;
            UserInput.PlateNo = tb_PlateNos[idx].Text;
            UserInput.RawMaterialCode = tb_RawMaterialCodes[idx].Text;

            UserInput.PlateRollNum = tb_PlateRollNums[idx].Text;
            UserInput.PlateRollPerLay = tb_PlateRollPerLays[idx].Text;
            UserInput.PlateLayer = tb_PlateLayers[idx].Text;
        }
        private void UpdateUserInputInSingleMode()
        {
            UpdateUserInputByIndex(0);
            
            int.TryParse(UserInput.PlateNo, out UserInput.CurPlatInfo.PLateNo);
            int.TryParse(UserInput.PlateRollPerLay, out UserInput.CurPlatInfo.LittleRollPerlayer);
            int.TryParse(UserInput.PlateLayer, out UserInput.CurPlatInfo.Layer);

            UserInput.LittleRollNo = GetCurLittleRollNo(tb_LittleRollNo);
        }
        private void UpdateUserInputInMutipleMode()
        {
            int idx = 0;
            idx = UserInput.SetWorkProductInfo.FindInfoIndexByComboIndex(MiscRollIndexInOneRound);
            if (idx == -1)
                return;
            UpdateUserInputByIndex(idx);
            int.TryParse(UserInput.PlateNo, out UserInput.CurPlatInfo.PLateNo);
            int.TryParse(UserInput.PlateRollPerLay, out UserInput.CurPlatInfo.LittleRollPerlayer);
            int.TryParse(UserInput.PlateLayer, out UserInput.CurPlatInfo.Layer);


            MiscCutProductData setWorkProductData = UserInput.SetWorkProductInfo.FindInfoByComboIndex(MiscRollIndexInOneRound);
            UserInput.RawMaterialCode = setWorkProductData.RawMaterialCode;
            UserInput.ProductLength = setWorkProductData.productLength;
            UserInput.ProductName = setWorkProductData.productName;
            UserInput.ProductWeight = setWorkProductData.productWeight;

            int LitNo = 0;
            Boolean value = int.TryParse(tb_LittleRollNo.Text, out LitNo);
            if (value)
            {
                UserInput.LittleRollNo = tb_LittleRollNo.Text;
                setWorkProductData.LittleRollNo = LitNo;
            }
            else
                UserInput.LittleRollNo = string.Format("{0:D2}", setWorkProductData.LittleRollNo); 

        }
        private void UpdateUserInput()
        {

            UserInput.ProductCode = GetProductCode();
            UserInput.Width = GetProductWidth();
            UserInput.MType = GetManufactureType();

            if (UserInput.MType == ManufactureType.M_SINGLE)
            {
                UpdateUserInputInSingleMode();
            }
            else
            {
                UpdateUserInputInMutipleMode();
            }
            UserInput.WorkNo = tb_WorkNo.Text;
            UserInput.WorkHour = tb_ManHour.Text;
            UserInput.WorkerNo = tb_worker.Text;
            UserInput.CutMachineNo = tb_CutMachineNo.Text;
            UserInput.JointCount = tb_JointCount.Text;

            UserInput.ProductState = GetProductState(cb_ProductState);
            UserInput.ProductStateIndex = cb_ProductState.SelectedIndex;
            if (cb_ProductState.SelectedIndex != 0)
            {
                UserInput.ProductQuality = GetProductQuality(cb_ProductQuality);
                UserInput.ProductQualityIndex = cb_ProductQuality.SelectedIndex;
            }
            else
            {
                UserInput.ProductQuality = "";
                UserInput.ProductQualityIndex = 0;
            }
            UserInput.ShowRealWeight = GetShowRealWeight(cb_ShowRealWight);
            //Get the Selected Item from ProductState/ ProductQuality / RealWeight
            UserInput.BigRollNo = GetCurBigRollNo(tb_BigRollNo);

            UserInput.Weight = tb_LittleRollWeight.Text;
            UserInput.Desc = GetDesc(tb_Desc);
            UserInput.WorkTType = GetWorkTimeType();
        }

        private void UpdateProductData()
        {
            CutSampleData SysData;
            SystemSetting SysSetting;
            SysSetting = GlobalConfig.Setting;
            //CutProductItem ProdItem;
            SysData = CutSampleData.Instance;
            //CutProductItem Item = SysData.GetCurProductItem();

            //.SelectedItem
            //label26.Text = ((DataRowView)cb_ShowRealWight.SelectedItem).Row["id"].ToString();

            UpdateUserInput();
        }

        #endregion

        #region Get the data from the UI
        private String GetProductState(ComboBox productState_cb)
        {
            return productState_cb.SelectedItem.ToString();
        }

        private String GetProductQuality(ComboBox productQuality_cb)
        {
            return productQuality_cb.SelectedItem.ToString();
        }

        private String GetShowRealWeight(ComboBox showRealWight_cb)
        {
            return showRealWight_cb.SelectedItem.ToString();
        }

        private String GetDesc(TextBox tb)
        {
            return tb.Text;
        }

        private String GetCurBigRollNo(TextBox tb)
        {
            return tb.Text;
        }

        private String GetCurLittleRollNo(TextBox tb)
        {
            return tb.Text;
        }
        #endregion



        private void CreateLocalDataBaseItem()
        {

            UserInput.insertOneRowMSateZero();
        }

        private void PrintLabel()
        {
            UserInput.PrintLabel();
        }

		private void SendPackItemToServer(string productCode, string packBarcode, int rollNumber, string[] rollBarcode, float totalWeight, float totalLength)
        {
        	string str=null;
			byte[] send_buf;

			//<产品代码>;<卷数>;<重量>;<长度>;<打包条码>;<卷条码1>;...;<卷条码N>
			str += productCode + ";";
			str += rollNumber + ";";
			str += totalWeight + ";";
			str += totalLength + ";";
			str += packBarcode + ";";
			foreach (string barcode in rollBarcode)
				str += barcode + ";";

			send_buf = System.Text.Encoding.Default.GetBytes(str);
			m_FilmSocket.sendDataPacketToServer(send_buf, COMMUNICATION_TYPE_SLIT_PROCESS_PACKAGE_BARCODE_UPLOAD, send_buf.Length);
			int rsp = m_FilmSocket.RecvResponse(1000);
			if (rsp == 0)	System.Windows.Forms.MessageBox.Show("发送成功！");
		}

        private void SendItemToServer(int communicationType)
        {
        	string str;
			byte[] send_buf;

        	if (communicationType == COMMUNICATION_TYPE_SLIT_PROCESS_MATERIAL_BARCODE_UPLOAD) {
				//<大卷/印刷条码>
				str = UserInput.InputBarcode;
        	}
			if (communicationType == COMMUNICATION_TYPE_SLIT_PROCESS_PRODUCT_BARCODE_UPLOAD) {
				//<小卷条码>;<卷重>;<接头数量>
				str = UserInput.OutputBarcode + ";" + UserInput.Roll_Weight + ";" + UserInput.JointCount;
			}

			send_buf = System.Text.Encoding.Default.GetBytes(str);
			m_FilmSocket.sendDataPacketToServer(send_buf, communicationType, send_buf.Length);
			int rsp = m_FilmSocket.RecvResponse(1000);
			if (rsp == 0)	System.Windows.Forms.MessageBox.Show("发送成功！");
        }

        private void UpdatePlateNo(int plateNo)
        {
            tb_PlateNo1.Text = plateNo.ToString();
        }

        delegate void AsynBarCode_BarCodeEvent(BardCodeHooK.BarCodes barCode);
        void BarCode_BarCodeEvent(BardCodeHooK.BarCodes barCode)
        {
            if (this.InvokeRequired)
            {
                Log.d("HH", "bar code Final Barcode=" + barCode.BarCode);
                this.BeginInvoke(new AsynBarCode_BarCodeEvent(BarCode_BarCodeEvent), new object[] { barCode });
            }
            else
            {
                Log.d("HH", "final keyboard =" + barCode.BarCode);
                if (barCode.BarCode != null && barCode.BarCode.Length > 5 && barCode.BarCode.Remove(1) == "`")
                    label51.Text = barCode.BarCode;
            }
        }



        //private void UpdateLittleRollNo(int bigno, int littleno)
        //{
        //    String bigStr = null;
        //    String littleStr = null;
        //    ///String FullLittle = null;
        //    bigStr = string.Format("{0:D3}", bigno);
        //    littleStr = string.Format("{0:D2}", littleno);
        //    tb_LittleRollNo.Text = bigStr + '-' + littleStr;
        //}

        //private void UpdateBigRollNo(int bigno)
        //{
        //    tb_BigRollNo.Text = string.Format("{0:D3}", bigno);
        //}
        private void PostUpdateProductData()
        {
            int plateNo;
            int maxRoll = 1;

            if (rb_SingleWork.Checked != true)
            {

                int CurMax = UserInput.SetWorkProductInfo.GetTotalLittleRollInOneRound();
                cb_ProductCodes[MiscRollIndexInOneRound].BackColor = Color.White;
                tb_Widths[MiscRollIndexInOneRound].BackColor = Color.White;
                MiscCutProductData misc = UserInput.SetWorkProductInfo.FindInfoByComboIndex(MiscRollIndexInOneRound);

                misc.LittleRollNo++;


                MiscRollIndexInOneRound = (MiscRollIndexInOneRound + 1) % CurMax;
                cb_ProductCodes[MiscRollIndexInOneRound].BackColor = Color.Yellow;
                tb_Widths[MiscRollIndexInOneRound].BackColor = Color.Yellow;
               
                int idx = UserInput.SetWorkProductInfo.FindInfoIndexByComboIndex(MiscRollIndexInOneRound);
                misc = UserInput.SetWorkProductInfo.FindInfoByComboIndex(MiscRollIndexInOneRound);
                tb_LittleRollNo.Text =   string.Format("{0:D2}", misc.LittleRollNo);
                if (idx!=-1)
                {
                    SetCurProductInfoTextBox(idx);

                }
            }
            else
            {
                //如果是合格品，总的卷数加一
                if (cb_ProductState.SelectedIndex == 0)
                    TotalRoll++;

                maxRoll = UserInput.CurPlatInfo.getMaxLittleRoll();

                if (maxRoll == 0)
                    maxRoll = 1;
                if ((TotalRoll % maxRoll) == 0)
                {
                    plateNo = UserInput.CurPlatInfo.IncreasePlateNo();
                    UpdatePlateNo(plateNo);
                    TotalRoll = 0;
                }

                LittleRoleNo++;
                tb_LittleRollNo.Text = CommonFormHelper.UpdateLittleRollNo(BigRollNo, LittleRoleNo);

            }
            return;
        }

        private void bt_Printing_Click(object sender, EventArgs e)
        {
            printOneLabel();
        }

        private void printOneLabel()
        {
            //update the data from UI or Scale.
            UpdateProductData();

            if (!UserInput.CheckUserInput())
                return;

            UserInput.UpdateDateTime();
            //tb_DateTime.Text = UserInput.GetDateTime();
            //Printing
            PrintLabel();
            //save the data to local database
            CreateLocalDataBaseItem();
            //Save the data to server
            SendItemToServer();
            PostUpdateProductData();
        }

        private void cb_ProductState_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cb_ProductState.SelectedIndex)
            {
                case 0:
                    //cb_ProductQuality.Enabled = false;
                    cb_ProductQuality.Visible = false;
                    lb_ProductQulity.Visible = false;
                    break;
                case 1:
                    //cb_ProductQuality.Enabled = true;
                    cb_ProductQuality.Visible = true;
                    lb_ProductQulity.Visible = true;
                    break;
            }
        }

        private void cb_ProductQuality_SelectedIndexChanged(object sender, EventArgs e)
        {
            String QulityStr = ProductQualityStr[cb_ProductQuality.SelectedIndex];
            //  label26.Text = QulityStr;
        }

        private void rb_SetWork_CheckedChanged(object sender, EventArgs e)
        {
            if (rb_SetWork.Checked)
            {


                SetComponentsByManufactureType(ManufactureType.M_MULTIPLE);
            }
        }

        private void rb_SingleWork_CheckedChanged(object sender, EventArgs e)
        {

            UserInput.SetWorkProductInfo.Reset();

            InitMiscUI();
            if (rb_SingleWork.Checked)
            {


                SetComponentsByManufactureType(ManufactureType.M_SINGLE);
            }
        }

        #region Limit the input of Controls
        private void tb_PlateRollPerLay1_KeyPress(object sender, KeyPressEventArgs e)
        {
            ControlHelper.LimitToDigitOnly(e);
            if (e.Handled != true)
            {
            }
        }

        private void tb_LittleRollWeight_KeyPress(object sender, KeyPressEventArgs e)
        {
            ControlHelper.LimitToDigitAndDotOnly(e);
        }

        private void tb_Width1_KeyPress(object sender, KeyPressEventArgs e)
        {
            ControlHelper.LimitToDigitAndDotOnly(e);
        }
        //需求v1.3
        //工时永远为 0，接头数量正常情况讲都是0，特殊情况时须手工输入，这两个数值都是整数值，不会有小数
        //
        private void tb_JointCount_KeyPress(object sender, KeyPressEventArgs e)
        {
            ControlHelper.LimitToDigitOnly(e);
        }

        private void tb_ManHour_KeyPress(object sender, KeyPressEventArgs e)
        {
            ControlHelper.LimitToDigitOnly(e);
            //ControlHelper.LimitToDigitAndDotOnly(e);
        }
        #endregion
        /*
         *           
         *           
         *           
         *           // salesorder: "S171109", batchNum: "1801306", dispatchCode:"S17110906L302", print process:"P", machine ID:"2", time:"1801201431", large roll ID:"05"  
str = "S17110906L302P2180120143105";



// salesorder: "S171109", 
batchNum: "1801306",
 dispatchCode:"S17110906L302", 
slit process:"S",
 machine ID:"1",
time:"1801201431", 
large roll ID:"05",
small roll ID:"001", 
customer:"0", 
inspection:"0"  
str = "S17110906L302S118012014310500100";
这个分别是大卷和小卷条码，从这里拼出来」
—————————


XXXXXXXXXX(工单编码)+X（工序）+X（机台号）+XXXXXXXX（日期）+ XX（卷号）+ XXX（分卷号）+ X（客户序号）+ X（质量编码）；WP
         */
        private void button1_Click(object sender, EventArgs e)
        {

            String barcode;
            String orderNo;
            String batchNo;
            String DevNo;
            String WorkNoSn;
            String productName;
            String fixture;

            ScanForm f = new ScanForm();
            f.ShowDialog();
            if (f.DialogResult == DialogResult.OK)
            {
                barcode = f.GetBarCodeValue();
                //barcode = "S17110906L302S118012014310500100";
                // barcode = "S17110906L302S11801201431050";
                barcode = "1804306121L32012230030";
                if (!UserInput.ParseBarCode(barcode))
                    return;


                tb_WorkNo.Text = UserInput.WorkNo;
                if (!UserInput.ParseWorkNo(tb_WorkNo.Text, out batchNo, out DevNo, out WorkNoSn))
                    return;

                tb_BatchNo1.Text = batchNo;


                if (gVariable.orderNo != null)
                {
                    orderNo = gVariable.orderNo;
                    if (!UserInput.GetProductInfoBySaleOrder(orderNo, out UserInput.ProductCode, out productName, out UserInput.CustomerName))
                        return;
                    if (!UserInput.GetInfoByProductCode(UserInput.ProductCode, out UserInput.Width, out UserInput.RecipeCode, out fixture, out UserInput.CustomerName))
                        return;

                    tb_Width1.Text = UserInput.Width;
                    tb_RecipeCode1.Text = UserInput.RecipeCode;
                    tb_CustomerName1.Text = UserInput.CustomerName;

                    tb_BigRollNo.Text = UserInput.BigRollNo;


                    //tb_BatchNo1.Text = UserInput.BatchNo;
                    SetManufactureType(UserInput.MType);
                    cb_ProductCode1.Text = UserInput.ProductCode;

                    tb_ManHour.Text = "0";
                    tb_Desc.Text = "";

                    BigRollNo = Convert.ToInt32(tb_BigRollNo.Text);

                }
            }
        }

        private void tb_WorkNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (tb_WorkNo.Text.Length == 18)
            {
                // UserInput.ParseWorkNo(tb_WorkNo.Text);
            }
            //ControlHelper.LimitToDigitOnly(e);
        }



        private void cb_ProductCode1_SelectedIndexChanged_single(object sender, EventArgs e)
        {
            String ProductCode;
            String Width;
            String RecipeCode;
            String Fixture;
            String CustomerName;
            int comBoxIndex = 0;

            ProductCode = cb_ProductCodes[comBoxIndex].Text;
            if (ProductCode == null || ProductCode == "")
                return;
            String RawMaterialCode = null;
            String productLength = null;
            String productName = null;
            String productWeight = null;
            try
            {
                //  ProductCode = cb_ProductCode1.Text;
                UserInput.GetInfoByProductCodeExt(ProductCode, out Width, out RecipeCode, out Fixture, out CustomerName, out RawMaterialCode, out productLength, out productName, out productWeight);

            }
            catch (Exception ex)
            {
                Log.e("CutForm", ex.Message);
                return;
            }

            UserInput.RawMaterialCode = RawMaterialCode;
            UserInput.ProductLength = productLength;
            UserInput.ProductName = productName;
            UserInput.ProductWeight = productWeight;
            UserInput.ParsePlateInfo(Fixture);

            tb_RawMaterialCode1.Text = UserInput.RawMaterialCode;
            tb_PlateRollNum1.Text = UserInput.PlateRollNum;
            tb_PlateRollPerLay1.Text = UserInput.PlateRollPerLay;
            tb_PlateLayer1.Text = UserInput.PlateLayer;


            tb_Width1.Text = Width;
            tb_RecipeCode1.Text = RecipeCode;
            tb_CustomerName1.Text = CustomerName;
        }

        int searchComBoxIndex(object sender)

        {
            for (int i = 0; i < MaxMiscLittleRoll; i++)
            {
                if (cb_ProductCodes[i] == sender)
                    return i;
            }
            return -1;
        }

        void InitMiscUI()
        {
            for (int i = 0; i < MaxMiscLittleRoll; i++)
            {
                cb_ProductCodes[i].Text = null;
                tb_Widths[i].Text = null;
            }
            for (int i = 0; i < 3; i++)
            {
                tb_RawMaterialCodes[i].Text = null;
                tb_CustomerNames[i].Text = null;
                tb_BatchNos[i].Text = null;
                tb_PlateNos[i].Text = null;
                tb_Recipes[i].Text = null;
                tb_PlateLayers[i].Text = null;
                tb_PlateRollPerLays[i].Text = null;
                tb_PlateRollNums[i].Text = null;
            }
        }

        void SetCurProductInfoTextBox(int idx)
        {
            for (int i=0; i<ProductTypeCount; i++)
            {
                if (i == idx) {
                    tb_CustomerNames[idx].BackColor = Color.Yellow;
                    tb_BatchNos[idx].BackColor = Color.Yellow;
                    tb_Recipes[idx].BackColor = Color.Yellow;
                    tb_PlateNos[idx].BackColor = Color.Yellow;
                    tb_RawMaterialCodes[idx].BackColor = Color.Yellow;
                    tb_PlateRollPerLays[idx].BackColor = Color.Yellow;
                    tb_PlateLayers[idx].BackColor = Color.Yellow;
                    tb_PlateRollNums[idx].BackColor = Color.Yellow;
                }
                else
                {
                    tb_CustomerNames[i].BackColor = Color.White;
                    tb_BatchNos[i].BackColor = Color.White;
                    tb_Recipes[i].BackColor = Color.White;
                    tb_PlateNos[i].BackColor = Color.White;
                    tb_RawMaterialCodes[i].BackColor = Color.White;

                    tb_PlateLayers[i].BackColor = Color.White;
                    tb_PlateRollPerLays[i].BackColor = Color.White;
                    tb_PlateRollNums[i].BackColor = Color.White;
                }
            }

        }

        void SetFirstLittleRollFocused()
        {
            for (int i =0; i<MaxLittleRoll; i++)
            {
                if (i==0)
                {
                    cb_ProductCodes[i].BackColor = Color.Yellow;
                    tb_Widths[i].BackColor = Color.Yellow;
                }
                else { 
                    cb_ProductCodes[i].BackColor = Color.White;
                    tb_Widths[i].BackColor = Color.White;
                }
            }
        }
        private void cb_ProductCode1_SelectedIndexChanged_Multiple(object sender, EventArgs e)
        {

           // String fixture = null;
            String ProductCode;
            String Width;
            String RecipeCode;
            String Fixture;
            String CustomerName;
            MiscCutProductData setWorkProductData;

            String RawMaterialCode = null;
            String productLength = null;
            String productName = null;
            String productWeight = null;

            int comBoxIndex = 0;

            //找到选择的combobox的index.
            comBoxIndex = searchComBoxIndex(sender);
            if (comBoxIndex == -1)
                return;

            ProductCode = cb_ProductCodes[comBoxIndex].Text;

            if (ProductCode == null || ProductCode == "")
                return;

            if (!UserInput.SetWorkProductInfo.IsEmptyMiscCutProductDataAvailable() && (UserInput.SetWorkProductInfo.SearchMiscCutProductDataWithoutCreation(ProductCode) == null))
            {
                MiscCutProductData miscData = UserInput.SetWorkProductInfo.FindInfoByComboIndex(comBoxIndex);
                if (miscData.UsedCount > 1) {
                    cb_ProductCodes[comBoxIndex].Text = miscData.ProductCode;
                    return;
                }
            }

                //删除该combox box前面已经有的选择项
                UserInput.SetWorkProductInfo.RemoveOldInfoByComboIndex(comBoxIndex);



            //如果不是已有选择项，而且数目已经达到3个，则退出
            if (!UserInput.SetWorkProductInfo.IsMiscCutProductDataAvailable(ProductCode))
            {
                cb_ProductCodes[comBoxIndex].Text = null;
                tb_Widths[comBoxIndex].Text = null;
                return;
            }
            try
            {
                //  ProductCode = cb_ProductCode1.Text;
                //UserInput.GetInfoByProductCode(ProductCode, out Width, out RecipeCode, out Fixture, out CustomerName);
                UserInput.GetInfoByProductCodeExt(ProductCode, out Width, out RecipeCode, out Fixture, out CustomerName, out RawMaterialCode, out productLength, out productName, out productWeight);
            }
            catch (Exception ex)
            {
                Log.e("CutForm", ex.Message);
                return;
            }

            //UserInput.customerCode = customerCode;
            //UserInput.productLength = productLength;
            //UserInput.productName = productName;
            //UserInput.productWeight = productWeight;


            if (UserInput.SetWorkProductInfo.IsEmptyMiscCutProductDataAvailable() &&
                (UserInput.SetWorkProductInfo.SearchMiscCutProductDataWithoutCreation(ProductCode) == null))
            {
                setWorkProductData = UserInput.SetWorkProductInfo.GetMiscCutProductData(ProductCode);
                setWorkProductData.ProductCode = ProductCode;
                setWorkProductData.Width = Width;
                setWorkProductData.RecipeCode = RecipeCode;
                setWorkProductData.Fixture = Fixture;
                setWorkProductData.CustomerName = CustomerName;
                setWorkProductData.PlateNo = 0;
                setWorkProductData.CurRollNum = 0;

                setWorkProductData.RawMaterialCode = RawMaterialCode;
                setWorkProductData.productLength = productLength;
                setWorkProductData.productName = productName;
                setWorkProductData.productWeight = productWeight;

                if (Fixture!=null)
                    UserInput.ParsePlateInfo(Fixture, out setWorkProductData.PlateRollPerLay, out setWorkProductData.PlateLayer, out setWorkProductData.PlateRollNum);

            }

            //MiscCutProductData setWorkProductData = UserInput.SetWorkProductInfo.GetMiscCutProductData(cb_ProductCode1.Text);
            //if (setWorkProductData == null)
            //    return;

            setWorkProductData = UserInput.SetWorkProductInfo.SearchMiscCutProductDataWithoutCreation(ProductCode);
            setWorkProductData.AddProductCodeComBoxIndex(comBoxIndex);
            //int index = setWorkProductData.SetWorkProductIndex;



            tb_Widths[comBoxIndex].Text = Width;
            
            //InitMiscUI();
            //tb_CustomerNames[i].Text = null;
            //tb_BatchNos[i].Text = null;
            //tb_PlateNos[i].Text = null;
            //tb_Recipes[i]
            UserInput.SetWorkProductInfo.UpdateUI(tb_CustomerNames, tb_BatchNos, tb_Recipes, tb_PlateNos, tb_RawMaterialCodes, tb_PlateRollPerLays,tb_PlateLayers, tb_PlateRollNums);




            //int idx = UserInput.SetWorkProductInfo.FindInfoIndexByComboIndex(comBoxIndex);

            MiscRollIndexInOneRound = 0;
            tb_LittleRollNo.Text = string.Format("{0:D2}", setWorkProductData.LittleRollNo);
            SetCurProductInfoTextBox(0);
            SetFirstLittleRollFocused();


            int littleRollCount = UserInput.SetWorkProductInfo.GetTotalLittleRollInOneRound();
            //产品种类没达到三个，则继续enable一个新的ProductCode+Width
            if (UserInput.SetWorkProductInfo.IsEmptyMiscCutProductDataAvailable())
            {

                if (littleRollCount < MaxMiscLittleRoll) { 
                    cb_ProductCodes[littleRollCount].Enabled = true;
                    tb_Widths[littleRollCount].Enabled = true;
                }
            }
            else//产品种类达到了三个，已经enable的，必须disable掉
            {
                if (comBoxIndex< littleRollCount)
                {
                    cb_ProductCodes[littleRollCount].Enabled = false;
                    tb_Widths[littleRollCount].Enabled = false;
                }

            }


        }

        private void cb_ProductCode1_SelectedIndexChanged(object sender, EventArgs e)
        {


            if (GetManufactureType() == ManufactureType.M_MULTIPLE)
                cb_ProductCode1_SelectedIndexChanged_Multiple(sender, e);
            else
                cb_ProductCode1_SelectedIndexChanged_single(sender, e);


        }

        private void tb_PlateRollPerLay1_TextChanged(object sender, EventArgs e)
        {
            //if (sender == tb_PlateLayer1)
            //{
            //    UserInput.CurPlatInfo.Layer = int.tryParse(tb_PlateLayer1.Text);


            //}
            //else if (sender == tb_PlateRollPerLay1)
            //{
            //    if (tb_PlateRollPerLay1.Text!=null&& tb_PlateRollPerLay1.Text!="")
            //    UserInput.CurPlatInfo.LittleRollPerlayer = int.Parse(tb_PlateRollPerLay1.Text);
            //}
            //int total = UserInput.CurPlatInfo.getMaxLittleRoll();
            //UserInput.PlateLayer = UserInput.CurPlatInfo.Layer.ToString();
            //UserInput.PlateRollPerLay = UserInput.CurPlatInfo.LittleRollPerlayer.ToString();
            //UserInput.PlateRollNum = total.ToString();
            //tb_PlateRollNum1.Text = UserInput.PlateRollNum;
        }


        //打印磅码单
        private void button2_Click(object sender, EventArgs e)
        {
            excelClass excelClassImpl = new excelClass();
            //excelClassImpl.slitReportFunc();
            excelClassImpl.weightListFunc();
            Thread.Sleep(2000);
            System.Diagnostics.Process.Start("..\\..\\outputTables\\磅码单\\2018-04-05.xlsx");
        }

        //生成交接班记录
        private void button3_Click(object sender, EventArgs e)
        {
            String JiaoJieRecord;

            JiaoJieBanForm f = new JiaoJieBanForm();
            f.ShowDialog();
            if (f.DialogResult == DialogResult.OK)
            {
                UpdateUserInput();
                JiaoJieRecord = f.GetJiaoBanRecord();
                if (JiaoJieRecord != null && JiaoJieRecord != "")
                {
                    UserInput.JiaoJiRecord = f.GetJiaoBanRecord();
                    UserInput.InsertJIaoJieRecord();
                    //write jiao JIe Record to DB

					//<工单编码>;<记录>
					string str = m_dispatchCode + ";" + UserInput.JiaoJiRecord;
					byte[] send_buf = System.Text.Encoding.Default.GetBytes(str);
					
					m_FilmSocket.sendDataPacketToServer(send_buf, COMMUNICATION_TYPE_SLIT_PROCESS_END, send_buf.Length);
					
					int rsp = m_FilmSocket.RecvResponse(1000);
					if (rsp == 0)	System.Windows.Forms.MessageBox.Show("发送成功！");


                }
            }
        }

        private void tb_LittleRollWeight_TextChanged(object sender, EventArgs e)
        {

        }
		
        //start work
        //To Do: Add start work button in UI
        private void button_StartWork_Click(object sender, EventArgs e)
        {
        	byte[] send_buf = System.Text.Encoding.Default.GetBytes(tb_worker.Text);
			byte[] recv_buf;
			string[] start_work;
        
        	m_FilmSocket.sendDataPacketToServer(send_buf, COMMUNICATION_TYPE_SLIT_PROCESS_START, tb_worker.Text.Length);

			recv_buf = m_FilmSocket.RecvData(1000);
			if (recv_buf != null) {
				start_work = recv_buf.ToString().Split(';');
				//To Do after communication
				//<工单编号>;<产品编号>
				m_dispatchCode = start_work[0];
			}
        }
    }
}
