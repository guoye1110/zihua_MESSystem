using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.Drawing;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using System.Threading;
using MESSystem.alarmFun;

namespace MESSystem.common
{
    public class toolClass
    {
        public const int dummy_craftNum = 8;
        public const int dummy_volcurNum = 4;
        public const int dummy_qualityNum = 3;
        public const int dummy_beatNum = 1;
        public const int dummy_materialnumberOfTypes = 3;

        public static string[] employeeNameArray;
        public static string[] employeeIDArray;

        public static float[] cp = new float[gVariable.maxCurveNum];
        public static float[] cpk = new float[gVariable.maxCurveNum];
        public static float[] ppk = new float[gVariable.maxCurveNum];

        private const string machinePosFile = "..\\..\\init\\machinePos.xlsx";
        private const string machineNameFile = "..\\..\\init\\machineNameList.xlsx";

        //a list between machine code and machine table, first column is code, like Y-129, second column is name, like grinding machine
        public static string[,] machineNameList = new string[gVariable.maxMachineNum, 2];

        private static int dispatchIndexForToday = 0;
        private static string dataOld = DateTime.Now.Date.ToString("yyMMdd");

        public static void addCrcCode(byte[] data, int len)
        {
            if (gVariable.CompanyIndex == gVariable.DONGFENG_23)
                addCrc16Code(data, len);
            else //if (gVariable.CompanyIndex == gVariable.ZIHUA_ENTERPRISE)
                addCrc32Code(data, len);
        }

        public static void addCrc32Code(byte[] data, int len)
        {
            int value = 0xffff;
            int sum = 0;

            for (int i = 0; i < len - 4; i++)
            {
                // 1.value 右移8位(相当于除以256)  
                // 2.value与进来的数据进行异或运算后再与0xFF进行与运算  
                // 得到一个索引index，然后查找CRC16_TABLE表相应索引的数据  
                // 1和2得到的数据再进行异或运算。  
                sum += data[i];
                value = (value >> 8) ^ CRC16_TABLE[(value ^ data[i]) & 0xff];
            }

            value = ~value & 0xffff;

            data[len - 4] = (byte)(value / 256);
            data[len - 3] = (byte)(value % 256);
            data[len - 2] = (byte)(sum / 256);
            data[len - 1] = (byte)(sum % 256);
        }

        public static void addCrc16Code(byte[] data, int len)
        {
            int value = 0xffff; 
 
            for (int i = 0; i < len - 2; i++)
            {
                // 1.value 右移8位(相当于除以256)  
                // 2.value与进来的数据进行异或运算后再与0xFF进行与运算  
                // 得到一个索引index，然后查找CRC16_TABLE表相应索引的数据  
                // 1和2得到的数据再进行异或运算。  
                value = (value >> 8) ^ CRC16_TABLE[(value ^ data[i]) & 0xff];
            }

            value = ~value & 0xffff;

            data[len - 2] = (byte)(value / 256);
            data[len - 1] = (byte)(value % 256);
        }

        public static void addCrc16Code(byte[] data)
        {
            int value = 0xffff;

            for (int i = 0; i < data.Length - 2; i++)
            {
                // 1.value 右移8位(相当于除以256)  
                // 2.value与进来的数据进行异或运算后再与0xFF进行与运算  
                // 得到一个索引index，然后查找CRC16_TABLE表相应索引的数据  
                // 1和2得到的数据再进行异或运算。  
                value = (value >> 8) ^ CRC16_TABLE[(value ^ data[i]) & 0xff];
            }

            value = ~value & 0xffff;

            data[data.Length - 2] = (byte)(value / 256);
            data[data.Length - 1] = (byte)(value % 256);
        }

        public static bool checkCrc32Code(byte[] data, int len)
        {
            int value = 0xffff;
            int sum = 0;

            if(len <= 4)
                return false;

            try
            {
                for (int i = 0; i < len - 4; i++)
                {
                    // 1.value 右移8位(相当于除以256)  
                    // 2.value与进来的数据进行异或运算后再与0xFF进行与运算  
                    // 得到一个索引index，然后查找CRC16_TABLE表相应索引的数据  
                    // 1和2得到的数据再进行异或运算。  
                    value = (value >> 8) ^ CRC16_TABLE[(value ^ data[i]) & 0xff];
                    sum += data[i];
                }

                value = ~value & 0xffff;

                if (data[len - 4] != (byte)(value / 256) || data[len - 3] != (byte)(value % 256) || data[len - 2] != (byte)(sum / 256) || data[len - 1] != (byte)(sum % 256))
                    return false;
                else
                    return true;
            }
            catch (Exception ex)
            {
                Console.Write("CRC check exception occurred, len =  " + len + "; " + ex);
                return false;
            }
        }

        public static bool checkCrc16Code(byte[] data, int len)
        {
            int value = 0xffff;

            if (len <= 2)
                return false;

            try
            {
                for (int i = 0; i < len - 2; i++)
                {
                    // 1.value 右移8位(相当于除以256)  
                    // 2.value与进来的数据进行异或运算后再与0xFF进行与运算  
                    // 得到一个索引index，然后查找CRC16_TABLE表相应索引的数据  
                    // 1和2得到的数据再进行异或运算。  
                    value = (value >> 8) ^ CRC16_TABLE[(value ^ data[i]) & 0xff];
                }

                value = ~value & 0xffff;

                if (data[len - 2] != (byte)(value / 256) || data[len - 1] != (byte)(value % 256))
                    return false;
                else
                    return true;
            }
            catch (Exception ex)
            {
                Console.Write("CRC check exception occurred, len =  " + len + "; " + ex);
                return false;
            }
        }

        private static int[] CRC16_TABLE = {  
        0x0000, 0x1189, 0x2312, 0x329b, 0x4624, 0x57ad, 0x6536, 0x74bf,  
        0x8c48, 0x9dc1, 0xaf5a, 0xbed3, 0xca6c, 0xdbe5, 0xe97e, 0xf8f7,  
        0x1081, 0x0108, 0x3393, 0x221a, 0x56a5, 0x472c, 0x75b7, 0x643e,  
        0x9cc9, 0x8d40, 0xbfdb, 0xae52, 0xdaed, 0xcb64, 0xf9ff, 0xe876,  
        0x2102, 0x308b, 0x0210, 0x1399, 0x6726, 0x76af, 0x4434, 0x55bd,  
        0xad4a, 0xbcc3, 0x8e58, 0x9fd1, 0xeb6e, 0xfae7, 0xc87c, 0xd9f5,  
        0x3183, 0x200a, 0x1291, 0x0318, 0x77a7, 0x662e, 0x54b5, 0x453c,  
        0xbdcb, 0xac42, 0x9ed9, 0x8f50, 0xfbef, 0xea66, 0xd8fd, 0xc974,  
        0x4204, 0x538d, 0x6116, 0x709f, 0x0420, 0x15a9, 0x2732, 0x36bb,  
        0xce4c, 0xdfc5, 0xed5e, 0xfcd7, 0x8868, 0x99e1, 0xab7a, 0xbaf3,  
        0x5285, 0x430c, 0x7197, 0x601e, 0x14a1, 0x0528, 0x37b3, 0x263a,  
        0xdecd, 0xcf44, 0xfddf, 0xec56, 0x98e9, 0x8960, 0xbbfb, 0xaa72,  
        0x6306, 0x728f, 0x4014, 0x519d, 0x2522, 0x34ab, 0x0630, 0x17b9,  
        0xef4e, 0xfec7, 0xcc5c, 0xddd5, 0xa96a, 0xb8e3, 0x8a78, 0x9bf1,  
        0x7387, 0x620e, 0x5095, 0x411c, 0x35a3, 0x242a, 0x16b1, 0x0738,  
        0xffcf, 0xee46, 0xdcdd, 0xcd54, 0xb9eb, 0xa862, 0x9af9, 0x8b70,  
        0x8408, 0x9581, 0xa71a, 0xb693, 0xc22c, 0xd3a5, 0xe13e, 0xf0b7,  
        0x0840, 0x19c9, 0x2b52, 0x3adb, 0x4e64, 0x5fed, 0x6d76, 0x7cff,  
        0x9489, 0x8500, 0xb79b, 0xa612, 0xd2ad, 0xc324, 0xf1bf, 0xe036,  
        0x18c1, 0x0948, 0x3bd3, 0x2a5a, 0x5ee5, 0x4f6c, 0x7df7, 0x6c7e,  
        0xa50a, 0xb483, 0x8618, 0x9791, 0xe32e, 0xf2a7, 0xc03c, 0xd1b5,  
        0x2942, 0x38cb, 0x0a50, 0x1bd9, 0x6f66, 0x7eef, 0x4c74, 0x5dfd,  
        0xb58b, 0xa402, 0x9699, 0x8710, 0xf3af, 0xe226, 0xd0bd, 0xc134,  
        0x39c3, 0x284a, 0x1ad1, 0x0b58, 0x7fe7, 0x6e6e, 0x5cf5, 0x4d7c,  
        0xc60c, 0xd785, 0xe51e, 0xf497, 0x8028, 0x91a1, 0xa33a, 0xb2b3,  
        0x4a44, 0x5bcd, 0x6956, 0x78df, 0x0c60, 0x1de9, 0x2f72, 0x3efb,  
        0xd68d, 0xc704, 0xf59f, 0xe416, 0x90a9, 0x8120, 0xb3bb, 0xa232,  
        0x5ac5, 0x4b4c, 0x79d7, 0x685e, 0x1ce1, 0x0d68, 0x3ff3, 0x2e7a,  
        0xe70e, 0xf687, 0xc41c, 0xd595, 0xa12a, 0xb0a3, 0x8238, 0x93b1,  
        0x6b46, 0x7acf, 0x4854, 0x59dd, 0x2d62, 0x3ceb, 0x0e70, 0x1ff9,  
        0xf78f, 0xe606, 0xd49d, 0xc514, 0xb1ab, 0xa022, 0x92b9, 0x8330,  
        0x7bc7, 0x6a4e, 0x58d5, 0x495c, 0x3de3, 0x2c6a, 0x1ef1, 0x0f78  
        };

        public static byte[] intToByte(int i)
        {
            byte[] abyte0 = new byte[4];
            abyte0[0] = (byte)(0xff & i);
            abyte0[1] = (byte)((0xff00 & i) >> 8);
            abyte0[2] = (byte)((0xff0000 & i) >> 16);
            abyte0[3] = (byte)((0xff000000 & i) >> 24);
            return abyte0;
        }

        public static String RealHexToStr(String str)
        {
            String hText = "0123456789ABCDEF";
            StringBuilder bin = new StringBuilder();
            for (int i = 0; i < str.Length; i++)
            {
                bin.Append(hText[str[i] / 16]).Append(hText[str[i] % 16]).Append(' ');
            }
            return bin.ToString();
        }


        public static string getAlarmMailList()
        {
            /*
                        int i, mailIndex;
                        string str;
                        string MailList = null;

                        mailIndex = 0;
                        //index for database start from 1
                        for (i = 1; ; i++)
                        {
                            str = mySQLClass.getAnothercolumnFromDatabaseByOneColumn(mySQLClass.basicInfoDatabaseName, gVariable.employeeTableName, "id", i.ToString(), mySQLClass.EMPLOYEE_MAIL_ADDR_COLIUMN);
                            if (str == null)
                                break;

                            if (str.Length > 5)  //a mail account should longer than 5 bytes
                            {
                                if (mailIndex == 0)
                                    MailList += str;
                                else
                                    MailList += ";" + str;

                                mailIndex++;
                            }
                        }

                        return MailList;
            */
            return "AndonList@minyi.com; jiaming.Luo@minyi.com; dong.liu@minyi.com";
        }


        //163 email sending function:
        //1. Open 163 email and enter your account.
        //2. Settings->normal settings->POP3/SMTP/IMAP->enable POP3/SMTP/IAMP
        //3. Enter client privillege password.
        //4. Windows7 -> control panels -> email setup -> E-mail account settings -> add new account
        //5. Enter: Tandi - 13621874141@163.com - pop.163.com - smtp.163.com - 13621874141 - mailbox password
        //6. More settings -> outgoing server
        //7. Check "Mu outgoing server (SMTP) requires authentication" -> tick "use same setting as my incoming email server" 
        //8. select Advanced tab, tick "This server requires an encrypted connection"
        //163 email setting OK.
        public static void sendEmail(string [] MailList, string subject, string body)    
	    { 
            int i;

            MailMessage msg = new MailMessage();

            try
            {    
                //receiver
                for (i = 0; i < MailList.Length; i++)
                {
                    if (MailList[i].Length < 5)
                        continue;
                    msg.To.Add(MailList[i]);
                }

                //sender
                msg.From = new MailAddress("13621874141@163.com");

                msg.Subject = subject;//mail title
		        msg.SubjectEncoding = System.Text.Encoding.UTF8;//mail title coding
    		    msg.Body = body;
	    	    msg.BodyEncoding = System.Text.Encoding.UTF8;//mail content coding
		        msg.IsBodyHtml = true;//HTML mail or not    
		        msg.Priority = MailPriority.High;

                SmtpClient smtpClient = new SmtpClient();

//            smtpClient.UseDefaultCredentials = true;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

                smtpClient.Credentials = new System.Net.NetworkCredential("13621874141", "minyi11");
                smtpClient.Host = "smtp.163.com";
                smtpClient.Send(msg);
 	     	}    
    		catch (System.Net.Mail.SmtpException ex)    
 	     	{    
		     	MessageBox.Show(ex.Message, "报警邮件发送出错，请查看网络连接情况。");    
 		    }
  
	    }

        [DllImport("gdi32.dll")]
        public static extern int GetDeviceCaps(IntPtr hdc, int Index);

        public enum DeviceCap
        {
            VERTRES = 10,
            DESKTOPVERTRES = 117,
            LOGPIXELSY = 90,
        }  

        //scale factor could be 100%, 125% and 150%, we need to adjust our screen according to this factor
        public static int getScalingFactorForScreen()
        {
            int logpixelsy;

            Graphics g = Graphics.FromHwnd(IntPtr.Zero);
            IntPtr desktop = g.GetHdc();
//            int LogicalScreenHeight = GetDeviceCaps(desktop, (int)DeviceCap.VERTRES);
//            int PhysicalScreenHeight = GetDeviceCaps(desktop, (int)DeviceCap.DESKTOPVERTRES);
            logpixelsy = GetDeviceCaps(desktop, (int)DeviceCap.LOGPIXELSY);

            g.ReleaseHdc(desktop);

            return logpixelsy;
        }

        //consumes no CPU performance, but will have some effect on mouse sensitvity
        public static void systemDelay(int milliSecond)
        {
            Thread.Sleep(milliSecond);
        }

        //will not affect mouse movement, button touch action, but will consume CPU performance
        public static void nonBlockingDelay(int milliSecond)
        {
            int start = Environment.TickCount;
            while (Math.Abs(Environment.TickCount - start) < milliSecond)
            {
                Application.DoEvents();
            }
        }


        public static string utf8_gb2312(string text)
        {
            //声明字符集   
            System.Text.Encoding utf8, gb2312;
            //utf8   
            utf8 = System.Text.Encoding.GetEncoding("utf-8");
            //gb2312   
            gb2312 = System.Text.Encoding.GetEncoding("gb2312");
            byte[] utf, gb;
            utf = utf8.GetBytes(text);
            gb = System.Text.Encoding.Convert(utf8, gb2312, utf);
            //返回转换后的字符   
            return gb2312.GetString(utf);
        }

        public static string gb2312_utf8(string text)
        {
            //声明字符集   
            System.Text.Encoding utf8, gb2312;
            //gb2312   
            gb2312 = System.Text.Encoding.GetEncoding("gb2312");
            //utf8   
            utf8 = System.Text.Encoding.GetEncoding("utf-8");
            byte[] utf, gb;
            gb = gb2312.GetBytes(text);
            utf = System.Text.Encoding.Convert(gb2312, utf8, gb);
            //返回转换后的字符   
            return utf8.GetString(utf);
        }

/*
        public object memcpy(object des_obj1, object src_obj2)
        {
            byte[] aaa = StructToBytes(des_obj1);
            IntPtr hPtr = Marshal.UnsafeAddrOfPinnedArrayElement(aaa, 0);//获取字节数组首地址
            byte[] bbb = StructToBytes(src_obj2);
            Marshal.Copy(bbb, 0, hPtr, bbb.Length);
            des_obj1 = BytesToStuct(aaa, src_obj2.GetType());
            return des_obj1;
        }
*/

        public static T DeepClone<T>(T obj)
        {
            try
            {
                using (var ms = new MemoryStream())
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(ms, obj);
                    ms.Position = 0;
                    return (T)bf.Deserialize(ms);
                }
            }
            catch (Exception ex)
            {
                Console.Write("DeepClone() fail" + ex);
                return default(T);
            }


        }

        //when copy MES parameters from one board to system UI, we need a deep clone function to copy all the contents into new parameters
        public static void cloneMESParameters(int myBoardIndex)
        {
//            gVariable.dispatchSheetForUI = toolClass.DeepClone(gVariable.dispatchSheet[myBoardIndex]);
//            gVariable.machineStatusForUI = toolClass.DeepClone(gVariable.machineStatus[myBoardIndex]);
//            gVariable.craftParamForUI = toolClass.DeepClone(gVariable.craftList[myBoardIndex]);
//            gVariable.qualityDataForUI = toolClass.DeepClone(gVariable.qualityList[myBoardIndex]);
//            gVariable.materialListTableForUI = toolClass.DeepClone(gVariable.materialListTable[myBoardIndex]);
        }


        //-, minus should only be found at the beginning
        //., should not be at the beginning 
        public static int isNumericOrNot(string str)
        {
            int i;
            int ret;
            char[] strArray;

            if (str == null)
                return 0;

            try
            {
                strArray = str.Trim().ToCharArray();

                ret = 0;
                for (i = 0; i < strArray.Length; i++)
                {
                    if ((strArray[i] < 0x30 || strArray[i] > 0x39) && strArray[i] != '-' && strArray[i] != '.')
                    {
                        ret = 0;
                        break;
                    }

                    if (strArray[i] == '-' && i != 0)
                    {
                        ret = 0;
                        break;
                    }

                    if (strArray[i] == '.' && i == 0)
                    {
                        ret = 0;
                        break;
                    }

                    ret = 1;
                }
                return ret;
            }
            catch (Exception ex)
            {
                Console.WriteLine("isDigitalNum() failed!" + ex);
                return 0;
            }
        }


        //return: 0, not a number
        //        1, it is a number string    
        public static int isDigitalNum(string str)
        {
            int i;
            int ret;
            char[] strArray;

            try
            {
                strArray = str.Trim().ToCharArray();

                ret = 0;
                for (i = 0; i < strArray.Length; i++)
                {
                    if (strArray[i] < 0x30 || strArray[i] > 0x39)
                    {
                        ret = 0;
                        break;
                    }
                    ret = 1;
                }
                return ret;
            }
            catch (Exception ex)
            {
                Console.WriteLine("isDigitalNum() failed!" + ex);
                return 0;
            }
        }

        //from 20160708121230 to 2016-07-08 12:12:30
        //there is a strange format from touchpad, like 20176\03\0, which means 20170603, we need to make this format correct
        public static string toStandardTimeformat(string input)
        {
            int i;
            string str;
            byte [] array;

            try
            {
                if (input.Length > 14)
                    str = input.Remove(14);
                else
                    str = input;

                array = System.Text.Encoding.Default.GetBytes(str);

                for (i = 1; i < array.Length; i++)
                {
                    if (array[i] == 0)
                    {
                        array[i] = array[i - 1];
                        array[i - 1] = (byte)'0';
                    }
                }

                str = System.Text.Encoding.ASCII.GetString(array);
                return str.Insert(12, ":").Insert(10, ":").Insert(8, " ").Insert(6, "-").Insert(4, "-");
            }
            catch (Exception ex)
            {
                Console.WriteLine("toStandardTimeformat() failed!" + ex);
                return null;
            }
        }

        //from 1572312336 to DateTime
        public static DateTime GetTime(string timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));

            try
            {
                long lTime = long.Parse(timeStamp + "0000000");
                TimeSpan toNow = new TimeSpan(lTime);
                return dtStart.Add(toNow);
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetTime() failed!" + ex);
                return dtStart;
            }
        }


        //DateTime to timestamp
        public static int ConvertDateTimeInt(System.DateTime time)
        {
            try
            {
                System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
                return (int)(time - startTime).TotalSeconds;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ConvertDateTimeInt() failed!" + ex);
                return -1;
            }
        }

        //from time string like 2017-12-29 to DateTime
        public static DateTime timeStringToDateTime(string timeStr)
        {
            try
            {
                System.DateTime dt = DateTime.Parse(timeStr);
                return dt;
            }
            catch (Exception ex)
            {
                Console.WriteLine("timeStringToDateTime() failed!" + ex);
                return DateTime.Now;
            }
        }

        //from time string like 2017-12-29 to stamp
        public static int timeStringToTimeStamp(string timeStr)
        {
            try
            {
                System.DateTime dt = DateTime.Parse(timeStr);
                return ConvertDateTimeInt(dt);
            }
            catch (Exception ex)
            {
                Console.WriteLine("timeStringToTimeStamp failed!" + ex);
                return -1;
            }
        }


        //for the recent 125/270 pieces of data in column/pie screen
        public static void dataInCategory(int index, int dataNum, float[,] dataInPoint)
        {
            int i, j;
            float delta, f1, f2;
            float v;

            try
            {
                f1 = gVariable.upperLimitValueForPie[index];
                f2 = gVariable.lowerLimitValueForPie[index];

                delta = (f1 - f2) / gVariable.numOfColumns;
                for (i = 0; i < gVariable.numOfColumns; i++)
                {
                    gVariable.columnLimits[index, i] = f2 + delta * i;
                }
                gVariable.columnLimits[index, i] = f1;

                for (i = 0; i < gVariable.numOfColumns; i++)
                {
                    gVariable.columnData[index, i] = 0;
                }

                for (i = 0; i < dataNum; i++)
                {
                    v = dataInPoint[index, i];

                    for (j = 0; j < gVariable.numOfColumns; j++)
                    {
                        if (v <= gVariable.columnLimits[index, j + 1])
                        {
                            gVariable.columnData[index, j]++;
                            break;
                        }
                    }

                    if (j == gVariable.numOfColumns)  //we should come to this place, just put code here in case anything special happens
                        gVariable.columnData[index, j]++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("dataInCategory failed!" + ex);
            }
        }

/*
        public static void readGPIOStatus()
        {
            if (gVariable.currentDispatchCode == null)  //no data collect board has been connected
                gVariable.gpioStatus = 0xffff;
            else
                mySQLClass.readGPIOStatus(gVariable.currentCurveDatabaseName, gVariable.currentDispatchCode + gVariable.gpioTableNameAppendex);
        }
*/

        public static void readPointDataToArrayForPie()
        {
            int i;

            try
            {
                for (i = 0; i < gVariable.totalCurveNum[gVariable.boardIndexSelected]; i++)
                {
                    gVariable.upperLimitValueForPie[i] = -0xffff;
                    gVariable.lowerLimitValueForPie[i] = 0xffff;
                }

                mySQLClass.readSmallPartOfDataToArray(gVariable.currentCurveDatabaseName, gVariable.numOfRecordsForPie);

                for (i = 0; i < gVariable.totalCurveNum[gVariable.boardIndexSelected]; i++)
                {
                    toolClass.dataInCategory(i, gVariable.numOfPointsForPie[i], gVariable.dataInPoint);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("readPointDataToArrayForPie failed!" + ex);
            }
        }


        /*
        public static void copyDatafileToDatabase()
        {
            int i, j;
            int index;   //file name index
            int lineNum, lineLength;
            FileInfo file;
            string filePath, oneLine;
            StreamReader streamReader;
            const int lineLengthForADFile = 22;
            const int lineLengthForVolCurFile = 49;
            float dData;

            string[] dateOfPoint = new string[11110];  //testing date and time

            try
            {
                for (index = 0; index < totalFileTypeNum; index++)
                {
                    if (index < 8)
                    {
                        filePath = gVariable.dataDirPath + gVariable.filename[index];
                    }
                    else
                    {
                        filePath = gVariable.dataDirPath + gVariable.filename[8];  //for voltage/current/power data file
                    }

                    file = new FileInfo(filePath);
                    if (!file.Exists)
                    {
                        return;
                    }

                    if (index < 8)
                        lineLength = lineLengthForADFile;
                    else
                        lineLength = lineLengthForVolCurFile;

                    lineNum = (int)file.Length / lineLength;  //当前记录数量

                    i = 0;
                    dData = 0;
                    streamReader = new StreamReader(filePath, System.Text.Encoding.Default);
                    streamReader.BaseStream.Seek(dataNumForCurve[index] * lineLength, SeekOrigin.Begin);
                    while (streamReader.Peek() > -1)
                    {
                        j = dataNumForCurve[index] + i;
                        oneLine = streamReader.ReadLine().Trim();

                        if (index < 8)
                        {
                            //                            dateOfPoint[j] = oneLine.Remove(9);
                            dData = (float)Convert.ToDouble(oneLine.Remove(0, 9));
                        }
                        else if (index == 8)
                        {
                            //                            dateOfPoint[j] = oneLine.Remove(9);
                            string str = oneLine.Remove(0, 10);
                            dData = (float)Convert.ToDouble(str.Remove(8));
                        }
                        else if (index == 9)
                        {
                            //                            dateOfPoint[j] = oneLine.Remove(9);
                            string str = oneLine.Remove(0, 19);
                            dData = (float)Convert.ToDouble(str.Remove(6));
                        }
                        else if (index == 10)
                        {
                            //                            dateOfPoint[j] = oneLine.Remove(9);
                            string str = oneLine.Remove(0, 28);
                            dData = (float)Convert.ToDouble(str.Remove(6));
                        }
                        else if (index == 11)
                        {
                            //                            dateOfPoint[j] = oneLine.Remove(9);
                            dData = (float)Convert.ToDouble(oneLine.Remove(0, 37));
                        }

//                        mySQL.writeDataToTable(dataTableName[index], dData);
                        i++;
                    }
                    streamReader.Close();
                }
            }
            catch (Exception ex)
            {
                Console.Write("draw curve error :" + ex);
            }

        }
        */


        //return 0 means ping OK
        //       1 means ping fail
        public static int checkRemoteTCPClientConnection(Socket socket)
        {
            string[] strArray;
            Ping pingSender;
            PingReply reply;

            try
            {
                pingSender = new Ping();
                strArray = socket.RemoteEndPoint.ToString().Split(':');
                reply = pingSender.Send(strArray[0], 100);  //ping function should get correct response within 100ms 
                if (reply.Status == IPStatus.Success)
                    return 0;
                else
                    return 1;
            }
            catch (Exception ex)
            {
                Console.Write("Ping fail" + ex);
                Console.WriteLine(ex.ToString());
                return 1;
            }
        }

        public static void writeFileHeader()
        {
            int i;
            string logFileName;
            string dataFileName;
            string errorFileName;

            try
            {
                logFileName = "..\\..\\log\\debuginfo.log";
                gVariable.infoWriter = new StreamWriter(logFileName, true, System.Text.Encoding.ASCII); //.Default);
                gVariable.infoWriter.AutoFlush = true;
                gVariable.infoWriter.WriteLine(DateTime.Now.ToString() + "data collect server started --");

                if (gVariable.debugMode == 2)
                {
                    dataFileName = "..\\..\\log\\dataRecord";
                    errorFileName = "..\\..\\log\\errorRecord";
                    switch (gVariable.CompanyIndex)
                    {
                        case gVariable.ZIHUA_ENTERPRIZE:
                            for (i = 0; i < gVariable.machineNameArray.Length; i++)
                            {
                                gVariable.dataLogWriter[i] = new StreamWriter(dataFileName + (i + 1) + ".log", true, System.Text.Encoding.ASCII); //.Default);
                                gVariable.dataLogWriter[i].AutoFlush = true;
                                gVariable.dataLogWriter[i].WriteLine(DateTime.Now.ToString() + "data collect server started --");

                                gVariable.errorLogWriter[i] = new StreamWriter(errorFileName + (i + 1) + ".log", true, System.Text.Encoding.ASCII); //.Default);
                                gVariable.errorLogWriter[i].AutoFlush = true;
                                gVariable.errorLogWriter[i].WriteLine(DateTime.Now.ToString() + "data collect server started --");
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                gVariable.infoWriter.WriteLine("open log fail !" + ex);
            }
        }
 
        public static void writeFileFooter()
        {
            int i;

            try
            {
                if (gVariable.debugMode == 2)
                {
                    switch (gVariable.CompanyIndex)
                    {
                        case gVariable.ZIHUA_ENTERPRIZE:
                            for (i = 0; i < gVariable.machineNameArray.Length; i++)
                            {
                                gVariable.dataLogWriter[i].Write(DateTime.Now.ToString());
                                gVariable.dataLogWriter[i].WriteLine(" ---- End of communication");
                                gVariable.dataLogWriter[i].Close();

                                gVariable.errorLogWriter[i].Write(DateTime.Now.ToString());
                                gVariable.errorLogWriter[i].WriteLine(" ---- End of communication");
                                gVariable.errorLogWriter[i].Close();
                            }
                            break;
                    }
                }
                gVariable.infoWriter.Close();
                gVariable.willClose = 0;
            }
            catch (Exception ex)
            {
                Console.Write("Save log file fail!" + ex);
            }
        }

        // all kinds of alarms will all go to this function, it will put alarm index in ring buffer so firstScreen has a chance to pop up a alarm message
        //this alarm could be a new alarm(that is already saved in alarm table), or an old alarm inside alarm list 
        // databaseName is the database name for a certain machine
        //alarmIDInMachineAlarmList means the index of this alarm in the list
        public static void processNewAlarm(string databaseName, int alarmIDInMachineAlarmList)
        {
            string tableName;

            gVariable.alarmTableStruct alarmTableStructImpl;

            //multi thread lock
            gVariable.AlarmStatusChangeMutex.WaitOne();

            try
            {
                //already 1000 alarms on screen? Stop pop up anything!
                if (gVariable.activeAlarmTotalNumber >= gVariable.maxActiveAlarmNum)
                {
                }
                else
                {
                    tableName = gVariable.alarmListTableName;
                    //this id means the index for this alarm in current machine's alarm table
                    alarmTableStructImpl = mySQLClass.getAlarmTableContent(databaseName, tableName, alarmIDInMachineAlarmList);

                    //we support 1000 alarms alive at the same time, recorded in the buffer of activeAlarmInstanceArray separately
                    SetAlarmClass.activeAlarmInstanceArray[gVariable.activeAlarmTotalNumber] = new SetAlarmClass(databaseName, alarmTableStructImpl, alarmIDInMachineAlarmList);

                    //gVariable.activeAlarmTotalNumber is the same as the index for next active alarm
                    gVariable.activeAlarmDatabaseNameArray[gVariable.activeAlarmTotalNumber] = databaseName;
                    gVariable.activeAlarmIDArray[gVariable.activeAlarmTotalNumber] = alarmIDInMachineAlarmList;
                    gVariable.activeAlarmNewStatus[gVariable.activeAlarmTotalNumber] = gVariable.ACTIVE_ALARM_NEW_ARRIVED;  //this is a new arrived alarm, client PC need to popup this alarm

                    gVariable.activeAlarmTotalNumber++;
                    gVariable.activeAlarmInfoUpdatedLocally = 1;
                    if (gVariable.thisIsHostPC == true)  //this is a host server, we need to inform all our client PCs about this
                        gVariable.activeAlarmInfoUpdatedCounterpart = 1;
                }
            }
            catch (Exception ex)
            {
                Console.Write("processNewAlarm error for " + databaseName + ", id = " + alarmIDInMachineAlarmList+ ex);
            }

            //alarm status change OK, release mutex to avoid sharing violation
            gVariable.AlarmStatusChangeMutex.ReleaseMutex();
        }

        public static int getIndexInActiveAlarmArray(string alarmDatabaseName, int id)
        {
            int i;

            for (i = 0; i < gVariable.activeAlarmTotalNumber; i++)
            {
                if (gVariable.activeAlarmDatabaseNameArray[i] == alarmDatabaseName && gVariable.activeAlarmIDArray[i] == id)
                    break;
            }

            if (i >= gVariable.activeAlarmTotalNumber)
                return -1;
            else
                return i;
        }


        public static void activeAlarmClosed(string databaseName, int alarmIDInMachineAlarmList)
        {
            int i, j;

            //multi thread lock
            gVariable.AlarmStatusChangeMutex.WaitOne();

            try
            {
                for (j = 0; j < gVariable.activeAlarmTotalNumber; j++)
                {
                    if (gVariable.activeAlarmDatabaseNameArray[j] == databaseName && gVariable.activeAlarmIDArray[j] == alarmIDInMachineAlarmList)
                        break;
                }

                if (j >= gVariable.activeAlarmTotalNumber)
                {
                    //this alarm is not displayed on screen, do nothing
                }
                else
                {
                    //move alarms after the closed alarm one step ahead
                    for (i = j; i < gVariable.activeAlarmTotalNumber - 1; i++)
                    {
                        gVariable.activeAlarmDatabaseNameArray[i] = gVariable.activeAlarmDatabaseNameArray[i + 1];
                        gVariable.activeAlarmIDArray[i] = gVariable.activeAlarmIDArray[i + 1];
                        SetAlarmClass.activeAlarmInstanceArray[i] = SetAlarmClass.activeAlarmInstanceArray[i + 1];
                    }

                    gVariable.activeAlarmTotalNumber--;
                }
            }
            catch (Exception ex)
            {
                Console.Write("activeAlarmClosed error for " + databaseName + ", id = " + alarmIDInMachineAlarmList + ex);
            }

            //active alarm closing OK, release mutex to avoid sharing violation
            gVariable.AlarmStatusChangeMutex.ReleaseMutex();
        }

        public static int getBoardIndexByDatabaseName(string dName)
        {
            try
            {
                return Convert.ToInt32(dName.Remove(0, 1)) - 1;
            }
            catch (Exception ex)
            {
                Console.WriteLine("getBoardIndexByDatabaseName failed!" + ex);
                return 1;
            }
        }

        public static string getDatabaseNameByMachineCode(string machineCode)
        {
            int i;

            try
            {
                for (i = 0; i < gVariable.machineCodeArray.Length; i++)
                {
                    if (machineCode == gVariable.machineCodeArray[i])
                    {
                        return gVariable.DBHeadString + (i + 1).ToString().PadLeft(3, '0');
                    }
                }
                MessageBox.Show("设备编码" + machineCode + "无效", "信息提示", MessageBoxButtons.OK);
            }
            catch (Exception ex)
            {
                Console.WriteLine("getBoardIndexByDatabaseName failed!" + ex);
            }
            return null;
        }

        //read data from excel
        //input:  fileUrl: file url
        //return: DataTable
        public static DataTable readExcelToDataTable(string fileUrl)
        {
            //            const string cmdText = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties='Excel 8.0;IMEX=1';";
            //suport both xls and xlsx, HDR=Yes means first line is titel not data
            const string cmdText = "Provider=Microsoft.Ace.OleDb.12.0;Data Source={0};Extended Properties='Excel 12.0; HDR=No'";

            DataTable dt = null;
            OleDbDataAdapter da;
            OleDbConnection conn;

            conn = new OleDbConnection(string.Format(cmdText, fileUrl));
            try
            {
                if (conn.State == ConnectionState.Broken || conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                System.Data.DataTable schemaTable = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

                string sheetName = schemaTable.Rows[0]["TABLE_NAME"].ToString().Trim();

                string strSql = "select * from [" + sheetName + "]";
                da = new OleDbDataAdapter(strSql, conn);
                DataSet ds = new DataSet();
                da.Fill(ds);
                dt = ds.Tables[0];
                da.Dispose();

                return dt;
            }
            catch (Exception ex)
            {
                gVariable.infoWriter.WriteLine("readExcelToDataTable() fail:" + ex);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
            return null;
        }


        //for a quality/craft table, how many data items exits inside one dispatch and also the spec limits for the items
        //for a quality table, first column is ID, second is timer, then data, data1 is thickness, data2 is weight, data3 is length, altogether 3 data, return value is 3
        //return valure means the number data items for a quality table, data1/data2/dat3/data4/data5, altogether 5 items
        public static int getCraftQualityData(string databaseName, string tableName, string dispatchCode, float[] specUpperLimitArray, float[] specLowerLimitArray)
        {
            int i;
            int index, nextRecordIndex;
            string[] qualityRecordArray = new string[50];

            i = 0;
            try
            {
                index = 0;
                while (true)
                {
                    //get spec limits for this data item, these data will be used for SPC calculation, 
                    //this function get whole record in quality list by the input dispatch code
                    nextRecordIndex = mySQLClass.getNextRecordByOneStrColumn(databaseName, tableName, "dispatchCode", dispatchCode, index, qualityRecordArray);
                    if (nextRecordIndex == -1)
                        break;

                    if (tableName == gVariable.qualityListTableName)
                    {
                        specLowerLimitArray[i] = (float)Convert.ToDouble(qualityRecordArray[5]);
                        specUpperLimitArray[i] = (float)Convert.ToDouble(qualityRecordArray[8]);
                    }
                    else  //if(tableName == gVariable.craftListTableName)
                    {
                        specLowerLimitArray[i] = (float)Convert.ToDouble(qualityRecordArray[2]);
                        specUpperLimitArray[i] = (float)Convert.ToDouble(qualityRecordArray[3]);
                    }
                    index = nextRecordIndex;
                    i++;
                }
            }
            catch (Exception ex)
            {
                Console.Write("getCurveInfoIngVariable() failed with exception: " + ex);
            }
            return i;
        }

        //get chart type for this quality data, indexintable means which item we care about in quality data
        public static int getQualityDataChartType(string databaseName, string tableName, string dispatchCode, int indexInTable)
        {
            int ret;
            int index, nextRecordIndex;
            string[] qualityRecordArray = new string[50];

            ret = 0;
            try
            {
                index = 0;            //means quality data item index in quality data table starting from the first data item for the current didpatch 
                nextRecordIndex = 0;  //means quality data item index in quality data table starting from the first dispatch 
                while (true)
                {
                    //this function get whole record in quality list by the input dispatch code
                    nextRecordIndex = mySQLClass.getNextRecordByOneStrColumn(databaseName, tableName, "dispatchCode", dispatchCode, nextRecordIndex, qualityRecordArray);
                    if (nextRecordIndex == -1)
                    {
                        ret = -1;
                        break;
                    }

                    if (index == indexInTable)
                    {
                        ret = Convert.ToInt16(qualityRecordArray[mySQLClass.QUALITY_LIST_ID_CHART_TYPE]);
                        break;
                    }

                    index++;
                }
            }
            catch (Exception ex)
            {
                Console.Write("getCurveInfoIngVariable() failed with exception: " + ex);
            }
            return ret;
        }

        public static DataTable getDispatchListFromDatabase(string databaseName, string tableName)
        {
            string commandText;

            try
            {
                commandText = "select * from `" + tableName + "`";
                return mySQLClass.queryDataTableAction(databaseName, commandText, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine("getDispatchListFromDatabase failed! ", ex);
                return null;
            }
        }

        //get quality data item list which inludes all items in a quality table
        public static int getQualityDataItemByDispatch(string databaseName, string tableName, string dispatchCode, string[] dataItemListArray)
        {
            int index, nextRecordIndex;
            string[] qualityRecordArray = new string[50];

            index = 0; //means quality data item index in quality data table starting from the first data item for the current didpatch 
            try
            {
                nextRecordIndex = 0;  //means quality data item index in quality data table starting from the first dispatch 
                while (true)
                {
                    //this function get whole record in quality list by the input dispatch code
                    nextRecordIndex = mySQLClass.getNextRecordByOneStrColumn(databaseName, tableName, "dispatchCode", dispatchCode, nextRecordIndex, qualityRecordArray);
                    if (nextRecordIndex == -1)
                    {
                        break;
                    }

                    dataItemListArray[index] = qualityRecordArray[mySQLClass.QUALITY_LIST_ID_ITEM_NAME];
                    index++;
                }
            }
            catch (Exception ex)
            {
                index = -1;
                Console.Write("getCurveInfoIngVariable() failed with exception: " + ex);
            }
            return index;
        }

        //MD5 encription, so that password won't be visible to administrator of this program
        public static string MD5Encrypt(string strPassword)
        {
            MD5 md5;
            byte[] result;

            try
            {
                md5 = new MD5CryptoServiceProvider();
                result = md5.ComputeHash(System.Text.Encoding.Default.GetBytes(strPassword));
                return System.Text.Encoding.Default.GetString(result);
            }
            catch (Exception ex)
            {
                Console.Write("MD5Encrypt() failed! " + ex);
                return null;
            }

        }

        //get all information about this user account, could be used to confirm whether this user has input a correct password
        public static DataTable getDataTableForAccount(string userAccount)
        {
            int flag;
            DataTable dt;
            string commandText;

            dt = null;
            try
            {
                commandText = "select * from `" + gVariable.employeeTableName + "` where workerID = " + "\'" + userAccount + "\'";
                dt = mySQLClass.queryDataTableAction(gVariable.basicInfoDatabaseName, commandText, null);

                flag = 0;
                foreach (DataRow dr in dt.Rows)
                {
                    flag = 1;
                    break;
                }

                //workID not found, so try name
                if (flag == 0)
                {
                    commandText = "select * from `" + gVariable.employeeTableName + "` where name = " + "\'" + userAccount + "\'";
                    dt = mySQLClass.queryDataTableAction(gVariable.basicInfoDatabaseName, commandText, null);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("getDataTableForAccount failed!" + ex);
            }
            return dt;
        }

        public static string getCurveInfoIngVariable(string databaseName, int myBoardIndex, int whoReadMe)
        {
            int i; //, num;
            int index, nextRecordIndex;
            int curveIndex, curveIndexPre;
            string dispatchCode;
            string commandText;
            string[] recordArray = new string[100];
            string[] titleVolCur = { "C相电压", "主轴相电流", "所有相总功率", "功率因数" };
            string[] unitVolCur = { "V", "A", "kW", "%" };
            float[] upperLimitVolCur = { 390, 40.0f, 32, 55 };
            float[] lowerLimitVolCur = { 370, 20.0f, 7, 45 };

            dispatchCode = "无工单";

            try
            {
                index = 0;
                curveIndex = 0;
                gVariable.totalCurveNum[myBoardIndex] = 0;

                //we use newest dispatch
                if (whoReadMe == gVariable.CURRENT_READING)
                {
                    //if dispatch already started, all dispatch/quality/craft/material info are put in gVariables, so don't need to read anything
                    if (gVariable.machineCurrentStatus[myBoardIndex] >= gVariable.MACHINE_STATUS_DISPATCH_APPLIED) //there is no dummy dispatch, so real dispatch is active, try to find it
                    {
                        //get newest dispatch in dispathList Table
                        commandText = "select * from `" + gVariable.dispatchListTableName + "` where status = '" + gVariable.MACHINE_STATUS_DISPATCH_APPLIED + "' order by id desc";
                        dispatchCode = mySQLClass.getColumnInfoByCommandText(databaseName, gVariable.dispatchListTableName, commandText, mySQLClass.DISPATCH_CODE_IN_DISPATCHLIST_DATABASE);  
                         if (dispatchCode == null)  //there is no new dispatch, so we keep in dummy mode
                            dispatchCode = gVariable.dummyDispatchTableName;
                    }
                    else
                    {
                        dispatchCode = gVariable.dummyDispatchTableName;
                    }
                }
                else
                {
                    //we are review old dispatch data, but maybe current dispatch is still undergoing, so don' change table name for craft/quality/volcur/beat
                    dispatchCode = gVariable.dispatchUnderReview;
                }

                //for craft
                while (true)
                {
                    nextRecordIndex = mySQLClass.getNextRecordByOneStrColumn(databaseName, gVariable.craftListTableName, "dispatchCode", dispatchCode, index, recordArray);
                    if (nextRecordIndex == -1)
                        break;

                    //dispatchBasedPortTableName is used when check database table name by port index, its index is port index
                    if (databaseName == gVariable.currentCurveDatabaseName)
                    {
                        gVariable.curveOrNot[curveIndex] = 1;
                        //all craft data will share one craft table name
                        gVariable.dispatchBasedCurveTableName[curveIndex] = dispatchCode + gVariable.craftTableNameAppendex;
                    }

                    //recordArray[0] is dispatch code, 
                    gVariable.curveTitle[curveIndex] = recordArray[1] + "(" + recordArray[5] + ")";  //title of this curve + ( unit )
                    gVariable.curveUpperLimit[curveIndex] = (float)Convert.ToDouble(recordArray[3]);  // limits for the curve
                    gVariable.curveLowerLimit[curveIndex] = (float)Convert.ToDouble(recordArray[2]);  //
                    curveIndex++;

                    index = nextRecordIndex;
                }
                if (databaseName == gVariable.currentCurveDatabaseName)
                    gVariable.numOfCurveForOneType[gVariable.CRAFT_DATA_IN_DATABASE] = curveIndex;

                //for voltage/current
                for (index = 0; index < 4; index++)
                {
                    //dispatchBasedPortTableName is used when check database table name by port index
                    if (databaseName == gVariable.currentCurveDatabaseName)
                    {
                        gVariable.curveOrNot[curveIndex] = 1;
                        //all vol/cur/power/factor data share one table name
                        gVariable.dispatchBasedCurveTableName[curveIndex] = dispatchCode + gVariable.volcurTableNameAppendex;
                    }

                    gVariable.curveTitle[curveIndex] = titleVolCur[index] + "(" + unitVolCur[index] + ")";  //title of this curve + (unit)
                    //all 0 means no limit
                    gVariable.curveUpperLimit[curveIndex] = upperLimitVolCur[index];  // limits for the curve
                    gVariable.curveLowerLimit[curveIndex] = lowerLimitVolCur[index];  //
                    curveIndex++;
                }
                if (databaseName == gVariable.currentCurveDatabaseName)
                    gVariable.numOfCurveForOneType[gVariable.VOLCUR_DATA_IN_DATABASE] = 4;

                //for beat
                index = 0;
                {
                    if (databaseName == gVariable.currentCurveDatabaseName)   //only when the current board is selected in room function, will these data be displayed on screen
                    {
                        gVariable.curveOrNot[curveIndex] = 1;
                        gVariable.dispatchBasedCurveTableName[curveIndex] = dispatchCode + gVariable.beatTableNameAppendex;
                    }

                    //recordArray[0] is dispatch code, 
                    gVariable.curveTitle[curveIndex] = "生产节拍(秒)";
                    gVariable.curveUpperLimit[curveIndex] = 0;
                    gVariable.curveLowerLimit[curveIndex] = 0;
                    curveIndex++;
                }
                if (databaseName == gVariable.currentCurveDatabaseName)
                    gVariable.numOfCurveForOneType[gVariable.BEAT_DATA_IN_DATABASE] = 1;

                curveIndexPre = curveIndex;

                //for quality
                index = 0;
                while (true)
                {
                    nextRecordIndex = mySQLClass.getNextRecordByOneStrColumn(databaseName, gVariable.qualityListTableName, "dispatchCode", dispatchCode, index, recordArray);
                    if (nextRecordIndex == -1)
                        break;

                    if (databaseName == gVariable.currentCurveDatabaseName)   //only when the current board is selected in room function, will these data be displayed on screen
                    {
                        gVariable.curveOrNot[curveIndex] = 1;
                        //all quality data share one table name
                        gVariable.dispatchBasedCurveTableName[curveIndex] = dispatchCode + gVariable.qualityTableNameAppendex;
                    }

                    //recordArray[0] is dispatch code, 
                    gVariable.curveTitle[curveIndex] = recordArray[1];  //title of this curve
                    gVariable.curveUpperLimit[curveIndex] = (float)Convert.ToDouble(recordArray[6]);  // limits for the curve
                    gVariable.curveLowerLimit[curveIndex] = (float)Convert.ToDouble(recordArray[4]);  //
                    curveIndex++;

                    index = nextRecordIndex;
                }
                if (databaseName == gVariable.currentCurveDatabaseName)
                    gVariable.numOfCurveForOneType[gVariable.QUALITY_DATA_IN_DATABASE] = curveIndex - curveIndexPre;

                if (databaseName == gVariable.currentCurveDatabaseName)   //only when the current board is selected in room function, will these data be displayed on screen
                    gVariable.totalCurveNum[myBoardIndex] = curveIndex;

                for (i = 0; i < curveIndex; i++)
                    gVariable.curveTextArray[i] = "0";
            }
            catch (Exception ex)
            {
                Console.Write("getCurveInfoIngVariable() failed with exception: " + ex);
            }
            return dispatchCode;
        }


        //dummy data means there is no dispatch available, this machine is in idle mode, and voltage/current value, craft value are still sending from boards, so we assume a dummy dispatch
        public static void getDummyData(int boardIndex)
        {
            int i;
            string dataNum;
            string space;
            string[] filePath = { "..\\..\\data\\craftDataDesc23.txt", "..\\..\\data\\craftDataDescZihua.txt", "..\\..\\data\\craftDataDesc20.txt" };
            StreamReader streamReader;

            try
            {
                streamReader = new StreamReader(filePath[gVariable.CompanyIndex], System.Text.Encoding.Default);
                dataNum = streamReader.ReadLine().Trim();

                gVariable.dispatchSheet[boardIndex].machineID = (boardIndex + 1).ToString();
                gVariable.dispatchSheet[boardIndex].dispatchCode = gVariable.dummyDispatchTableName;
                gVariable.dispatchSheet[boardIndex].planTime1 = "未定义";
                gVariable.dispatchSheet[boardIndex].planTime2 = "未定义";
                gVariable.dispatchSheet[boardIndex].productCode = "未定义";
                gVariable.dispatchSheet[boardIndex].productName = "未定义";
                gVariable.dispatchSheet[boardIndex].operatorName = "未定义";
                gVariable.dispatchSheet[boardIndex].plannedNumber = 0;
                gVariable.dispatchSheet[boardIndex].outputNumber = 0;
                gVariable.dispatchSheet[boardIndex].processName = "未定义";
                gVariable.dispatchSheet[boardIndex].realStartTime = "未定义";

                //input default craft data
                gVariable.craftList[boardIndex].paramNumber = dummy_craftNum;
                for (i = 0; i < dummy_craftNum; i++)
                {
                    space = streamReader.ReadLine();  //empty line
                    gVariable.craftList[boardIndex].paramName[i] = "ADC" + (i + 1) + "通道";
                    gVariable.craftList[boardIndex].paramUnit[i] = " ";
                    streamReader.ReadLine().Trim();  //name of the parameter
                    gVariable.craftList[boardIndex].paramLowerLimit[i] = (float)Convert.ToDouble(streamReader.ReadLine().Trim()); //spec low thresh
                    gVariable.craftList[boardIndex].paramUpperLimit[i] = (float)Convert.ToDouble(streamReader.ReadLine().Trim());  //spec high thresh
                    gVariable.craftList[boardIndex].rangeLowerLimit[i] = (float)Convert.ToDouble(streamReader.ReadLine().Trim()); //range low thresh
                    gVariable.craftList[boardIndex].rangeUpperLimit[i] = (float)Convert.ToDouble(streamReader.ReadLine().Trim());  //range high thresh
                    streamReader.ReadLine().Trim();  //name of the parameter
                }

                //input default quality data
                gVariable.qualityList[boardIndex].checkItemNumber = dummy_qualityNum;
                for (i = 0; i < dummy_qualityNum; i++)
                {
                    gVariable.qualityList[boardIndex].checkItem[i] = "模拟检测数据 " + (i + 1);
                    gVariable.qualityList[boardIndex].checkRequirement[i] = "模拟检测需求 " + (i + 1);
                    gVariable.qualityList[boardIndex].specLowerLimit[i] = 0;
                    gVariable.qualityList[boardIndex].controlLowerLimit1[i] = 0f;
                    gVariable.qualityList[boardIndex].specUpperLimit[i] = 3.0f;
                    gVariable.qualityList[boardIndex].controlUpperLimit1[i] = 0f;
                    gVariable.qualityList[boardIndex].sampleRatio[i] = 3;
                    gVariable.qualityList[boardIndex].checkResultData[i] = "0";
                    gVariable.qualityList[boardIndex].chartType[i] = i % 2 + 1;  //  1 or 2, Xbar-s or C chart
                }

                gVariable.machineStatus[boardIndex].machineCode = gVariable.machineCodeArray[boardIndex];
                gVariable.machineStatus[boardIndex].machineName = gVariable.machineNameArray[boardIndex];
                gVariable.machineStatus[boardIndex].totalWorkingTime = 0;
                gVariable.machineStatus[boardIndex].productBeat = 0;
                gVariable.machineStatus[boardIndex].powerConsumed = 0;
                gVariable.machineStatus[boardIndex].standbyTime = 0;
                gVariable.machineStatus[boardIndex].power = 0;
                gVariable.machineStatus[boardIndex].collectedNumber = 0;
                gVariable.machineStatus[boardIndex].revolution = 0;
                gVariable.machineStatus[boardIndex].prepareTime = 0;
                gVariable.machineStatus[boardIndex].workingTime = 0;

                gVariable.materialList[boardIndex].numberOfTypes = dummy_materialnumberOfTypes;
                gVariable.materialList[boardIndex].dispatchCode = gVariable.dispatchSheet[boardIndex].dispatchCode;
                gVariable.materialList[boardIndex].machineCode = gVariable.machineStatus[boardIndex].machineCode;
                for (i = 0; i < dummy_materialnumberOfTypes; i++)
                {
                    gVariable.materialList[boardIndex].materialCode[i] = "j.345-" + (i + 1);
                    gVariable.materialList[boardIndex].materialRequired[i] = (i + 5) * 3;
                }

                streamReader.Close();
            }
            catch (Exception ex)
            {
                Console.Write("getDummyData() error: " + ex);
            }
        }

        //dispatchIndex is a index number that will keep on increasing after a dispatch is generated
        public static string generatingNewDispatch(int dispatchType, int boardIndex, int dispatchIndex)
        {
            string dataNew;
            string prefix;
            string dispatchStr;

            switch(dispatchType)
            {
                case gVariable.DISPATCH_TYPE_PRODUCTION:
                    dispatchStr = gVariable.machineCodeArray[boardIndex] + "_" + dispatchIndex.ToString().PadLeft(4, '0');
                    return dispatchStr;
                case gVariable.DISPATCH_TYPE_ROUTINE_CHECK:
                    prefix = "R";
                    break;
                case gVariable.DISPATCH_TYPE_ADD_OIL:
                    prefix = "A";
                    break;
                case gVariable.DISPATCH_TYPE_WASHUP:
                    prefix = "W";
                    break;
                case gVariable.DISPATCH_TYPE_MAINTENANCE:
                    prefix = "M";
                    break;
                case gVariable.DISPATCH_TYPE_REPAIR:
                    prefix = "R";
                    break;
                default:
                    dispatchStr = null;
                    return dispatchStr;
            }

            dataNew = DateTime.Now.Date.ToString("yyMMdd");

            if(dataNew != dataOld)
            {
                dataOld = dataNew;
                dispatchIndexForToday = 0;
            }
            dispatchStr = prefix + DateTime.Now.Date.ToString("yyMMdd") + dispatchIndexForToday.ToString().PadLeft(4, '0');

            return dispatchStr;
        }

        public static string[] getCustomerList(int addAllItem)
        {
            string addItem;

            if (addAllItem == 1)
                addItem = "全部客户";
            else
                addItem = null;

            return getArrayFromDatabase(gVariable.basicInfoDatabaseName, gVariable.customerListTableName, 2, addItem);  //2 is the index for customer name in this table
        }

        public static string [] getMachineList(int addAllItem)
        {
            string addItem;

            if (addAllItem == 1)
                addItem = "全部设备";
            else
                addItem = null;

            return getArrayFromDatabase(gVariable.basicInfoDatabaseName, gVariable.machineTableName, 3, addItem);  //3 is the index for customer name in this table
        }

        public static void getWorkerNameAndIDArray()
        {
            int i;
            int len;
            string commandText;
            string[,] tableArray;

            try
            {
                //rank = 4 means the frontline worker 
                commandText = "select * from `" + gVariable.employeeTableName + "`";

                //get machine info
                tableArray = mySQLClass.databaseCommonReading(gVariable.basicInfoDatabaseName, commandText);
                if (tableArray == null)
                {
                    MessageBox.Show("读取 MES 基础数据中的员工列表出错", "提示信息", MessageBoxButtons.OK);
                    return;
                }

                len = tableArray.GetLength(0);
                employeeNameArray = new string[len];
                employeeIDArray = new string[len];

                for (i = 0; i < len; i++)
                {
                    employeeNameArray[i] = tableArray[i, mySQLClass.EMPLOYEE_NAME_COLIUMN];
                    employeeIDArray[i] = tableArray[i, mySQLClass.EMPLOYEE_WORKERID_COLIUMN];
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("getWorkerNameAndIDArray() failed!" + ex);
            }
        }

        public static string getNameByIDAndIDByName(string name, string ID)
        {
            int i;

            if (name == null && ID == null)
            {
                return null;
            }
            else if (name == null && ID != null)
            {
                for(i = 0; i < employeeIDArray.Length; i++)
                {
                    if (employeeIDArray[i] == ID)
                        return employeeNameArray[i];
                }
                if (ID == "0990")
                    return "机器人#2";
            }
            else if (name != null && ID == null)
            {
                for (i = 0; i < employeeIDArray.Length; i++)
                {
                    if (employeeNameArray[i] == name)
                        return employeeIDArray[i];
                }
            }
            else
            {
                Console.WriteLine("If we want to get name by ID or get ID byt name, we sould put name string and clear ID string or verse versa");
            }

            return null;
        }

        public static string[] getWorkerInfoList(int addAllItem, int workerID)
        {
            int i;
            int len;
            int index;
            string addItem;
            string commandText;
            string[] outputArray;
            string[,] tableArray;

            if (addAllItem == 1)
                addItem = "未选择";
            else
                addItem = null;

            outputArray = null;

            if (workerID == 0)
                index = 2;  //2 is name in database
            else
                index = 1;  //1 is ID in database

            try
            {
                //rank = 4 means the frontline worker 
                commandText = "select * from `" + gVariable.employeeTableName + "` where rank = '4'";

                //get machine info
                tableArray = mySQLClass.databaseCommonReading(gVariable.basicInfoDatabaseName, commandText);
                if (tableArray == null)
                {
                    MessageBox.Show("读取 MES 基础数据中的员工表出错", "提示信息", MessageBoxButtons.OK);
                    return null;
                }

                if (addItem == null)
                {
                    len = tableArray.GetLength(0);
                    outputArray = new string[len];

                    for (i = 0; i < len; i++)
                        outputArray[i] = tableArray[i, index];
                }
                else
                {
                    len = tableArray.GetLength(0) + 1;
                    outputArray = new string[len];
                    outputArray[0] = addItem;

                    for (i = 0; i < len - 1; i++)
                        outputArray[i + 1] = tableArray[i, index];
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("getWorkerInfoList() failed!" + ex);
            }

            return outputArray;
        }

        public static string[] getVendorList(int addAllItem)
        {
            string addItem;

            if (addAllItem == 1)
                addItem = "全部供应商";
            else
                addItem = null;

            return getArrayFromDatabase(gVariable.basicInfoDatabaseName, gVariable.vendorListTableName, 2, addItem);  //2 is the index for customer name in this table
        }

        public static string[] getArrayFromDatabase(string databaseName, string tableName, int tableIndex, string addItem)
        {
            int i;
            int len;
            string commandText;
            string [] outputArray;
            string[,] tableArray;

            outputArray = null;
            try
            {
                commandText = "select * from `" + tableName + "`";

                //get machine info
                tableArray = mySQLClass.databaseCommonReading(databaseName, commandText);
                if (tableArray == null)
                {
                    MessageBox.Show("读取 MES 基础数据中的设备列表出错", "提示信息", MessageBoxButtons.OK);
                    return null;
                }

                if (addItem == null)
                {
                    len = tableArray.GetLength(0);
                    outputArray = new string[len];

                    for (i = 0; i < len; i++)
                        outputArray[i] = tableArray[i, tableIndex];
                }
                else
                {
                    len = tableArray.GetLength(0) + 1;
                    outputArray = new string[len];
                    outputArray[0] = addItem;

                    for (i = 0; i < len - 1; i++)
                        outputArray[i + 1] = tableArray[i, tableIndex];
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("getArrayFromBasicInfo() failed from database " + databaseName + " table " + tableName + "!" + ex);
            }

            return outputArray;
        }


        //strArray contains the original string array
        //indexArray contains the index value which is in accordance with strArray
        //after soring for strArray, we need to make sure index array also re-ordered according to strArray contents
        public static void stringSortingRecordIndex(string[] strArray, int[] indexArray)
        {
            int i, j;
            int left;
            int right;
            int mid;
            int indexValue;
            string strValue;

            for (i = 1; i < strArray.Length; i++)
            {
                    left = 0;
                    right = i - 1;
                    mid = 0; //insert position

                    strValue = strArray[i];
                    indexValue = indexArray[i];

                    while (left <= right)
                    {
                        mid = (left + right) / 2;
                        if (strValue.CompareTo(strArray[mid]) < 0)
                        {
                            right = mid - 1;
                        }
                        else
                        {
                            left = mid + 1;
                        }
                    }

                    //move all the data after this postion one step forward, then insert new data
                    for (j = i; j > mid; j--) //previous data are already sorted
                    {
                        strArray[j] = strArray[j - 1];
                        indexArray[j] = indexArray[j - 1];
                    }
                    strArray[left] = strValue;
                    indexArray[left] = indexValue;
            }
        }

        //integerArray contains the original integer
        //indexArray contains the index value which is in accordance with strArray
        //after soring for strArray, we need to make sure index array also re-ordered according to strArray contents
        public static void integerSortingRecordIndex(int[] integerArray, int[] indexArray)
        {
            int i, j;
            int left;
            int right;
            int mid;
            int indexValue;
            int intValue;

            for (i = 1; i < integerArray.Length; i++)
            {
                left = 0;
                right = i - 1;
                mid = 0; //insert position

                intValue = integerArray[i];
                indexValue = indexArray[i];

                while (left <= right)
                {
                    mid = (left + right) / 2;
                    if (intValue < integerArray[mid])
                    {
                        right = mid - 1;
                    }
                    else
                    {
                        left = mid + 1;
                    }
                }

                //move all the data after this postion one step forward, then insert new data
                for (j = i; j > mid; j--) //previous data are already sorted
                {
                    integerArray[j] = integerArray[j - 1];
                    indexArray[j] = indexArray[j - 1];
                }
                integerArray[left] = intValue;
                indexArray[left] = indexValue;
            }
        }

        public static void stringSorting(string[] strArray)
        {
            int i, j;
            int left;
            int right;
            int mid;
            string strValue;

            for (i = 1; i < strArray.Length; i++)
            {
                left = 0;
                right = i - 1;
                mid = 0; //insert position

                strValue = strArray[i];

                while (left <= right)
                {
                    mid = (left + right) / 2;
                    if (strValue.CompareTo(strArray[mid]) < 0)
                    {
                        right = mid - 1;
                    }
                    else
                    {
                        left = mid + 1;
                    }
                }

                //move all the data after this postion one step forward, then insert new data
                for (j = i; j > mid; j--) //previous data are already sorted
                {
                    strArray[j] = strArray[j - 1];
                }
                strArray[left] = strValue;
            }
        }

        static void integerSorting(int[] dataArray)
        {
            int i, j;
            int left;
            int right;
            int mid;
            int iValue;

            for (i = 1; i < dataArray.Length; i++)
            {
                left = 0;
                right = i - 1;
                mid = 0; //insert position
                iValue = dataArray[i];

                while (left <= right)
                {
                    mid = (left + right) / 2;
                    if (iValue < dataArray[mid])
                    {
                        right = mid - 1;
                    }
                    else
                    {
                        left = mid + 1;
                    }
                }
                for (j = i; j > mid; j--) //previous data are already sorted
                {
                    dataArray[j] = dataArray[j - 1];
                }
                dataArray[left] = iValue;
            }
        }

        static public void getMachineInfoFromDatabase()
        {
            int i;
            int len;
            string commandText;
            string[,] tableArray;

            try
            {
                commandText = "select * from `" + gVariable.machineTableName + "`";

                //get machine info
                tableArray = mySQLClass.databaseCommonReading(gVariable.basicInfoDatabaseName, commandText);
                if (tableArray == null)
                {
                    MessageBox.Show("读取 MES 基础数据中的设备列表出错", "提示信息", MessageBoxButtons.OK);
                    return;
                }

                len = tableArray.GetLength(0);
                gVariable.maxMachineNum = len;

                gVariable.machineNameArray = new string[len];
                gVariable.allMachineWorkshopForZihua = new string[len];
                gVariable.machineCodeArray = new string[len];

                for (i = 0; i < len; i++)
                {
                    gVariable.machineNameArray[i] = tableArray[i, 3];
                    gVariable.allMachineWorkshopForZihua[i] = tableArray[i, 13];
                    gVariable.machineCodeArray[i] = tableArray[i, 2];
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("getMachineInfoFromDatabase() failed!" + ex);
            }
        }
    
    }
}
