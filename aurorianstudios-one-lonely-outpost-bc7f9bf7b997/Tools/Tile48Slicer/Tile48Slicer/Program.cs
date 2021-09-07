using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace Tile48Slicer
{


    class Program
    {
        private static int Resolution = 16;
        private static Image SourceImage;
        private static Dictionary<string, List<Bitmap>> MiniTiles = new Dictionary<string, List<Bitmap>>(4);
        private static string OutputName = "C:\\Generated48Tile\\TileSet";
        private static int TileCounter = 0;
        private static List<Bitmap> RenderedBitmaps = new List<Bitmap>(48);

        static void Main(string[] args)
        {
            if (args.Length < 1)
                return;
            SourceImage = Image.FromFile(args[0]);

            if (args.Length >1)
                OutputName = args[1];
            
            MiniTiles["NWQElements"] = new List<Bitmap>(5);
            MiniTiles["NEQElements"] = new List<Bitmap>(5);
            MiniTiles["SWQElements"] = new List<Bitmap>(5);
            MiniTiles["SEQElements"] = new List<Bitmap>(5);

            //Horizontal Boarder, Vertical Boader, Normal Corner, No Boarder, Diagonal Corner

            MiniTiles["NWQElements"].Add(cropTile(0, 4));
            MiniTiles["NWQElements"].Add(cropTile(2, 2));
            MiniTiles["NWQElements"].Add(cropTile(0, 2));
            MiniTiles["NWQElements"].Add(cropTile(2, 4));
            MiniTiles["NWQElements"].Add(cropTile(0, 0));

            MiniTiles["SWQElements"].Add(cropTile(3, 4));
            MiniTiles["SWQElements"].Add(cropTile(1, 2));
            MiniTiles["SWQElements"].Add(cropTile(3, 2));
            MiniTiles["SWQElements"].Add(cropTile(1, 4));
            MiniTiles["SWQElements"].Add(cropTile(1, 0));

            MiniTiles["NEQElements"].Add(cropTile(0, 3));
            MiniTiles["NEQElements"].Add(cropTile(2, 5));
            MiniTiles["NEQElements"].Add(cropTile(0, 5));
            MiniTiles["NEQElements"].Add(cropTile(2, 3));
            MiniTiles["NEQElements"].Add(cropTile(0, 1));

            MiniTiles["SEQElements"].Add(cropTile(3, 3));
            MiniTiles["SEQElements"].Add(cropTile(1, 5));
            MiniTiles["SEQElements"].Add(cropTile(3, 5));
            MiniTiles["SEQElements"].Add(cropTile(1, 3));
            MiniTiles["SEQElements"].Add(cropTile(1, 1));
            
            generateTile(false, false, false,false);
            generateTile(true, true, true, true);

            generateTile(true, false, false, false);
            generateTile(false, true, false, false);
            generateTile(false, false, true, false);
            generateTile(false, false, false, true);

            generateTile(true, true, false, false);
            generateTile(true, false, true, false);
            generateTile(true, false, false, true);
            generateTile(false, true, true, false);
            generateTile(false, true, false, true);
            generateTile(false, false, true, true);

            generateTile(true, true, true, false);
            generateTile(true, true, false, true);
            generateTile(true, false, true, true);
            generateTile(false, true, true, true);

            outputTileSet();
        }

        private static Tuple<int,int,int,int> getTileMapCardinal(bool N, bool E, bool S, bool W, bool NW, bool NE, bool SW, bool SE)
        {
            int nwTile = 0;
            int neTile = 0;
            int swTile = 0;
            int seTile = 0;

            if (N && E)
                neTile = NE ? 3 : 4;
            else if (N)
                neTile = 1;
            else if (E)
                neTile = 0;
            else
                neTile = 2;

            if (N && W)
                nwTile = NW ? 3 : 4;
            else if (N)
                nwTile = 1;
            else if (W)
                nwTile = 0;
            else
                nwTile = 2;

            if (S && E)
                seTile = SE ? 3 : 4;
            else if (S)
                seTile = 1;
            else if (E)
                seTile = 0;
            else
                seTile = 2;

            if (S && W)
                swTile = SW ? 3 : 4;
            else if (S)
                swTile = 1;
            else if (W)
                swTile = 0;
            else
                swTile = 2;

            var tileMap = new Tuple<int, int, int, int>(nwTile, neTile, swTile, seTile);

            return tileMap;
        }

        private static void generateTile(bool N, bool E, bool S, bool W)
        {
            outputTile(getTileMapCardinal(N, E, S, W, false, false, false, false), $"{N}{E}{S}{W},FFFF");
            if (N && E)
                outputTile(getTileMapCardinal(N, E, S, W, false, true, false, false), $"{N}{E}{S}{W},FTFF");                
            if (N && W)
                outputTile(getTileMapCardinal(N, E, S, W, true, false, false, false), $"{N}{E}{S}{W},TFFF");
            if (S && E)
                outputTile(getTileMapCardinal(N, E, S, W, false, false, false, true), $"{N}{E}{S}{W},FFFT");
            if (S && W)
                outputTile(getTileMapCardinal(N, E, S, W, false, false, true, false), $"{N}{E}{S}{W},FFTF");
            if (N && E && S)
                outputTile(getTileMapCardinal(N, E, S, W, false, true, false, true), $"{N}{E}{S}{W},FTFT");
            if (N && W && E)
                outputTile(getTileMapCardinal(N, E, S, W, true, true, false, false), $"{N}{E}{S}{W},TTFF");
            if (S && E && W)
                outputTile(getTileMapCardinal(N, E, S, W, false, false, true, true), $"{N}{E}{S}{W},FFTT");
            if (S && W && N)
                outputTile(getTileMapCardinal(N, E, S, W, true, false, true, false),$"{N}{E}{S}{W},TFTF");
            if (S && W && N && E)
            {
                outputTile(getTileMapCardinal(N, E, S, W, false, true, true, true), $"{N}{E}{S}{W},FTTT");
                outputTile(getTileMapCardinal(N, E, S, W, true, false, true, true), $"{N}{E}{S}{W},TFTT");
                outputTile(getTileMapCardinal(N, E, S, W, true, true, false, true), $"{N}{E}{S}{W},TTFT");
                outputTile(getTileMapCardinal(N, E, S, W, true, true, true, false), $"{N}{E}{S}{W},TTTF");

                outputTile(getTileMapCardinal(N, E, S, W, true, false, false, true), $"{N}{E}{S}{W},TFFT");
                outputTile(getTileMapCardinal(N, E, S, W, false, true, true, false), $"{N}{E}{S}{W},FTTF");

                outputTile(getTileMapCardinal(N, E, S, W, true, true, true, true), $"{N}{E}{S}{W},TTTT");
            }
        }

        private static void outputTileSet()
        {
            var outputTilesetBitmap = new Bitmap(16 * Resolution, 12 * Resolution);
            var g = Graphics.FromImage(outputTilesetBitmap);
            var position = 0;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    
                    if (RenderedBitmaps.Count > position)
                    {
                        g.DrawImage(RenderedBitmaps[position], new Point(i * Resolution * 2,j * Resolution * 2));
                    }

                    position++;
                }
            }

            var saveString = $"{OutputName}_Tileset.png";

            outputTilesetBitmap.Save(saveString, ImageFormat.Png);
        }

        private static void outputTile(Tuple<int, int, int, int> tileMap, string debugString = "")
        {
            var outputBitmap = new Bitmap(Resolution * 2, Resolution * 2);

            Graphics g = Graphics.FromImage(outputBitmap);
            g.DrawImage(MiniTiles["NWQElements"][tileMap.Item1], new Point(0, 0));
            g.DrawImage(MiniTiles["NEQElements"][tileMap.Item2], new Point(0, Resolution));
            g.DrawImage(MiniTiles["SWQElements"][tileMap.Item3], new Point(Resolution, 0));
            g.DrawImage(MiniTiles["SEQElements"][tileMap.Item4], new Point(Resolution, Resolution));

            var saveString = $"{OutputName}_{TileCounter}.png";

            if (File.Exists(saveString))
            {
                return;
            }

            if(RenderedBitmaps.Any())
                foreach (var renderedBitmap in RenderedBitmaps)
                {
                    if(CompareBitmaps(outputBitmap, renderedBitmap))
                        return;
                }
            RenderedBitmaps.Add(outputBitmap);

            outputBitmap.Save(saveString, ImageFormat.Png);
            TileCounter++;
        }

        private static Bitmap cropTile(int x, int y)
        {
            Bitmap bmpImage = new Bitmap(SourceImage);
            return bmpImage.Clone(new Rectangle(x * Resolution,y * Resolution, Resolution, Resolution), bmpImage.PixelFormat);
        }

        public static bool CompareBitmaps(Bitmap bmp1, Bitmap bmp2)
        {
            if (!bmp1.Size.Equals(bmp2.Size))
            {
                return false;
            }
            for (int x = 0; x < bmp1.Width; ++x)
            {
                for (int y = 0; y < bmp1.Height; ++y)
                {
                    if (bmp1.GetPixel(x, y) != bmp2.GetPixel(x, y))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
