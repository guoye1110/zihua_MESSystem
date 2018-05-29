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
		//header(4) + len(2) + communicationtype(1) + time(12) + index(4) + reserved(4) + type(1) + CRC(4)
		const int MIN_PACKET_LEN_WITHOUT_DATA = 32;
		const int MIN_PACKET_LEN = 33;
		const int PACKET_LEN_POS = 4;
		const int PACKET_COMMUNICATION_TYPE_POS = 6;
		const int PACKET_DATASTATUS_POS = 28;
		const int COMMUNICATION_TYPE_HEART_BEAT = 0xB3;
		//5秒
		const int HEART_BEAT_INTERVAL = 50000;
		//1秒
		const int COMMUNICATION_TIMEOUT = 3000;
        const int RESPONSE_OK = 0;
        const int RESPONSE_FAIL = 1;

		private Boolean m_Abort = false;
        private Socket m_sock;
        private IPAddress m_hostIP;
		private int m_port;
		private Thread m_comThread;
		private Thread m_heartbeatThread;

		public delegate void networkstatehandler(bool        connected);
		public delegate void networkdatahandler(int communicationType, byte[] data_buf, int len);

		public event networkstatehandler network_state_event;
		public event networkdatahandler network_data_event;
		
/*
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

        public int SendPacketLen = 0;
*/
		public FilmSocket()
		{
			m_hostIP = IPAddress.Parse(GlobalConfig.Setting.CurSettingInfo.ServerIP);
			m_port = 8899;
			m_sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			//m_comThread = new Thread((startCommunication));
			m_comThread = new Thread((receive_thread));
			m_comThread.Start();
			m_heartbeatThread = new Thread((heartbeat_thread));
			m_heartbeatThread.Start();
		}
		~ FilmSocket()
		{
			m_heartbeatThread.Abort();
			m_comThread.Abort();
			m_sock.Shutdown(SocketShutdown.Both);
			m_sock.Close();
		}
		
        private void inputDataHeader(byte[] packet, int len, int type, int dataType)
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

		private void heartbeat_thread()
        {
        	byte[] send_buf = new byte[MIN_PACKET_LEN];

			try {
				while(true) {
					try {
						inputDataHeader(send_buf, MIN_PACKET_LEN, COMMUNICATION_TYPE_HEART_BEAT, 0);
						send_buf[PACKET_DATASTATUS_POS] = 0;
						toolClass.addCrc32Code(send_buf, MIN_PACKET_LEN);

						m_sock.Send(send_buf, MIN_PACKET_LEN, 0);
					}
					catch (Exception e) {
						Console.WriteLine("heartbeat failed: " + e);
					}
					finally {
						//心跳协议每隔5秒发送
                        Thread.Sleep(HEART_BEAT_INTERVAL);
					}
				}
			}
			catch (ThreadAbortException abortException) {
				Console.WriteLine("heartbeat_thread abort: "+ (string)abortException.ExceptionState);
			}
		}

		private void receive_thread()
		{
			int recCount;
			int len;
			int retry_cnt = 3;
			byte[] recv_buf = new byte[2*RECV_BUFFER_SIZE];
			byte[] pack_buf, data_buf;
			IPEndPoint point = new IPEndPoint(m_hostIP, m_port);
			int start_pos, pack_len;

			try {
				while (true) {
					try {
						if (!m_sock.Connected) {
							m_sock.Connect(point);
							if (m_sock.Connected)
								network_state_event(m_sock.Connected);
						}

						if (m_sock.Connected) {
							retry_cnt = 3;

							recCount = m_sock.Receive(recv_buf, recv_buf.Length, 0);
							if (recCount == 0) {
								//length is 0, means TCP/IP disconnected, retry 3 times
								network_state_event(m_sock.Connected);
								retry_cnt--;
								continue;
							}

							for (start_pos=0;start_pos<recCount;) {
								pack_len = recv_buf[start_pos+PACKET_LEN_POS] + recv_buf[start_pos+PACKET_LEN_POS+1]*0x100;
								if (start_pos+pack_len <= recCount) {//At least one packet
									pack_buf = new byte[pack_len];
									Array.Copy(recv_buf, start_pos, pack_buf, 0, pack_len);
									if (toolClass.checkCrc32Code(pack_buf, pack_len)) {
										int communicationType = pack_buf[PACKET_COMMUNICATION_TYPE_POS];
										data_buf = new byte[pack_len-MIN_PACKET_LEN_WITHOUT_DATA];
										Array.Copy(pack_buf, PACKET_DATASTATUS_POS, data_buf, 0, data_buf.Length);
										network_data_event(communicationType, data_buf, data_buf.Length);
									}
								}
								start_pos += pack_len;
							}
						}
					}
					catch (Exception e) {
						Console.WriteLine("通讯失败，可能服务器端未启动或者网络连接出错: "+ e);
						network_state_event(m_sock.Connected);
						retry_cnt--;						
					}
				}
			} 
			catch (ThreadAbortException abortException) {
				Console.WriteLine("receive_thread abort: "+ (string)abortException.ExceptionState);
			}
		}

		//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
		//始终尝试连接，发送心跳，接受回复，若失败表示连接中断，尝试再连3次，若3次失败，则60秒后重试。
		//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        private void startCommunication()
        {
            int recCount;
            int len;
			int retry_cnt = 3;
			byte[] buf = new byte[RECV_BUFFER_SIZE];
			IPEndPoint point = new IPEndPoint(m_hostIP, m_port);
			int old_timeout = m_sock.ReceiveTimeout;

			while (true) {
				if (m_Abort == true)	break;

				if (retry_cnt==0)	Thread.Sleep(60000);//retry 3 times failed, sleep 60 seconds
				
            	try {
					if (!m_sock.Connected) {
						m_sock.Connect(point);
						if (m_sock.Connected)
							network_state_event(m_sock.Connected);
					}

                	if (m_sock.Connected) {
						//心跳协议每隔5秒发送
						Thread.Sleep(5000);
						
						retry_cnt = 3;

						inputDataHeader(buf, MIN_PACKET_LEN, COMMUNICATION_TYPE_HEART_BEAT, 0);
						buf[PACKET_DATASTATUS_POS] = 0;
						toolClass.addCrc32Code(buf, MIN_PACKET_LEN);

						m_sock.Send(buf, MIN_PACKET_LEN, 0);
						recCount = m_sock.Receive(buf, RECV_BUFFER_SIZE, 0);
						if (recCount == 0) {
							//length is 0, means TCP/IP disconnected, retry 3 times
							network_state_event(m_sock.Connected);
							retry_cnt--;
							continue;
						}
                	}
            	}
            	catch (Exception ex)
            	{
                	Console.WriteLine("通讯失败，可能服务器端未启动或者网络连接出错: "+ex);
					network_state_event(m_sock.Connected);
					retry_cnt--;
            	}
			}
        }

        public int sendDataPacketToServer(byte[] data, int type, int len)
        {
            int j;
            int PacketLen = 0;
			byte[] send_packet = new byte[RECV_BUFFER_SIZE];
			
            if (data != null)
				Array.ConstrainedCopy(data, 0, send_packet, PACKET_DATASTATUS_POS, len);

            PacketLen = MIN_PACKET_LEN_WITHOUT_DATA + len;
            inputDataHeader(send_packet, PacketLen, type, 0);
            toolClass.addCrc32Code(send_packet, PacketLen);

			return m_sock.Send(send_packet, PacketLen, 0);
        }

        public int RecvResponse(int timeout)
        {
            int len = 0;
			int old_timeout;
			byte[] buf = new byte[RECV_BUFFER_SIZE];

			old_timeout = m_sock.ReceiveTimeout;

			try {
	            m_sock.ReceiveTimeout = timeout;
    	        len = m_sock.Receive(buf, RECV_BUFFER_SIZE, 0);
				m_sock.ReceiveTimeout = old_timeout;

				//Tcp connection disconnected
        	    if (len == 0)   return -1;

				if(!toolClass.checkCrc32Code(buf, len))
					return -1;
				
				return Convert.ToInt16(buf[PACKET_DATASTATUS_POS]);
			}
			catch (SocketException e) {
				m_sock.ReceiveTimeout = old_timeout;
				Console.WriteLine("RecvResponse: {0} Error code: {1}.", e.Message, e.ErrorCode);
				return -1;
			}
        }

        public byte[] RecvData(int timeout)
        {
            int len = 0;
			int old_timeout;
			byte[] buf = new byte[RECV_BUFFER_SIZE];
			byte[] data;

			old_timeout = m_sock.ReceiveTimeout;

			try {
	            m_sock.ReceiveTimeout = timeout;
    	        len = m_sock.Receive(buf, RECV_BUFFER_SIZE, 0);
				m_sock.ReceiveTimeout = old_timeout;

				//Tcp connection disconnected
        	    if (len == 0)   return null;

				if(!toolClass.checkCrc32Code(buf, len))
					return null;

				data = new byte[len-MIN_PACKET_LEN_WITHOUT_DATA];
				Array.Copy(buf, PACKET_DATASTATUS_POS, data, 0, len-MIN_PACKET_LEN_WITHOUT_DATA);
				return data;
			}
			catch (SocketException e) {
				m_sock.ReceiveTimeout = old_timeout;
				Console.WriteLine("RecvData: {0} Error code: {1}.", e.Message, e.ErrorCode);
				return null;
			}
        }

		public bool get_status()
        {
        	return m_sock.Connected;
		}
    }
}
