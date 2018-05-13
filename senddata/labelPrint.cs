using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using clientFunc;

namespace tcpClient
{
    public partial class Form1 : Form
    {
        const int PRINTING_MACHINE_ID_WAREHOUSE = 101;
        const int PRINTING_MACHINE_ID_CAST1 = 141;
        const int PRINTING_MACHINE_ID_CAST2 = 142;
        const int PRINTING_MACHINE_ID_CAST3 = 143;
        const int PRINTING_MACHINE_ID_CAST4 = 144;
        const int PRINTING_MACHINE_ID_CAST5 = 145;
        const int PRINTING_MACHINE_ID_PRINT1 = 161;
        const int PRINTING_MACHINE_ID_PRINT2 = 162;
        const int PRINTING_MACHINE_ID_PRINT3 = 163;
        const int PRINTING_MACHINE_ID_SLIT1 = 181;
        const int PRINTING_MACHINE_ID_SLIT2 = 182;
        const int PRINTING_MACHINE_ID_SLIT3 = 183;
        const int PRINTING_MACHINE_ID_SLIT4 = 184;
        const int PRINTING_MACHINE_ID_SLIT5 = 185;
        const int PRINTING_MACHINE_ID_INSPECTION = 201;
        const int PRINTING_MACHINE_ID_REUSE = 221;
        const int PRINTING_MACHINE_ID_PACKING = 241;

        private void startCommunicationForPrint()
        {
            int recCount;
            int len;

            if (connectedMachineArray[selectedComboBoxIndex] == 1)
            {
                MessageBox.Show("该设备已经在通讯中, 无法再次开启通讯功能", "信息提示", MessageBoxButtons.OK);
                return;
            }

            try
            {
                connectedMachineArray[selectedComboBoxIndex] = 1;
                len = MIN_PACKET_LEN_MINUS_ONE + 4;

                if (startCommnuicateAs[selectedMachineIndex] == communicateAsPrintingSW)
                {
                    inputDataHeader(handshake_packet, len, COMMUNICATION_TYPE_HANDSHAKE_PRINT_MACHINE_ID, DATA_TYPE_ADC_DEVICE);

                    switch (comboBox3.SelectedIndex)
                    {
                        case 0: 
                            handshake_packet[PROTOCOL_DATA_POS] = PRINTING_MACHINE_ID_WAREHOUSE;
                            handshake_packet[PROTOCOL_DATA_POS + 1] = PRINTING_MACHINE_ID_WAREHOUSE / 0x100;
                            break;
                        case 1:
                            handshake_packet[PROTOCOL_DATA_POS] = PRINTING_MACHINE_ID_CAST1;
                            handshake_packet[PROTOCOL_DATA_POS + 1] = PRINTING_MACHINE_ID_CAST1 / 0x100;
                            break;
                        case 2:
                            handshake_packet[PROTOCOL_DATA_POS] = PRINTING_MACHINE_ID_CAST2;
                            handshake_packet[PROTOCOL_DATA_POS + 1] = PRINTING_MACHINE_ID_CAST2 / 0x100;
                            break;
                        case 3:
                            handshake_packet[PROTOCOL_DATA_POS] = PRINTING_MACHINE_ID_CAST3;
                            handshake_packet[PROTOCOL_DATA_POS + 1] = PRINTING_MACHINE_ID_CAST3 / 0x100;
                            break;
                        case 4:
                            handshake_packet[PROTOCOL_DATA_POS] = PRINTING_MACHINE_ID_CAST4;
                            handshake_packet[PROTOCOL_DATA_POS + 1] = PRINTING_MACHINE_ID_CAST4 / 0x100;
                            break;
                        case 5:
                            handshake_packet[PROTOCOL_DATA_POS] = PRINTING_MACHINE_ID_CAST5;
                            handshake_packet[PROTOCOL_DATA_POS + 1] = PRINTING_MACHINE_ID_CAST5 / 0x100;
                            break;
                        case 6:
                            handshake_packet[PROTOCOL_DATA_POS] = PRINTING_MACHINE_ID_PRINT1;
                            handshake_packet[PROTOCOL_DATA_POS + 1] = PRINTING_MACHINE_ID_PRINT1 / 0x100;
                            break;
                        case 7:
                            handshake_packet[PROTOCOL_DATA_POS] = PRINTING_MACHINE_ID_PRINT2;
                            handshake_packet[PROTOCOL_DATA_POS + 1] = PRINTING_MACHINE_ID_PRINT2 / 0x100;
                            break;
                        case 8:
                            handshake_packet[PROTOCOL_DATA_POS] = PRINTING_MACHINE_ID_PRINT3;
                            handshake_packet[PROTOCOL_DATA_POS + 1] = PRINTING_MACHINE_ID_PRINT3 / 0x100;
                            break;
                        case 9:
                            handshake_packet[PROTOCOL_DATA_POS] = PRINTING_MACHINE_ID_SLIT1;
                            handshake_packet[PROTOCOL_DATA_POS + 1] = PRINTING_MACHINE_ID_SLIT1 / 0x100;
                            break;
                        case 10:
                            handshake_packet[PROTOCOL_DATA_POS] = PRINTING_MACHINE_ID_SLIT2;
                            handshake_packet[PROTOCOL_DATA_POS + 1] = PRINTING_MACHINE_ID_SLIT2 / 0x100;
                            break;
                        case 11:
                            handshake_packet[PROTOCOL_DATA_POS] = PRINTING_MACHINE_ID_SLIT3;
                            handshake_packet[PROTOCOL_DATA_POS + 1] = PRINTING_MACHINE_ID_SLIT3 / 0x100;
                            break;
                        case 12:                            handshake_packet[PROTOCOL_DATA_POS] = PRINTING_MACHINE_ID_SLIT4;
                            handshake_packet[PROTOCOL_DATA_POS + 1] = PRINTING_MACHINE_ID_SLIT4 / 0x100;
                            break;
                        case 13:
                            handshake_packet[PROTOCOL_DATA_POS] = PRINTING_MACHINE_ID_SLIT5;
                            handshake_packet[PROTOCOL_DATA_POS + 1] = PRINTING_MACHINE_ID_SLIT5 / 0x100;
                            break;
                        case 14:
                            handshake_packet[PROTOCOL_DATA_POS] = PRINTING_MACHINE_ID_INSPECTION;
                            handshake_packet[PROTOCOL_DATA_POS + 1] = PRINTING_MACHINE_ID_INSPECTION / 0x100;
                            break;
                        case 15:
                            handshake_packet[PROTOCOL_DATA_POS] = PRINTING_MACHINE_ID_REUSE;
                            handshake_packet[PROTOCOL_DATA_POS + 1] = PRINTING_MACHINE_ID_REUSE / 0x100;
                            break;
                        case 16:
                            handshake_packet[PROTOCOL_DATA_POS] = PRINTING_MACHINE_ID_PACKING;
                            handshake_packet[PROTOCOL_DATA_POS + 1] = PRINTING_MACHINE_ID_PACKING / 0x100;
                            break;
                        default:
                            handshake_packet[PROTOCOL_DATA_POS] = PRINTING_MACHINE_ID_WAREHOUSE;
                            handshake_packet[PROTOCOL_DATA_POS + 1] = PRINTING_MACHINE_ID_WAREHOUSE / 0x100;
                            break;

                    }
                    handshake_packet[PROTOCOL_DATA_POS + 2] = 0;
                    handshake_packet[PROTOCOL_DATA_POS + 3] = 0;

                    point = new IPEndPoint(HostIP, 8899);
                    socketArray[selectedMachineIndex] = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    socketArray[selectedMachineIndex].Connect(point);

                    if (socketArray[selectedMachineIndex].Connected)
                    {
                        CRC16.addCrcCode(handshake_packet, len);
                        recCount = socketArray[selectedMachineIndex].Send(handshake_packet, len, 0);

                        recCount = socketArray[selectedMachineIndex].Receive(receiveByte, REV_LEN, 0);

                        communicationStatusArray[selectedMachineIndex] = handshakeOK;

                        currentConnectedNum++;
                        connectedMachineArray[selectedComboBoxIndex] = 1;
                    }
                }
                else
                {
                    inputDataHeader(handshake_packet, len, COMMUNICATION_TYPE_START_HANDSHAKE_WITH_ID_TO_PC, DATA_TYPE_ADC_DEVICE);

                    handshake_packet[PROTOCOL_DATA_POS] = (byte)selectedMachineID;
                    handshake_packet[PROTOCOL_DATA_POS + 1] = 0;
                    handshake_packet[PROTOCOL_DATA_POS + 2] = 0;

                    if (startCommnuicateAs[selectedMachineIndex] == communicateAsTouchpad)
                        handshake_packet[PROTOCOL_DATA_POS + 3] = 0; //so the final value is ID
                    else
                        handshake_packet[PROTOCOL_DATA_POS + 3] = 1; //so the final value is 0x1000000 + ID

                    point = new IPEndPoint(HostIP, 8899);
                    socketArray[selectedMachineIndex] = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    socketArray[selectedMachineIndex].Connect(point);

                    if (socketArray[selectedMachineIndex].Connected)
                    {
                        CRC16.addCrcCode(handshake_packet, len);
                        recCount = socketArray[selectedMachineIndex].Send(handshake_packet, len, 0);

                        len = MIN_PACKET_LEN_MINUS_ONE + 12;  //12 is the length of a time value
                        recCount = socketArray[selectedMachineIndex].Receive(receiveByte, REV_LEN, 0);

                        len = MIN_PACKET_LEN_MINUS_ONE + 11;  //length of the dummy data

                        inputDataHeader(dummyMachine_packet, len, COMMUNICATION_TYPE_SEND_DUMMY_MACHINE_CODE_TO_PC, DATA_TYPE_MES_INSTRUCTION);
                        CRC16.addCrcCode(dummyMachine_packet, len);
                        recCount = socketArray[selectedMachineIndex].Send(dummyMachine_packet, len, 0);

                        Thread.Sleep(WAIT_BETWEEN_SEND_RECEIVE);

                        recCount = socketArray[selectedMachineIndex].Receive(receiveByte, REV_LEN, 0);
                        communicationStatusArray[selectedMachineIndex] = handshakeOK;

                        currentConnectedNum++;
                        connectedMachineArray[selectedComboBoxIndex] = 1;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                MessageBox.Show("通讯失败，可能服务器端未启动或者 ..\\..\\init\\init.txt 中输入的 IP 不是服务器的 IP 地址。", "信息提示", MessageBoxButtons.OK);
                return;
            }

        }


        //start working for printing SW  
        //申请物料（0xB5）
        private void button41_Click(object sender, EventArgs e)
        {
        	string title = "申请物料（0xB5）";
            string prompt = "数据：<员工号>\n\n";
			prompt += "返回：7台设备，每台7个料仓，每个料仓包括：<物料代码>;<物料数量>";

			do_processStart(COMMUNICATION_TYPE_WAREHOUE_OUT_START, prompt, title);
        }


        //label scan: material sent to feed machine from warehouse
        //出库扫描（0xB6）
        private void button42_Click(object sender, EventArgs e)
        {
        	string title = "出库扫描（0xB6）";
            string prompt = "数据：<原料代码>;<原料批次号>;<目标设备号>;<料仓号>;<重量>\n\n";

			do_productBarcodeUpload(COMMUNICATION_TYPE_WAREHOUSE_OUT_BARCODE, prompt, title);
        }


        //label scan: remanent material return to warehouse
        //入库扫描（0xB7）
        private void button43_Click(object sender, EventArgs e)
        {
        	string title = "入库扫描（0xB7）";
            string prompt = "数据：<原料代码>;<原料批次号>;<目标设备号>;<料仓号>;<重量>\n\n";

			do_productBarcodeUpload(COMMUNICATION_TYPE_WAREHOUSE_IN_BARCODE, prompt, title);
        }

        //cast process start, send cast machine ID to server
        //流延开工（0xB8）
        private void button44_Click(object sender, EventArgs e)
        {
        	string title = "流延开工（0xB8）";
            string prompt = "数据：<员工号>\n\n";
			prompt += "返回：<工单号：12>;<产品代码>";

			do_processStart(COMMUNICATION_TYPE_CAST_PROCESS_START, prompt, title);

            //put received dispatch info into dispatch structure
            //getDispatchInfo(dispatchList, strArray);
            
        }

        //label scan(cast): scan cast label then upload to server
        //流延产品扫描（0xB9）
        private void button45_Click(object sender, EventArgs e)
        {
        	string title = "流延膜标签上传（0xB9）";
            string prompt = "数据：<条码>;<卷重>\n\n";
            prompt += "       条码: <工单号：12><时分：4><大卷号：3><质量：1>";
			prompt += "返回：<0/0xff>";

			do_productBarcodeUpload(COMMUNICATION_TYPE_CAST_PROCESS_PRODUCT_BARCODE_UPLOAD, prompt, title);
        }

        //label scan(print): scan cast label for printing
        //印刷原料标签（0xBC）
        private void button46_Click(object sender, EventArgs e)
        {
			string title = "印刷原料标签（0xBC）";
			string prompt = "数据：<条码>\n\n";
			prompt += " 	  条码: <工单号：12><时分：4><大卷号：3><质量：1>";
			prompt += "返回：<0/0xff>";
		
			do_productBarcodeUpload(COMMUNICATION_TYPE_PRINT_PROCESS_MATERIAL_BARCODE_UPLOAD, prompt, title);

            //put received dispatch info into dispatch structure
            //getDispatchInfo(dispatchList, strArray);
        }

        //label scan(print): get output label for print then upload to server
        //印刷膜标签上传（0xBD）
        private void button47_Click(object sender, EventArgs e)
        {
        	string title = "印刷膜标签上传（0xBD）";
            string prompt = "数据：<条码>;<卷重>\n\n";
            prompt += "       条码: <工单号：12><时分：4><大卷号：3><质量：1>";
			prompt += "返回：<0/0xff>";

			do_productBarcodeUpload(COMMUNICATION_TYPE_PRINT_PROCESS_PRODUCT_BARCODE_UPLOAD,prompt, title);
        }

        //label scan(slit): get cast/print label for slit
        //分切原料标签上传（0xC0）
        private void button48_Click(object sender, EventArgs e)
        {
        	string title = "分切原料标签上传（0xC0）";
            string prompt = "数据：<条码>\n\n";
            prompt += "       条码: <工单号：12><时分：4><大卷号：3><质量：1>";
			prompt += "返回：<0/0xff>";

			do_productBarcodeUpload(COMMUNICATION_TYPE_SLIT_PROCESS_MATERIAL_BARCODE_UPLOAD,prompt, title);
            //put received dispatch info into dispatch structure
            //getDispatchInfo(dispatchList, strArray);
        }

        //label scan(slit multi-product): get cast/print label for slit
        private void button16_Click_1(object sender, EventArgs e)
        {
            int i;
            int len;
            byte[] data = new byte[1000];
            byte[] buf;
            string str;
            string[] strArray;
            gVariable.dispatchSheetStruct dispatchList = new gVariable.dispatchSheetStruct();

            if (communicationStatusArray[selectedMachineIndex] < handshakeOK)
            {
                MessageBox.Show("设备尚未握手成功，请先开始模拟通讯.", "信息提示", MessageBoxButtons.OK);
                return;
            }

            //based on PC ID for slit function
            data[0] = PRINTING_MACHINE_ID_SLIT2;
            data[1] = 0;
            data[2] = 0;
            data[3] = 0;

            // dispatchCode:"S171108O004", cast process:"3", cast machine ID:"2", time:"1801201431", large roll ID:"05"  
            str = "S171108O00432180120143106";
            buf = System.Text.Encoding.Default.GetBytes(str);

            for (i = 0; i < buf.Length; i++)
            {
                data[i + 4] = buf[i];
            }

            sendDataToServer(data, COMMUNICATION_TYPE_SLIT_PROCESS_START, buf.Length + 4);

            Thread.Sleep(WAIT_BETWEEN_SEND_RECEIVE);

            len = socketArray[selectedMachineIndex].Receive(receiveByte, REV_LEN, 0);
            str = System.Text.Encoding.GetEncoding("gb2312").GetString(receiveByte, PROTOCOL_DATA_POS, len - MIN_PACKET_LEN_MINUS_ONE);
            if (receiveByte[PROTOCOL_DATA_POS] == 0xff)
            {
                MessageBox.Show("服务器返回错误", "信息提示", MessageBoxButtons.OK);
                return;
            }
            strArray = str.Split(';');

            //put received dispatch info into dispatch structure
            getDispatchInfo(dispatchList, strArray);
        }

        //label scan(slit): get output label for slit then upload to server
        //分切小卷膜标签上传（0xC1）
        private void button49_Click(object sender, EventArgs e)
        {
        	string title = "分切小卷膜标签上传（0xC1）";
            string prompt = "数据：<条码>;<卷重>;<接头数量>\n\n";
            prompt += "       条码: <工单号：12><时分：4><大卷号：3><小卷号：2><客户序号：1><质量：1>";
			prompt += "返回：<0/0xff>";

			do_productBarcodeUpload(COMMUNICATION_TYPE_SLIT_PROCESS_PRODUCT_BARCODE_UPLOAD,prompt, title);
        }

        //label scan: get label for inspection
        private void button51_Click(object sender, EventArgs e)
        {
            int len;
            byte[] data = new byte[4];
            string str;
            string[] strArray;
            gVariable.dispatchSheetStruct dispatchList = new gVariable.dispatchSheetStruct();

            if (communicationStatusArray[selectedMachineIndex] < handshakeOK)
            {
                MessageBox.Show("设备尚未握手成功，请先开始模拟通讯.", "信息提示", MessageBoxButtons.OK);
                return;
            }

            // dispatchCode:"S171109Q004", slit process:"5", slit machine ID:"2", time:"1801201431", large roll ID:"05", small roll ID:"001", customer:"0", inspection:"0"  
            str = "S171109Q0045218012014310500100";
            sendStringToServer(str, COMMUNICATION_TYPE_INSPECTION_PROCESS_MATERIAL_BARCODE_UPLOAD);

            Thread.Sleep(WAIT_BETWEEN_SEND_RECEIVE);

            len = socketArray[selectedMachineIndex].Receive(receiveByte, REV_LEN, 0);
            str = System.Text.Encoding.GetEncoding("gb2312").GetString(receiveByte, PROTOCOL_DATA_POS, len - MIN_PACKET_LEN_MINUS_ONE);
            if (receiveByte[PROTOCOL_DATA_POS] == 0xff)
            {
                MessageBox.Show("服务器返回错误", "信息提示", MessageBoxButtons.OK);
                return;
            }
            strArray = str.Split(';');

            //put received dispatch info into dispatch structure
            getDispatchInfo(dispatchList, strArray);
        }

        //label scan: get output label for the result of inspection then upload to server
        //质检膜标签上传（0xC5）
        private void button50_Click(object sender, EventArgs e)
        {
        	string title = "质检膜标签上传（0xC4）";
            string prompt = "数据：<条码>;<检验员员工号>\n\n";
            prompt += "       条码: <被检产品工单号中工序变为J：12><质量：1>";
			prompt += "返回：<0/0xff>";

			do_productBarcodeUpload(COMMUNICATION_TYPE_INSPECTION_PROCESS_PRODUCT_BARCODE_UPLOAD,prompt, title);
        }

        //label scan: get label for reusing material
        //质检原料标签上传（0xC4）
        private void button53_Click(object sender, EventArgs e)
        {
			string title = "质检原料标签上传（0xC4）";
			string prompt = "数据：<条码>\n\n";
			prompt += " 	  小卷条码：<工单号：12><时分：4><大卷号：3><小卷号：2><客户序号：1><质量：1>";
			prompt += "       大卷条码：<工单号：12><时分：4><大卷号：3><质量：1>";
			prompt += "返回：<0/0xff>";
		
			do_productBarcodeUpload(COMMUNICATION_TYPE_INSPECTION_PROCESS_MATERIAL_BARCODE_UPLOAD,prompt, title);

            //put received dispatch info into dispatch structure
            //getDispatchInfo(dispatchList, strArray);
        }

        //label scan: get output label for reusing material then upload to server
        //再造料标签上传（0xC6）
        private void button52_Click(object sender, EventArgs e)
        {
        	string title = "再造料标签上传（0xC6）";
            string prompt = "数据：<条码>;<重量>;<不良条码个数>;<条码1>;...;<配方单>;<员工号>\n\n";
            prompt += "       条码: <不合格大卷/小卷产品工单号中工序变为Z>";
			prompt += "返回：<0/0xff>";

			do_productBarcodeUpload(COMMUNICATION_TYPE_REUSE_PROCESS_BARCODE_UPLOAD,prompt, title);
        }

        //label scan: get slit label for packing
        private void button55_Click(object sender, EventArgs e)
        {
            int len;
            byte[] data = new byte[4];
            string str;
            string[] strArray;
            gVariable.dispatchSheetStruct dispatchList = new gVariable.dispatchSheetStruct();

            if (communicationStatusArray[selectedMachineIndex] < handshakeOK)
            {
                MessageBox.Show("设备尚未握手成功，请先开始模拟通讯.", "信息提示", MessageBoxButtons.OK);
                return;
            }

            // dispatchCode:"S171109Q004", slit process:"5", slit machine ID:"2", time:"1801201431", large roll ID:"05", small roll ID:"001", customer:"0", inspection:"0"  
            str = "S171109Q0045218012014310500100";
            //sendStringToServer(str, COMMUNICATION_TYPE_PACKING_PROCESS_START);

            Thread.Sleep(WAIT_BETWEEN_SEND_RECEIVE);

            len = socketArray[selectedMachineIndex].Receive(receiveByte, REV_LEN, 0);
            str = System.Text.Encoding.GetEncoding("gb2312").GetString(receiveByte, PROTOCOL_DATA_POS, len - MIN_PACKET_LEN_MINUS_ONE);
            if (receiveByte[PROTOCOL_DATA_POS] == 0xff)
            {
                MessageBox.Show("服务器返回错误", "信息提示", MessageBoxButtons.OK);
                return;
            }
            strArray = str.Split(';');

            //put received dispatch info into dispatch structure
            getDispatchInfo(dispatchList, strArray);
        }

        //label scan: get output label for packing then upload to server
        //打包完成标签上传（0xC7）
        private void button54_Click(object sender, EventArgs e)
        {
        	string title = "打包完成标签上传（0xC7）";
            string prompt = "数据：<条码>;<员工号>\n\n";
            prompt += "       条码: <>";
			prompt += "返回：<0/0xff>";

			do_productBarcodeUpload(COMMUNICATION_TYPE_PACKING_PROCESS_PACKAGE_BARCODE_UPLOAD ,prompt, title);
        }

        //update basic info by heart beat
        private void button56_Click(object sender, EventArgs e)
        {
            int len;
            byte[] data = new byte[1];
            string str;
            //string[] strArray;

            if (communicationStatusArray[selectedMachineIndex] < handshakeOK)
            {
                MessageBox.Show("设备尚未握手成功，请先开始模拟通讯.", "信息提示", MessageBoxButtons.OK);
                return;
            }

            data[0] = 0;
            sendDataToServer(data, COMMUNICATION_TYPE_PRINTING_HEART_BEAT, 1);

            Thread.Sleep(WAIT_BETWEEN_SEND_RECEIVE);

            len = socketArray[selectedMachineIndex].Receive(receiveByte, REV_LEN, 0);
            str = System.Text.Encoding.GetEncoding("gb2312").GetString(receiveByte, PROTOCOL_DATA_POS, len - MIN_PACKET_LEN_MINUS_ONE);
            if (receiveByte[PROTOCOL_DATA_POS] == 0xff)
            {
                MessageBox.Show("服务器返回错误", "信息提示", MessageBoxButtons.OK);
                return;
            }
        }

		//印刷开工0xBB
        private void button58_Click(object sender, EventArgs e)
        {
        	string title = "印刷开工（0xBB） ";
            string prompt = "数据：<员工号>\n\n";
			prompt += "返回：<工单号：12>;<产品代码>";

			do_processStart(COMMUNICATION_TYPE_PRINT_PROCESS_START, prompt, title);
            //put received dispatch info into dispatch structure
           //getDispatchInfo(dispatchList, strArray);
        }

		//流延交接班0xBA
        private void button59_Click(object sender, EventArgs e)
        {
        	string title = "流延交接班（0xBA）";
			string prompt = "数据：<工单编号：12>;<交接班记录>\n\n";
			prompt += "返回：<0/0xff>";
			
        	do_processEnd(COMMUNICATION_TYPE_CAST_PROCESS_END , prompt, title);

            //put received dispatch info into dispatch structure
            //getDispatchInfo(dispatchList, strArray);

        }

        /*private void label22_Click(object sender, EventArgs e)
        {

        }*/

		//印刷交接班0xBE
        private void button60_Click(object sender, EventArgs e)
        {
        	string title = "印刷交接班（0xBE）";
			string prompt = "数据：<工单编号：12>;<交接班记录>\n\n";
			prompt += "返回：<0/0xff>";
			
        	do_processEnd(COMMUNICATION_TYPE_PRINT_PROCESS_END, prompt, title);
        }

		//分切开工（0xBF）
        private void button61_Click(object sender, EventArgs e)
        {
			 string title = "分切开工（0xBF） ";
			 string prompt = "数据：<员工号>\n\n";
			 prompt += "返回：<工单号：12>;<产品代码>";
			
			 do_processStart(COMMUNICATION_TYPE_SLIT_PROCESS_START,prompt, title);
			 //put received dispatch info into dispatch structure
			//getDispatchInfo(dispatchList, strArray);

        }

		//分切交接班0xC3
        private void button62_Click(object sender, EventArgs e)
        {
        	string title = "分切交接班（0xC3）";
			string prompt = "数据：<工单编号：12>;<交接班记录>\n\n";
			prompt += "返回：<0/0xff>";
			
        	do_processEnd(COMMUNICATION_TYPE_SLIT_PROCESS_END,prompt, title);
        }

		//创建打包条码（0xC2）
        private void button57_Click(object sender, EventArgs e)
        {
        	string title = "创建打包条码（0xC2）";
            string prompt = "数据：<产品代码>;<卷数>;<重量>;<长度>;<打包条码>;<卷条码1>;...;<卷条码N>\n\n";
            prompt += "       条码: <>";
			prompt += "返回：<0/0xff>";

			do_productBarcodeUpload(COMMUNICATION_TYPE_SLIT_PROCESS_PACKAGE_BARCODE_UPLOAD ,prompt, title);
        }

		void do_processStart(int communicationType, string prompt, string title)
        {
            //int i;
            int len;
            byte[] data = new byte[1000];
            string str;
            string[] strArray;
            gVariable.dispatchSheetStruct dispatchList = new gVariable.dispatchSheetStruct();

            if (communicationStatusArray[selectedMachineIndex] < handshakeOK)
            {
                MessageBox.Show("设备尚未握手成功，请先开始模拟通讯.", "信息提示", MessageBoxButtons.OK);
                return;
            }

            string response = Microsoft.VisualBasic.Interaction.InputBox(prompt, title, "", -1, -1);
			if (response == "")
            	return;

			Array.Copy(System.Text.Encoding.Default.GetBytes(response), 0, data, PROTOCOL_DATA_POS, response.Length);
            /*data[PROTOCOL_DATA_POS] = PRINTING_MACHINE_ID_CAST5;
            data[PROTOCOL_DATA_POS + 1] = 0;
            data[PROTOCOL_DATA_POS + 2] = 0;
            data[PROTOCOL_DATA_POS + 3] = 0;

            len = MIN_PACKET_LEN + 3;*/
            len = MIN_PACKET_LEN + response.Length - 1;
            inputDataHeader(data, len, communicationType, 0);
            CRC16.addCrcCode(data, len);
            socketArray[selectedMachineIndex].Send(data, len, 0);

			string show_value = "===>" + title + response;
			listBox1.Items.Add(show_value);

            Thread.Sleep(WAIT_BETWEEN_SEND_RECEIVE);

            len = socketArray[selectedMachineIndex].Receive(receiveByte, REV_LEN, 0);
            str = System.Text.Encoding.GetEncoding("gb2312").GetString(receiveByte, PROTOCOL_DATA_POS, len - MIN_PACKET_LEN_MINUS_ONE);
            if (receiveByte[PROTOCOL_DATA_POS] == 0xff)
            {
                MessageBox.Show("服务器返回错误", "信息提示", MessageBoxButtons.OK);
                return;
            }
            strArray = str.Split(';');
			show_value = "<=== ";
			if (strArray.Length <= 2)
				show_value += "没有工单";
			else
				show_value += "工单编号：" + strArray[0] + "   产品代码：" + strArray[1];

			listBox1.Items.Add(show_value);

		}

		void do_processEnd(int communicationType, string prompt, string title)
        {
            //int i;
            int len;
            byte[] data = new byte[1000];
            string str;
            string[] strArray;
            gVariable.dispatchSheetStruct dispatchList = new gVariable.dispatchSheetStruct();

            if (communicationStatusArray[selectedMachineIndex] < handshakeOK)
            {
                MessageBox.Show("设备尚未握手成功，请先开始模拟通讯.", "信息提示", MessageBoxButtons.OK);
                return;
            }

            string response = Microsoft.VisualBasic.Interaction.InputBox(prompt, title, "", -1, -1);
			if (response == "")
            	return;

			Array.Copy(System.Text.Encoding.Default.GetBytes(response), 0, data, PROTOCOL_DATA_POS, response.Length);
            /*data[PROTOCOL_DATA_POS] = PRINTING_MACHINE_ID_CAST5;
            data[PROTOCOL_DATA_POS + 1] = 0;
            data[PROTOCOL_DATA_POS + 2] = 0;
            data[PROTOCOL_DATA_POS + 3] = 0;

            len = MIN_PACKET_LEN + 3;*/
            len = MIN_PACKET_LEN + response.Length - 1;
            inputDataHeader(data, len, communicationType, 0);
            CRC16.addCrcCode(data, len);
            socketArray[selectedMachineIndex].Send(data, len, 0);

			string show_value = "===>" + title + response;
			listBox1.Items.Add(show_value);

            Thread.Sleep(WAIT_BETWEEN_SEND_RECEIVE);

            len = socketArray[selectedMachineIndex].Receive(receiveByte, REV_LEN, 0);
            str = System.Text.Encoding.GetEncoding("gb2312").GetString(receiveByte, PROTOCOL_DATA_POS, len - MIN_PACKET_LEN_MINUS_ONE);
            if (receiveByte[PROTOCOL_DATA_POS] == 0xff)
            {
                //MessageBox.Show("服务器返回错误", "信息提示", MessageBoxButtons.OK);
            	listBox1.Items.Add("<=== ！失败！");
            }
			if (receiveByte[PROTOCOL_DATA_POS] == 0)
				listBox1.Items.Add("<=== ！成功！");

			return;
		}

		public void do_productBarcodeUpload(int communicationType, string prompt, string title)
		{
            int len;
            string str;
            byte[] data = new byte[1000];
            //byte[] buf;

            if (communicationStatusArray[selectedMachineIndex] < handshakeOK)
            {
                MessageBox.Show("设备尚未握手成功，请先开始模拟通讯.", "信息提示", MessageBoxButtons.OK);
                return;
            }

            string response = Microsoft.VisualBasic.Interaction.InputBox(prompt, title,"", -1, -1);
			if (response == "")
            	return;
            // dispatchCode:"S171109J002", cast process:"3", cast machine ID:"5", time:"1801201431", large roll ID:"05"  
            //str = "S171109J00235180120143105";
            sendStringToServer(response, communicationType);

			string show_value = "===>" + title + response;
			listBox1.Items.Add(show_value);

            Thread.Sleep(WAIT_BETWEEN_SEND_RECEIVE);

            len = socketArray[selectedMachineIndex].Receive(receiveByte, REV_LEN, 0);

            if (receiveByte[PROTOCOL_DATA_POS] == 0xff)
            {
                //MessageBox.Show("服务器返回错误", "信息提示", MessageBoxButtons.OK);
            	listBox1.Items.Add("<=== ！失败！");
            }
			if (receiveByte[PROTOCOL_DATA_POS] == 0)
				listBox1.Items.Add("<=== ！成功！");
		}
		
        public void sendStringToServer(string str, int type)
        {
            int j;
            int len;
            byte[] buf;
            byte[] data = new byte[MAX_PACKET_LEN];

            buf = System.Text.Encoding.Default.GetBytes(str);
            for (j = 0; j < str.Length; j++)
            {
                data[j + PROTOCOL_DATA_POS] = (byte)buf[j];
            }

            len = MIN_PACKET_LEN_MINUS_ONE + buf.Length;
            inputDataHeader(data, len, type, 0);

            CRC16.addCrcCode(data, len);
            socketArray[selectedMachineIndex].Send(data, len, 0);
        }

        public void sendDataToServer(byte[] buf, int type, int len)
        {
            int j;
            byte[] data = new byte[MAX_PACKET_LEN];

            for (j = 0; j < len; j++)
            {
                data[j + PROTOCOL_DATA_POS] = (byte)buf[j];
            }

            len = MIN_PACKET_LEN_MINUS_ONE + buf.Length;
            inputDataHeader(data, len, type, 0);

            CRC16.addCrcCode(data, len);
            socketArray[selectedMachineIndex].Send(data, len, 0);
        }

        //put received dispatch info into dispatch structure
        void getDispatchInfo(gVariable.dispatchSheetStruct dispatchList, string[] strArray)
        {
            int i;

            i = 0;
            dispatchList.machineID = strArray[i++];
            dispatchList.dispatchCode = strArray[i++];
            dispatchList.planTime1 = strArray[i++];
            dispatchList.planTime2 = strArray[i++];
            dispatchList.productCode = strArray[i++];
            dispatchList.productName = strArray[i++];
            dispatchList.operatorName = strArray[i++];
            if (strArray[i] != null)
                dispatchList.plannedNumber = Convert.ToInt32(strArray[i++]);
            else
                dispatchList.plannedNumber = 0;

            if (strArray[i] != null)
                dispatchList.outputNumber = Convert.ToInt32(strArray[i++]);
            else
                dispatchList.outputNumber = 0;

            if (strArray[i] != null)
                dispatchList.qualifiedNumber = Convert.ToInt32(strArray[i++]);
            else
                dispatchList.qualifiedNumber = 0;

            if (strArray[i] != null)
                dispatchList.unqualifiedNumber = Convert.ToInt32(strArray[i++]);
            else
                dispatchList.unqualifiedNumber = 0;

            dispatchList.processName = strArray[i++];
            dispatchList.realStartTime = strArray[i++];
            dispatchList.realFinishTime = strArray[i++];
            dispatchList.prepareTimePoint = strArray[i++];
            if (strArray[i] != null)
                dispatchList.status = Convert.ToInt32(strArray[i++]);
            else
                dispatchList.status = 0;

            if (strArray[i] != null)
                dispatchList.toolLifeTimes = Convert.ToInt32(strArray[i++]);
            else
                dispatchList.toolLifeTimes = 0;

            if (strArray[i] != null)
                dispatchList.toolUsedTimes = Convert.ToInt32(strArray[i++]);
            else
                dispatchList.status = 0;

            if (strArray[i] != null)
                dispatchList.outputRatio = Convert.ToInt32(strArray[i++]);
            else
                dispatchList.outputRatio = 0;

            dispatchList.serialNumber = strArray[i++];
            dispatchList.reportor = strArray[i++];
            dispatchList.workshop = strArray[i++];
            dispatchList.workshift = strArray[i++];
            dispatchList.salesOrderCode = strArray[i++];
            dispatchList.BOMCode = strArray[i++];
            dispatchList.customer = strArray[i++];
            dispatchList.barCode = strArray[i++];  //
            dispatchList.barcodeForReuse = strArray[i++];  //再造料条码 
            dispatchList.quantityOfReused = strArray[i++];  //再造料用量
            dispatchList.multiProduct = strArray[i++];  //是否套作，0： 单作，1： 套作
            dispatchList.productCode2 = strArray[i++];  // 套作中的第二种产品，0 表示无 
            dispatchList.productCode3 = strArray[i++];  // 套作中的第三种产品，0 表示无
            dispatchList.operatorName2 = strArray[i++];  // 第二位操作工
            dispatchList.operatorName3 = strArray[i++];  // 第三位操作工
        }
    }
}
