using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace TimiSoft.InformationCollectionService
{
    public partial class ICService : ServiceBase
    {
        private InformationCollection.SourceContentWatcher watcher;

        public ICService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            this.watcher = new InformationCollection.SourceContentWatcher();
            watcher.StartCollect();
        }

        protected override void OnStop()
        {
            watcher.StopCollect();
        }

    }
}
