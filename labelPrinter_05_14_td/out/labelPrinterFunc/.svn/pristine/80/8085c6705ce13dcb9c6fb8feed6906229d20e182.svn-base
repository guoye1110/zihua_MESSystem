using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Diagnostics;
using System.Timers;

namespace LabelPrint.Util
{
    /// <summary>
    /// 获取键盘输入或者USB扫描枪数据 可以是没有焦点 应为使用的是全局钩子
    /// USB扫描枪 是模拟键盘按下
    /// 这里主要处理扫描枪的值，手动输入的值不太好处理
    /// </summary>
    public class BardCodeHooK
    {


        public delegate void BardCodeDeletegate(BarCodes barCode);
        public event BardCodeDeletegate BarCodeEvent;


        //定义成静态，这样不会抛出回收异常
        private static HookProc hookproc;



        public struct BarCodes
        {
            public int VirtKey;//虚拟吗
            public int ScanCode;//扫描码
            public string KeyName;//键名
            public uint Ascll;//Ascll
            public char Chr;//字符
            public string OriginalChrs; //原始 字符
            public string OriginalAsciis;//原始 ASCII



            public string OriginalBarCode; //原始数据条码


            public string BarCode;//条码信息 保存最终的条码
            public bool IsValid;//条码是否有效
            public DateTime Time;//扫描时间,
        }


        private struct EventMsg
        {
#pragma warning disable CS0649 // Field 'BardCodeHooK.EventMsg.message' is never assigned to, and will always have its default value 0
            public int message;
#pragma warning restore CS0649 // Field 'BardCodeHooK.EventMsg.message' is never assigned to, and will always have its default value 0
#pragma warning disable CS0649 // Field 'BardCodeHooK.EventMsg.paramL' is never assigned to, and will always have its default value 0
            public int paramL;
#pragma warning restore CS0649 // Field 'BardCodeHooK.EventMsg.paramL' is never assigned to, and will always have its default value 0
#pragma warning disable CS0649 // Field 'BardCodeHooK.EventMsg.paramH' is never assigned to, and will always have its default value 0
            public int paramH;
#pragma warning restore CS0649 // Field 'BardCodeHooK.EventMsg.paramH' is never assigned to, and will always have its default value 0
#pragma warning disable CS0649 // Field 'BardCodeHooK.EventMsg.Time' is never assigned to, and will always have its default value 0
            public int Time;
#pragma warning restore CS0649 // Field 'BardCodeHooK.EventMsg.Time' is never assigned to, and will always have its default value 0
#pragma warning disable CS0649 // Field 'BardCodeHooK.EventMsg.hwnd' is never assigned to, and will always have its default value 0
            public int hwnd;
#pragma warning restore CS0649 // Field 'BardCodeHooK.EventMsg.hwnd' is never assigned to, and will always have its default value 0
        }


        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);


        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern bool UnhookWindowsHookEx(int idHook);


        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern int CallNextHookEx(int idHook, int nCode, Int32 wParam, IntPtr lParam);


        [DllImport("user32", EntryPoint = "GetKeyNameText")]
        private static extern int GetKeyNameText(int IParam, StringBuilder lpBuffer, int nSize);


        [DllImport("user32", EntryPoint = "GetKeyboardState")]
        private static extern int GetKeyboardState(byte[] pbKeyState);

        [DllImport("user32", EntryPoint = "ToAscii")]
        private static extern bool ToAscii(int VirtualKey, int ScanCode, byte[] lpKeySate, ref uint lpChar, int uFlags);


        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandle(string name);



        delegate int HookProc(int nCode, Int32 wParam, IntPtr lParam);
        BarCodes barCode = new BarCodes();
        int hKeyboardHook = 0;
        //string strBarCode = "";
        StringBuilder sbBarCode = new StringBuilder();
        static int i = 0;
#pragma warning disable CS0169 // The field 'BardCodeHooK.t' is never used
        System.Timers.Timer t;
#pragma warning restore CS0169 // The field 'BardCodeHooK.t' is never used


        #region 按键间隔超时限定值
        int _TimeOut = 100;
        Timer _Timer = new Timer();//计时器
        DateTime _TickTime = DateTime.MinValue;//记录前一次按键周期的时间
#pragma warning disable CS0414 // The field 'BardCodeHooK._MiniLength' is assigned but its value is never used
        int _MiniLength = 20;
#pragma warning restore CS0414 // The field 'BardCodeHooK._MiniLength' is assigned but its value is never used

        const int VK_LSHIFT = 0xa0;
        const int VK_RSHIFT = 0xa1;
        const int VK_CAPITAL = 0x14;
        const int VK_OEM_PLUS = 0xBB; //'+'
        const int VK_OEM_COMA = 0xBC; //','
        const int VK_OEM_MINUS = 0xBD; //'-'
        const int VK_OEM_PERIOD = 0xBE; //'.'

        const int VK_OEM_1 = 0xBA; //';:'
        const int VK_OEM_2 = 0xBF; //'/?'
        const int VK_OEM_3 = 0xC0; //'~'
        const int VK_OEM_4 = 0xDB; //'[{'
        const int VK_OEM_5 = 0xDC; //'\|'
        const int VK_OEM_6 = 0xDD; //']}|'
        const int VK_OEM_7 = 0xDE; //single-quote/double-qutoe'

        const int WM_KEYUP = 0x0101;
        const int WM_SYSKEYUP = 0x0105;

        public BardCodeHooK()
        {
            _Timer.Elapsed += Timer_Elapsed;
        }

        /// <summary>
        /// 读取或设置按键间隔超时限定值
        /// </summary>
        public int TimeOut
        {
            get { return _TimeOut; }
            set { _TimeOut = value; }
        }

        #endregion
        #region 时钟周期
        int _ClockTick = 100;
        /// <summary>
        /// 读取或设置内置时钟周期
        /// </summary>
        public int ClockTick
        {
            get { return _ClockTick; }
            set { _ClockTick = value; }
        }
        #endregion
        /// <summary>
        /// 停止检测时间间隔，重置状态，停止内置时钟
        /// </summary>
        private void StopCheckGap()
        {
            Log.d("HHH", "StopCheckGap");
            i = 0;
            _Timer.Enabled = false;
            _TickTime = DateTime.MinValue;
            Log.d("HHH", "Now Minvalue" + _TickTime);
        }
        // <summary>
        /// 时钟事件方法
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="e">事件对象</param>
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {

            int gap = e.SignalTime.Subtract(_TickTime).Milliseconds;
            if (gap > _TimeOut)
            {
                Log.d("HHH", "Timer_elapsed ");
                StopCheckGap();//停止检测,可能是扫描枪扫描结束，也可能是手工输入间隔
                               //if (_TempText.Length < _MiniLength)//进一步检查长度，如果较短，说明为手工输入
                               //{
                               //  _TempText = "";//清空本地文本
                               //                    SendInputTimeOutEvent();//发送输入超时事件
                               //              }

                if ((sbBarCode.Length > 3))
                {//回车
                 //barCode.BarCode = strBarCode;
                    barCode.BarCode = sbBarCode.ToString();// barCode.OriginalBarCode;
                    barCode.IsValid = true;
                    sbBarCode.Remove(0, sbBarCode.Length);
                    //Log.d("HHH", "Barcode++" + barCode.BarCode + " len=" + sbBarCode.Length);

                    if (BarCodeEvent != null && barCode.IsValid)
                    {
                        AsyncCallback callback = new AsyncCallback(AsyncBack);
                        //object obj;
                        Delegate[] delArray = BarCodeEvent.GetInvocationList();
                        //foreach (Delegate del in delArray)
                        foreach (BardCodeDeletegate del in delArray)
                        {
                            try
                            {
                                //方法1
                                //obj = del.DynamicInvoke(barCode);
                                //方法2
                                del.BeginInvoke(barCode, callback, del);//异步调用防止界面卡死
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }
                        }
                        //BarCodeEvent(barCode);//触发事件
                        barCode.BarCode = "";
                        barCode.OriginalChrs = "";
                        barCode.OriginalAsciis = "";
                        barCode.OriginalBarCode = "";
                        barCode.IsValid = false;
                    }
                }
            }

        }

        Boolean capsLockOn = false;
        Boolean shiftPressed = false;
        private int KeyboardHookProc(int nCode, Int32 wParam, IntPtr lParam)
        {
            Boolean visibleKey = true;
            Boolean isCap = false;

            Log.d("EE", " barcode Length=" + sbBarCode.Length + " wParam= " + wParam +"lParam"+ wParam);
            if (nCode == 0)
            {
                EventMsg msg = (EventMsg)Marshal.PtrToStructure(lParam, typeof(EventMsg));
                barCode.VirtKey = msg.message & 0xff;//虚拟吗
                barCode.ScanCode = msg.paramL & 0xff;//扫描码

                //Log.d("EE", " barcode Length=" + sbBarCode.Length + "message" + msg.message);
                if (wParam == 0x100)//WM_KEYDOWN=0x100 
                {

                    StringBuilder strKeyName = new StringBuilder(225);

                    if (barCode.VirtKey == VK_LSHIFT || barCode.VirtKey == VK_RSHIFT)
                    {
                        shiftPressed = true;
                    }
                    else if (barCode.VirtKey == VK_CAPITAL)
                    {
                        capsLockOn = true;
                    }
                    else
                    {
                        isCap = shiftPressed ^ capsLockOn;
                        //char charPressed = barCode.VirtKey;
                        if ((barCode.VirtKey >= 'A') && (barCode.VirtKey <= 'Z') && !isCap)
                        {
                            barCode.VirtKey = barCode.VirtKey + 32;
                            barCode.Ascll = (uint)barCode.VirtKey;
                            barCode.Chr = Convert.ToChar(barCode.VirtKey);
                        }
                        else if ((barCode.VirtKey >= 'A') && (barCode.VirtKey <= 'Z') && isCap)
                        {
                            barCode.Chr = Convert.ToChar(barCode.VirtKey);
                            barCode.Ascll = (uint)barCode.VirtKey;
                        }
                        else if ((barCode.VirtKey >= '0') && (barCode.VirtKey <= '9') && !isCap)
                        {
                            barCode.Chr = Convert.ToChar(barCode.VirtKey);
                            barCode.Ascll = (uint)barCode.VirtKey;
                        }
                        else if ((barCode.VirtKey >= '0') && (barCode.VirtKey <= '9') && isCap)
                        {
                            switch (barCode.VirtKey)
                            {
                                case '0':
                                    barCode.Ascll = (uint)')';
                                    break;
                                case '1':
                                    barCode.Ascll = (uint)'!';
                                    break;
                                case '2':
                                    barCode.Ascll = (uint)'@';
                                    break;
                                case '3':
                                    barCode.Ascll = (uint)'#';
                                    break;
                                case '4':
                                    barCode.Ascll = (uint)'$';
                                    break;
                                case '5':
                                    barCode.Ascll = (uint)'%';
                                    break;
                                case '6':
                                    barCode.Ascll = (uint)'^';
                                    break;
                                case '7':
                                    barCode.Ascll = (uint)'&';
                                    break;
                                case '8':
                                    barCode.Ascll = (uint)'*';
                                    break;
                                case '9':
                                    barCode.Ascll = (uint)'(';
                                    break;
                                default:
                                    break;
                            }
                            barCode.Chr = Convert.ToChar(barCode.Ascll);
                        }
                        else
                        {
                            switch (barCode.VirtKey)
                            {
                                case VK_OEM_PLUS:
                                    barCode.Ascll = isCap ? '+' : '=';
                                    break;
                                case VK_OEM_COMA:
                                    barCode.Ascll = isCap ? '<' : ',';
                                    break;
                                case VK_OEM_MINUS:
                                    barCode.Ascll = isCap ? '_' : '-';
                                    break;
                                case VK_OEM_PERIOD:
                                    barCode.Ascll = isCap ? '>' : '.';
                                    break;
                                case VK_OEM_1:
                                    barCode.Ascll = isCap ? ':' : ';';
                                    break;
                                case VK_OEM_2:
                                    barCode.Ascll = isCap ? '?' : '/';
                                    break;
                                case VK_OEM_3:
                                    barCode.Ascll = isCap ? '~' : '`';
                                    break;
                                case VK_OEM_4:
                                    barCode.Ascll = isCap ? '{' : '[';
                                    break;
                                case VK_OEM_5:
                                    barCode.Ascll = isCap ? '|' : '\\';
                                    break;
                                case VK_OEM_6:
                                    barCode.Ascll = isCap ? '}' : ']';
                                    break;
                                case VK_OEM_7:
                                    barCode.Ascll = isCap ? '"' : '\'';
                                    break;
                                default:
                                    visibleKey = false;
                                    break;
                            }

                            if (visibleKey)
                            {
                                barCode.Chr = Convert.ToChar(barCode.Ascll);
                            }
                        }

                        if (GetKeyNameText(barCode.ScanCode << 16, strKeyName, 255) > 0)
                        {
                            barCode.KeyName = strKeyName.ToString().Trim(new char[] { ' ', '\0' });
                        }
                        else
                        {
                            barCode.KeyName = "";
                        }


                        //byte[] kbArray = new byte[256];
                        //uint uKey = 0;
                        //GetKeyboardState(kbArray);


                        //if (ToAscii(barCode.VirtKey, barCode.ScanCode, kbArray, ref uKey, 0))
                        //{
                        //    barCode.Ascll = uKey;
                        //    barCode.Chr = Convert.ToChar(uKey);
                        //}


                        TimeSpan ts = DateTime.Now.Subtract(barCode.Time);
                        if (visibleKey)
                            i++;


                        int gap;
                        DateTime thisTime = DateTime.Now;
                        gap = thisTime.Subtract(_TickTime).Milliseconds;

                        {
                            if (_TickTime == DateTime.MinValue)//第一次
                            {
                                _Timer.Interval = _ClockTick;
                                _Timer.Enabled = true;//开启时钟
                                                      //SendInputTimeOutEvent();//发送输入超时事件
                                                      //时间戳，大于xx 毫秒表示手动输入
                                                      //strBarCode = barCode.Chr.ToString();
                                sbBarCode.Remove(0, sbBarCode.Length);
                                sbBarCode.Append(barCode.Chr.ToString());
                                Log.d("TT", "Timer Start Data=" + sbBarCode.ToString());
                                barCode.OriginalChrs = " " + Convert.ToString(barCode.Chr);
                                barCode.OriginalAsciis = " " + Convert.ToString(barCode.Ascll);
                                barCode.OriginalBarCode = Convert.ToString(barCode.Chr);
                                _TickTime = thisTime;//保存时间现场，用于下一周期判断依据
                                return CallNextHookEx(hKeyboardHook, nCode, wParam, lParam); ;//保留当前输入字符
                            }
                            else
                            {
                                if (gap > _TimeOut)
                                {
                                    Log.d("TT", "Gap is too big, Stop checking.");
                                    StopCheckGap();//停止检查输入间隔
                                    barCode.IsValid = true;
                                }
                                else
                                {
                                    //Log.d("TT", "Still in gap, checking.");
                                    if (visibleKey)
                                        sbBarCode.Append(barCode.Chr.ToString());
                                    if ((msg.message & 0xff) == 13 && sbBarCode.Length > 6)
                                    {//回车
                                        Log.d("TT", "Enter key");
                                        //barCode.BarCode = strBarCode;
                                        StopCheckGap();
                                        barCode.BarCode = sbBarCode.ToString();// barCode.OriginalBarCode;
                                        barCode.IsValid = true;
                                        sbBarCode.Remove(0, sbBarCode.Length);
                                    }
                                    else
                                    {
                                        _TickTime = thisTime;//保存时间现场，用于下一周期判断依据
                                    }
                                }
                            }

                            try
                            {
                                if (BarCodeEvent != null && barCode.IsValid)
                                {
                                    AsyncCallback callback = new AsyncCallback(AsyncBack);
                                    Delegate[] delArray = BarCodeEvent.GetInvocationList();
                                    foreach (BardCodeDeletegate del in delArray)
                                    {
                                        try
                                        {
                                            //方法1
                                            //obj = del.DynamicInvoke(barCode);
                                            //方法2
                                            del.BeginInvoke(barCode, callback, del);//异步调用防止界面卡死
                                        }
                                        catch (Exception ex)
                                        {
                                            throw ex;
                                        }
                                    }
                                    barCode.BarCode = "";
                                    barCode.OriginalChrs = "";
                                    barCode.OriginalAsciis = "";
                                    barCode.OriginalBarCode = "";
                                }
                            }
                            catch (Exception)
                            {
                                throw;
                            }
                            finally
                            {
                                barCode.IsValid = false; //最后一定要 设置barCode无效
                                barCode.Time = DateTime.Now;
                            }
                        }

                    }
                }
                else if ((wParam == WM_SYSKEYUP) || (wParam == WM_KEYUP))
                {

                    if (barCode.VirtKey == VK_LSHIFT || barCode.VirtKey == VK_RSHIFT)
                    {
                        shiftPressed = false;
                    }
                    else if (barCode.VirtKey == VK_CAPITAL)
                    {
                        capsLockOn = false;
                    }
                }
            }
            return CallNextHookEx(hKeyboardHook, nCode, wParam, lParam);
        }


        //异步返回方法
        public void AsyncBack(IAsyncResult ar)
        {
            BardCodeDeletegate del = ar.AsyncState as BardCodeDeletegate;
            del.EndInvoke(ar);
        }


        //安装钩子
        public bool Start()
        {
            if (hKeyboardHook == 0)
            {
                hookproc = new HookProc(KeyboardHookProc);



                //GetModuleHandle 函数 替代 Marshal.GetHINSTANCE
                //防止在 framework4.0中 注册钩子不成功
                IntPtr modulePtr = GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName);


                //WH_KEYBOARD_LL=13
                //全局钩子 WH_KEYBOARD_LL
                // hKeyboardHook = SetWindowsHookEx(13, hookproc, Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]), 0);


                hKeyboardHook = SetWindowsHookEx(13, hookproc, modulePtr, 0);
            }
            return (hKeyboardHook != 0);
        }


        //卸载钩子
        public bool Stop()
        {
            if (hKeyboardHook != 0)
            {
                return UnhookWindowsHookEx(hKeyboardHook);
            }
            _Timer.Dispose();
            return true;
        }

        /*
        void Method1()
        {

        }

        void Method2(object source, System.Timers.ElapsedEventArgs e)
        {
            TimeSpan ts = DateTime.Now.Subtract(barCode.Time);
            Log.d("TT", "time=" + ts.TotalMilliseconds);

            if (ts.TotalMilliseconds > 500)
            {
                t.Enabled = false;
                AsyncCallback callback = new AsyncCallback(AsyncBack);
                //object obj;
                Delegate[] delArray = BarCodeEvent.GetInvocationList();
                //foreach (Delegate del in delArray)
                foreach (BardCodeDeletegate del in delArray)
                {
                    try
                    {
                        //方法1
                        //obj = del.DynamicInvoke(barCode);
                        //方法2
                        Log.d("DD", "Barcode="+barCode.BarCode);
                        del.BeginInvoke(barCode, callback, del);//异步调用防止界面卡死
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
                //BarCodeEvent(barCode);//触发事件
                barCode.BarCode = "";
                barCode.OriginalChrs = "";
                barCode.OriginalAsciis = "";
                barCode.OriginalBarCode = "";

            }

        }
        */
    }
}
