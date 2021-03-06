﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Delta.Audio
{
    public interface ISound : IPoolable
    {
        string Name
        {
            get;
        }

        Action<string> OnFinished
        {
            get;
            set;
        }

        void Play();
        void Pause();
        void Resume();
        void Stop();
        bool IsPlaying();

        void SetVolume(float amount);
        void SetPitch(float amount);

        bool Update(DeltaGameTime time);
    }
}
