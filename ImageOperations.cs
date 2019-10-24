using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using Priority_Queue;
using ImageEncryptCompress;
using System.IO;


///Algorithms Project
///Intelligent Scissors
///

namespace ImageQuantization
{
    /// <summary>
    /// Holds the pixel color in 3 byte values: red, green and blue
    /// </summary>
    public struct RGBPixel
    {
        public byte red, green, blue;
    }
    public struct RGBPixelD
    {
        public double red, green, blue;
    }

    /// <summary>
    /// Library of static functions that deal with images
    /// </summary>
    public class ImageOperations
    {
        public static long SavedSeed;
        public static long pass(long seed, int pos, int len)  //ϴ(1)
        {
            long tap;
            long MostSign;
            long XOR;
            long ShiftedSeed;
            for (int i = 0; i < 8; i++) //ϴ(1)
            {
                MostSign = seed >> (len - 1); //ϴ(1)
                MostSign &= 1; //ϴ(1)
                tap = seed >> pos; //ϴ(1)
                tap &= 1; //ϴ(1)
                XOR = MostSign ^ tap; //ϴ(1)
                seed <<= 1; //ϴ(1)
                seed |= XOR; //ϴ(1)
            }
            ShiftedSeed = seed & 255; //ϴ(1)
            SavedSeed = seed; //ϴ(1)
            return ShiftedSeed;
        }
        public static RGBPixel[,] enc(int pass, RGBPixel[,] pic, int position, int len) //ϴ(n^2)
        {
            long ShiftedPassRED = ImageOperations.pass(pass, position, len); //ϴ(1)
            long ShiftedPassGREEN = ImageOperations.pass(SavedSeed, position, len); //ϴ(1)
            long ShiftedPassBLUE = ImageOperations.pass(SavedSeed, position, len);  //ϴ(1)     
            int width = ImageOperations.GetWidth(pic); //ϴ(1)
            int height = ImageOperations.GetHeight(pic); //ϴ(1)
            for (int i = 0; i < height; i++) //ϴ(n^2)
            {
                for (int j = 0; j < width; j++) //ϴ(n)
                {
                    pic[i, j].red ^= Convert.ToByte(ShiftedPassRED); //ϴ(1)
                    pic[i, j].green ^= Convert.ToByte(ShiftedPassGREEN); //ϴ(1)
                    pic[i, j].blue ^= Convert.ToByte(ShiftedPassBLUE); //ϴ(1) 
                    //Note that complexity of convert is O(n) ,but the number of n is limited (8) 
                    ShiftedPassRED = ImageOperations.pass(SavedSeed, position, len); //ϴ(1)
                    ShiftedPassGREEN = ImageOperations.pass(SavedSeed, position, len); //ϴ(1)
                    ShiftedPassBLUE = ImageOperations.pass(SavedSeed, position, len); //ϴ(1)
                }
            }
            return pic;
        }
        public static RGBPixel[,] dec(int pass, RGBPixel[,] pic, int position, int len) //ϴ(n^2)
        {
            RGBPixel[,] decrepted;
            decrepted = ImageOperations.enc(pass, pic, position, len); //ϴ(n^2)
            return decrepted;
        }

        public static int[] CountRedFrequency(RGBPixel[,] image) //ϴ(n^2)
        {

            int[] RedArr = new int[256]; //ϴ(1)
            int width = ImageOperations.GetWidth(image); //ϴ(1)
            int height = ImageOperations.GetHeight(image); //ϴ(1)
            for (int i = 0; i < height; i++) //ϴ(n^2)
            {
                for (int j = 0; j < width; j++) //ϴ(n)
                {

                    RedArr[image[i, j].red] += 1; //ϴ(1)
                }
            }


            return RedArr;

        }

        public static int[] CountGreenFrequency(RGBPixel[,] image) //ϴ(n^2)
        {
            int[] GreenArr = new int[256]; //ϴ(1)
            int width = ImageOperations.GetWidth(image); //ϴ(1)
            int height = ImageOperations.GetHeight(image); //ϴ(1)
            for (int i = 0; i < height; i++) //ϴ(n^2)
            {
                for (int j = 0; j < width; j++) //ϴ(n)
                {

                    GreenArr[image[i, j].green] += 1; //ϴ(1)
                }
            }
            return GreenArr;
        }
        public static int[] CountBlueFrequency(RGBPixel[,] image) //ϴ(n^2)
        {
            int[] BlueArr = new int[256]; //ϴ(1)
            int width = ImageOperations.GetWidth(image); //ϴ(1)
            int height = ImageOperations.GetHeight(image); //ϴ(1)
            for (int i = 0; i < height; i++) //ϴ(n^2)
            {
                for (int j = 0; j < width; j++) //ϴ(n)
                {

                    BlueArr[image[i, j].blue] += 1; //ϴ(1)

                }
            }
            return BlueArr;
        }
        
        public static FileStream fs = new FileStream(@"C:\Users\Lenovo\Desktop\[TEMPLATE] ImageEncryptCompressssss\Huff.bin",FileMode.Create);
        public static BinaryWriter s = new BinaryWriter(fs);
        public static StreamWriter writer = new System.IO.StreamWriter(@"C:\Users\Lenovo\Desktop\[TEMPLATE] ImageEncryptCompressssss\Huffman.txt");
        public static List<Node> fill_list(int[] arr) //O(n)
        {
            List<Node> l = new List<Node>();

            for (int i = 0; i <= 255; i++) //O(1)*O(n) => //O(n)
            {
                Node n = new Node(i, arr[i]); //ϴ(1)

                if (n.frequency != 0)
                {
                    l.Add(n); //O(n)
                }
            }
            return l;
        }


        public static SimplePriorityQueue<Node> Queue = new SimplePriorityQueue<Node>();
        public static void ENQ(List<Node> l) //O(N log N)
        {

            foreach (Node X in l) //O(N log N)
            {
                Queue.Enqueue(X, X.frequency); //O(log N)

            }
        }

        public static void HuffmanTree() //O(N log N)
        {
            
            while (Queue.Count != 1) //O(N log N)
            {
                Node left = Queue.Dequeue(); //O(log N)
                Node right = Queue.Dequeue(); //O(log N)
                int sum;
                sum = left.frequency + right.frequency; //ϴ(1)
                Node parent = new Node(256, sum, left, right); //ϴ(1)
                Queue.Enqueue(parent, parent.frequency); //O(log N)

            }

        }

        public static int sum = 0;
        public static int sum2 = 0;

        public static void encode(Node node, string code, string color) //O(n)
        {
            if (node.left == null && node.right == null) //ϴ(1)
            {
                if (!File.Exists(@"C:\Users\Lenovo\Desktop\[TEMPLATE] ImageEncryptCompressssss\Huffman.txt")) //ϴ(1)
                {
                    File.Create(@"C:\Users\Lenovo\Desktop\[TEMPLATE] ImageEncryptCompressssss\Huffman.txt"); //ϴ(1)
                }
                writer.WriteLine("Value={0},Frequency={1}, Path={2},Bits={3},Bits*Freq={4},TotalFreq={5},CompressionOutput={6},color={7}", node.Value,
                node.frequency, code, code.Length - 1, (node.frequency) * (code.Length - 1),
                sum += (node.frequency) * (code.Length - 1), sum2 += (node.frequency) * (code.Length - 1), color); //ϴ(1)           
                return;
            }
            if (node.right != null) //ϴ(1)
            {
                encode(node.right, code + "1", color); //O(n)

            }
            if (node.left != null) //ϴ(1)
            {
                encode(node.left, code + "0", color); //O(n)
            }
            sum = 0; //ϴ(1)

        }

        public static void SaveCodes(string color) //ϴ(n)
        {
            Node root = Queue.Dequeue(); //ϴ(log N)

           encode(root, " ", color); //ϴ(n)

        }

        public static void close() //ϴ(1)
        {
            writer.Close(); //ϴ(1)
        }

        public static long c = 0;
        public static int l = 0;
        public static path p;
        public static Dictionary<int, path> red = new Dictionary<int, path>();
        //public static void comp(Node node, string code) //O(n)
        //{

        //    if (node.left == null && node.right == null) //ϴ(1)
        //    {
        //        l = code.Length - 1; //ϴ(1)
        //        c = long.Parse((code)); //ϴ(1)   
        //        p.Len = l; //ϴ(1)
        //        p.Path = c; //ϴ(1)
        //        red.Add(node.Value, p); //O(1)
        //        return;
        //    }
        //    if (node.right != null) //ϴ(1)
        //    {
        //        comp(node.right, code + "1"); //O(n)

        //    }
        //    if (node.left != null) //ϴ(1)
        //    {
        //        comp(node.left, code + "0"); //O(n)
        //    }
        //}
        //public static void toComp() //O(n)
        //{
        //    comp(root, " "); //O(n)
        //}
        public static void compress(RGBPixel[,] m) //ϴ(n^2)
        {

            long y = 0;
            long x;
            int l;
            int acc = 0;
            int diff = 0;
            int bitsremaining = 0;
            int width = ImageOperations.GetWidth(m); //ϴ(1)
            int height = ImageOperations.GetHeight(m); //ϴ(1)
            for (int i = 0; i < height; i++) //ϴ(n^2)
            {
                for (int j = 0; j < width; j++) //ϴ(n)
                {
                    if (red.ContainsKey(m[i, j].red)) //ϴ(1)
                    {

                        bool c = red.TryGetValue((m[i, j].red), out p); //ϴ(1)
                        if (c) //ϴ(1)
                        {
                            x = p.Path; //ϴ(1)

                            l = p.Len; //ϴ(1)
                            bitsremaining = 64 - acc; //ϴ(1)

                            if (l == bitsremaining) //ϴ(1)
                            {
                                y = (y | x); //ϴ(1)
                                s.Write(y); //ϴ(1)
                                acc = 0; //ϴ(1)
                                y = 0; //ϴ(1)
                                
                            }

                            else if (l < bitsremaining) //ϴ(1)
                            {
                                acc += l; //ϴ(1)

                                y = (y | x) << bitsremaining; //ϴ(1)
                            }
                            else if (l > bitsremaining) //ϴ(1)
                            {
                                diff = l - bitsremaining; //ϴ(1)
                                y = y | (x >> diff); //ϴ(1)
                                s.Write(y); //ϴ(1)
                                y = 0; //ϴ(1)
                                acc = 0; //ϴ(1)
                                y = x << (64 - diff); //ϴ(1)
                                acc += diff; //ϴ(1)
                            }
                        }


                    }
                }
            }
        }
        public struct path
        {
            public int Len;
            public long Path;
        }
    
        //<summary>
        //Open an image and load it into 2D array of colors (size: Height x Width)
        //</summary>
        /// <param name="ImagePath">Image file path</param>
        /// <returns>2D array of colors</returns>
        public static RGBPixel[,] OpenImage(string ImagePath)
        {
            Bitmap original_bm = new Bitmap(ImagePath);
            int Height = original_bm.Height;
            int Width = original_bm.Width;

            RGBPixel[,] Buffer = new RGBPixel[Height, Width];

            unsafe
            {
                BitmapData bmd = original_bm.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadWrite, original_bm.PixelFormat);
                int x, y;
                int nWidth = 0;
                bool Format32 = false;
                bool Format24 = false;
                bool Format8 = false;

                if (original_bm.PixelFormat == PixelFormat.Format24bppRgb)
                {
                    Format24 = true;
                    nWidth = Width * 3;
                }
                else if (original_bm.PixelFormat == PixelFormat.Format32bppArgb || original_bm.PixelFormat == PixelFormat.Format32bppRgb || original_bm.PixelFormat == PixelFormat.Format32bppPArgb)
                {
                    Format32 = true;
                    nWidth = Width * 4;
                }
                else if (original_bm.PixelFormat == PixelFormat.Format8bppIndexed)
                {
                    Format8 = true;
                    nWidth = Width;
                }
                int nOffset = bmd.Stride - nWidth;
                byte* p = (byte*)bmd.Scan0;
                for (y = 0; y < Height; y++)
                {
                    for (x = 0; x < Width; x++)
                    {
                        if (Format8)
                        {
                            Buffer[y, x].red = Buffer[y, x].green = Buffer[y, x].blue = p[0];
                            p++;
                        }
                        else
                        {
                            Buffer[y, x].red = p[2];
                            Buffer[y, x].green = p[1];
                            Buffer[y, x].blue = p[0];
                            if (Format24) p += 3;
                            else if (Format32) p += 4;
                        }
                    }
                    p += nOffset;
                }
                original_bm.UnlockBits(bmd);
            }

            return Buffer;
        }

        /// <summary>
        /// Get the height of the image 
        /// </summary>
        /// <param name="ImageMatrix">2D array that contains the image</param>
        /// <returns>Image Height</returns>
        public static int GetHeight(RGBPixel[,] ImageMatrix)
        {
            return ImageMatrix.GetLength(0);
        }

        /// <summary>
        /// Get the width of the image 
        /// </summary>
        /// <param name="ImageMatrix">2D array that contains the image</param>
        /// <returns>Image Width</returns>
        public static int GetWidth(RGBPixel[,] ImageMatrix)
        {
            return ImageMatrix.GetLength(1);
        }


        /// <summary>
        /// Display the given image on the given PictureBox object
        /// </summary>
        /// <param name="ImageMatrix">2D array that contains the image</param>
        /// <param name="PicBox">PictureBox object to display the image on it</param>
        public static void DisplayImage(RGBPixel[,] ImageMatrix, PictureBox PicBox)
        {
            // Create Image:
            //==============
            int Height = ImageMatrix.GetLength(0);
            int Width = ImageMatrix.GetLength(1);

            Bitmap ImageBMP = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);

            unsafe
            {
                BitmapData bmd = ImageBMP.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadWrite, ImageBMP.PixelFormat);
                int nWidth = 0;
                nWidth = Width * 3;
                int nOffset = bmd.Stride - nWidth;
                byte* p = (byte*)bmd.Scan0;
                for (int i = 0; i < Height; i++)
                {
                    for (int j = 0; j < Width; j++)
                    {
                        p[2] = ImageMatrix[i, j].red;
                        p[1] = ImageMatrix[i, j].green;
                        p[0] = ImageMatrix[i, j].blue;
                        p += 3;
                    }

                    p += nOffset;
                }
                ImageBMP.UnlockBits(bmd);
            }
            PicBox.Image = ImageBMP;
        }


        /// <summary>
        /// Apply Gaussian smoothing filter to enhance the edge detection 
        /// </summary>
        /// <param name="ImageMatrix">Colored image matrix</param>
        /// <param name="filterSize">Gaussian mask size</param>
        /// <param name="sigma">Gaussian sigma</param>
        /// <returns>smoothed color image</returns>
        public static RGBPixel[,] GaussianFilter1D(RGBPixel[,] ImageMatrix, int filterSize, double sigma)
        {
            int Height = GetHeight(ImageMatrix);
            int Width = GetWidth(ImageMatrix);

            RGBPixelD[,] VerFiltered = new RGBPixelD[Height, Width];
            RGBPixel[,] Filtered = new RGBPixel[Height, Width];


            // Create Filter in Spatial Domain:
            //=================================
            //make the filter ODD size
            if (filterSize % 2 == 0) filterSize++;

            double[] Filter = new double[filterSize];

            //Compute Filter in Spatial Domain :
            //==================================
            double Sum1 = 0;
            int HalfSize = filterSize / 2;
            for (int y = -HalfSize; y <= HalfSize; y++)
            {
                //Filter[y+HalfSize] = (1.0 / (Math.Sqrt(2 * 22.0/7.0) * Segma)) * Math.Exp(-(double)(y*y) / (double)(2 * Segma * Segma)) ;
                Filter[y + HalfSize] = Math.Exp(-(double)(y * y) / (double)(2 * sigma * sigma));
                Sum1 += Filter[y + HalfSize];
            }
            for (int y = -HalfSize; y <= HalfSize; y++)
            {
                Filter[y + HalfSize] /= Sum1;
            }

            //Filter Original Image Vertically:
            //=================================
            int ii, jj;
            RGBPixelD Sum;
            RGBPixel Item1;
            RGBPixelD Item2;

            for (int j = 0; j < Width; j++)
                for (int i = 0; i < Height; i++)
                {
                    Sum.red = 0;
                    Sum.green = 0;
                    Sum.blue = 0;
                    for (int y = -HalfSize; y <= HalfSize; y++)
                    {
                        ii = i + y;
                        if (ii >= 0 && ii < Height)
                        {
                            Item1 = ImageMatrix[ii, j];
                            Sum.red += Filter[y + HalfSize] * Item1.red;
                            Sum.green += Filter[y + HalfSize] * Item1.green;
                            Sum.blue += Filter[y + HalfSize] * Item1.blue;
                        }
                    }
                    VerFiltered[i, j] = Sum;
                }

            //Filter Resulting Image Horizontally:
            //===================================
            for (int i = 0; i < Height; i++)
                for (int j = 0; j < Width; j++)
                {
                    Sum.red = 0;
                    Sum.green = 0;
                    Sum.blue = 0;
                    for (int x = -HalfSize; x <= HalfSize; x++)
                    {
                        jj = j + x;
                        if (jj >= 0 && jj < Width)
                        {
                            Item2 = VerFiltered[i, jj];
                            Sum.red += Filter[x + HalfSize] * Item2.red;
                            Sum.green += Filter[x + HalfSize] * Item2.green;
                            Sum.blue += Filter[x + HalfSize] * Item2.blue;
                        }
                    }
                    Filtered[i, j].red = (byte)Sum.red;
                    Filtered[i, j].green = (byte)Sum.green;
                    Filtered[i, j].blue = (byte)Sum.blue;
                }

            return Filtered;
        }


    }
}
