﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LabelPrint.NetWork
{
    public class OutBoundingSockets : FilmSocket
    {

        //       出入库工序 申请物料    打印软件-〉MES服务器	0xB5	0	0		
        //物料申请回复（OK）	打印软件<-MES服务器	0xB5	0	7台设备，每台7个料仓，一共49组数据，每组数据格式如下：物料代码;物料数量;		03.1.20.018;560;03.1.20.017343;;;;;;;;;;;03.1.20.015;235;03.1.20.029;2304;03.1.20.011;2456;03.1.20.009;342;;;;;;;03.1.20.001;23;03.1.20.014;23;03.1.20.018;432;;;;;;;;;03.1.20.018;560;03.1.20.017343;;;;;;;;;;;03.1.20.015;235;03.1.20.029;2304;03.1.20.011;2456;03.1.20.009;342;;;;;;;03.1.20.001;23;03.1.20.014;23;03.1.20.018;432;;;;;;;;;03.1.20.001;23;03.1.20.014;23;03.1.20.018;432;;;;;;;;;
        //物料申请回复（fail）	打印软件<-MES服务器	0xB5	〉0		打印软件须再次申请


        //   出库扫描    打印软件-〉MES服务器	0xB6	0	原料代码;原料批次号;目标设备号;料仓号;重量		03.1.20.018;aazz20180201sinochem;3;2;340
        //扫描回复（OK）	打印软件<-MES服务器	0xB6	0			

        //   扫描回复（fail）	打印软件<-MES服务器	0xB6	>0		打印软件须再次发送


        //   入库扫描    打印软件-〉MES服务器	0xB7	0	原料代码;原料批次号;目标设备号;料仓号;重量		03.1.20.018;aazz20180201sinochem;3;2;340
        //扫描回复（OK）	打印软件<-MES服务器	0xB7	0			

        //   扫描回复（fail）	打印软件<-MES服务器	0xB7	>0		打印软件须再次发送
    }
}
