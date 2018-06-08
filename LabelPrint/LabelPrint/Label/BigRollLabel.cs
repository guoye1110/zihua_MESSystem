using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabelPrint
{
    class BigRollLabel
    {
        
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

 
    }
}
