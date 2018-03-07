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
    public class AchievementManager : MonoBehaviour
    {
        public const string achievementTimeStatName = "CurrentAchievementTime";

        public GenericAchievement[] achievementTemplates;
        public IAchievement currenAchievement;

        private StatsManager statsManager;

        private void Awake()
        {
            GameObjectUtil.RegisterSingleton<AchievementManager>(this);
        }

        private void Start()
        {
            statsManager = GameObjectUtil.GetSingleton<StatsManager>();
        }

        // Update is called once per frame
        private void Update()
        {
            if (currenAchievement != null)
                if (!currenAchievement.IsCompleted())
                {
                    currenAchievement.GetCompletionPercent();
                    statsManager.UpdateStatValue(achievementTimeStatName, Time.deltaTime);
                }
        }

        public float GetCompletionPercent()
        {
            //TODO for now this is hardcoded
            return currenAchievement != null ? currenAchievement.GetCompletionPercent() : 0;
        }

        public IAchievement PickAchievement()
        {
            if (achievementTemplates.Length < 1)
            {
                Debug.Log("No Achivements to select");
                return null;
            }

            return SetCurrentAchievement(achievementTemplates[Random.Range(0, achievementTemplates.Length - 1)]
                .Clone());
        }

        public IAchievement SetCurrentAchievement(IAchievement target)
        {
            currenAchievement = target;
            currenAchievement.Activate();

            RestartTimer();

            return currenAchievement;
        }

        public void RestartTimer()
        {
            statsManager.ResetStat(achievementTimeStatName);
        }
    }
}