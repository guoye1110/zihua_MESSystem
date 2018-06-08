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
using System.Diagnostics;
using MESSystem.common;

namespace MESSystem.communication
{
    public partial class communicate
    {
        public partial class ClientThread
        {
            private int boardIDReviewedByApp;
            private int fileDataOffset;
            private FileStream fs;

            private void sendResponseOKBack(int result)
            {
                onePacket[PROTOCOL_DATA_POS] = (byte)result;
                onePacket[PROTOCOL_LEN_POS] = (byte)MIN_PACKET_LEN;
                onePacket[PROTOCOL_LEN_POS + 1] = 0;
                toolClass.addCrc32Code(onePacket, MIN_PACKET_LEN);
                clientSocketInServer.Send(onePacket, MIN_PACKET_LEN, 0);   //response OK
            }

            public void processClientMobileApp(int communicationType, int len)
            {
                string str;
                string appDeviceName;
                string appFileName;

                try
                {
                    switch (communicationType)
                    {
                        case COMMUNICATION_TYPE_APP_WORKING_BOARD_ID_TO_PC:
                            boardIDReviewedByApp = onePacket[PROTOCOL_DATA_POS];
                            handshakeWithClientOK = 1;
                            sendResponseOKBack(0);
                            fs = null;
                            break;
                        case COMMUNICATION_TYPE_APP_FILE_NAME_TO_PC:
                            len -= MIN_PACKET_LEN_MINUS_ONE;  //MIN_PACKET_LEN include one byte of data, so we need to delete this byte
                            appFileName = System.Text.Encoding.Default.GetString(onePacket, PROTOCOL_DATA_POS, len);
                            fs = new FileStream("..\\..\\files\\" + boardIDReviewedByApp + "\\" + appFileName, FileMode.Create);
                            sendResponseOKBack(0);
                            fileDataOffset = 0;
                            while(gVariable.fileDataInWriting == 1)  //someone else is now doing video file upload, wait for the completion of this action
                                toolClass.nonBlockingDelay(1);

                            //now no one is using this function, set our own flag to avoid other people doing the same action
                            gVariable.fileDataInWriting = 1;

                            str = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
                            Console.WriteLine(str + ": : file write starts!");
                            break;
                        case COMMUNICATION_TYPE_APP_FILE_DATA_TO_PC:
                            len -= MIN_PACKET_LEN_MINUS_ONE;  //MIN_PACKET_LEN include one byte of data, so we need to delete this byte

                            if (fileDataOffset + len > gVariable.fileDataLength)
                                break;
                            Array.Copy(onePacket, PROTOCOL_DATA_POS, gVariable.fileDataBuf, fileDataOffset, len);
                            fileDataOffset += len;
//                            str = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
//                            Console.WriteLine(str + ": got file data packet from app, now file length now is " + fileDataOffset);
                            sendResponseOKBack(0);
                            break;
                        case COMMUNICATION_TYPE_APP_FILE_END_TO_PC:
                            fs.Write(gVariable.fileDataBuf, 0, fileDataOffset);
                            fs.Close();
                            fs = null;
                            gVariable.fileDataInWriting = 0;
                            str = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
                            Console.WriteLine(str + ": : file write complte!");
                            sendResponseOKBack(0);
                            break;
                        case COMMUNICATION_TYPE_APP_DEVICE_NAME:
                            len -= MIN_PACKET_LEN_MINUS_ONE;  //MIN_PACKET_LEN include one byte of data, so we need to delete this byte
                            if (len <= 1)  //length is not correct
                                break;
                            appDeviceName = System.Text.Encoding.Default.GetString(onePacket, PROTOCOL_DATA_POS + 1, len - 1);
                            sendResponseOKBack(0);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("dealWithDataFromApp(" + communicationType + "," + len + ") for myBoardIndex " + myBoardIndex + "failed, " + ex);
                }
            }

        }
    }
}