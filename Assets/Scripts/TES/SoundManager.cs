using System.Collections.Generic;
using System.IO;
using TES3Unity.ESM;
using UnityEngine;

namespace TES3Unity
{
    /// <summary>
    /// A proof of concept sound manager.
    /// </summary>
    public static class SoundManager
    {
        private static Dictionary<string, AudioClip> ClipStore = new Dictionary<string, AudioClip>();

        public static AudioClip GetAudioClip(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }

            if (ClipStore.ContainsKey(id))
            {
                return ClipStore[id];
            }

            var path = TES3Manager.MWDataReader.GetSound(id);
            if (!File.Exists(path))
            {
                return null;
            }

            var pcm = AudioUtils.ReadWAV(path);
            var clip = AudioUtils.CreateAudioClip(id, pcm);

            ClipStore.Add(id, clip);

            return clip;
        }
    }
}
