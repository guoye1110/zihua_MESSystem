﻿using System;
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
        private void button41_Click(object sender, EventArgs e)
        {
            int len;
            byte[] data = new byte[1000];
            string str;
            string[] strArray;

            if (communicationStatusArray[selectedMachineIndex] < handshakeOK)
            {
                MessageBox.Show("设备尚未握手成功，请先开始模拟通讯.", "信息提示", MessageBoxButtons.OK);
                return;
            }

            len = MIN_PACKET_LEN_MINUS_ONE + 1;
            inputDataHeader(data, len, COMMUNICATION_TYPE_WAREHOUE_OUT_START, 0);
            data[PROTOCOL_DATA_POS] = 0;

            CRC16.addCrcCode(data, len);
            socketArray[selectedMachineIndex].Send(data, len, 0);

            Thread.Sleep(WAIT_BETWEEN_SEND_RECEIVE);

            len = socketArray[selectedMachineIndex].Receive(receiveByte, REV_LEN, 0);

            str = System.Text.Encoding.GetEncoding("gb2312").GetString(receiveByte, PROTOCOL_DATA_POS, len - MIN_PACKET_LEN_MINUS_ONE);

            strArray = str.Split(';');
        }


        //label scan: material sent to feed machine from warehouse
        private void button42_Click(object sender, EventArgs e)
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

            str = "as1245;5;1;20;";
            sendStringToServer(str, COMMUNICATION_TYPE_WAREHOUSE_OUT_BARCODE);

            Thread.Sleep(WAIT_BETWEEN_SEND_RECEIVE);

            len = socketArray[selectedMachineIndex].Receive(receiveByte, REV_LEN, 0);
            if (len != MIN_PACKET_LEN || receiveByte[PROTOCOL_DATA_POS] != 0)
            {
                MessageBox.Show("服务器返回错误", "信息提示", MessageBoxButtons.OK);
            }
        }


        //label scan: remanent material return to warehouse
        private void button43_Click(object sender, EventArgs e)
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
            //
            str = "as1245;5;1;20;";
            sendStringToServer(str, COMMUNICATION_TYPE_WAREHOUSE_IN_BARCODE);

            Thread.Sleep(WAIT_BETWEEN_SEND_RECEIVE);

            len = socketArray[selectedMachineIndex].Receive(receiveByte, REV_LEN, 0);

            if (len != MIN_PACKET_LEN || receiveByte[PROTOCOL_DATA_POS] != 0)
            {
                MessageBox.Show("服务器返回错误", "信息提示", MessageBoxButtons.OK);
            }
        }

        //cast process start, send cast machine ID to server
        private void button44_Click(object sender, EventArgs e)
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

            data[PROTOCOL_DATA_POS] = PRINTING_MACHINE_ID_CAST5;
            data[PROTOCOL_DATA_POS + 1] = 0;
            data[PROTOCOL_DATA_POS + 2] = 0;
            data[PROTOCOL_DATA_POS + 3] = 0;

            len = MIN_PACKET_LEN + 3;
            inputDataHeader(data, len, COMMUNICATION_TYPE_CAST_PROCESS_START, 0);
            CRC16.addCrcCode(data, len);
            socketArray[selectedMachineIndex].Send(data, len, 0);

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

        //label scan(cast): scan cast label then upload to server
        private void button45_Click(object sender, EventArgs e)
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

            // dispatchCode:"S171109J002", cast process:"3", cast machine ID:"5", time:"1801201431", large roll ID:"05"  
            str = "S171109J00235180120143105";
            sendStringToServer(str, COMMUNICATION_TYPE_CAST_PROCESS_PRODUCT_BARCODE_UPLOAD);

            Thread.Sleep(WAIT_BETWEEN_SEND_RECEIVE);

            len = socketArray[selectedMachineIndex].Receive(receiveByte, REV_LEN, 0);

            if (len != MIN_PACKET_LEN || receiveByte[PROTOCOL_DATA_POS] != 0)
            {
                MessageBox.Show("服务器返回错误", "信息提示", MessageBoxButtons.OK);
            }
        }

        //label scan(print): scan cast label for printing
        private void button46_Click(object sender, EventArgs e)
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

            data[0] = PRINTING_MACHINE_ID_PRINT2;
            data[1] = 0;
            data[2] = 0;
            data[3] = 0;

            // dispatchCode:"S171109J002", cast process:"3", cast machine ID:"5", time:"1801201431", large roll ID:"05"  
            str = "S171109J00235180120143105";
            buf = System.Text.Encoding.Default.GetBytes(str);

            for (i = 0; i < buf.Length; i++)
            {
                data[i + 4] = buf[i];
            }

            sendDataToServer(data, COMMUNICATION_TYPE_PRINT_PROCESS_START, buf.Length + 4);

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

        //label scan(print): get output label for print then upload to server
        private void button47_Click(object sender, EventArgs e)
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

            // dispatchCode:"S171109N003", print process:"4", print machine ID:"3", time:"1801201431", large roll ID:"05"  
            str = "S171109N00343180120143105";
            sendStringToServer(str, COMMUNICATION_TYPE_PRINT_PROCESS_PRODUCT_BARCODE_UPLOAD);

            Thread.Sleep(WAIT_BETWEEN_SEND_RECEIVE);

            len = socketArray[selectedMachineIndex].Receive(receiveByte, REV_LEN, 0);
            if (len != MIN_PACKET_LEN || receiveByte[PROTOCOL_DATA_POS] != 0)
            {
                MessageBox.Show("服务器返回错误", "信息提示", MessageBoxButtons.OK);
            }
        }

        //label scan(slit): get cast/print label for slit
        private void button48_Click(object sender, EventArgs e)
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
            str = "S171108O00432180120143105";
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
        private void button49_Click(object sender, EventArgs e)
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

            // dispatchCode:"S171109Q004", slit process:"5", slit machine ID:"5", time:"1801201431", large roll ID:"05", small roll ID:"001", customer:"0", inspection:"0"  
            str = "S171109Q0045518012014310500100";
            sendStringToServer(str, COMMUNICATION_TYPE_SLIT_PROCESS_PRODUCT_BARCODE_UPLOAD);

            Thread.Sleep(WAIT_BETWEEN_SEND_RECEIVE);

            len = socketArray[selectedMachineIndex].Receive(receiveByte, REV_LEN, 0);
            if (len != MIN_PACKET_LEN || receiveByte[PROTOCOL_DATA_POS] != 0)
            {
                MessageBox.Show("服务器返回错误", "信息提示", MessageBoxButtons.OK);
            }
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
        private void button50_Click(object sender, EventArgs e)
        {
            int len;
            string str;

            if (communicationStatusArray[selectedMachineIndex] < handshakeOK)
            {
                MessageBox.Show("设备尚未握手成功，请先开始模拟通讯.", "信息提示", MessageBoxButtons.OK);
                return;
            }

            // dispatchCode:"S171109Q004", inspection process:"5", slit machine ID:"5", time:"1801201431", large roll ID:"05", small roll ID:"001", customer:"0", inspection result:"3"  
            str = "S171109Q0055518012014310500103";
            sendStringToServer(str, COMMUNICATION_TYPE_INSPECTION_PROCESS_PRODUCT_BARCODE_UPLOAD);

            Thread.Sleep(WAIT_BETWEEN_SEND_RECEIVE);

            len = socketArray[selectedMachineIndex].Receive(receiveByte, REV_LEN, 0);
            if (receiveByte[PROTOCOL_DATA_POS] != 0)
            {
                MessageBox.Show("服务器返回错误", "信息提示", MessageBoxButtons.OK);
                return;
            }
        }

        //label scan: get label for reusing material
        private void button53_Click(object sender, EventArgs e)
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
            //sendStringToServer(str, COMMUNICATION_TYPE_REUSE_PROCESS_START);

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

        //label scan: get output label for reusing material then upload to server
        private void button52_Click(object sender, EventArgs e)
        {
            int len;
            string str;

            if (communicationStatusArray[selectedMachineIndex] < handshakeOK)
            {
                MessageBox.Show("设备尚未握手成功，请先开始模拟通讯.", "信息提示", MessageBoxButtons.OK);
                return;
            }

            //reuse process:"6"  means this is reuse material
            // dispatchCode:"S171109Q005", reuse process:"6", slit machine ID:"2", time:"1801201431", large roll ID:"05", small roll ID:"001", customer:"0", inspection result:"0"  
            str = "S171109Q0056218012014310500103";
            sendStringToServer(str, COMMUNICATION_TYPE_REUSE_PROCESS_BARCODE_UPLOAD);

            Thread.Sleep(WAIT_BETWEEN_SEND_RECEIVE);

            len = socketArray[selectedMachineIndex].Receive(receiveByte, REV_LEN, 0);
            if (receiveByte[PROTOCOL_DATA_POS] != 0)
            {
                MessageBox.Show("服务器返回错误", "信息提示", MessageBoxButtons.OK);
                return;
            }
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
        private void button54_Click(object sender, EventArgs e)
        {
            if (communicationStatusArray[selectedMachineIndex] < handshakeOK)
            {
                MessageBox.Show("设备尚未握手成功，请先开始模拟通讯.", "信息提示", MessageBoxButtons.OK);
                return;
            }

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