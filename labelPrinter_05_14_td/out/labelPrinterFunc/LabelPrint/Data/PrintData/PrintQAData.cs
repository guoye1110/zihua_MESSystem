using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LabelPrint.Data
{
    public partial class  QAUserinputData : ProcessData
    {
              /*
         * 大标签包含的栏位内容：客户名称、原材料代码、产品批号、材料名称、宽度、生产者、生产日期、卷重/卷长、卷号、检验结果；
         */
        override public void UpdatePrintPrintData(DynamicPrintLabelData DyData)
    {
        String str = null;
        //DyData.DataTime = WorkDate + WorkTime;

        //DyData.RawMaterialCode = RawMaterialCode;
        //DyData.MaterialName = MaterialName;
        //DyData.CustomName = CustomerName;
        //DyData.BatchNo = BatchNo;
        //DyData.WorkNo = WorkNo;
        //DyData.WorkerNo = WorkerNo;
        //DyData.DataTime = WorkDate + WorkTime;
        //DyData.Width = Width;
        //DyData.LittleRollNoStr = LittleRollCount;
        base.UpdatePrintPrintData(DyData);
        DyData.BigRollNoStr = BigRollNo;
        DyData.RollWeightLength = Weight;

        DyData.BigRollNoStr = BigRollNo;
        DyData.LittleRollNoStr = BigRollNo + "-" + LittleRollNo;
        DyData.RollWeightLength = Weight;
        DyData.WorkerNo = WorkerNo + "  " + WorkTime.Substring(0, 5);

        if (ProductState == "合格品"|| ProductState == null || ProductState == "")
        {
            DyData.Quality = "合格品";
        }
        else
        {
            DyData.Quality = "米数不足";
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
        DyData.RollWeightLength = str + " kg " + ProductLength;


        if (InputBarcode != null && InputBarcode.Length == LittleRollBarcode.getTotalStrLen())
        {
            LittleRollBarcode barcode = new LittleRollBarcode();
            //XXXXXXXXXX(工单编码) + X（工序）+X（机台号）+XXXXXXXX（日期）+XX（卷号）+XXX（分卷号）+X（客户序号）+X（质量编码）；
            //客户序号：单作为0，套作为序号 1 - 3
            //质量编码：共一位，0（未检验），1（合格），2以上为错误原因
            barcode.WorkNoStr = WorkNo;
            barcode.BatchNo = BatchNo;
            //barcode.ProcessStr = "J";
            // barcode.MachineIDStr = QAMachineNo;
            barcode.TimeStr = GetDateTimeForBarcode();
            barcode.BigRStr = GetRealBigRollNo(BigRollNo);
            barcode.LittleRStr = GetRealLittleRollNo(LittleRollNo);
            barcode.VendorStr = (MType == ManufactureType.M_SINGLE) ? "0" : "1";
            barcode.QAStr = "1";


            //DyData.QRBarCode =  "S17110906L302S118012014310500100";
            DyData.QRBarCode = barcode.createBarcode();
            DyData.QRBarCode = DyData.QRBarCode.Remove(10) + "Z1" + DyData.QRBarCode.Remove(0, 12);
            OutputBarcode = DyData.QRBarCode;
            System.Diagnostics.Debug.Assert(DyData.QRBarCode.Length == LittleRollBarcode.getTotalStrLen());
        }
        else if (InputBarcode != null && InputBarcode.Length == BigRollBarcode.getTotalStrLen())
        {
            BigRollBarcode barcode = new BigRollBarcode();
            //XXXXXXXXXX(工单编码) + X（工序）+X（机台号）+XXXXXXXX（日期）+XX（卷号）+XXX（分卷号）+X（客户序号）+X（质量编码）；
            //客户序号：单作为0，套作为序号 1 - 3
            //质量编码：共一位，0（未检验），1（合格），2以上为错误原因
            barcode.WorkNoStr = WorkNo;
            barcode.BatchNo = BatchNo;
            // barcode.ProcessStr = "J";
            //barcode.MachineIDStr = QAMachineNo;
            barcode.TimeStr = GetDateTimeForBarcode();
            barcode.BigRStr = GetRealBigRollNo(BigRollNo);
            //barcode.LittleRStr = GetRealLittleRollNo(LittleRollNo);
            //barcode.VendorStr = (MType == ManufactureType.M_SINGLE) ? "0" : "1";
            //barcode.QAStr = "1";


            //DyData.QRBarCode =  "S17110906L302S118012014310500100";
            DyData.QRBarCode = barcode.createBarcode(); ;
            DyData.QRBarCode = DyData.QRBarCode.Remove(10) + "Z1" + DyData.QRBarCode.Remove(0, 12);
            OutputBarcode = DyData.QRBarCode;
            //System.Diagnostics.Debug.Assert(DyData.QRBarCode.Length == BigRollBarcode.getTotalStrLen());

        }
        else
        {
            if (LittleRollNo == null || LittleRollNo == "")
            {
                BigRollBarcode barcode = new BigRollBarcode();
                //XXXXXXXXXX(工单编码) + X（工序）+X（机台号）+XXXXXXXX（日期）+XX（卷号）+XXX（分卷号）+X（客户序号）+X（质量编码）；
                //客户序号：单作为0，套作为序号 1 - 3
                //质量编码：共一位，0（未检验），1（合格），2以上为错误原因
                barcode.WorkNoStr = WorkNo;
                barcode.BatchNo = BatchNo;
                // barcode.ProcessStr = "J";
                //barcode.MachineIDStr = QAMachineNo;
                barcode.TimeStr = GetDateTimeForBarcode();
                barcode.BigRStr = GetRealBigRollNo(BigRollNo);
                //barcode.LittleRStr = GetRealLittleRollNo(LittleRollNo);
                //barcode.VendorStr = (MType == ManufactureType.M_SINGLE) ? "0" : "1";
                //barcode.QAStr = "1";


                //DyData.QRBarCode =  "S17110906L302S118012014310500100";
                DyData.QRBarCode = barcode.createBarcode(); ;
                OutputBarcode = DyData.QRBarCode;
                System.Diagnostics.Debug.Assert(DyData.QRBarCode.Length == BigRollBarcode.getTotalStrLen());

            }
            else
            {

                LittleRollBarcode barcode = new LittleRollBarcode();
                //XXXXXXXXXX(工单编码) + X（工序）+X（机台号）+XXXXXXXX（日期）+XX（卷号）+XXX（分卷号）+X（客户序号）+X（质量编码）；
                //客户序号：单作为0，套作为序号 1 - 3
                //质量编码：共一位，0（未检验），1（合格），2以上为错误原因
                barcode.WorkNoStr = WorkNo;
                barcode.BatchNo = BatchNo;
                //barcode.ProcessStr = "J";
                // barcode.MachineIDStr = QAMachineNo;
                barcode.TimeStr = GetDateTimeForBarcode();
                barcode.BigRStr = GetRealBigRollNo(BigRollNo);
                barcode.LittleRStr = GetRealLittleRollNo(LittleRollNo);
                barcode.VendorStr = (MType == ManufactureType.M_SINGLE) ? "0" : "1";
                barcode.QAStr = "1";


                //DyData.QRBarCode =  "S17110906L302S118012014310500100";
                DyData.QRBarCode = barcode.createBarcode(); ;
                OutputBarcode = DyData.QRBarCode;
                System.Diagnostics.Debug.Assert(DyData.QRBarCode.Length == LittleRollBarcode.getTotalStrLen());

            }

        }
    }
}
}
