using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using System.Windows.Forms;
using LabelPrint.Util;
namespace LabelPrint.NetWork
{
    public class FilmSocket
    {
        IPEndPoint point;
        Boolean bConnected = false;
        public Socket sock;
        IPAddress HostIP;
        byte[] handshake_packet = new byte[100];

        byte[] dummyMachine_packet = new byte[100];

        public byte[]  data_packet = new byte[100];
        public byte[] send_packet = new byte[100];
        public const int SITE_TO_SERVER = 0;
        public const int SERVER_TO_SITE = 1;


        const int COMMUNICATION_TYPE_START_HANDSHAKE_WITHOUT_ID_TO_PC = 0;
        const int COMMUNICATION_TYPE_REDO_HANDSHAKE_TO_BOARD = 0x82;
        const int COMMUNICATION_TYPE_START_HANDSHAKE_WITH_ID_TO_PC = 3;

        const int DATA_TYPE_ADC_DEVICE = 0; //device type definition

        public const int MIN_PACKET_LEN = 12;
        public const int PACKET_DATASTATUS_POS = 7;

        public const int REV_LEN = 2000;
        public byte[] receiveByte = new byte[REV_LEN];

        public int RESPONSE_OK = 0;
        public int RESPONSE_FAIL = 0;
        public int SendPacketLen = 0;
        public void inputDataHeader(byte[] packet, int len,int direction,  int type, int dataStatus)
        {
            int i;

            i = 0;

            packet[i++] = (byte)'w';
            packet[i++] = (byte)'I';
            packet[i++] = (byte)'F';
            packet[i++] = (byte)'i';

            packet[i++] = (byte)len;
            packet[i++] = (byte)(len >>8);

            packet[i++] = (byte)type;
            packet[i++] = (byte)dataStatus;
        }


        const int MIN_PACKET_LEN_MINUS_ONE = 32;   //header(4) + len(2) + communicationtype(1) + time(12) + index(4) + reserved(4) + type(1) + CRC(4)


        const int WAIT_BETWEEN_SEND_RECEIVE = 200;


        public  void startCommunication()
        {
            int recCount;
            int len;
            HostIP =  new IPAddress(new byte[] { 10,200,1,109 });
            if (bConnected == true)
            {
                MessageBox.Show("该设备已经在通讯中, 无法再次开启通讯功能", "信息提示", MessageBoxButtons.OK);
                return;
            }

            try
            {
                bConnected = true;
                len = MIN_PACKET_LEN_MINUS_ONE + 4;

                inputDataHeader(handshake_packet, len, SITE_TO_SERVER, COMMUNICATION_TYPE_START_HANDSHAKE_WITH_ID_TO_PC, DATA_TYPE_ADC_DEVICE);


                //if (startCommnuicateAs[selectedMachineIndex] == communicateAsTouchpad)
                //    handshake_packet[PROTOCOL_DATA_POS + 3] = 0; //so the final value is ID
                //else
                //    handshake_packet[PROTOCOL_DATA_POS + 3] = 1; //so the final value is 0x1000000 + ID

                point = new IPEndPoint(HostIP, 8899);
                sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                sock.Connect(point);

                if (sock.Connected)
                {
             //       CRC16.addCrcCode(handshake_packet, len);
             //       recCount = socketArray[selectedMachineIndex].Send(handshake_packet, len, 0);

                    //len = MIN_PACKET_LEN_MINUS_ONE + 12;  //12 is the length of a time value
                    //recCount = socketArray[selectedMachineIndex].Receive(receiveByte, REV_LEN, 0);

                    //len = MIN_PACKET_LEN_MINUS_ONE + 11;  //length of the dummy data

                    //inputDataHeader(dummyMachine_packet, len, COMMUNICATION_TYPE_SEND_DUMMY_MACHINE_CODE_TO_PC, DATA_TYPE_MES_INSTRUCTION);
                    //CRC16.addCrcCode(dummyMachine_packet, len);
                    //recCount = socketArray[selectedMachineIndex].Send(dummyMachine_packet, len, 0);

                    //Thread.Sleep(WAIT_BETWEEN_SEND_RECEIVE);

                    //recCount = socketArray[selectedMachineIndex].Receive(receiveByte, REV_LEN, 0);
                    //communicationStatusArray[selectedMachineIndex] = handshakeOK;

                    //currentConnectedNum++;
                    //connectedMachineArray[selectedComboBoxIndex] = 1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                MessageBox.Show("通讯失败，可能服务器端未启动或者网络连接出错", "信息提示", MessageBoxButtons.OK);
                return;
            }

        }

        private string getCommunicationHostIP()
        {
        //    string filePath = "..\\..\\init\\init.txt";
        //    StreamReader streamReader;
            string IPString = null;

        //    try
        //    {
        //        streamReader = new StreamReader(filePath, System.Text.Encoding.Default);
        //        IPString = streamReader.ReadLine().Trim();
        //        streamReader.Close();

        //        return IPString;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.Write("无法开启 ..\\..\\init\\init.txt 文件, 因此无法得知服务器 ip 地址");
        //        Console.WriteLine(ex.ToString());
        //        return null;
        //    }
            return IPString;
        }


        Socket socketArray ;


        public int sendDataPacketToServer(byte[] buf, int type, int len)
        {
            int j;
            int PacketLen = 0;
            if (buf != null)
            {
                for (j = 0; j < len; j++)
                {
                    send_packet[j + PACKET_DATASTATUS_POS] = (byte)buf[j];
                }
            }

            PacketLen = MIN_PACKET_LEN - 4 + len;
            SendPacketLen = PacketLen;
            inputDataHeader(send_packet, PacketLen, SITE_TO_SERVER, type, 0);

            CRC16.addCrcCode(send_packet, PacketLen);
            return sock.Send(send_packet, PacketLen, 0);
        }
        public int sendPackedWithNoDataToServer(int type)
        {
            int PacketLen = 0;
            PacketLen = MIN_PACKET_LEN - 4;
            inputDataHeader(send_packet, PacketLen, SITE_TO_SERVER, type, 0);

            CRC16.addCrcCode(send_packet, PacketLen);
            return sock.Send(send_packet, PacketLen, 0);
        }

        public int RecvResponseWithNoData()
        {
            int len = 0;

            Thread.Sleep(WAIT_BETWEEN_SEND_RECEIVE);

            len = sock.Receive(receiveByte, REV_LEN, 0);
            if (len == 0)
                return RESPONSE_FAIL;
            if (receiveByte[PACKET_DATASTATUS_POS] != RESPONSE_OK)
            {
                // MessageBox.Show("服务器返回错误", "信息提示", MessageBoxButtons.OK);
                return RESPONSE_FAIL;
            }
            return RESPONSE_OK;
        }


        public Boolean CheckRespAndRetry()
        {
            int i = 0;
            for (i = 0; i < 3; i++)
            {
                if (RecvResponseWithNoData() == RESPONSE_OK)
                    return true;
                sock.Send(send_packet, SendPacketLen, 0);
            }
            return false;
        }
    }
}
