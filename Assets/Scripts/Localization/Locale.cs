using System.Collections.Generic;
using System.IO;
using System.Text;

using UnityEngine;

public class Locale
{
    Dictionary<string, string> table = new Dictionary<string, string>();

    public Locale(string filePath)
    {
        TextAsset loc = Resources.Load<TextAsset>(filePath);
        if (loc is null) return;
        byte[] byteArray = Encoding.ASCII.GetBytes(loc.text);
        MemoryStream stream = new MemoryStream(byteArray);
        StreamReader file = new StreamReader(stream);
        // Open config file
        {
            // Read first line with locales list
            while (!file.EndOfStream)
            {
                string line = file.ReadLine();
                if (line.Length == 0) break;

                string[] content = line.Split('|');
                if (content[1].Contains("\\n"))
                    content[1] = content[1].Replace("\\n", "\n");

                table.Add(content[0], content[1]);
            }
        }
        file.Close();
    }

    public string GetValue(string key)
    {
        string value = "";
        table.TryGetValue(key, out value);
        return value;
    }
}
