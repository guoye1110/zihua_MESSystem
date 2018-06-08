using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace LabelPrint
{
    public enum DB_ROW_INDEX : int
    {
        DB_ID = 0,
        DB_DATE,
        DB_TIME,
        DB_BATCH_NO,
        DB_ORDER_NO,
        DB_CUSTOMER_NAME, //5
        DB_PRODUCT_NO,
        DB_PRODUCT_NAME,
        DB_RAW_MATERIAL_NO,
        DB_BIG_ROLL_NUM,
        DB_LITTLE_ROLL_NUM, //10
        DB_LITTLE_ROLL_WEIGHT,
        DB_PRODUCT_QUALITY,
        DB_PRODUCT_STATE,
    };

    enum WorkOrder
    {
        OUTBOUND,
        CAST_FILM,
        PRINTING,
        CUT,
        RECYCLE,
        PRODUCT_CHECK,
    };

    public class PlateInfo
    {
        public int LittleRollPerlayer;
        public int Layer;
        public int PLateNo;

        public PlateInfo()
        {
            LittleRollPerlayer = 6;
            Layer = 9;
            PLateNo = 1;
        }
        public PlateInfo(int rollPerLayer, int platelayer)
        {
            LittleRollPerlayer = rollPerLayer;
            Layer = platelayer;
            PLateNo = 1;
        }

        public int getMaxLittleRoll()
        {
            return LittleRollPerlayer * Layer;
        }

       static  public int getMaxLittleRoll( int littleRollPerlayer, int paramlayer)
        {
            return littleRollPerlayer * paramlayer;
        }
        public int IncreasePlateNo()
        {
            PLateNo++;
            return PLateNo;
        }
    }


    class CutSampleData
    {

        private int _rowIndex = 0;
        private DataTable dt;
        public int RowIndex
        {
            get { return _rowIndex; }
            set { _rowIndex = value; }
        }

        //CutProductItem[]
        private static readonly CutSampleData instance = new CutSampleData();
        private CutSampleData() { }
        public static CutSampleData Instance
        {
            get
            {
                return instance;
            }
        }
        void AddDataTableOneColumn(DataTable dt, String title)
        {
            if (title != null)
                dt.Columns.Add(new DataColumn(title));
        }

        void AddDataTableColumns(DataTable dt, List<String> titles)
        {
            foreach (String s in titles)
                AddDataTableOneColumn(dt, s);
        }

        void AddDataTableOneRow(DataTable dt, CutProductItem productItem)
        {
            //dt.Rows.Add(new Object[] {
            //    productItem.
            //    });
        }


        DataTable mydt;
        public void SetCurDataTable(DataTable curdt)
        {
            dt = curdt;
           // String[] titles = { "序号", "生产日期", "时间", "生产批号", "订购单", "客户名称", "产品品号", "产品名称", "配方编号", "大卷编号", "小卷编号", "小卷重量", "产品质量", "产品状态" };
           // List<String> titlelist = new List<String>(titles);
            //AddDataTableColumns(dt, titlelist);
        }
        public DataTable GetCurDataTable()
        {
            return dt;
        }

        public DataTable CreateCutDataTable()
        {
            dt = new DataTable();
            String[] titles = { "序号", "生产日期", "时间", "生产批号", "订购单", "客户名称", "产品代号", "产品名称", "配方编号", "大卷编号", "小卷编号", "小卷重量", "产品质量", "产品状态" };
            List<String> titlelist = new List<String>(titles);
            AddDataTableColumns(dt, titlelist);


            dt.Rows.Add(new object[] { "1", "2017-12-18", "12:22", "1712522", "", "尤妮佳", "CPE014011", "底膜 21.5gsm", "DM56-17-1", "042", "042-10", "10.9", "C", "不合格品", });
            dt.Rows.Add(new object[] { "2", "2017-12-19", "12:22", "1712522", "", "尤妮佳", "CPE014011", "底膜 21.5gsm", "DM56-17-1", "042", "042-11", "10.9", "", "合格品", });
            dt.Rows.Add(new object[] { "3", "2017-12-20", "12:21", "1712522", "", "尤妮佳", "CPE014011", "底膜 21.5gsm", "DM56-17-1", "042", "042-12", "10.9", "", "合格品", });
            dt.Rows.Add(new object[] { "4", "2017-12-21", "12:21", "1712523", "", "尤妮佳", "CPE014011", "底膜 21.5gsm", "DM56-17-1", "042", "042-13", "10.9", "", "合格品", });
            dt.Rows.Add(new object[] { "5", "2017-12-22", "12:21", "1712524", "", "尤妮佳", "CPE014011", "底膜 21.5gsm", "DM56-17-1", "042", "042-06", "11.2", "", "合格品", });
            dt.Rows.Add(new object[] { "6", "2017-12-23", "12:21", "1712525", "", "尤妮佳", "CPE014011", "底膜 21.5gsm", "DM56-17-1", "042", "042-07", "10.9", "", "合格品", });


            for (int i = 0; i < 100; i++)
            {
                var item = new object[] { "" + i, "2017-12-18", "12:21", "1712522", "", "尤妮佳", "CPE014011", "底膜 21.5gsm", "DM56-17-1", "042", "042-07", "10.9", "", "合格品", };
                dt.Rows.Add(item);
            }
            mydt = dt;
            return dt;
        }


        private DataTable GetNewDataTable(DataTable dt, string condition, string sortstr)
        {
            DataTable newdt = new DataTable();
            newdt = dt.Clone();
            DataRow[] dr = dt.Select(condition, sortstr);
            for (int i = 0; i < dr.Length; i++)
            {
                newdt.ImportRow((DataRow)dr[i]);
            }
            return newdt;//返回的查询结果
        }

        /*
        Select();

            Select("id>='3' and name='3--hello'");//支持and

            Select("id>='3' or id='1'");//支持or

            Select("name like '%hello%'");//支持like   

            Select("id>5","id desc");

            Select("id>5", "id desc", DataViewRowState.Added)

DataRow[] dr = dt.Select(“col = 'XXXX'”);//条件：就是字段名='某某'
*/

        /// 执行DataTable中的查询返回新的DataTable

        /// </summary>
        /// <param name="dt">源数据DataTable</param>
        /// <param name="condition">查询条件</param>
        /// <returns></returns>
        public DataTable search(String str)
        {
            //DataRow[] a =mydt.Select("序号='3'");
            if (str == null)
                return mydt;
            else
                return GetNewDataTable(mydt, str, null);
            //return a;
            //return null;
        }
        //
        // convert dt info according to current row index
        //
        public CutProductItem GetCurProductItem(DataRowCollection dtRow, int index)
        {
            CutProductItem item = new CutProductItem();

            item.Id = dtRow[index][(int)DB_ROW_INDEX.DB_ID].ToString();
            item.ProductCode[0] = dtRow[index][(int)DB_ROW_INDEX.DB_PRODUCT_NO].ToString();
            item.CustomerName[0] = dtRow[index][(int)DB_ROW_INDEX.DB_CUSTOMER_NAME].ToString();
            item.RawMaterialCode[0] = dtRow[index][(int)DB_ROW_INDEX.DB_RAW_MATERIAL_NO].ToString();
            item.BatchNo[0] = dtRow[index][(int)DB_ROW_INDEX.DB_BATCH_NO].ToString();

            item.BigRollNo = dtRow[index][(int)DB_ROW_INDEX.DB_BIG_ROLL_NUM].ToString();
            item.LittleRoleNo = dtRow[index][(int)DB_ROW_INDEX.DB_LITTLE_ROLL_NUM].ToString();
            item.OrderNo = dtRow[index][(int)DB_ROW_INDEX.DB_ORDER_NO].ToString();
            item.ManHour = 0;
            item.Weight = dtRow[index][(int)DB_ROW_INDEX.DB_LITTLE_ROLL_WEIGHT].ToString();
            //item.PlateNo[0]=


            //item.MajorNum = dtRow[0].
            item.WorkerNumber = "Hello";
            return item;
            //return dt.Rows[index];
        }

        //
        // convert dt info according to current row inde
        //
        public CutProductItem GetCurProductItem()
        {

            return GetCurProductItem(dt.Rows, RowIndex);
        }


        //
        // convert dt info according to current row inde
        //
        public bool AddProductItemToDATATable(DataTable dt, CutProductItem item)
        {
            return false;
        }

        /*
         * 产品编码：共九位，包含产品类别信息，配方、品相、色系、克重等；数据来自于服务器通讯下发。
         * 工序：共一位，（1：出库；2：流延；3：印刷；4：分切；5：再造料；6：质量检验）
         * 机台号：共一位，设备编号。对出库的原料标签不需要改标签；其他工序标签的该栏位或者来自生产产品的设备编号
         *       （如流延机、印刷机、分切机或再造料机），或者来源于半产品的自带标签（如质量检验工序和再造料工序，
         *       来自于扫描待检验产品和废料回收料的已带标签）。
         * 日期：共十位，年、月、日、时、分各两位，用数字表示；
         * 工单号：共两位，表示生产该产品的工单编号，原料出库和再造料工序标签不需要该栏位；
         * 大卷号：共三位，表示大卷号；
         * 小卷号：共两位，表示小卷编号；
         * 在原料出库和再造料工序标签中，大卷号和小卷号合起来表示原料编号
         * 大标签
         *XXXXXXXXX(产品编码)+X（工序）+X（机台号）+XXXXXXXXXX（日期）+XX(工单号)+XXX（卷号）
         */
         //9+1+1+10+2+3
        public String CreateNormalLabelString(CutProductItem item)
        {
            String normalLabel;
            String productIndex;//产品编码
            String workProcess;//工序
            String machineNum;//机台号
            String dataTime;//日期
            String workorder;//工单号
            String rollNum; //卷号
            productIndex = "XXXXXXXXX";
            workProcess = "X";
            machineNum = "X";
            dataTime = "XXXXXXXXXX";
            workorder = "XX";
            rollNum = "XXX"; 
            normalLabel = productIndex + workProcess + machineNum + dataTime + workorder + rollNum;
            if (normalLabel.Length != 26)
                Console.WriteLine("The Normal Label Length is not 26");
            return normalLabel;
        }

    /*
     * 小标签
     * XXXXXXXXX(产品编码)+X（工序）+X（机台号）+XXXXXXXXXX（日期）+XX(工单号)+XXX（卷号）+XX（分卷号）；
     * 9+1+1+10+2+3+2
     */
        public String CreateLittleLabelString(CutProductItem item)
        {
            String littlelLabel;
            String productIndex;//产品编码
            String workProcess;//工序
            String machineNum;//机台号
            String dataTime;//日期
            String workorder;//工单号
            String rollNum; //卷号
            String littleRollNum; //卷号
            productIndex = "XXXXXXXXX";
            workProcess = "X";
            machineNum = "X";
            dataTime = "XXXXXXXXXX";
            workorder = "XX";
            rollNum = "XXX";
            littleRollNum = "XX";
            littlelLabel = productIndex + workProcess + machineNum + dataTime + workorder + rollNum + littleRollNum;
            if (littlelLabel.Length != 28)
                Console.WriteLine("The Normal Label Length is not 28");
            return littlelLabel;
        }

    }
}
