using Svg;
using System;
using System.Xml;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Text;

namespace WC
{
    class Program
    {
        static string imgpath = "";
        static string xmlpath = "";

        static byte[] image;
        static string xml = "";

        static string outputfile = "";

        static string targetDir = "";

        static string pngdir = "";
        static string xmldir = "";

        static string svgdir = "";

        static void Main(string[] args)
        {
            Output("(Q) for bulk WC maker, (W) for bulk SVG -> PNG, (E) for solo maker : ");
            var x = Console.ReadKey();
            Console.WriteLine();

            if (x.Key == ConsoleKey.E)
            {
                Output("Enter path to PNG : ");
                imgpath = Console.ReadLine();
                Output("Enter path to XML : ");
                xmlpath = Console.ReadLine();
                Output("Output Filename (location: ./items/) (default is xml filename) : ");
                outputfile = Console.ReadLine();
                if (string.IsNullOrEmpty(outputfile))
                    outputfile = Path.GetFileNameWithoutExtension(xmlpath)+".wc";
                DoStuffs(true, true);
                Process.Start("explorer.exe", Directory.GetCurrentDirectory() + "\\items");
                Console.WriteLine("Press any key to quit.");
                Console.ReadKey();
            }
            else if(x.Key == ConsoleKey.Q)
            {
                Output("-- Files in PNG and XML directory must have the same name except for extension. This recurses child folders too --\n");
                Output("Directory of PNGs : ");
                pngdir = Console.ReadLine();
                Output("Directory of XMLs : ");
                xmldir = Console.ReadLine();
                
                string[] files;
                files = Directory.GetFiles(pngdir, "*.*", SearchOption.AllDirectories).ToArray();
                foreach(string dir in Directory.GetDirectories(pngdir, "*.*", SearchOption.AllDirectories))
                {
                    string diri = "./pack/"+dir.Remove(0,pngdir.Length);
                    Directory.CreateDirectory(diri);
                }

                for (int i = 0; i < files.Length; i++)
                {
                    imgpath = files[i];
                    string sub = files[i].Remove(0, pngdir.Length);
                    xmlpath = xmldir + sub.Substring(0, sub.LastIndexOf('\\')) + "\\" + Path.GetFileNameWithoutExtension(files[i])+".xml";

                    string of = files[i].Remove(0, pngdir.Length);
                    outputfile = of.Substring(0, of.LastIndexOf('\\')) + "\\" + Path.GetFileNameWithoutExtension(xmlpath)+".wc";
                    DoStuffs(false, false);
                }
                Process.Start("explorer.exe", Directory.GetCurrentDirectory() + "\\pack");
                Console.WriteLine("Press any key to quit.");
                Console.ReadKey();
            }
            else if(x.Key == ConsoleKey.W)
            {
                Output("SVGs Directory (recursed) : ");
                svgdir = Console.ReadLine();
                // writes to ./pngs/
                string[] files;
                files = Directory.GetFiles(svgdir, "*.*", SearchOption.AllDirectories).ToArray();

                foreach (string dir in Directory.GetDirectories(svgdir, "*.*", SearchOption.AllDirectories))
                {
                    string diri = "./pngs/" + dir.Remove(0, svgdir.Length);
                    Directory.CreateDirectory(diri);
                }

                foreach (string file in files)
                {
                    var svgDocument = SvgDocument.Open(file);
                    var bitmap = svgDocument.Draw();

                    string of = file.Remove(0, svgdir.Length);
                    of = of.Substring(0, of.LastIndexOf('\\')) + "\\" + Path.GetFileNameWithoutExtension(file)+".png";
                    
                    bitmap.Save("./pngs"+of, ImageFormat.Png);
                    Output2(Path.GetFileName(file));
                }
                Process.Start("explorer.exe", Directory.GetCurrentDirectory()+"\\pngs");
                Console.WriteLine("Press any key to quit.");
                Console.ReadKey();
            }
        }

        static void DoStuffs(bool loc, bool e)
        {
            if (e)
                Output2("Loading image...");
            image = File.ReadAllBytes(imgpath);
            if(!imgpath.EndsWith("png"))
            {
                OutputE($"Not a .png file : {imgpath}");
                return;
            }
            if(e)
                Output2("Loading xml...");
            //if(loc)
            try
            {
                xml = File.ReadAllText(xmlpath);
            }
            catch(FileNotFoundException ex)
            {
                OutputE("Nonmatching filename : "+xmlpath);
                return;
            }
            try
            {
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.LoadXml(xml);
            }
            catch (Exception ex)
            {
                OutputE($"\u2717 XML : {xmlpath} : {ex.Message}");
                return;
            }
            if(e)
                Output2("Formatting...");
            List<byte> bytes = new List<byte>();
            bytes.AddRange(Encoding.UTF8.GetBytes((xml.Length + 2 + (xml.Length + 2).ToString().Length).ToString()+"WC"));
            bytes.AddRange(Encoding.UTF8.GetBytes(xml));
            bytes.AddRange(image);
            
            if (e)
                Output2("Writing...");
            else
                Output2("\u221A XML : " + Path.GetFileNameWithoutExtension(imgpath));

            if (loc)
                File.WriteAllBytes("./items/" + outputfile, bytes.ToArray());
            else
                File.WriteAllBytes("./pack/" + outputfile, bytes.ToArray());
        }

        static void Output(string i)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("[*]");
            Console.ResetColor();
            Console.Write(" "+i);
        }

        static void Output2(string i)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("    [*]");
            Console.ResetColor();
            Console.WriteLine(" " + i);
        }

        static void OutputE(string i)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("    [*]");
            Console.ResetColor();
            Console.WriteLine(" " + i);
        }
    }
}