using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UltimateNoiseLibrary
{
    public class Connection : MonoBehaviour
    {
        public Node From;
        public Node To;
        public int ToInputIndex;
        public LineRenderer Line;

        public void UpdateLine()
        {
            Line.positionCount = 2;
            Line.SetPosition(0, From.OutputButton.transform.position);
            Line.SetPosition(1, To.InputButtons[ToInputIndex].transform.position);
        }
    }
}
