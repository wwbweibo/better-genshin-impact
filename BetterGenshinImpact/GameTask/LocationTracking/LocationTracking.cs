using BetterGenshinImpact.Core.Recognition;
using BetterGenshinImpact.Core.Simulator;
using BetterGenshinImpact.GameTask.AutoSkip.Assets;
using BetterGenshinImpact.GameTask.Model;
using Microsoft.Extensions.Logging;
using cvAutoTrack;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterGenshinImpact.GameTask.LocationTracking
{
    public class LocationTracker : ITaskTrigger
    {

        public string Name => "LocationTracking";

        public bool IsEnabled { get; set; }

        public int Priority => 999;

        public bool IsExclusive => false;

        public void Init()
        {
            var info = TaskContext.Instance().SystemInfo;
            if (!cvAutoTrack.cvAutoTrack.init())
            {
                Common.TaskControl.Logger.LogWarning("初始化cvAutoTrack失败");
            }
            if (!cvAutoTrack.cvAutoTrack.SetUseBitbltCaptureMode())
            {
                Common.TaskControl.Logger.LogWarning("设置BitBlt截图方式失败");
            }
            var handle = info.GameProcess.Handle;
            if (!cvAutoTrack.cvAutoTrack.SetHandle(0)) {
                Common.TaskControl.Logger.LogWarning("设置游戏句柄失败");
            }
            if (!cvAutoTrack.cvAutoTrack.startServe())
            {
                Common.TaskControl.Logger.LogWarning("启动cvAutoTrack失败");
            }
            Common.TaskControl.Logger.LogInformation("初始化cvAutoTrack成功");
            IsEnabled = true;
        }

        public void OnCapture(CaptureContent content)
        {
            double x = 0, y = 0, a = 0;
            int mapId = 0;
            cvAutoTrack.cvAutoTrack.GetTransformOfMap(ref  x, ref  y, ref  a, ref  mapId);
            Common.TaskControl.Logger.LogInformation("用户当前位置: {x}, {y}, {z}", x, y, a);
        }

        ~LocationTracker()
        {
            cvAutoTrack.cvAutoTrack.stopServe();
            cvAutoTrack.cvAutoTrack.uninit();
        }
    }

}
