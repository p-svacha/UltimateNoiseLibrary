using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UltimateNoiseLibrary
{
    public class UI_NoisePreset : MonoBehaviour
    {
        private NodeEditor NodeEditor;

        [Header("Elements")]
        public Image PreviewImage;
        public TextMeshProUGUI NameText;
        public Button RandomizeButton;
        public Button CreateNodeButton;

        private GradientNoise Noise;

        public void Init(NodeEditor editor, GradientNoise noise)
        {
            NodeEditor = editor;
            Noise = noise;
            NameText.text = noise.Name;
            UpdatePreviewImage();
            RandomizeButton.onClick.AddListener(Randomize);
            CreateNodeButton.onClick.AddListener(() => NodeEditor.AddSourceNoiseNode(noise));
        }

        private void UpdatePreviewImage()
        {
            Sprite previewSprite = Noise.CreateTestSprite();
            PreviewImage.sprite = previewSprite;
        }

        private void Randomize()
        {
            Noise.RandomizeSeed();
            UpdatePreviewImage();
        }
    }
}
