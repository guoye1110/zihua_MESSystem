using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LabelPrint.NetWork
{
    public class HeartBeatSockets : FilmSocket
    {
 //       心跳包 发送心跳    打印软件-〉MES服务器	0xB3	0	0		
	//心跳回复（OK）	打印软件<-MES服务器	0xB3	0	0		
 //   心跳回复（fail）	打印软件<-MES服务器	0xB3	〉0	0	打印软件不需要特殊处理错误包，下次继续发送心跳包。但假如未收到心跳回复，需弹出对话框告知用户通讯失败



    }
}
