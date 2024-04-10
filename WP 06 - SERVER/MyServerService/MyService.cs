using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace MyServerService
{
    public partial class MyService : ServiceBase
    {
        public MyService()
        {
            InitializeComponent();
            CanPauseAndContinue = false;
        }

        protected override void OnStart(string[] args)
        {
            Logger.Log("Service Started");
        }

        protected override void OnStop()
        {
            Logger.Log("Service Stopped");
        }
    }
}
