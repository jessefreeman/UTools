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
    public class ChangeSpriteID : MonoBehaviour
    {
        public int defaultSprite;
        public bool randomStart;
        public Sprite[] spriteReferences;

        private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();

            if (randomStart)
                defaultSprite = Random.Range(0, spriteReferences.Length - 1);

            // Need to pull out the first item which 
            ChangeSprite(defaultSprite);
        }

        public void ChangeSprite(int id)
        {
            if (id >= spriteReferences.Length || id < 0)
                return;

            spriteRenderer.sprite = spriteReferences[id];
        }

        public void RandomSprite()
        {
            ChangeSprite(Random.Range(0, spriteReferences.Length - 1));
        }
    }
}