using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading;
using RTEtherCAT;

namespace Test_read
{
    internal class MemoryMappedIPCService
    {

        ulong startTime = 0;
        ulong endTime = 0;
        private string _memorySharedFilePath;
        private string _protocolFilePath;

        private int _memorySharedFileSize = 13000;
        private MemoryMappedViewAccessor _memoryAccessor;

        private MemoryMappedFile _memoryMappedFile;

        private static Dictionary<string, List<ControlPoint>> _controlPointDict;

        public MemoryMappedIPCService(string memSharedFilePath, string protocolFilePath)
        {
            _memorySharedFilePath = memSharedFilePath;
            _protocolFilePath = protocolFilePath;
        }

        public Dictionary<string, List<ControlPoint>> ControlPointDict { get { return _controlPointDict; } }

        public void InitIPC()
        {
            InitProtocol();

            if (!File.Exists(_memorySharedFilePath))
            {
                using (FileStream fs = File.Create(_memorySharedFilePath))
                {
                    long offset = fs.Seek(_memorySharedFileSize - 2, SeekOrigin.Begin);
                    fs.WriteByte(new byte());
                    fs.Close();
                }
            }

            try
            {
                if (OperatingSystem.IsLinux())
                {
                    _memoryMappedFile = MemoryMappedFile.CreateFromFile(_memorySharedFilePath, FileMode.Open);
                }
                else if (OperatingSystem.IsWindows())
                {
                    // _memoryMappedFile = MemoryMappedFile.OpenExisting("test");
                    _memoryMappedFile = MemoryMappedFile.OpenExisting("Global\\test");
                }
                else
                {
                    throw new PlatformNotSupportedException();
                }

                _memoryAccessor = _memoryMappedFile.CreateViewAccessor();

                if (OperatingSystem.IsLinux())
                {
                    byte[] buffer = Encoding.UTF8.GetBytes("".PadRight(_memorySharedFileSize - 1000, ' '));
                    _memoryAccessor.WriteArray(0, buffer, 0, buffer.Length);
                }
            }
            catch (Exception ex)
            {
            }
        }
       

        public void ReadDataFromMemoryShared()
        {
            foreach (ControlPoint controlPoint in _controlPointDict["PEStatus"])
            {
                startTime = RealTimeTask.GetCurrentSystemTime();
                string data = ReadData(controlPoint);
                endTime = RealTimeTask.GetCurrentSystemTime();
                if((endTime-startTime)>1_000_000 *0.5)
                    Console.WriteLine($"{endTime - startTime}");
            }
        }
        private void InitProtocol()
        {
            if (!File.Exists(this._protocolFilePath))
            {

                throw new Exception($"IPCProtocol.json not exist. Path:{_protocolFilePath}");
            }

            _controlPointDict = new Dictionary<string, List<ControlPoint>>();

            StreamReader streamReader = File.OpenText(_protocolFilePath);
            string JsonText = streamReader.ReadToEnd();
            streamReader.Close();

            if (string.IsNullOrEmpty(JsonText)) return;

            JObject FormatDict = (JObject)JsonConvert.DeserializeObject(JsonText);
            foreach (JToken info in FormatDict[Key.ControlPoints].ToList())
            {
                {
                    ControlPoint controlPoint = new ControlPoint();
                    controlPoint.PE = info[Key.PE].ToObject<string>();
                    controlPoint.DivertModule = info[Key.DivertModule].ToObject<int>();
                    controlPoint.DivertLane = info[Key.DivertLane].ToObject<int>();
                    controlPoint.PickLane = info[Key.PickLane].ToObject<int>();
                    controlPoint.PowerSequence = info[Key.PowerSequence].ToObject<int>();
                    controlPoint.ControlPanel = info[Key.ControlPanel].ToObject<string>();
                    controlPoint.IOAlias = info[Key.IOAlias].ToObject<string>();
                    controlPoint.Bit = info[Key.Bit].ToObject<int>();
                    controlPoint.HasSubBit = info[Key.HasSubBit].ToObject<int>();
                    controlPoint.BitCount = info[Key.BitCount].ToObject<int>();
                    controlPoint.BitWidth = info[Key.BitWidth].ToObject<int>();
                    string function = controlPoint.IOAlias;
                    if (new List<string>() { "ScannerPE", "SendPE", "EntrancePE", "DivertSuccessPE", "DivertLaneFullPE" }.Contains(controlPoint.IOAlias))
                    {
                        function = "PEStatus";
                    }

                    if (controlPoint.IOAlias == "EStopNC" || controlPoint.IOAlias == "EStopNO")
                    {
                        function = "EStop";
                    }

                    if (new List<string>() { "RecycleLineUpstreamPE", "RecycleLineDownstreamPE", "MainLineUpstreamPE", "MainLineDownStreamPE" }.Contains(controlPoint.IOAlias) && controlPoint.DivertModule > -1)
                    {
                        function = "LoopMerge";
                    }

                    if (!_controlPointDict.ContainsKey(function))
                    {
                        _controlPointDict.Add(function, new List<ControlPoint>());
                    }

                    _controlPointDict[function].Add(controlPoint);
                }
            }
        }

        public void WriteData(long position, byte[] inputBuffer)
        {
            _memoryAccessor.WriteArray(position, inputBuffer, 0, inputBuffer.Length);
        }

        public string ReadData(ControlPoint controlPoint)
        {
            int index = controlPoint.Bit;
            int length = controlPoint.BitCount * controlPoint.BitWidth;
            string result = string.Empty;

            if (controlPoint.HasSubBit == 0)
            {
                byte[] tmpResult = new byte[length];
                _memoryAccessor.ReadArray(index, tmpResult, 0, tmpResult.Length);
                result = Encoding.UTF8.GetString(tmpResult);
            }
            else
            {
                //byte[] tmpResult = new byte[length];
                //_memoryAccessor.ReadArray(index, tmpResult, 0, tmpResult.Length);
                //result = Encoding.UTF8.GetString(tmpResult);
                //if ("1".Equals(result))
                //{
                //    for (int i = protocol.SubBit; i < protocol.SubBit + protocol.SubBitCount * protocol.SubBitWidth; i += protocol.SubBitWidth)
                //    {
                //        tmpResult = new byte[protocol.SubBitWidth];
                //        _memoryAccessor.ReadArray(i, tmpResult, 0, tmpResult.Length);
                //        if ("1".Equals(Encoding.UTF8.GetString(tmpResult)))
                //        {
                //            result += "," + protocol.MessageDetail[i - protocol.SubBit];
                //        }
                //    }
                //}
            }
            return result;
        }

        public void Dispose()
        {
            _controlPointDict.Clear();
            _memoryAccessor.Dispose();
            _memoryMappedFile.Dispose();
        }
    }

    public class ControlPoint
    {
        public string PE { get; set; }
        public int DivertModule { get; set; }
        public int DivertLane { get; set; }
        public int Merge { get; set; }
        public int MainLane { get; set; }
        public int PickLane { get; set; }
        public int PowerSequence { get; set; }
        public string ControlPanel { get; set; }
        public string IOAlias { get; set; }
        public int Bit { get; set; }
        public int HasSubBit { get; set; }
        public int BitCount { get; set; }
        public int BitWidth { get; set; }
        public string MemSharedValue { get; set; }
        public bool UploadStatus { get; set; }
    }

    public class Key
    {
        public static readonly string ControlPoints = "ControlPoints";
        public static readonly string PE = "PE";
        public static readonly string DivertModule = "DivertModule";
        public static readonly string DivertLane = "DivertLane";
        public static readonly string Merge = "Merge";
        public static readonly string MainLane = "MainLane";
        public static readonly string PickLane = "PickLane";
        public static readonly string PowerSequence = "PowerSequence";
        public static readonly string ControlPanel = "ControlPanel";
        public static readonly string IOAlias = "IOAlias";
        public static readonly string Bit = "Bit";
        public static readonly string HasSubBit = "HasSubBit";
        public static readonly string BitCount = "BitCount";
        public static readonly string BitWidth = "BitWidth";
        public static readonly string UploadStatus = "UploadStatus";
    }
}
