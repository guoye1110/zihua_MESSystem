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

namespace MESSystem.latheRelated
{
    public class fanucClass
    {
       
        public static ushort handleFanuc;

        public static void connectToFanucDevice(string ip, string port, int timeout)
        {
            int ret;
            int status;
            Focas1.ODBST fanbuf; // CNC Status

            ret = Focas1.cnc_allclibhndl3(ip, Convert.ToUInt16(port), timeout, out handleFanuc);
            if (ret != Focas1.EW_OK)
            {
                Console.WriteLine("Fanuc lathe connection error£¡");
                return;
            }

            //coordination of the arm
            Focas1.ODBAXIS odbaxis = new Focas1.ODBAXIS();
            for (short i = 0; i < 3; i++)
            {
                ret = Focas1.cnc_machine(handleFanuc, (short)(i + 1), 8, odbaxis);
                Console.WriteLine(odbaxis.data[0] * Math.Pow(10, -4));
            }

            fanbuf = new Focas1.ODBST();
    		ret = Focas1.cnc_statinfo(handleFanuc, fanbuf);
            if (ret == Focas1.EW_OK)
            {
                status = fanbuf.run;

                switch (status)
                {
                    case 0:
                        Console.WriteLine("Fanuc lathe in shutdown mode£¡");
                        break;
                    case 1:
                        Console.WriteLine("Fanuc lathe in standby mode£¡");
                        break;
                    case 2:
                        Console.WriteLine("Fanuc lathe in hold mode£¡");
                        break;
                    case 3:
                        Console.WriteLine("Fanuc lathe in start mode£¡");
                        break;
                    case 4:
                        Console.WriteLine("Fanuc lathe in manual mode, retraction and re-positioning of tools");
                        break;
                }
            }	
        }


        public static void disconnectFromFanucDevice()
        {
            int ret;

            ret = Focas1.cnc_freelibhndl(handleFanuc);
            if (ret != Focas1.EW_OK)
            {
                Console.WriteLine("Failed to disconnect from Fanuc lathe£¡");
            }
        }
    }
}