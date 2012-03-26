using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace EatMyDust
{
    /// <summary>
    /// Class used to play sound effects.
    /// For in-game feedback sounds, use playSound.
    /// </summary>
    public static class SoundManager
    {
        /// <summary>
        /// Starts a sound. Used for ambient sounds
        /// </summary>
        /// <param name="sound">The sound to play</param>
        public static void startLoopedSound(SoundEffectInstance sound, float volume)
        {
            if (sound.State == SoundState.Stopped)
            {
                sound.Volume = volume;
                sound.IsLooped = true;
                sound.Play();
            }
            else
            {
                sound.Resume();
            }
        }

        /// <summary>
        /// Plays a sound effect,(not a song)
        /// </summary>
        /// <param name="sound">The SoundEffectInstance to play</param>
        /// <param name="volume">The volume of the sound. 
        /// 0.01f is the volume of the background sound, 0.6f is in-game effects.</param>
        public static void playSound(SoundEffectInstance sound, float volume)
        {
            if (sound.State == SoundState.Stopped)
            {
                sound.Volume = volume;
                sound.Play();
            }
            else
            {
                sound.Resume();
            }
        }

        /// <summary>
        /// Pauses a sound effect
        /// </summary>
        /// <param name="sound">The SoundEffectInstance to pause</param>
        public static void pauseSound(SoundEffectInstance sound)
        {
            if (sound.State == SoundState.Playing)
                sound.Pause();
        }

        /// <summary>
        /// Stops the sound effect.
        /// </summary>
        /// <param name="sound">The SoundEffectInstance to stop</param>
        public static void stopSound(SoundEffectInstance sound)
        {
            if (sound.State == SoundState.Playing)
                sound.Stop();
        }
    }
}