using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UltimateNoiseLibrary
{
    /// <summary>
    /// Base class for noise containing all relevant logic regarding seed.
    /// <br/> All noise types must inherit this class.
    /// </summary>
    public abstract class Noise
    {
        public int Seed { get; private set; }
        public abstract string Name { get; }
        protected System.Random RNG { get; private set; }

        public Noise()
        {
            RandomizeSeed();
        }
        public Noise(int seed)
        {
            SetSeed(seed);
        }

        public void SetSeed(int newSeed)
        {
            Seed = newSeed;
            RNG = new System.Random(Seed);
            OnNewSeed();
        }

        /// <summary>
        /// Gets called when the noise receives a new seed to set new random values.
        /// <br/> RNG has already been set with the new seed when this gets called.
        /// </summary>
        protected virtual void OnNewSeed() { }

        public void RandomizeSeed()
        {
            int randomSeed = Random.Range(int.MinValue / 2, int.MaxValue / 2);
            SetSeed(randomSeed);
        }

        protected float GetRandomFloat(float min, float max)
        {
            float range = max - min;
            return (float)(RNG.NextDouble() * range + min);
        }

        public abstract Sprite CreateTestSprite(int size = 128);
    }
}
