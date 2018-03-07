// Copyright 2015 - 2018 Jesse Freeman
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation the
// rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of
// the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using UnityEngine;

namespace jessefreeman.utools
{
    public class SoundManager : MonoBehaviour
    {
        private ObjectPool _soundPool;

        public AudioClip[] audioClips = new AudioClip[0];
        public bool mute;
        public int populateSoundObjectPool = 10;
        public GameObject soundTemplate;
        public float volume = 1f;

        private ObjectPool soundPool
        {
            get
            {
                if (_soundPool == null)
                {
                    var go = GameObject.Find(soundTemplate.name + "ObjectPool");
                    if (go != null) _soundPool = go.GetComponent<ObjectPool>();
                }

                return _soundPool;
            }
        }

        // Use this for initialization
        private void Awake()
        {
            if (!GameObjectUtil.HasSingleton<SoundManager>()) GameObjectUtil.RegisterSingleton<SoundManager>(this);

            volume = PlayerPrefs.GetInt("Volume", 1);
            mute = Convert.ToBoolean(PlayerPrefs.GetInt("Mute", 0));

            soundTemplate = new GameObject("RecycledSound");
            soundTemplate.AddComponent<RecycledSound>();
        }

        private void Start()
        {
            for (var i = 0; i < populateSoundObjectPool; i++)
            {
                var instance = GameObjectUtil.Instantiate(soundTemplate, new Vector3());
                instance.GetComponent<RecycleGameObject>().Shutdown();
            }
        }

        public virtual RecycledSound PlayClip(int id, Vector3 pos = new Vector3(), bool unique = false,
            float volume = 1f,
            bool selfDestruct = true, bool loop = false)
        {
            if (id >= audioClips.Length)
                return null;

            var clip = audioClips[id];
            var sRef = "Audio: " + clip.name;

            if (unique)
                if (GameObject.Find(sRef))
                    return null;

            var recycledSound = GameObjectUtil.Instantiate(soundTemplate, pos);
            recycledSound.name = sRef;
            var source = recycledSound.GetComponent<RecycledSound>();
            source.clip = clip;
            source.volume = volume;
            source.mute = mute;
            source.loop = loop;
            source.selfDestruct = selfDestruct;
            source.Play();

            return source;
        }

        public void MuteSounds()
        {
            volume = 0;
            UpdateActiveSoundInstances();
        }

        public void UnmuteSounds()
        {
            volume = PlayerPrefs.GetInt("Volume", 1);
            UpdateActiveSoundInstances(true);
        }

        private void UpdateActiveSoundInstances(bool usePreviousVolume = false)
        {
            if (soundPool == null)
                return;

            var sounds = soundPool.GetActiveInsatnces();

            foreach (var sound in sounds)
            {
                var instance = (RecycledSound) sound;
                instance.volume = usePreviousVolume ? instance.previousVolume : volume;
                instance.mute = mute;
            }
        }

        public bool ToggleMute()
        {
            mute = !mute;

            if (mute)
                MuteSounds();
            else
                UnmuteSounds();

            PlayerPrefs.SetInt("Mute", Convert.ToInt16(mute));

            return mute;
        }

        public void PauseSounds(bool value)
        {
            if (soundPool == null)
                return;

            var sounds = soundPool.GetActiveInsatnces();

            foreach (var sound in sounds)
            {
                var instance = (RecycledSound) sound;
                if (value)
                    instance.Pause();
                else
                    instance.Play();
            }
        }

        public void FadeActiveSounds(float start, float end, float durration = 1f)
        {
            var sounds = soundPool.GetActiveInsatnces();

            foreach (var sound in sounds)
            {
                var instance = (RecycledSound) sound;
                instance.FadeSound(start, end, durration);
            }
        }
    }
}