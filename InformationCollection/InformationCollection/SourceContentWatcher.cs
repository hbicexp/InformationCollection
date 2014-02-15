using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace TimiSoft.InformationCollection
{
    public class SourceContentWatcher
    {
        /// <summary>
        /// Gets or sets thread state
        /// </summary>
        public ThreadState ThreadState { get; set; }

        /// <summary>
        /// Gets watched sources
        /// </summary>
        public List<Models.Source> WatchedSources { get; private set; }

        /// <summary>
        /// log writer
        /// </summary>
        private log4net.ILog log = log4net.LogManager.GetLogger(typeof(SourceContentWatcher));

        /// <summary>
        /// 
        /// </summary>
        public SourceContentWatcher()
        {
            this.WatchedSources = new List<Models.Source>();
        }

        /// <summary>
        /// Start to collect informations
        /// </summary>
        public void StartCollect()
        {
            if (this.ThreadState != System.Threading.ThreadState.StopRequested)
            {
                Thread t = new Thread(Collect);
                t.Start();
            }
        }

        /// <summary>
        /// Stop collecting informations
        /// </summary>
        public void StopCollect()
        {
            this.ThreadState = System.Threading.ThreadState.StopRequested;

            while (this.ThreadState != System.Threading.ThreadState.Stopped)
            {
                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// Collect informations
        /// </summary>
        private void Collect()
        {
            DateTime beginTime = DateTime.Now;
            this.ThreadState = System.Threading.ThreadState.Running;

            while (this.ThreadState != System.Threading.ThreadState.StopRequested)
            {
                this.log.Info("Start Collect");
                this.Collect(beginTime);

                beginTime = beginTime.AddHours(0.1);
                while (beginTime > DateTime.Now && this.ThreadState != System.Threading.ThreadState.StopRequested)
                {
                    Thread.Sleep(5000);
                }
            }

            this.ThreadState = System.Threading.ThreadState.Stopped;
        }

        /// <summary>
        /// Collect informations
        /// </summary>
        /// <param name="collectTime">collect</param>
        private void Collect(DateTime collectTime)
        {
            try
            {
                this.GetWatchedSources();

                for (int i = this.WatchedSources.Count - 1; i >= 0; i--)
                {
                    var source = this.WatchedSources[i];
                    SourceContentManager.Collect(source, collectTime, SourceContentType.Content);

                    if (source.Interval == 1)
                    {
                        this.WatchedSources.RemoveAt(i);
                    }
                    else
                    {
                        source.Interval -= 1;
                    }
                }
            }
            catch (Exception ex)
            {
                this.log.Error(ex);
            }
        }

        private void GetWatchedSources()
        {
            using (Models.ICContext context = new Models.ICContext())
            {
                var addSourceList = (from p in context.Sources
                                     join q in this.WatchedSources
                                     on p.SourceId equals q.SourceId into watchedSourcesDefault
                                     from r in watchedSourcesDefault.DefaultIfEmpty()
                                     where r == null
                                     select p).ToList();
                this.WatchedSources.AddRange(addSourceList);
            }
        }
    }
}
