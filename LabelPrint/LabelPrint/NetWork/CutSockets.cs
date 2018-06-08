using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using LabelPrint.Util;
namespace LabelPrint.NetWork
{
    public class CutSockets :FilmSocket
    {



        //const int WAIT_BETWEEN_SEND_RECEIVE = 200;



        //const int COMMUNICATION_TYPE_SLIT_PROCESS_START = 0xBF;
        ////分切工序 分切开工    打印软件-〉MES服务器	0xBF	0	0		
        ////        开工回复（OK）	打印软件<-MES服务器	0xBF	0	分切工单;产品代码		1804306121F3;MPF003001
        ////        开工回复（fail）	打印软件<-MES服务器	0xBF	>0		打印软件须再次发送
        //Boolean SendCutBegin()
        //{
        //    sendPackedWithNoDataToServer(COMMUNICATION_TYPE_SLIT_PROCESS_START);

        //    return true;
        //}

        ////String  SendCutBegin()
        ////{
        ////    String str;
        ////    int len = 0;
        ////    byte[] data = new byte[1000];
        ////    CreateCutBeginPacket();
        ////    len = sock.Receive(receiveByte, REV_LEN, 0);

        ////    str = System.Text.Encoding.GetEncoding("gb2312").GetString(receiveByte, PACKET_DATASTATUS_POS, len - MIN_PACKET_LEN);
        ////    if (receiveByte[PACKET_DATASTATUS_POS] != 0x0)
        ////    {
        ////        MessageBox.Show("服务器返回错误", "信息提示", MessageBoxButtons.OK);
        ////        return null;
        ////    }
        ////    return str;
        ////}

        //const int COMMUNICATION_TYPE_SLIT_BIG_BARCODE_UPLOAD = 0xC0;
        ////        大卷标签上传  打印软件-〉MES服务器	0xC0	0	条码信息		1804306121Y321340030
        ////        扫描回复（OK）	打印软件<-MES服务器	0xC0	0			

        ////        扫描回复（fail）	打印软件<-MES服务器	0xC0	>0		打印软件须再次发送
        //Boolean CreateCutBarCodePacket(String barcode)
        //{
        //    byte[] buf;
        //    if (barcode == null|| barcode == "")
        //        return false;
        //    buf = System.Text.Encoding.Default.GetBytes(barcode);

        //    for (int i = 0; i < buf.Length; i++)
        //    {
        //        send_packet[i + 8] = buf[i];
        //    }

        //    inputDataHeader(send_packet, MIN_PACKET_LEN+buf.Length, SITE_TO_SERVER, COMMUNICATION_TYPE_SLIT_BIG_BARCODE_UPLOAD, 0);
        //    return true;
        //}


          
        //Boolean SendCutBigLabel(String barcode)
        //{
        //    byte[] buf;
        //    if (barcode == null || barcode == "")
        //        return false;
        //    buf = System.Text.Encoding.Default.GetBytes(barcode);
        //    if (buf == null )
        //        return false;

        //    sendDataPacketToServer(buf, COMMUNICATION_TYPE_SLIT_BIG_BARCODE_UPLOAD, buf.Length);
        //    return CheckRespAndRetry();
        //}

        //const int COMMUNICATION_TYPE_SLIT_LITTLE_BARCODE_UPLOAD = 0xC0;
        ////        分切膜小卷标签上传   打印软件-〉MES服务器	0xC1	0	条码信息;卷重		1804306121L3213420030400;23.5kg
        ////        扫描回复（OK）	打印软件<-MES服务器	0xC1	0			

        ////        扫描回复（fail）	打印软件<-MES服务器	0xC1	>0		打印软件须再次发送

        //Boolean SendCutLittleLabel(String barcode)
        //{
        //    byte[] buf;
        //    if (barcode == null || barcode == "")
        //        return false;
        //    buf = System.Text.Encoding.Default.GetBytes(barcode);
        //    if (buf == null)
        //        return false;

        //    sendDataPacketToServer(buf, COMMUNICATION_TYPE_SLIT_LITTLE_BARCODE_UPLOAD, buf.Length);
        //    return CheckRespAndRetry();
        //}



        //const int COMMUNICATION_TYPE_SLIT_DABAO_LABEL_UPLOAD = 0xC0;

        ////        打包标签上传  打印软件-〉MES服务器	0xC2	0	铲板号;卷数;重量;长度;流延卷号		27;63;711.9kg;180km;127(4-13)128(1-13)129(1-13)130(1-13)131(1-6,8-13)
        ////        扫描回复（OK）	打印软件<-MES服务器	0xC2	0			

        ////        扫描回复（fail）	打印软件<-MES服务器	0xC2	>0		打印软件须再次发送
        //Boolean SendCutDabaoLabel(String str)
        //{
        //    byte[] buf;
        //    if (str == null || str == "")
        //        return false;
        //    buf = System.Text.Encoding.Default.GetBytes(str);
        //    if (buf == null)
        //        return false;

        //    sendDataPacketToServer(buf, COMMUNICATION_TYPE_SLIT_DABAO_LABEL_UPLOAD, buf.Length);
        //    return CheckRespAndRetry();
        //}

        //const int COMMUNICATION_TYPE_SLIT_JIAOJIE_RECORD_UPLOAD = 0xC0;
        ////        分切交接班记录上传   打印软件-〉MES服务器	0xC3	0	交接班信息   备注信息, 字符串    aasdfsaadsfadsfasdf
        ////        扫描回复（OK）	打印软件<-MES服务器	0xC3	0			
        ////        扫描回复（fail）	打印软件<-MES服务器	0xC3	>0		打印软件须再次发送
        //Boolean SendCutJiaoJieLabel(String str)
        //{
        //    byte[] buf;
        //    if (str == null || str == "")
        //        return false;
        //    buf = System.Text.Encoding.Default.GetBytes(str);
        //    if (buf == null)
        //        return false;

        //    sendDataPacketToServer(buf, COMMUNICATION_TYPE_SLIT_JIAOJIE_RECORD_UPLOAD, buf.Length);
        //    return CheckRespAndRetry();
        //}


    }
}
