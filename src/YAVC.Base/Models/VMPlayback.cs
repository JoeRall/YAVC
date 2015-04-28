using System;
using System.Linq;
using System.Threading.Tasks;
using YAVC.Base.Commands;
using YAVC.Base.Data;
using YAVC.Base.Requests;

namespace YAVC.Base.Models
{
    public class VMPlayback : AVM
    {

        public MetaInfo[] Infos { get; set; }
        public PlaybackInfo PlayInfo { get; set; }

        public bool CanPlay { get { return Control.CanPlay && !IsPlaying; } }
        public bool CanPause { get { return Control.CanPause && PlayInfo == PlaybackInfo.Play; } }
        public bool CanStop { get { return Control.CanStop; } }
        public bool CanNext { get { return Control.CanNext; } }
        public bool CanPrevious { get { return Control.CanPrev; } }

        public bool IsPlaying { get { return PlayInfo == PlaybackInfo.Play; } }
        public bool IsPaused { get { return PlayInfo == PlaybackInfo.Pause; } }
        public bool IsStopped { get { return PlayInfo == PlaybackInfo.Stop; } }

        public string PlaybackString
        {
            get
            {
                switch (PlayInfo)
                {
                    case PlaybackInfo.Play:
                        return "Playing";
                    case PlaybackInfo.Pause:
                        return "Paused";
                    case PlaybackInfo.Stop:
                        return "Stopped";
                    case PlaybackInfo.Unknown:
                    default:
                        return "Unknown";
                }
            }
        }

        private PlayControl Control;
        private bool InvalidState
        {
            get { return null == Zone || null == Zone.SelectedInput || string.IsNullOrEmpty(Zone.SelectedInput.Src_Name); }
        }
        private VMZone Zone;

        public VMPlayback(VMZone zone)
            : base(zone.TheController)
        {
            Zone = zone;
            Control = new PlayControl();
        }

        public async Task Refresh()
        {
            if (null == Zone.SelectedInput || string.IsNullOrEmpty(Zone.SelectedInput.Src_Name))
            {
                Infos = new MetaInfo[0];
                PlayInfo = PlaybackInfo.Unknown;
                UI.Invoke(NotifyAll);
                return;
            }
            var info = new PlayInfo(Zone.SelectedInput);
            var result = await info.SendAsync(TheController.RequestProccessor);

            if (result.Success)
            {
                Infos = info.MetaInfo;
                PlayInfo = info.Playback;
                UpdateCans();
                UI.Invoke(NotifyAll);
            }
            else
            {
                Infos = new MetaInfo[0];
                PlayInfo = PlaybackInfo.Unknown;
            }
        }

        #region Playback Methods
        public async Task Next()
        {
            await Skip(CanNext, SkipDirection.Forward);
        }

        public async Task Pause()
        {
            await ChangePlayback(CanPause, PlaybackInfo.Pause);
        }

        public async Task Play()
        {
            await ChangePlayback(CanPlay, PlaybackInfo.Play);
        }

        public async Task Previous()
        {
            await Skip(CanPrevious, SkipDirection.Backward);
        }

        public async Task Stop()
        {
            await ChangePlayback(CanStop, PlaybackInfo.Stop);
        }
        #endregion

        #region Helper Methods
        private async Task ChangePlayback(bool canChange, PlaybackInfo mode)
        {
            if (!canChange || InvalidState) return;
            var pb = new Playback(Zone.SelectedInput, mode);
            var result = await pb.SendAsync(TheController.RequestProccessor);
            await RefreshOnSuccess(result);
        }

        private async Task RefreshOnSuccess(SendResult result)
        {
            if (result.Success) await Refresh();
        }

        private async Task Skip(bool canSkip, SkipDirection direction)
        {
            if (!canSkip || InvalidState) return;
            var skip = new Skip(Zone.SelectedInput, direction);
            var result = await skip.SendAsync(TheController.RequestProccessor);
            await RefreshOnSuccess(result);
        }

        private void UpdateCans()
        {
            if (
                null == TheController || null == TheController.Sources ||
                null == Zone || null == Zone.SelectedInput)
            {
                Control = new PlayControl();
                return;
            }
            var source = TheController.Sources.FirstOrDefault(s => s.SourceName == Zone.SelectedInput.Src_Name);

            if (null == source || null == source.Control)
            {
                Control = new PlayControl();
                return;
            }

            Control = source.Control;
        }
        #endregion
    }
}
