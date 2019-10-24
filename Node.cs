using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageQuantization
{
    public class Node
    {
        public int Value;
        public int frequency;
        public Node left;
        public Node right;
        public Node (int c, int f)
        {
            Value = c;
            frequency = f;
        }
        public Node (int c, int f, Node l,Node r)
        {
            Value = c;
            frequency = f;
            left = l;
            right = r;
        }

    }
}
