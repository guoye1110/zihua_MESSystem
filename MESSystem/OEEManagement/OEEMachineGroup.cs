using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MESSystem.OEEManagement
{
    public class OEEMachineGroup
    {
        /*******************************************************Constant********************************************/
        public const string CASTING_NAME = "流延机";
        public const string PRINTER_NAME = "印刷机";
        public const string CUTTER_NAME = "分切机";

        private const int CASTING_TOTAL = 5;
        private const int PRINTER_TOTAL = 3;
        private const int CUTTER_TOTAL = 3;

        /**************************************************Types*******************************************************/
     

        /********************************************************Variables*************************************************/
        private string _name;
        private OEETypes.GROUP_TYPE _type;
        private int _count;
        private DateTime _startDate;
        private DateTime _endDate;

        private OEEMachine[] machines;

        /********************************************Property**************************************************/
        public string Name
        {
            get { return _name; }
            set { _name = value;  }
        }

        public OEETypes.GROUP_TYPE Type
        {
            get { return _type; }
        }

        public int Count
        {
           get { return _count; }
        }

        public OEEMachine[] Machines
        {
            get { return machines; }
        }

        public DateTime QueryStartTime
        {
            get { return _startDate; }
            set { _startDate = new DateTime(value.Year, value.Month, value.Day, 0, 0, 0); }
        }

        public DateTime QueryEndTime
        {
            get { return _endDate; }
            set { _endDate = new DateTime(value.Year, value.Month, value.Day, 23, 59, 59); }
        }

        /*****************************************Functions*********************************************/
        public OEEMachineGroup(OEETypes.GROUP_TYPE type)
        {
            _type = type;

            switch (type)
            {
                case OEETypes.GROUP_TYPE.CASTING:
                    _name = CASTING_NAME;
                    _count = CASTING_TOTAL;
                    break;
                case OEETypes.GROUP_TYPE.CUTTER:
                    _name = CUTTER_NAME;
                    _count = CUTTER_TOTAL;
                    break;
                case OEETypes.GROUP_TYPE.PRINTER:
                    _name = PRINTER_NAME;
                    _count = PRINTER_TOTAL;
                    break;
                default:
                    return;
            }

            CreateGroupMachines(_name, _type);
        }

        private void CreateGroupMachines(string _name, OEETypes.GROUP_TYPE _type)
        {
            machines = new OEEMachine[_count];
            for (int i = 0; i < _count; i++)
                machines[i] = new OEEMachine(i, _name, _type);
        }


    }
}
