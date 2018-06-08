using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LabelPrint.NetWork
{
    public class PrintSockets : FilmSocket
    {

 //       印刷工序 印刷开工    打印软件-〉MES服务器	0xBB	0	0		
	//开工回复（OK）	打印软件<-MES服务器	0xBB	0	印刷工单;产品代码		1804306121Y3;MPF003001
 //   开工回复（fail）	打印软件<-MES服务器	0xBB	>0		打印软件须再次发送

 //   流延膜标签上传 打印软件-〉MES服务器	0xBC	0	条码信息;卷重		1804306121L321340030;852kg
 //   扫描回复（OK）	打印软件<-MES服务器	0xBC	0			

 //   扫描回复（fail）	打印软件<-MES服务器	0xBC	>0		打印软件须再次发送


 //   印刷膜标签上传 打印软件-〉MES服务器	0xBD	0	条码信息		1804306121Y321340030

 //   扫描回复（OK）	打印软件<-MES服务器	0xBD	0			

 //   扫描回复（fail）	打印软件<-MES服务器	0xBD	>0		打印软件须再次发送


 //   印刷交接班记录上传   打印软件-〉MES服务器	0xBE	0	交接班信息   备注信息, 字符串    aasdfsaadsfadsfasdf

 //   扫描回复（OK）	打印软件<-MES服务器	0xBE	0			

 //   扫描回复（fail）	打印软件<-MES服务器	0xBE	>0		打印软件须再次发送

    }
}
