using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace UltimateNoiseLibrary
{
    public class Node : MonoBehaviour
    {
        private NodeEditor NodeEditor;
        public GradientNoise[] Inputs;
        public GradientNoise Output;
        public Connection[] InputConnections;
        public List<Connection> OutputConnections = new List<Connection>();

        [Header("Elements")]
        public string NodeDisplayName;
        public GameObject Header;
        public TMP_Text Title;
        public GameObject InfoButton;
        public GameObject RemoveButton;
        public SpriteRenderer Preview;
        public GameObject OutputButton;
        public GameObject[] InputButtons;

        public GameObject TooltipContainer;
        public TMP_Text TooltipText;

        private NodeAttributes Attributes;

        // Drag
        private bool IsDragging;
        private Vector3 DragOffset;

        public void Init(NodeEditor editor)
        {
            NodeEditor = editor;
            Title.text = NodeDisplayName;

            Inputs = new GradientNoise[InputButtons.Length];
            InputConnections = new Connection[InputButtons.Length];

            Attributes = GetComponent<NodeAttributes>();
            Attributes.Init(this);

            TooltipText.text = Attributes.Description;
        }

        public void Update()
        {
            // Check buttons
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);
            if (hit.collider != null)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (hit.transform.gameObject == Header)
                    {
                        IsDragging = true;
                        DragOffset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        NodeEditor.IsDraggingCamera = false;
                    }
                    if (hit.transform.gameObject == RemoveButton)
                    {
                        NodeEditor.RemoveNode(this);
                        return;
                    }
                    if (hit.transform.gameObject == OutputButton)
                    {
                        NodeEditor.StartConnecting(this);
                        return;
                    }
                    for (int i = 0; i < InputButtons.Length; i++)
                    {
                        if (hit.transform.gameObject == InputButtons[i])
                        {
                            NodeEditor.ConnectTo(this, i);
                            return;
                        }
                    }


                    if (hit.transform.gameObject != null && Attributes != null)
                    {
                        Attributes.OnClick(hit.transform.gameObject);
                    }
                }

                if (Input.GetMouseButtonDown(1))
                {
                    for (int i = 0; i < InputButtons.Length; i++)
                    {
                        if (hit.transform.gameObject == InputButtons[i])
                        {
                            if (InputConnections[i] != null) NodeEditor.RemoveConnection(InputConnections[i]);
                            return;
                        }
                    }
                }
            }

            // Tooltip
            TooltipContainer.SetActive(hit.collider != null && hit.transform.gameObject == InfoButton);

            // Drag & Drop
            if (Input.GetMouseButtonUp(0)) IsDragging = false;

            if (IsDragging)
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                transform.position = new Vector3(mousePos.x + DragOffset.x, mousePos.y + DragOffset.y, transform.position.z);
                NodeEditor.OnNodeMove(this);
            }
        }

        public void SetInput(int index, GradientNoise noise)
        {
            Inputs[index] = noise;
            RecalculateOutput();
        }

        public void RecalculateOutput()
        {
            Output = Attributes.GetOutput();
            UpdateOutputPreview();

            // Propagate output
            foreach (Connection outputCon in OutputConnections)
                outputCon.To.SetInput(outputCon.ToInputIndex, Output);
        }

        private void UpdateOutputPreview()
        {
            if (Output == null)
            {
                Preview.sprite = NodeEditor.EmptySprite;
                return;
            }

            Preview.sprite = Output.CreateTestSprite(Attributes.DISPLAY_SIZE);
        }

        public List<Node> GetAllPreviousNodes()
        {
            List<Node> nodes = new List<Node>();
            foreach (Connection inputCon in InputConnections)
            {
                if (inputCon != null)
                {
                    nodes.Add(inputCon.From);
                    nodes.AddRange(inputCon.From.GetAllPreviousNodes());
                }
            }
            return nodes;
        }
    }
}
