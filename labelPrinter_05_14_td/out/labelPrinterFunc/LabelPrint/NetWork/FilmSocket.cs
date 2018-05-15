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
		const int RECV_BUFFER_SIZE = 2000;
		const int COMMUNICATION_TYPE_HEART_BEAT = 0xB3;
		private Boolean m_isConnected = false;
        private Socket m_sock;
        private IPAddress m_hostIP;
		private int m_port;
		private Thread m_comThread;
		
		
		

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

		public FilmSocket(string ip, int port)
		{
			m_hostIP = IPAddress.Parse(ip);
			m_port = port;
			m_sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			m_comThread = new Thread((startCommunication));
			m_comThread.Start();
		}
		
        void inputDataHeader(byte[] packet, int len, int type, int dataType)
        {
            int i;

            i = 0;

            packet[i++] = (byte)'w';
            packet[i++] = (byte)'I';
            packet[i++] = (byte)'F';
            packet[i++] = (byte)'i';

            packet[i++] = (byte)len;
            packet[i++] = (byte) (len / 0x100);

            packet[i++] = (byte)type;

            packet[i++] = (byte)(DateTime.Now.Year % 100 / 10 + '0');
            packet[i++] = (byte)(DateTime.Now.Year % 100 % 10 + '0');
            packet[i++] = (byte)(DateTime.Now.Month / 10 + '0');
            packet[i++] = (byte)(DateTime.Now.Month % 10 + '0');
            packet[i++] = (byte)(DateTime.Now.Day / 10 + '0');
            packet[i++] = (byte)(DateTime.Now.Day % 10 + '0');
            packet[i++] = (byte)(DateTime.Now.Hour / 10 + '0');
            packet[i++] = (byte)(DateTime.Now.Hour % 10 + '0');
            packet[i++] = (byte)(DateTime.Now.Minute / 10 + '0');
            packet[i++] = (byte)(DateTime.Now.Minute % 10 + '0');
            packet[i++] = (byte)(DateTime.Now.Second / 10 + '0');
            packet[i++] = (byte)(DateTime.Now.Second % 10 + '0');

            packet[i++] = (byte)0;
            packet[i++] = (byte)0;
            packet[i++] = (byte)0;
            packet[i++] = (byte)0;

            packet[i++] = 0;
            packet[i++] = 0;
            packet[i++] = 0;
            packet[i++] = 0;

            packet[i++] = (byte)dataType;
         }

        const int MIN_PACKET_LEN_MINUS_ONE = 32;   //header(4) + len(2) + communicationtype(1) + time(12) + index(4) + reserved(4) + type(1) + CRC(4)


        const int WAIT_BETWEEN_SEND_RECEIVE = 200;

        public void sendHeartBeat(byte[] buf, int type, int len)
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

		//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
		//始终尝试连接，发送心跳，接受回复，若失败表示连接中断，尝试再连3次，若再失败，则60秒后重试。
		//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        public void startCommunication()
        {
            int recCount;
            int len;
			int retry_cnt = 3;
			byte[] buf = new byte[RECV_BUFFER_SIZE];
			IPEndPoint point = new IPEndPoint(m_hostIP, m_port);

			while (1) {
				if (retry_cnt==0)	Thread.Sleep(60000);//retry 3 times failed, sleep 60 seconds
				
            	try {
                	bConnected = true;
                	len = MIN_PACKET_LEN_MINUS_ONE + 4;

					m_sock.Connect(point);

                	if (m_sock.Connected) {
						retry_cnt = 3;
						m_isConnected = true;

						inputDataHeader(buf, MIN_PACKET_LEN_MINUS_ONE+1, COMMUNICATION_TYPE_HEART_BEAT, 0);
						CRC16.addCrcCode(buf, MIN_PACKET_LEN_MINUS_ONE+1);
						m_sock.Send(buf, MIN_PACKET_LEN_MINUS_ONE+1, 0);
						recCount = m_sock.Receive(buf, RECV_BUFFER_SIZE, 0);
						if (!recCount){
							//length is 0, means TCP/IP disconnected, retry 3 times
							retry_cnt--;
							m_isConnected = false;
							continue;
						}
                	}
            	}
            	catch (Exception ex)
            	{
                	Console.WriteLine("通讯失败，可能服务器端未启动或者网络连接出错: "+ex);
					retry_cnt--;
					m_isConnected = false;
            	}
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
