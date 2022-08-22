using System;
using System.IO;
using System.Text;

public class FileHandler {
    public readonly string path;

    private readonly UTF8Encoding encoder = new(true);

    public FileHandler(string file_path) {
        path = file_path;
        string[] split = file_path.Split(".", 2);

        int i = 0;
        while (File.Exists(path)) {
            path = $"{split[0]}_{i}.{split[1]}";
            i++;
        }

        File.Create(path).Close();
    }

    public void Write(string value, bool append = true) {
        using FileStream fs = File.OpenWrite(path);
        byte[] data = encoder.GetBytes(value);

        if (!append) {
            fs.Seek(0, SeekOrigin.Begin);
        } else {
            fs.Seek(0, SeekOrigin.End);
        }

        fs.Write(data, 0, data.Length);
    }
}
