using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LabelPrint.NetWork
{
    public class RecoverySockets :FilmSocket
    {
        //const int COMMUNICATION_TYPE_RECOVERY_RECORD_UPLOAD = 0xC5;

        ////      再造料工序 再造料标签上传 打印软件-〉MES服务器	0xC5	0	条码信息		1804306121Z321340030
        ////扫描回复（OK）	打印软件<-MES服务器	0xC5	0			

        ////扫描回复（fail）	打印软件<-MES服务器	0xC5	>0		打印软件须再次发送


        //Boolean ProcRecoveryLabel(String str)
        //{
        //    byte[] buf;
        //    if (str == null || str == "")
        //        return false;
        //    buf = System.Text.Encoding.Default.GetBytes(str);
        //    if (buf == null)
        //        return false;

        //    sendDataPacketToServer(buf, COMMUNICATION_TYPE_RECOVERY_RECORD_UPLOAD, buf.Length);
        //    return CheckRespAndRetry();
        //}

    }
}
