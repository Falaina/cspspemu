﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using CSPspEmu.Core;
using CSPspEmu.Core.Audio;
using CSPspEmu.Core.Audio.Impl.Openal;

namespace CSPspEmu.Runner.Components.Audio
{
	sealed public class AudioComponentThread : ComponentThread
	{
		private PspAudio PspAudio;

		public override void InitializeComponent()
		{
			PspAudio = PspEmulatorContext.GetInstance<PspAudio>();
		}

		protected override string ThreadName { get { return "AudioThread"; } }

		protected override void Main()
		{
			Thread.CurrentThread.Priority = ThreadPriority.AboveNormal;
			while (true)
			{
				ThreadTaskQueue.HandleEnqueued();
				if (!Running) return;

				PspAudio.Update();
				Thread.Sleep(1);
			}
		}
	}
}
