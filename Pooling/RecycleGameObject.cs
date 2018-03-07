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

using System.Collections.Generic;
using UnityEngine;

namespace jessefreeman.utools
{
    public interface IRecyle
    {
        void Restart();
        void Shutdown();
    }

    public class RecycleGameObject : MonoBehaviour
    {
        private List<IRecyle> recycleComponents;

        private void Awake()
        {
            var components = GetComponents<MonoBehaviour>();
            recycleComponents = new List<IRecyle>();
            foreach (var component in components)
                if (component is IRecyle)
                    recycleComponents.Add(component as IRecyle);
        }


        public void Restart()
        {
            gameObject.SetActive(true);

            if (recycleComponents == null)
                return;

            foreach (var component in recycleComponents) component.Restart();
        }

        public void Shutdown()
        {
            gameObject.SetActive(false);

            if (recycleComponents == null)
                return;

            foreach (var component in recycleComponents) component.Shutdown();
        }
    }
}