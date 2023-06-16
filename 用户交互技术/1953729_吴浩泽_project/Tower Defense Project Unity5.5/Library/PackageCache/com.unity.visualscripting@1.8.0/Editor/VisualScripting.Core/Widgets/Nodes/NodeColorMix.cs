using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    public struct NodeColorMix : IEnumerable<KeyValuePair<NodeColor, float>>
    {
        public float gray { get; set; }
        public float blue { get; set; }
        public float teal { get; set; }
        public float green { get; set; }
        public float yellow { get; set; }
        public float orange { get; set; }
        public float red { get; set; }

        public static NodeColorMix TealReadable
        {
            get
            {
                if (EditorGUIUtility.isProSkin)
                {
                    return new NodeColorMix() { teal = 1, gray = 0.25f };
                }
                else
                {
                    return new NodeColorMix() { teal = 1, gray = 0 };
                }
            }
        }

        public NodeColorMix(NodeColor color) : this()
        {
            this[color] = 1;
        }

        public static implicit operator NodeColorMix(NodeColor color)
        {
            return new NodeColorMix(color);
        }

        public IEnumerable<KeyValuePair<NodeColor, float>> colors => this;

        public bool IsPure(NodeColor color)
        {
            if (this[color] == 0)
            {
                return false;
            }

            foreach (var _color in GraphGUI.nodeColors)
            {
                if (_color != color && this[_color] != 0)
                {
                    return false;
                }
            }

            return true;
        }

        public void Normalize()
        {
            var sum = gray + blue + teal + green + yellow + orange + red;

            gray /= sum;
            blue /= sum;
            teal /= sum;
            green /= sum;
            yellow /= sum;
            orange /= sum;
            red /= sum;
        }

        public NodeColorMix normalized
        {
            get
            {
                var sum = gray + blue + teal + green + yellow + orange + red;

                return new NodeColorMix()
                {
                    gray = gray / sum,
                    blue = blue / sum,
                    teal = teal / sum,
                    green = green / sum,
                    yellow = yellow / sum,
                    orange = orange / sum,
                    red = red / sum
                };
            }
        }

        public float this[NodeColor color]
        {
            get
            {
                switch (color)
                {
                    case NodeColor.Gray:
                        return gray;
                    case NodeColor.Blue:
                        return blue;
                    case NodeColor.Teal:
                        return teal;
                    case NodeColor.Green:
                        return green;
                    case NodeColor.Yellow:
                        return yellow;
                    case NodeColor.Orange:
                        return orange;
                    case NodeColor.Red:
                        return red;
                    default:
                        throw new UnexpectedEnumValueException<NodeColor>(color);
                }
            }
            set
            {
                value = Mathf.Clamp01(value);

                switch (color)
                {
                    case NodeColor.Gray:
                        gray = value;
                        return;
                    case NodeColor.Blue:
                        blue = value;
                        return;
                    case NodeColor.Teal:
                        teal = value;
                        return;
                    case NodeColor.Green:
                        green = value;
                        return;
                    case NodeColor.Yellow:
                        yellow = value;
                        return;
                    case NodeColor.Orange:
                        orange = value;
                        return;
                    case NodeColor.Red:
                        red = value;
                        return;
                    default:
                        throw new UnexpectedEnumValueException<NodeColor>(color);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<KeyValuePair<NodeColor, float>> GetEnumerator()
        {
            // TODO: Optimize away allocs
            // Returns the color values from highest to lowest alpha for drawing

            var mix = this;

            foreach (var kvp in GraphGUI.nodeColors.Select(c => new KeyValuePair<NodeColor, float>(c, mix[c]))
                     .Where(kvp => kvp.Value != 0)
                     .OrderByDescending(kvp => kvp.Value))
            {
                yield return kvp;
            }
        }

        public void PopulateColorsList(List<KeyValuePair<NodeColor, float>> list) // No-alloc
        {
            list.Clear();

            foreach (var nodeColor in GraphGUI.nodeColors)
            {
                if (this[nodeColor] > 0)
                {
                    list.Add(new KeyValuePair<NodeColor, float>(nodeColor, this[nodeColor]));
                }
            }

            list.Sort((a, b) => b.Value.CompareTo(a.Value));
        }
    }
}
