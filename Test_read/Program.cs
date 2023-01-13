using System;
using System.Collections.Generic;
using RTEtherCAT;


namespace Test_read
{

    class Program
    {
        public enum DivertStatus
        {
            Sent = 0,
            Failed = 1,
            Success = 2,
            Recuirate = 3
        }
        static void Main(string[] args)
        {
            Program.DivertStatus a = DivertStatus.Sent;
            string b = null;
            bool c = a == DivertStatus.Failed;
            


            //MemoryMappedIPCService memoryMappedIPCService = new MemoryMappedIPCService("/tmp/sharedfile", "EtherCatProtocol.json");
            //memoryMappedIPCService.InitIPC();

            //Test test1 = new Test(11111);
            //test1.RunRealTimeThread(memoryMappedIPCService.ReadDataFromMemoryShared, 99);

            ////Test test2 = new Test(22222);
            ////test2.RunRealTimeThread(null, 99);

            ////Test test3 = new Test(33333);
            ////test3.RunRealTimeThread(null, 99);

            ////Test test4 = new Test(44444);
            ////test2.RunRealTimeThread(null, 99);

            //Console.ReadKey();

            //test1.Running = false;
  
                
            //while (true)
            //{
            //    if (test1.IsExited)
            //    {
            //        break;
            //    }
            //}

        }
    }
    class Test
    {
        //private readonly ulong MILLIONS = 10_000_000;

        //private bool _running;
        //public bool Running { set { _running = value; } }

        //private int _count;

        //public int Count { get { return _count; } }

        //private bool _isExited;
        //public bool IsExited { get { return _isExited; } }

        //private RealTimeTask _realtimethread;

        //private int _threadNumber;

        //Action _action;
        //public Test(int threadNumber)
        //{
        //    _running = true;
        //    _isExited = false;
        //    _threadNumber = threadNumber;
        //    _count = 0;
        //}

        //public void RunRealTimeThread(Action action, int pro)
        //{
        //    _realtimethread = RealTimeTask.StartNewTask($"test{_threadNumber}", TestThread, pro);
        //    _action = action;
        //}

        //public void DisposeThead()
        //{
        //    _realtimethread.Dispose();
        //}

        //private void TestThread()
        //{
        //    ulong startTime = 0;
        //    ulong endTime = 0;

        //    ulong actionCount = 0;
        //    string record = "";

        //    List<string> outTimeCountList = new List<string>();

        //    while (_running)
        //    {
        //        if (_action != null)
        //        {
        //            startTime = RealTimeTask.GetCurrentSystemTime();
        //            _action();
        //            endTime = RealTimeTask.GetCurrentSystemTime();
        //            break;

        //            if ((endTime - startTime) > 0.1 * MILLIONS)
        //            {
        //                actionCount++;
        //                record = $"Action Count: {actionCount}, {endTime - startTime} us.";
        //                _count++;
        //            }
        //        }

        //        startTime = RealTimeTask.GetCurrentSystemTime();
        //        RTEtherCAT.RealTimeTask.Sleep(MILLIONS);
        //        endTime = RealTimeTask.GetCurrentSystemTime();

        //        if ((endTime - startTime) > 1.2 * MILLIONS)
        //        {
        //            record += $"Delay: {(endTime - startTime) / MILLIONS } ms.";
        //        }

        //        if (record != "")
        //        {
        //            outTimeCountList.Add(record);
        //            record = "";
        //        }
        //    }

        //    foreach (string outTime in outTimeCountList)
        //    {
        //        Console.WriteLine(outTime);
        //    }

        //    _isExited = true;
        //}
    }


}
