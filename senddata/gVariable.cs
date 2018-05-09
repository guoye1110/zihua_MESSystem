using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Threading;
using System.Net.Sockets;

namespace clientFunc
{
    public class gVariable
    {
        public const int CLIENT_PC_WAREHOUSE_ID1 = 101;  //�����

        public const int CLIENT_PC_FEED_ID1 = 121;  //����ϵͳ
        public const int CLIENT_PC_FEED_ID2 = 122;  //����ϵͳ
        public const int CLIENT_PC_FEED_ID3 = 123;  //����ϵͳ
        public const int CLIENT_PC_FEED_ID4 = 124;  //����ϵͳ
        public const int CLIENT_PC_FEED_ID5 = 125;  //����ϵͳ

        public const int CLIENT_PC_CAST_ID1 = 141;  //�����豸
        public const int CLIENT_PC_CAST_ID2 = 142;  //�����豸
        public const int CLIENT_PC_CAST_ID3 = 143;  //�����豸
        public const int CLIENT_PC_CAST_ID4 = 144;  //�����豸
        public const int CLIENT_PC_CAST_ID5 = 145;  //�����豸

        public const int CLIENT_PC_PRINT_ID1 = 161;  //ӡˢ�豸
        public const int CLIENT_PC_PRINT_ID2 = 162;  //ӡˢ�豸
        public const int CLIENT_PC_PRINT_ID3 = 163;  //ӡˢ�豸

        public const int CLIENT_PC_SLIT_ID1 = 181;  //�����豸
        public const int CLIENT_PC_SLIT_ID2 = 182;  //�����豸
        public const int CLIENT_PC_SLIT_ID3 = 183;  //�����豸
        public const int CLIENT_PC_SLIT_ID4 = 184;  //�����豸
        public const int CLIENT_PC_SLIT_ID5 = 185;  //�����豸

        public const int CLIENT_PC_INSPECTION_ID1 = 201;  //�ʼ칤��

        public const int CLIENT_PC_REUSE_ID1 = 221;  //�����Ϲ���

        public const int CLIENT_PC_PACKING_ID1 = 241;  //�������

        public struct dispatchSheetStruct
        {
            public string machineID;   //�豸��� 
            public string dispatchCode;  //dispatch code
            public string planTime1;	//Ԥ�ƿ�ʼʱ��
            public string planTime2;  //Ԥ�ƽ���ʱ��
            public string productCode;	 //��Ʒ����
            public string productName;  //��Ʒ����
            public string operatorName; //����Ա
            public int plannedNumber;	//�ƻ���������
            public int outputNumber;  //��������
            public int qualifiedNumber;  //�ϸ�����
            public int unqualifiedNumber;  //���ϸ�����
            public string processName; //���򣨹���·���а����������
            public string realStartTime;	  //ʵ�ʿ�ʼʱ��
            public string realFinishTime;	  //ʵ���깤ʱ��
            public string prepareTimePoint;   //����ʱ�䣨�Բ�ʱ�䣩���������Ⱦ�������ʱ�䣬Ȼ������ʽ����
            public int status;	  //0��dummy/prepared ����׼����ɵ�δ������1:�����깤���¹���δ������2�������������豸 3������������ 4���Բ�������ʱ�䣩 OK
            public int toolLifeTimes;  //������������
            public int toolUsedTimes;  //����ʹ�ô���
            public int outputRatio;  //����ϵ��
            public string serialNumber; //��ˮ��
            public string reportor; //����Ա����, ����Ա�Ͳ���Ա���ܲ���ͬһ����
            public string workshop;	 //�������ƣ���������ˮ�ߵ�����
            public string workshift;	 //��Σ�������ࣩ
            public string salesOrderCode; //������
            public string BOMCode; // 
            public string customer;
            public string barCode;
            public string barcodeForReuse;
            public string quantityOfReused;
            public string multiProduct;
            public string productCode2;
            public string productCode3;
            public string operatorName2; //����Ա
            public string operatorName3; //����Ա
        }
    }
}
