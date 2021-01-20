using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ml_img
{
    class AddImageToModel
    {
        void EditCvs()
        {
            string path = @"C:\CSV.txt";
            List<string> lines = new List<string>();

            if (File.Exists(path)) 
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    string line;

                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.Contains(","))
                        {
                            string[] split = line.Split(',');

                            if (split[1].Contains("34"))
                            {
                                split[1] = "100";
                                line = string.Join(",", split);
                            }
                        }

                        lines.Add(line);
                    }
                }

                using (StreamWriter writer = new StreamWriter(path, false))
                {
                    foreach (string line in lines)
                        writer.WriteLine(line);
                }
            }
        }
    }
}
