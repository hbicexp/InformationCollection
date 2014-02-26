// -----------------------------------------------------------------------
// <copyright file="SourceContentWatcher.cs" company="TimiSoft">
// Copyright TimiSoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace TimiSoft.InformationCollection
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;

    /// <summary>
    /// Source content watcher class
    /// </summary>
    public class SourceContentWatcher
    {
        /// <summary>
        /// log writer
        /// </summary>
        private log4net.ILog log = log4net.LogManager.GetLogger(typeof(SourceContentWatcher));

        /// <summary>
        /// Initializes a new instance of the <see cref="SourceContentWatcher" /> class.
        /// </summary>
        public SourceContentWatcher()
        {
            this.WatchedSources = new List<Models.Source>();
        }

        /// <summary>
        /// Gets or sets thread state
        /// </summary>
        public ThreadState ThreadState { get; set; }

        /// <summary>
        /// Gets watched sources
        /// </summary>
        public List<Models.Source> WatchedSources { get; private set; }

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

                beginTime = beginTime.AddHours(1);
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
        /// <param name="collectTime">collect time</param>
        private void Collect(DateTime collectTime)
        {
            try
            {
                SourceContentManager.ReloadSourceRegexes();

                var watchedSources = this.GetWatchedSources();
                foreach (var source in watchedSources)
                {
                    this.log.Info("Collect info for " + source.Url);
                    SourceContentManager.Collect(source, collectTime, SourceContentType.Content);
                }

                this.WatchedSources.AddRange(watchedSources);
                for (int i = this.WatchedSources.Count - 1; i >= 0; i--)
                {
                    var source = this.WatchedSources[i];
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

        /// <summary>
        /// Get watched sources
        /// </summary>
        private IList<Models.Source> GetWatchedSources()
        {
            using (Models.ICContext context = new Models.ICContext())
            {
                return (from p in context.Sources.Where(p => p.Interval > 0).ToList()
                                     join q in this.WatchedSources
                                     on p.SourceId equals q.SourceId into watchedSourcesDefault
                                     from r in watchedSourcesDefault.DefaultIfEmpty()
                                     where r == null
                                     select p).ToList();
            }
        }
    }
}
