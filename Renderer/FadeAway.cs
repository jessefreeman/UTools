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

using UnityEngine;

namespace jessefreeman.utools
{
    public class FadeAway : MonoBehaviour
    {
        private bool _readyToDissapear;
        public bool autoStart;
        public float delay = 2;
        private Color end;
        private RecycleGameObject recycleGameObject;

        public float speedFactor = 0.5f; // Larger values are slower

        private SpriteRenderer spriteRenderer;
        private Color start;
        private float startTime;
        private float t;

        public bool readyToDissapear
        {
            get { return _readyToDissapear; }

            set
            {
                _readyToDissapear = value;
                startTime = Time.time;
            }
        }

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            recycleGameObject = GetComponent<RecycleGameObject>();
            start = spriteRenderer.color;
            end = new Color(start.r, start.g, start.b, 0.0f);
        }

        private void Start()
        {
            readyToDissapear = autoStart;
        }

        private void Update()
        {
            if (readyToDissapear && Time.time - startTime > delay)
            {
                t += Time.deltaTime;
                spriteRenderer.material.color = Color.Lerp(start, end, t / speedFactor);
                if (spriteRenderer.material.color.a <= 0.0)
                {
                    if (recycleGameObject != null)
                        recycleGameObject.Shutdown();
                    else
                        Destroy(gameObject);

                    readyToDissapear = false;
                }
            }
        }

        public void Restart()
        {
//        if (recycleGameObject != null)
//            recycleGameObject.Restart();
            t = 0.0f;
            spriteRenderer.material.color = start;
            readyToDissapear = autoStart;
        }
    }
}