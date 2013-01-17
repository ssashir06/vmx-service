using System;
using System.Collections.Generic;
using System.Text;

namespace VMXService.Tools
{
    class VMTargetNames
    {
        protected Dictionary<VMWareInfo.VMCoreTypes, string> map_type_to_short_name = new Dictionary<VMWareInfo.VMCoreTypes, string>()
        {
            {VMWareInfo.VMCoreTypes.VMWarePlayer, "player"},
            {VMWareInfo.VMCoreTypes.VMWareWorkstation, "workstation"},
            {VMWareInfo.VMCoreTypes.UNKNOWN, "workstation"},
        };
        protected Dictionary<string, VMWareInfo.VMCoreTypes> map_short_name_to_type = new Dictionary<string, VMWareInfo.VMCoreTypes>()
        {
            {"player", VMWareInfo.VMCoreTypes.VMWarePlayer},
            {"workstation", VMWareInfo.VMCoreTypes.VMWareWorkstation},
        };

        public string GetShortNameByType(VMWareInfo.VMCoreTypes type)
        {
            if (map_type_to_short_name.ContainsKey(type))
                return map_type_to_short_name[type];
            else
                return map_type_to_short_name[VMWareInfo.VMCoreTypes.UNKNOWN];
        }

        public VMWareInfo.VMCoreTypes GetTypeByShortName(string short_name)
        {
            if (map_short_name_to_type.ContainsKey(short_name))
                return map_short_name_to_type[short_name];
            else
                return VMWareInfo.VMCoreTypes.UNKNOWN;
        }
    }
}
