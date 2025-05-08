using System;
using UnityEngine;

namespace _Scripts.NoiseSystem
{
    public abstract class NoiseLayer
    {
        protected Vector2 _offsetFromWorldOrigin;
        public abstract float Evaluate(float x, float y);
    }
}