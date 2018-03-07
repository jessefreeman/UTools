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

using System.Collections;
using UnityEngine;

namespace jessefreeman.utools
{
    public class RecycledSound : RecycleGameObject
    {
        private AudioSource audioSource;
        private float durationScale = .1f;
        public bool fadingSounds;
        public bool selfDestruct = true;
        public float volumeBuffer;
        public float volumeEnd = 1;
        public float volumeStart;

        public RecycledSound()
        {
            previousVolume = 0;
        }

        public AudioClip clip
        {
            get { return audioSource.clip; }
            set { audioSource.clip = value; }
        }

        public float previousVolume { get; private set; }

        public float volume
        {
            get { return audioSource.volume; }
            set
            {
                previousVolume = audioSource.volume;
                audioSource.volume = value;
            }
        }

        public bool mute
        {
            get { return audioSource.mute; }
            set { audioSource.mute = value; }
        }

        public bool loop
        {
            get { return audioSource.loop; }
            set { audioSource.loop = value; }
        }

        public void Play()
        {
            audioSource.Play();
            if (selfDestruct)
                StartCoroutine(OnDone());
        }

        public void Pause()
        {
            audioSource.Pause();
        }

        private IEnumerator OnDone()
        {
            yield return new WaitForSeconds(audioSource.clip.length);
            Shutdown();
        }

        private void Awake()
        {
            var source = gameObject.GetComponent<AudioSource>();
            if (source != null && audioSource == null)
                audioSource = source;
            else
                audioSource = gameObject.AddComponent<AudioSource>();
        }

        public void FadeSound(float start, float end, float duration = 1f)
        {
            volume = start;
            volumeStart = start;
            volumeEnd = end;
            //volumeBuffer = start;
            fadingSounds = true;
            durationScale = duration;
        }

        private void Update()
        {
            if (fadingSounds)
            {
                //var durationScale = .1f;
                var scale = durationScale;

                if (volumeEnd < volumeStart) scale *= -1;

                volume += scale * Time.deltaTime;


                if (volumeEnd < volumeStart)
                {
                    if (volume <= volumeEnd)
                    {
                        volume = volumeEnd;
                        fadingSounds = false;
                    }
                }
                else
                {
                    if (volume >= volumeEnd)
                    {
                        volume = volumeEnd;
                        fadingSounds = false;
                    }
                }
            }
        }
    }
}