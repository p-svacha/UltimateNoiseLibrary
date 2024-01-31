using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UltimateNoiseLibrary
{
    public abstract class NodeAttributes : MonoBehaviour
    {
        protected Node Node;
        public abstract string Description { get; }
        public virtual int DISPLAY_SIZE => 128;

        public void Init(Node node)
        {
            Node = node;
            OnInit();
        }

        protected virtual void OnInit() { }
        public abstract GradientNoise GetOutput();
        public virtual void OnClick(GameObject target) { }
    }
}
