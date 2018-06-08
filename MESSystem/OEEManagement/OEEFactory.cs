using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

using MESSystem.common;
namespace MESSystem.OEEManagement
{
    public class OEEFactory
    {
        /**************************************************** Constant ****************************************************/
        // Change const variable will add supported device group dynamically
        private const int HAS_CUTTER_GROUP = 0;
        private const int HAS_PRINTER_GROUP = 0;
        // Default supported device group
        private const int HAS_CASTING_GROUP = 1;


        private const int GROUP_CASTING = (int)OEETypes.GROUP_TYPE.CASTING;
        private const int GROUP_CUTTER = (int)OEETypes.GROUP_TYPE.CUTTER;
        private const int GROUP_PRINTER = (int)OEETypes.GROUP_TYPE.PRINTER;

        private const int STAFF_WORKID = 1;
        private const int STAFF_NAME = 2;
        private const int STAFF_WORKSHOP = 5;
        /***************************************************** Types ******************************************************/


        /********************************************************Variables*************************************************/
        private static string[] workshopName = { "生产一部", "生产二部" };

        private OEEWorkshop[] _workshops;
        private OEEMachineGroup[] _machineGroups;
        private Dictionary<string, OEEStaff> _staffs;
        /***************************************************** Property ***************************************************/
        public Dictionary<string, OEEStaff> Staffs
        {
            get { return _staffs; }
        }

        public OEEWorkshop[] Workshops
        {
            get { return _workshops; }
        }

        public OEEMachineGroup[] MachineGroups
        {
            get { return _machineGroups; }
        }
        /***************************************************** Function ***************************************************/
        public OEEFactory()
        {
           
            InitWorkshops();
            InitMachineGroup();
            InitStaffs();
  
        }

        private void InitWorkshops()
        {
            _workshops = new OEEWorkshop[workshopName.Count()];
            for (int i = 0; i < workshopName.Count(); i++)
                _workshops[i] = new OEEWorkshop(workshopName[i]);
        }
        
        private void InitMachineGroup()
        {
            int groupCount = HAS_CASTING_GROUP + HAS_PRINTER_GROUP + HAS_CUTTER_GROUP;
            _machineGroups = new OEEMachineGroup[groupCount];
            AddMachineGroup();
        }

        private void InitStaffs()
        {
            DataTable dt = new DataTable();
            _staffs = new Dictionary<string, OEEStaff>();
            string dataBaseName = gVariable.basicInfoDatabaseName;
            string commandText = "select * from `" + gVariable.employeeTableName + "`";
            dt = mySQLClass.queryDataTableAction(dataBaseName, commandText, null);
            try
            {
                for (int row = 0; row < dt.Rows.Count; row++)
                {
                    OEEStaff staff;
                    staff = new OEEStaff(dt.Rows[row][STAFF_WORKID].ToString(),
                        dt.Rows[row][STAFF_NAME].ToString(),
                        dt.Rows[row][STAFF_WORKSHOP].ToString());
                    _staffs.Add(dt.Rows[row][STAFF_WORKID].ToString(), staff);
                }
            }
            catch (Exception)
            {
                System.Console.WriteLine("Error: No Employees in the database");
            }
        }

        private void AddMachineGroup()
        {
            if (HAS_CASTING_GROUP > 0)
                _machineGroups[GROUP_CASTING] = new OEEMachineGroup(OEETypes.GROUP_TYPE.CASTING);

            //if (HAS_CUTTER_GROUP > 0)
            //    _machineGroups[GROUP_CUTTER] = new OEEMachineGroup(OEETypes.GROUP_TYPE.CUTTER);

            //if (HAS_PRINTER_GROUP > 0)
            //    _machineGroups[GROUP_PRINTER] = new OEEMachineGroup(OEETypes.GROUP_TYPE.PRINTER);
        }
    }
}
