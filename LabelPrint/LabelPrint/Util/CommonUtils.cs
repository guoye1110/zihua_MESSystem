using System;
using System.IO;
using System.Text.RegularExpressions;

using System.Collections.Generic;
using System.Text;
//using System.Windows.Browser;

namespace LabelPrinting
{
    class CommonUtils
    {
        #region String????  

        /**/
        /// <summary>  
        /// ????  
        /// </summary>  

        public static string Replace(string strOriginal, string oldchar, string newchar)
        {
            if (string.IsNullOrEmpty(strOriginal))
                return "";
            string tempChar = strOriginal;
            tempChar = tempChar.Replace(oldchar, newchar);

            return tempChar;
        }

        /**/
        /// <summary>  
        /// ??????  
        /// </summary>  
        /// <param name="str"></param>  
        /// <returns></returns>  
        public static string ReplaceBadChar(string str)
        {
            if (string.IsNullOrEmpty(str))
                return "";
            string strBadChar, tempChar;
            string[] arrBadChar;
            strBadChar = "@@,+,',--,%,^,&,?,(,),<,>,[,],{,},/,\\,;,:,\",\"\",";
            arrBadChar = SplitString(strBadChar, ",");
            tempChar = str;
            for (int i = 0; i < arrBadChar.Length; i++)
            {
                if (arrBadChar[i].Length > 0)
                    tempChar = tempChar.Replace(arrBadChar[i], "");
            }
            return tempChar;
        }


        /**/
        /// <summary>  
        /// ??????????  
        /// </summary>  
        /// <param name="str">???????</param>  
        /// <returns></returns>  
        public static bool ChkBadChar(string str)
        {
            bool result = false;
            if (string.IsNullOrEmpty(str))
                return result;
            string strBadChar, tempChar;
            string[] arrBadChar;
            strBadChar = "@@,+,',--,%,^,&,?,(,),<,>,[,],{,},/,\\,;,:,\",\"\"";
            arrBadChar = SplitString(strBadChar, ",");
            tempChar = str;
            for (int i = 0; i < arrBadChar.Length; i++)
            {
                if (tempChar.IndexOf(arrBadChar[i]) >= 0)
                    result = true;
            }
            return result;
        }


        /**/
        /// <summary>  
        /// ?????  
        /// </summary>  
        public static string[] SplitString(string strContent, string strSplit)
        {
            if (string.IsNullOrEmpty(strContent))
            {
                return null;
            }
            int i = strContent.IndexOf(strSplit);
            if (strContent.IndexOf(strSplit) < 0)
            {
                string[] tmp = { strContent };
                return tmp;
            }
            //return Regex.Split(strContent, @strSplit.Replace(".", @"\."), RegexOptions.IgnoreCase);  

            return Regex.Split(strContent, @strSplit.Replace(".", @"\."));
        }


        /**/
        /// <summary>  
        /// string????int?  
        /// </summary>  
        /// <param name="strValue">???????</param>  
        /// <returns>????int????.?????????????,???-1.</returns>  
        public static int StrToInt(object strValue)
        {
            int defValue = -1;
            if ((strValue == null) || (strValue.ToString() == string.Empty) || (strValue.ToString().Length > 10))
            {
                return defValue;
            }

            string val = strValue.ToString();
            string firstletter = val[0].ToString();

            if (val.Length == 10 && IsNumber(firstletter) && int.Parse(firstletter) > 1)
            {
                return defValue;
            }
            else if (val.Length == 10 && !IsNumber(firstletter))
            {
                return defValue;
            }


            int intValue = defValue;
            if (strValue != null)
            {
                bool IsInt = new Regex(@"^([-]|[0-9])[0-9]*$").IsMatch(strValue.ToString());
                if (IsInt)
                {
                    intValue = Convert.ToInt32(strValue);
                }
            }

            return intValue;
        }

        /**/
        /// <summary>  
        /// string????int?  
        /// </summary>  
        /// <param name="strValue">???????</param>  
        /// <param name="defValue">???</param>  
        /// <returns>????int????</returns>  
        public static int StrToInt(object strValue, int defValue)
        {
            if ((strValue == null) || (strValue.ToString() == string.Empty) || (strValue.ToString().Length > 10))
            {
                return defValue;
            }

            string val = strValue.ToString();
            string firstletter = val[0].ToString();

            if (val.Length == 10 && IsNumber(firstletter) && int.Parse(firstletter) > 1)
            {
                return defValue;
            }
            else if (val.Length == 10 && !IsNumber(firstletter))
            {
                return defValue;
            }


            int intValue = defValue;
            if (strValue != null)
            {
                bool IsInt = new Regex(@"^([-]|[0-9])[0-9]*$").IsMatch(strValue.ToString());
                if (IsInt)
                {
                    intValue = Convert.ToInt32(strValue);
                }
            }

            return intValue;
        }



        /**/
        /// <summary>  
        /// string???????  
        /// </summary>  
        /// <param name="strValue">???????</param>  
        /// <param name="defValue">???</param>  
        /// <returns>??????????</returns>  
        public static DateTime StrToDateTime(object strValue, DateTime defValue)
        {
            if ((strValue == null) || (strValue.ToString().Length > 20))
            {
                return defValue;
            }

            DateTime intValue;

            if (!DateTime.TryParse(strValue.ToString(), out intValue))
            {
                intValue = defValue;
            }
            return intValue;
        }


        /**/
        /// <summary>  
        /// ????????(strNumber)??????  
        /// </summary>  
        /// <param name="strNumber">???????</param>  
        /// <returns>????true ????? false</returns>  
        public static bool IsNumber(string strNumber)
        {
            return new Regex(@"^([0-9])[0-9]*(\.\w*)?$").IsMatch(strNumber);
        }


        /**/
        /// <summary>  
        /// ??????email??  
        /// </summary>  
        /// <param name="strEmail">????email???</param>  
        /// <returns>????</returns>  
        public static bool IsValidEmail(string strEmail)
        {
            return Regex.IsMatch(strEmail, @"^([\w-\.]+)@((
[0-9]1,3\.[0-9]1,3\.[0-9]1,3\.)|(([\w-]+\.)+))([a-zA-Z]2,4|[0-9]1,3)(
?)$");
        }


        /**/
        /// <summary>  
        /// ??????url??,??????http://  
        /// </summary>  
        /// <param name="url"></param>  
        /// <returns></returns>  
        public static bool IsURL(string url)
        {
            return Regex.IsMatch(url, @"^http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?$");
        }

        /**/
        /// <summary>  
        /// ??????????  
        /// </summary>  
        /// <param name="phoneNumber"></param>  
        /// <returns></returns>  
        public static bool IsPhoneNumber(string phoneNumber)
        {
            return Regex.IsMatch(phoneNumber, @"^(\d3|\d{3}-)?\d{7,8}$");
        }



        /**/
        /// <summary>  
        /// ?????????????  
        /// </summary>  
        /// <param name="num"></param>  
        /// <returns></returns>  
        public static bool IsIdentityNumber(string num)
        {
            return Regex.IsMatch(num, @"^\d{17}[\d|X]|\d{15}$");
        }




        #endregion

        #region Sql?  

        /**/
        /// <summary>  
        /// ?????Sql????  
        /// </summary>  
        /// <param name="str">??????</param>  
        /// <returns>????</returns>  
        public static bool IsSafeSqlString(string str)
        {

            return !Regex.IsMatch(str, @"[-|;|,|\/|||
|
|\}|\{|%|@|\*|!|\']");
        }


        /**/
        /// <summary>  
        /// ??sql???????  
        /// </summary>  
        public static string ReplaceBadSQL(string str)
        {
            string str2;

            if (str == null)
            {
                str2 = "";
            }
            else
            {
                str = str.Replace("'", "''");
                str2 = str;
            }
            return str2;
        }



        #endregion

        #region Html?  

        /**/
        /// <summary>  
        /// ?? HTML ????????  
        /// </summary>  
        /// <param name="str">???</param>  
        /// <returns>????</returns>  
       // public static string HtmlDecode(string str)
        //{
            //str = str.Replace("''", "'");  
          //  return HttpUtility.HtmlDecode(str);
       // }

        /**/
        /// <summary>  
        /// ??html??  
        /// </summary>  
        public static string EncodeHtml(string strHtml)
        {
            if (strHtml != "")
            {
                strHtml = strHtml.Replace(",", "&def");
                strHtml = strHtml.Replace("'", "&dot");
                strHtml = strHtml.Replace(";", "&dec");
                return strHtml;
            }
            return "";
        }

        /**/
        /// <summary>  
        /// ????????html???  
        /// </summary>  
        public static string StrFormat(string str)
        {
            string str2;

            if (str == null)
            {
                str2 = "";
            }
            else
            {
                str = str.Replace("\r\n", "<br />");
                str = str.Replace("\n", "<br />");
                str2 = str;
            }
            return str2;
        }
        #endregion

        #region DateTime?  
        /**/
        /// <summary>  
        /// ?????????? yyyy-MM-dd ????string    
        /// </summary>  
        public static string GetDate()
        {
            return DateTime.Now.ToString("yyyy-MM-dd");
        }

        /**/
        /// <summary>  
        ///????????????????string HH:mm:ss  
        /// </summary>  
        public static string GetTime()
        {
            return DateTime.Now.ToString("HH:mm:ss");
        }
        /**/
        /// <summary>  
        /// ????????????????string yyyy-MM-dd HH:mm:ss  
        /// </summary>  
        public static string GetDateTime()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        /**/
        /// <summary>  
        /// ????????????????string yyyy-MM-dd HH:mm:ss:fffffff  
        /// </summary>  
        public static string GetDateTimeF()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fffffff");
        }

        /**/
        /// <summary>  
        /// ?string???fDateTime???formatStr???????  
        /// </summary>        
        public static string GetStandardDateTime(string fDateTime, string formatStr)
        {
            DateTime s = Convert.ToDateTime(fDateTime);
            return s.ToString(formatStr);
        }

        /**/
        /// <summary>  
        ///?string???fDateTime??????? yyyy-MM-dd HH:mm:ss  
        /// </sumary>  
        public static string GetStandardDateTime(string fDateTime)
        {
            return GetStandardDateTime(fDateTime, "yyyy-MM-dd HH:mm:ss");
        }
        /**/
        /// <summary>  
        /// ???????  
        /// </summary>  
        /// <param name="Time"></param>  
        /// <param name="Sec"></param>  
        /// <returns></returns>  
        public static int StrDateDiffSeconds(string Time, int Sec)
        {
            TimeSpan ts = DateTime.Now - DateTime.Parse(Time).AddSeconds(Sec);
            if (ts.TotalSeconds > int.MaxValue)
            {
                return int.MaxValue;
            }
            else if (ts.TotalSeconds < int.MinValue)
            {
                return int.MinValue;
            }
            return (int)ts.TotalSeconds;
        }

        /**/
        /// <summary>  
        /// ????????  
        /// </summary>  
        /// <param name="time"></param>  
        /// <param name="minutes"></param>  
        /// <returns></returns>  
        public static int StrDateDiffMinutes(string time, int minutes)
        {
            if (time == "" || time == null)
                return 1;
            TimeSpan ts = DateTime.Now - DateTime.Parse(time).AddMinutes(minutes);
            if (ts.TotalMinutes > int.MaxValue)
            {
                return int.MaxValue;
            }
            else if (ts.TotalMinutes < int.MinValue)
            {
                return int.MinValue;
            }
            return (int)ts.TotalMinutes;
        }

        /**/
        /// <summary>  
        /// ????????  
        /// </summary>  
        /// <param name="time"></param>  
        /// <param name="hours"></param>  
        /// <returns></returns>  
        public static int StrDateDiffHours(string time, int hours)
        {
            if (time == "" || time == null)
                return 1;
            TimeSpan ts = DateTime.Now - DateTime.Parse(time).AddHours(hours);
            if (ts.TotalHours > int.MaxValue)
            {
                return int.MaxValue;
            }
            else if (ts.TotalHours < int.MinValue)
            {
                return int.MinValue;
            }
            return (int)ts.TotalHours;
        }

        #endregion

        #region file?  
        /**/
        /// <summary>  
        /// ??????  
        /// </summary>  
        /// <param name="filePath">????</param>  
        /// <returns></returns>  
      /*  public static bool FileExists(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return false;
            filePath = HttpContext.Current.Server.MapPath(filePath);
            DirectoryInfo dirInfo = new DirectoryInfo(filePath);
            if (dirInfo.Exists)
                return true;
            return false;
        }
        */
        #endregion
    }
}
