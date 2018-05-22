using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LabelPrint.Data
{
    public partial class LiuYanUserinputData : ProcessData
    {
        /*
 * 包含的栏位内容：客户名称、产品代号、材料名称、基重、宽度、生产者、生产日期、卷重/卷长、卷号；
 */
        override public void UpdatePrintPrintData(DynamicPrintLabelData DyData)
        {
            String str = null;
            base.UpdatePrintPrintData(DyData);
            DyData.BigRollNoStr = BigRollNo;
              DyData.RollWeightLength = "234.7";
            //  DyData.QRBarCode = ;
              DyData.BigRollNoStr = "012"; // BigRollNo;
              DyData.LittleRollNoStr = "013"; // BigRollNo + "-" + LittleRollNo;
            DyData.RollWeightLength = Weight;
            DyData.MaterialName = MaterialName;
            DyData.WorkerNo = WorkerNo + "  " + WorkTime.Substring(0, 5);

            if (ProductState == "合格品"|| ProductState == null || ProductState == "")
            {
                DyData.Quality = "合格品";
            }
            else
            {
            	//print error details
            }

            DyData.RawMaterialCode = RawMaterialCode;
            // UserInput.customerCode = customerCode;
            // UserInput.productLength = productLength;
            //    UserInput.productName = productName;
            //  UserInput.productWeight = productWeight;
            DyData.RecipeCode = ProductName + " " + ProductWeight;
            str = Weight;
            if (Weight == null || Weight == "")
                str = "0";
            DyData.RollWeightLength = "234.3kg " + ProductLength;


            //  DyData.RollWeightLength = Weight;
            BigRollBarcode barcode = new BigRollBarcode();
            //XXXXXXXXXX(工单编码) + X（工序）+X（机台号）+XXXXXXXX（日期）+XXX（卷号）+XX（分卷号）+X（客户序号）+X（质量编码）；
            //客户序号：单作为0，套作为序号 1 - 3
            //质量编码：共一位，0（未检验），1（合格），2以上为错误原因
            barcode.WorkNoStr = WorkNo;
            barcode.BatchNo = BatchNo;
            //barcode.ProcessStr = "L";
            //barcode.MachineIDStr = LiuYanMachineNo;
            barcode.TimeStr = GetDateTimeForBarcode();
            barcode.BigRStr = GetRealBigRollNo(BigRollNo);
            // barcode.LittleRStr = LittleRollNo;
            // barcode.VendorStr = "0";
            // barcode.QAStr = "0";
            //String barcode = WorkNoStr + ProcessStr + MachineIDStr + TimeStr + BigRStr + LittleRStr + VendorStr + QAStr;
            //WorkDate



            //DyData.QRBarCode =  "S17110906L302S118012014310500100";
            DyData.QRBarCode = barcode.createBarcode(); ;
            OutputBarcode = DyData.QRBarCode;
            //System.Diagnostics.Debug.Assert(DyData.QRBarCode.Length == LittleRollBarcode.getTotalStrLen());
        }
    }
}
