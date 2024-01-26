using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigPreviewNode : NodeAttributes
{
    public override string Description => "Displays the input way bigger.";
    public override int DISPLAY_SIZE => 256;

    public override GradientNoise GetOutput()
    {
        if (Node.Inputs[0] == null) return null;

        return Node.Inputs[0].GetCopy();
    }
}
