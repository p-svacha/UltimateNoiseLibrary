using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CutoffNode : NodeAttributes
{
    public override string Description => "All values below the minimum are set to 0. Are values above the maximum are set to 1. All values in-between are normalized";

    [Header("Elements")]
    public TMP_InputField Min;
    public TMP_InputField Max;

    protected override void OnInit()
    {
        Min.onValueChanged.AddListener((x) => Node.RecalculateOutput());
        Max.onValueChanged.AddListener((x) => Node.RecalculateOutput());
    }


    public override GradientNoise GetOutput()
    {
        if (Node.Inputs[0] == null) return null;
        if (Min.text == "") return null;
        if (Max.text == "") return null;

        float min = float.Parse(Min.text);
        float max = float.Parse(Max.text);

        return new ModularGradientNoise(Node.Inputs, new CutoffOperation(min, max));
    }
}
