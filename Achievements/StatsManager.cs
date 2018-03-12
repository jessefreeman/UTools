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
using System.Collections.Generic;
using UnityEngine;

namespace jessefreeman.utools
{
    public class StatsManager : MonoBehaviour
    {
        
        public Dictionary<string, float> stats = new Dictionary<string, float>();

        // Use this for initialization
        private void Start()
        {
            Reset();
        }

        private void Update()
        {
            UpdateStatValue("Time", Time.deltaTime);
        }

        public void Reset()
        {
            stats.Clear();
        }

        public float UpdateStatValue(string stat, float value)
        {
            CreateKey(stat);

            stats[stat] += value;

            return stats[stat];
        }

        public float GetStatValue(string stat)
        {
            CreateKey(stat);

            return stats[stat];
        }

        public void ResetStat(string stat, float value = 0)
        {
            CreateKey(stat);

            stats[stat] = value;
        }

        private void CreateKey(string stat)
        {
            if (!stats.ContainsKey(stat)) stats.Add(stat, 0);
        }

    }
}