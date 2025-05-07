using System;

namespace _Scripts.NoiseSystem
{
    public abstract class NoiseLayer
    {
        public bool Enabled { get; set; } = true;
        public abstract float Evaluate(float x, float y);
    }
}