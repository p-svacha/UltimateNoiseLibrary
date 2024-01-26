using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScaleNode : NodeAttributes
{
    public override string Description => "Scales the input noise by a given value.";

    [Header("Elements")]
    public TMP_InputField Input;

    protected override void OnInit()
    {
        Input.onValueChanged.AddListener((x) => Node.RecalculateOutput());
    }


    public override GradientNoise GetOutput()
    {
        if (Node.Inputs[0] == null) return null;
        if (Input.text == "") return null;

        float scale = float.Parse(Input.text);

        return new ModularGradientNoise(Node.Inputs, new ScaleOperation(scale));
    }
}
