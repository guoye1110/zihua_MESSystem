using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabelPrint
{
    public static class GlobalConfig
    {
        public static SystemSetting Setting;

    }
    public enum ManufacturePhaseType
    {
        OUTBOUNING,
        LIUYAN,
        CUT,
        PRINT,
        QUALITYCHECK,
        RECOVERY,
        PACK
    };
    public class gVariable
    {
        public static int rebuild_database = 0;
        public const int MAX_CLIENT_ID = 50;
        public static String employeeTableName = "employee";
        public static String basicInfoDatabaseName = "basicInfo";
        public static String userAccount;
        public static String companyName = "上海紫华企业有限公司";
        public static Boolean checkWorkNoLen = false;
        public static String[] SiteSystems =
{
            "原料出库系统",
            "流延系统",
            "分切系统",
            "印刷系统",
            "质量检验系统",
            "再造料系统",
            "打包系统",

     };

        public static String orderNo;
        public static Boolean bShowPreview = true;
    }
}
