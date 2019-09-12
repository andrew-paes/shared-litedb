using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace LiteDB.Windows.Service
{
    public partial class Service1 : ServiceBase
    {
        private Server server = null;

        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
#if DEBUG
            Debugger.Launch();
#endif

            server = new Server();
            server.StartServer();
        }

        protected override void OnStop()
        {
            server.StopServer();
            server = null;
        }
    }
}