using System.ServiceProcess;
using VMXService.Tools;

namespace VMXService.Service
{
    static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        static void Main(string[] args)
        {
            Service service;

            //Arg1  ... full path to VMX file
            //Arg2  ... VMWare type ('workstation' or 'player', see VMTargetNames.cs)

            if (args.Length != 2)
            {
                service = new Service();
            }
            else
            {
                VMWareInfo.VMCoreTypes vmtype = new VMTargetNames().GetTypeByShortName(args[1]);
                service = new Service(args[0], vmtype);

                switch (vmtype)
                {
                    case VMWareInfo.VMCoreTypes.VMWarePlayer:
                    case VMWareInfo.VMCoreTypes.UNKNOWN:
                        service.CanPauseAndContinue = false;
                        break;
                }

            }
            ServiceBase.Run(service);
        }
    }
}
