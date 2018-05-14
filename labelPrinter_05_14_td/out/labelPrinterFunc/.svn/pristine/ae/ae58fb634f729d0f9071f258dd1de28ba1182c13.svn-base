using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabelPrint.Label
{
    class SmallRollLabel
    {
        /*
  * 小标签
  * XXXXXXXXX(产品编码)+X（工序）+X（机台号）+XXXXXXXXXX（日期）+XX(工单号)+XXX（卷号）+XX（分卷号）；
  * 9+1+1+10+2+3+2
  */
        public String CreateLittleLabelString(CutProductItem item)
        {
            String littleLabel;
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
            littleLabel = productIndex + workProcess + machineNum + dataTime + workorder + rollNum + littleRollNum;
            if (littleLabel.Length != 28)
                Console.WriteLine("The Normal Label Length is not 28");
            return littleLabel;
        }

        public static String CreateLittleLabelString(String productIndex, String workProcess, String machineNo, String dataTime, String workOrder, String rollNo, String littleRollNo)
        {
            String littleLabel;
            littleLabel = productIndex + workProcess + machineNo + dataTime + workOrder + rollNo + littleRollNo;
            if (littleLabel.Length != 28)
                Console.WriteLine("The Normal Label Length is not 28");
            return littleLabel;
        }
    }
}
