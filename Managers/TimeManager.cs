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
    public class TimeManager : MonoBehaviour
    {
        public void ManipulateTime(float newTime, float duration)
        {
            if (Time.timeScale == 0)
                Time.timeScale = 0.1f;

            StartCoroutine(FadeTo(newTime, duration));
        }

        private IEnumerator FadeTo(float value, float time)
        {
            for (var t = 0f; t < 1; t += Time.deltaTime / time)
            {
                Time.timeScale = Mathf.Lerp(Time.timeScale, value, t);

                if (Mathf.Abs(value - Time.timeScale) < .01f)
                {
                    Time.timeScale = value;
                    yield break;
                }

                yield return null;
            }
        }
    }
}