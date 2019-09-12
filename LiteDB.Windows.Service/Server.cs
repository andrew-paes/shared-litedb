using LiteDB.Windows.Service.Context;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace LiteDB.Windows.Service
{
    public class Server
    {
        readonly System.Timers.Timer _timer = new System.Timers.Timer();
        public string _fdb = @"Filename=D:\LiteDB\Shared.db";
        public Random _rnd = new Random();
        public object thisLock = new object();

        private delegate void delegateForTaskInsert(int threadId);

        public Server()
        {

        }

        /// <summary>
        /// Destructor.
        /// </summary>
        ~Server()
        {
            StopServer();
        }

        /// <summary>
        /// 
        /// </summary>
        public void StartServer()
        {
            //_timer.Elapsed += Timer_Elapsed;
            //_timer.Interval = 10000;
            //_timer.Enabled = true;

            int totalThreads = 5000;
            bool flagJoin = false;

            using (var db = new LiteDatabase(_fdb)) // To create the file previously
            {
                var col = db.GetCollection<Log>("logs");

                var context = new Log { Name = "Thread_Main", CreatedDate = DateTime.Now, StartedOn = DateTime.Now };

                col.Insert(context);
            }

            List<Action> actionsList = new List<Action>();

            for (int i = 0; i < totalThreads; i++)
            {
                int threadId = (i + 1);

                using (var db = new LiteDatabase(_fdb))
                {
                    LiteCollection<Log> col = db.GetCollection<Log>("logs");

                    Log context = new Log { Name = String.Concat("Thread_", threadId), CreatedDate = DateTime.Now };

                    col.Insert(context);
                }

                void action() => TaskInsert(threadId);

                actionsList.Add(action);
            }

            Parallel.ForEach(actionsList, (o => o()));

            /*
            Thread[] threads = new Thread[totalThreads];

            for (int i = 0; i < totalThreads; i++)
            {
                Thread t = new Thread(new ParameterizedThreadStart(TaskInsert));

                threads[i] = t;
            }

            for (int i = 0; i < totalThreads; i++)
            {
                int threadId = (i + 1);

                using (var db = new LiteDatabase(_fdb))
                {
                    LiteCollection<Log> col = db.GetCollection<Log>("logs");

                    Log context = new Log { Name = String.Concat("Thread_", threadId), CreatedDate = DateTime.Now };

                    col.Insert(context);
                }

                threads[i].Start(threadId);
            }

            if (flagJoin)
            {
                for (int i = 0; i < totalThreads; i++)
                {
                    threads[i].Join();
                }
            }
            */

            //lock (thisLock)
            {
                using (var db = new LiteDatabase(_fdb))
                {
                    LiteCollection<Log> col = db.GetCollection<Log>("logs");

                    Log result = col.FindOne(x => x.Name.Equals("Thread_Main"));

                    if (result != null)
                    {
                        result.FinishedOn = DateTime.Now;
                        result.ModifiedDate = DateTime.Now;
                        result.TotalMiliSeconds = (int)(DateTime.Now - result.StartedOn.Value).TotalMilliseconds;

                        col.Update(result);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void StopServer()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                using (var db = new LiteDatabase(@"Filename=D:\LiteDB\Shared.db"))
                {
                    var col = db.GetCollection<State>("states");

                    var context = new State { Name = "SÃO PAULO", Code = "SP" };

                    col.Insert(context);

                    //Thread.Sleep(2000);

                    context.Name = "RIO GRANDE DO SUL";

                    col.Update(context);

                    col.EnsureIndex(x => x.Name); // Index document using a document property

                    var result = col.Find(x => x.Name.StartsWith("RI")); // Simple Linq support
                }
            }
            catch (Exception ex)
            {

            }
        }

        public void TaskInsert(object threadId)
        {
            //lock (thisLock)
            {
                string threadName = String.Concat("Thread_", (int)threadId);
                string id = Guid.NewGuid().ToString();
                string code = id.Substring(0, 4);
                int totalBytes = 0; // 1 * 1024 * (1025 / _rnd.Next(1, 5)); //A random total bytes between 6 and 30 Mb
                byte[] file = new byte[totalBytes];
                int sleepingMiliSeconds = 0; // _rnd.Next(200);

                try
                {
                    //Update Ini Thread Ini
                    Log logResult = new Log();

                    using (var db = new LiteDatabase(_fdb))
                    {
                        LiteCollection<Log> col = db.GetCollection<Log>("logs");

                        logResult = col.FindOne(x => x.Name.Equals(threadName));
                    }

                    using (var db = new LiteDatabase(_fdb))
                    {
                        if (logResult != null && logResult.Id > 0)
                        {
                            LiteCollection<Log> col = db.GetCollection<Log>("logs");

                            logResult.StartedOn = DateTime.Now;
                            logResult.SleepingMiliseconds = sleepingMiliSeconds;
                            logResult.TotaBytes = totalBytes;

                            col.Update(logResult);

                            logResult = new Log();
                        }
                    }

                    //System.Threading.Thread.Sleep(sleepingMiliSeconds);

                    ////Upload Stream
                    //using (var db = new LiteDatabase(_fdb))
                    //{
                    //    db.FileStorage.Upload(id, String.Concat(id, ".txt"), new MemoryStream(file));
                    //}

                    //Insert State
                    using (var db = new LiteDatabase(_fdb))
                    {
                        LiteCollection<State> col = db.GetCollection<State>("states");

                        State context = new State { Name = id, Code = code };

                        col.Insert(context);

                        col.EnsureIndex(x => x.Name);

                        var list = col.Find(x => x.Name.StartsWith(code));
                    }

                    //Update Thread End
                    using (var db = new LiteDatabase(_fdb))
                    {
                        LiteCollection<Log> col = db.GetCollection<Log>("logs");

                        logResult = col.FindOne(x => x.Name.Equals(threadName));
                    }

                    using (var db = new LiteDatabase(_fdb))
                    {
                        if (logResult != null && logResult.Id > 0)
                        {
                            LiteCollection<Log> col = db.GetCollection<Log>("logs");

                            logResult.FinishedOn = DateTime.Now;
                            logResult.ModifiedDate = DateTime.Now;

                            if (logResult.StartedOn != null && logResult.StartedOn.Value != null)
                            {
                                logResult.TotalMiliSeconds = (int)(DateTime.Now - logResult.StartedOn.Value).TotalMilliseconds;
                                logResult.RealJobMiliSeconds = (int)(DateTime.Now - logResult.StartedOn.Value).TotalMilliseconds - sleepingMiliSeconds;
                            }
                            else
                            {
                                logResult.TotalMiliSeconds = null;
                                logResult.RealJobMiliSeconds = null;
                            }

                            col.Update(logResult);

                            logResult = new Log();
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (EventLog eventLog = new EventLog("Application"))
                    {
                        eventLog.Source = "Application";
                        eventLog.Log = "Application";
                        eventLog.WriteEntry(String.Concat("Ex: ", ex.Message, "\r\n StackTrace: ", ex.StackTrace), EventLogEntryType.Error, 4001);
                    }
                }
            }
        }
    }
}