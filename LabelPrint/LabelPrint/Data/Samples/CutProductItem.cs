using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabelPrint
{
    public enum ManufactureType
    {
        M_SINGLE,
        M_MULTIPLE,
    };

    public enum WorkClassType
    {
        JIA,
        YI,
        BING,
//        DING,
    };

    public enum WorkTimeType
    {
        DAYWORK,
        NIGHTWORK,
     //   MIDDLEWORK,
    }

    //This info should come from the Server.
    public class BaseProductInfo
    {

        /**
         * 班别，班次，产品代号，宽度，原材料代码，客户名，材料名称，生产批号，铲板卷数，铲板号码等可以从服务器中读取
         * 
         */

        private WorkClassType _workclassType;
        public WorkClassType ClassType
        {
            get { return _workclassType; }
            set { _workclassType = value; }
        }

        private WorkTimeType _workTimeType;
        public WorkTimeType WorkTimeType
        {
            get { return _workTimeType; }
            set { _workTimeType = value; }
        }

        public String[] ProductCode = new String[3];

        public String[] RawMaterialCode = new string[3];
        public String[] MaterialName = new String[3];
        public String[] CustomerName = new String[3];
        public String[] BatchNo = new String[3];
        public String[] PlateRollNo = new String[3];//铲板卷数
        public String[] PlateNo = new String[3];//铲板号码


        public String[] Width = new String[3];

        private String _orderNo;
        public String OrderNo
        {
            get { return _orderNo; }
            set { _orderNo = value; }
        }
    }

    class CutProductItem
    {
        private String _index;
        public String Id
        {
            get { return _index; }
            set { _index = value; }
        }

        private ManufactureType _manutype;
        public ManufactureType ManuType
        {
            get { return _manutype; }
            set { _manutype = value; }
        }

        private WorkClassType _workclassType;
        public WorkClassType ClassType
        {
            get { return _workclassType; }
            set { _workclassType = value; }
        }

        private WorkTimeType _workTimeType;
        public WorkTimeType WorkTimeType
        {
            get { return _workTimeType; }
            set { _workTimeType = value; }
        }

        const int PRODUCT_TYPE_COUNT = 4;
        public String[] ProductCode = new String[PRODUCT_TYPE_COUNT];

        public String[] RawMaterialCode = new string[PRODUCT_TYPE_COUNT];
        public String[] MaterialName = new String[PRODUCT_TYPE_COUNT];
        public String[] CustomerName = new String[PRODUCT_TYPE_COUNT];
        public String[] BatchNo = new String[PRODUCT_TYPE_COUNT];
        public String[] PlateRollNo = new String[PRODUCT_TYPE_COUNT];//铲板卷数
        public String[] PlateNo = new String[PRODUCT_TYPE_COUNT];//铲板号码


        public String[] Width = new String[PRODUCT_TYPE_COUNT];

        private String _orderNo;
        public String OrderNo
        {
            get { return _orderNo; }
            set { _orderNo = value; }
        }

        private float _manHour;
        public float ManHour
        {
            get { return _manHour; }
            set { _manHour = value; }
        }


        private String _workerNumber;
        public String WorkerNumber
        {
            get { return _workerNumber; }
            set { _workerNumber = value; }
        }

        private int _productState;
        public int ProductState
        {
            get { return _productState; }
            set { _productState = value; }
        }

        private String _desc;
        public String Description
        {
            get { return _desc; }
            set { _desc = value; }
        }

        private String _majoRollrNum;
        public String BigRollNo
        {
            get { return _majoRollrNum; }
            set { _majoRollrNum = value; }
        }
        private String _minorRollNum;
        public String LittleRoleNo
        {
            get { return _minorRollNum; }
            set { _minorRollNum = value; }
        }

        private Boolean _showRealWeight;
        public Boolean ShowRealWeight
        {
            get { return _showRealWeight; }
            set { _showRealWeight = value; }
        }

        private String _weight;
        public String Weight
        {
            get { return _weight; }
            set { _weight = value; }
        }
    }
}
