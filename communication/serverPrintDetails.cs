using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using MESSystem.quality;
using MESSystem.common;

namespace MESSystem.communication
{
    public partial class communicate
    {
        /*
        //private const int COMMUNICATION_TYPE_HANDSHAKE_PRINT_MACHINE_ID = 3;
        private const int COMMUNICATION_TYPE_PRINTING_HEART_BEAT = 0xB3;
        //出入库工序
        private const int COMMUNICATION_TYPE_WAREHOUE_OUT_START = 0xB5;  //printing SW started and ask for material info, server send material info for all feeding machine to printing SW 
        private const int COMMUNICATION_TYPE_WAREHOUSE_OUT_BARCODE = 0xB6;  //printing machine send barcode info to server whever a stack of material is moved out of the warehouse
        private const int COMMUNICATION_TYPE_WAREHOUSE_IN_BARCODE = 0xB7;  //printing machine send barcode info to server whever a stack of material is moved into the warehouse
        //流延工序
        private const int COMMUNICATION_TYPE_CAST_PROCESS_START = 0xB8;  //printing SW started cast process, server need to send dispatch info to printing SW
        private const int COMMUNICATION_TYPE_CAST_PROCESS_PRODUCT_BARCODE_UPLOAD = 0xB9;  //printing SW send large roll info to server
        private const int COMMUNICATION_TYPE_CAST_PROCESS_END = 0xBA;
        //印刷工序
        private const int COMMUNICATION_TYPE_PRINT_PROCESS_START = 0xBB;
        private const int COMMUNICATION_TYPE_PRINT_PROCESS_MATERIAL_BARCODE_UPLOAD = 0xBC;
        private const int COMMUNICATION_TYPE_PRINT_PROCESS_PRODUCT_BARCODE_UPLOAD = 0xBD;
        private const int COMMUNICATION_TYPE_PRINT_PROCESS_END = 0xBE;
        //分切工序
        private const int COMMUNICATION_TYPE_SLIT_PROCESS_START = 0xBF;
        private const int COMMUNICATION_TYPE_SLIT_PROCESS_MATERIAL_BARCODE_UPLOAD = 0xC0;
        private const int COMMUNICATION_TYPE_SLIT_PROCESS_PRODUCT_BARCODE_UPLOAD = 0xC1;
        private const int COMMUNICATION_TYPE_SLIT_PROCESS_PACKAGE_BARCODE_UPLOAD = 0xC2;
        private const int COMMUNICATION_TYPE_SLIT_PROCESS_END = 0xC3;
        //质检工序
        private const int COMMUNICATION_TYPE_INSPECTION_PROCESS_MATERIAL_BARCODE_UPLOAD = 0xC4;
        private const int COMMUNICATION_TYPE_INSPECTION_PROCESS_PRODUCT_BARCODE_UPLOAD = 0xC5;
        //再造料工序
        private const int COMMUNICATION_TYPE_REUSE_PROCESS_BARCODE_UPLOAD = 0xC6;
        //打包工序		
        private const int COMMUNICATION_TYPE_PACKING_PROCESS_PACKAGE_BARCODE_UPLOAD = 0xC7;
        //end of communication between PC host and label printing SW

       */

        int checkMaterialCorrectness(int myBoardIndex, string str)
        {
            int i;
            int machineID;
            int feedbinID;
            string materialCode;
            string materialBatchCode;
            string commandText;
            string[] stringArray;
            string[,] tableArray;

            try
            {
                myBoardIndex = 0;
                stringArray = str.Split(';');

                materialCode = stringArray[0];
                materialBatchCode = stringArray[1];
                machineID = Convert.ToInt32(stringArray[2]);
                feedbinID = Convert.ToInt32(stringArray[3]);

                //if this is a mixed material or rebuild material, make sure ingredient is the same as product spec
                if (materialCode == "0000" || materialCode == "1111")
                {
                    if(gVariable.dispatchSheet[myBoardIndex].BOMCode == materialBatchCode)
                        return 0;  //OK
                    else
                        return 2;  //ingrediance not the same
                }

                commandText = "select * from `" + gVariable.productTableName + "` where productCode = '" + gVariable.dispatchSheet[myBoardIndex].productCode + "'";
                tableArray = mySQLClass.databaseCommonReading(gVariable.basicInfoDatabaseName, commandText);
                if (tableArray == null)
                {
                    return 1;  //product code not found
                }

                //check for materiual type one by one
                for(i = 0; i < gVariable.maxMaterialTypeNum; i++)
                {
                    if (tableArray[0, i + 16] == materialCode)
                        return 0;
                }

                return 3;
            }
            catch (Exception ex)
            {
                Console.Write("checkMaterialCorrectness fail" + ex);
                return RESULT_ERR_EXCEPTION_OCCURRED;
            }
        }

        public static int saveUploadedRollInfoIntoDispatchRecord(int myBoardIndex, string strInput)
        {
            float weight;
            string strStatus;
            string barcode;
            string databaseName;
            string[] strArray;

            try
            {
                strArray = strInput.Split(';');
                barcode = strArray[0];
                weight = (float)(Convert.ToDouble(strArray[1]));

                databaseName = gVariable.DBHeadString + (myBoardIndex + 1).ToString().PadLeft(3, '0');

                if (gVariable.machineCurrentStatus[myBoardIndex] >= gVariable.MACHINE_STATUS_DISPATCH_STARTED)  //we can use quality/craft/andon data now
                {
                    //this is small roll bar code like:1804306121L320120030400
                    if (barcode.Length > gVariable.LARGE_BARCODE_LEN)
                    {
                        strStatus = barcode.Remove(0, 22);
                    }
                    else  //this is a large roll bar code like:1804306121L320120030
                    {
                        strStatus = barcode.Remove(0, 19);
                    }
                    gVariable.dispatchSheet[myBoardIndex].outputNumber += 205;

                    if(strStatus == "0")  //qualified product
                    {
                        gVariable.dispatchSheet[myBoardIndex].qualifiedNumber += weight;
                        gVariable.dispatchSheet[myBoardIndex].outputNumber += weight;
                        mySQLClass.writeProductBeatTable(databaseName, gVariable.productBeatTableName, gVariable.dispatchSheet[myBoardIndex], weight, strStatus);
                    }
                    else if (strStatus == "1" || strStatus == "2")  //unqualified or need more investigation
                    {
                        gVariable.dispatchSheet[myBoardIndex].unqualifiedNumber += weight;
                        gVariable.dispatchSheet[myBoardIndex].outputNumber += weight;
                        mySQLClass.writeProductBeatTable(databaseName, gVariable.productBeatTableName, gVariable.dispatchSheet[myBoardIndex], weight, strStatus);
                    }
                    else  //边角料和废料不作处理
                    {

                    }

                    //save result to dispatch table
                    mySQLClass.updateDispatchTable(databaseName, gVariable.dispatchListTableName, myBoardIndex, gVariable.dispatchSheet[myBoardIndex].status, null);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("saveUploadedRollInfoIntoDispatchRecord failed! " + ex);
            }
            return 0;
        }

        public void actionRightAfterGetTouchData(int communicationType, int myBoardIndex, string str)
        {
            switch (communicationType)
            {
            
                //material label uploaded                            
                case 0x80:
                    //saveMaterialData(str);
                    checkMaterialCorrectness(myBoardIndex, str);                                               
                    break;                                           
                                                                     
                //apply for material sheet                           
                case 0x81:                                           
                    break;                                           

                //update material sheet                              
                case 0x82:                                           
                    break;                                           
                                                                     
                //material dispatch complete                         
                case 0x83:                                           
                    break;                                           
                                                                     
                //material alarm                                     
                case 0x84:                                           
                    break;                                           
                                                                     
                //material alarm updated                             
                case 0x85:                                           
                    break;                                           

                //cast barcode update                                
                case 0x86:                                           
                    break;                                           
                                                                     
                                                                     
                //apply for cast dispatch                            
                case 0x87:                                           
                    break;                                           
                                                                     
                //cast dispatch started                              
                case 0x88:                                           
                    break;                                           

                //cast dispatch update                               
                case 0x89:                                           
                    break;                                           
                                                                     
                //cast dispatch completed                            
                case 0x8A:                                           
                    break;                                           
                                                                     
                //cast device alarm                                  
                case 0x8B:                                           
                    break;                                           
                                                                     
                //cast device alarm updated                          
                case 0x8C:                                           
                    break;                                           

                //apply for cast product transfer sheet              
                case 0x8D:                                           
                    break;
                                                                     
                //cast product transfer sheet upload                 
                case 0x8E:                                           
                    break;

                //cast barcode upload
                case 0x8F:
                    break;

                //apply for printing dispatch
                case 0x90:
                    break;

                //print dispatch start
                case 0x91:
                    break;

                //print dispatch update
                case 0x92:
                    break;

                //print dispatch completed
                case 0x93:
                    break;

                //print device alarm started
                case 0x94:
                    break;

                //print alarm updated
                case 0x95:
                    break;

                //apply print product transfer sheet
                case 0x96:
                    break;

                //print product transfer sheet upload
                case 0x97:
                    break;

                //slit barcode upload
                case 0x98:
                    break;

                //apply for slit dispatch
                case 0x99:
                    break;

                //slit dispatch started
                case 0x9A:
                    break;

                //slit dispatch update
                case 0x9B:
                    break;

                //slit dispatch completed
                case 0x9C:
                    break;

                //slit device alarm triggered
                case 0x9D:
                    break;

                //slit alarm status updated
                case 0x9E:
                    break;

                //apply slit product transfer sheet
                case 0x9F:
                    break;

                //slit product transfer sheet upload
                case 0xA0:
                    break;
            }
        }

        /*
        public void actionRightAfterGetPrintData(int communicationType, string str)
        {
            switch (communicationType)
            {
                //common process
                case COMMUNICATION_TYPE_PRINTING_HEART_BEAT:
                    break;
                //input/output process
                case COMMUNICATION_TYPE_WAREHOUE_OUT_START:

                    break;
                case COMMUNICATION_TYPE_WAREHOUSE_OUT_BARCODE:   //material out from warehouse, need to do some check

                    break;
                case COMMUNICATION_TYPE_WAREHOUSE_IN_BARCODE:

                    break;
                //cast process
                case COMMUNICATION_TYPE_CAST_PROCESS_START:

                    break;
                case COMMUNICATION_TYPE_CAST_PROCESS_PRODUCT_BARCODE_UPLOAD:
                    break;
                case COMMUNICATION_TYPE_CAST_PROCESS_END:
                    break;
                //print process
                case COMMUNICATION_TYPE_PRINT_PROCESS_START:
                    break;
                case COMMUNICATION_TYPE_PRINT_PROCESS_MATERIAL_BARCODE_UPLOAD:
                    break;
                case COMMUNICATION_TYPE_PRINT_PROCESS_PRODUCT_BARCODE_UPLOAD:
                    break;
                case COMMUNICATION_TYPE_PRINT_PROCESS_END:
                    break;
                //slit process
                case COMMUNICATION_TYPE_SLIT_PROCESS_START:
                    break;
                case COMMUNICATION_TYPE_SLIT_PROCESS_MATERIAL_BARCODE_UPLOAD:
                    break;
                case COMMUNICATION_TYPE_SLIT_PROCESS_PRODUCT_BARCODE_UPLOAD:
                    break;
                case COMMUNICATION_TYPE_SLIT_PROCESS_PACKAGE_BARCODE_UPLOAD:
                    break;
                case COMMUNICATION_TYPE_SLIT_PROCESS_END:
                    break;
                //inspection process
                case COMMUNICATION_TYPE_INSPECTION_PROCESS_MATERIAL_BARCODE_UPLOAD:
                    break;
                case COMMUNICATION_TYPE_INSPECTION_PROCESS_PRODUCT_BARCODE_UPLOAD:
                    break;
                //rebuild process
                case COMMUNICATION_TYPE_REUSE_PROCESS_BARCODE_UPLOAD:
                    break;
                //packing process
                case COMMUNICATION_TYPE_PACKING_PROCESS_PACKAGE_BARCODE_UPLOAD:
                    break;
            }
        }


        public void actionAfterGetPrintData(int communicationType, string str)
        {
            switch (communicationType)
            {
                //common process
                 case COMMUNICATION_TYPE_PRINTING_HEART_BEAT:
                    break;
                //input/output process
                case COMMUNICATION_TYPE_WAREHOUE_OUT_START:

                    break;
                case COMMUNICATION_TYPE_WAREHOUSE_OUT_BARCODE:   //material out from warehouse, need to do some check

                    break;
                case COMMUNICATION_TYPE_WAREHOUSE_IN_BARCODE:

                    break;
                //cast process
                case COMMUNICATION_TYPE_CAST_PROCESS_START:

                    break;
                case COMMUNICATION_TYPE_CAST_PROCESS_PRODUCT_BARCODE_UPLOAD:

                    break;
                case COMMUNICATION_TYPE_CAST_PROCESS_END:
                    break;
                //print process
                case COMMUNICATION_TYPE_PRINT_PROCESS_START:
                    break;
                case COMMUNICATION_TYPE_PRINT_PROCESS_MATERIAL_BARCODE_UPLOAD:
                    break;
                case COMMUNICATION_TYPE_PRINT_PROCESS_PRODUCT_BARCODE_UPLOAD:
                    break;
                case COMMUNICATION_TYPE_PRINT_PROCESS_END:
                    break;
                //slit process
                case COMMUNICATION_TYPE_SLIT_PROCESS_START:
                    break;
                case COMMUNICATION_TYPE_SLIT_PROCESS_MATERIAL_BARCODE_UPLOAD:
                    break;
                case COMMUNICATION_TYPE_SLIT_PROCESS_PRODUCT_BARCODE_UPLOAD:
                    break;
                case COMMUNICATION_TYPE_SLIT_PROCESS_PACKAGE_BARCODE_UPLOAD:
                    break;
                case COMMUNICATION_TYPE_SLIT_PROCESS_END:
                    break;
                //inspection process
                case COMMUNICATION_TYPE_INSPECTION_PROCESS_MATERIAL_BARCODE_UPLOAD:
                    break;
                case COMMUNICATION_TYPE_INSPECTION_PROCESS_PRODUCT_BARCODE_UPLOAD:
                    break;
                //rebuild process
                case COMMUNICATION_TYPE_REUSE_PROCESS_BARCODE_UPLOAD:
                    break;
                //packing process
                case COMMUNICATION_TYPE_PACKING_PROCESS_PACKAGE_BARCODE_UPLOAD:
                    break;
            }
        }
         */
    }
}