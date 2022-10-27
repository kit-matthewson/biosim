using System;
using System.IO;
using System.Text;

public class FileHandler {
    public readonly string filePath;
    public readonly string fileName;
    public readonly string fileType;

    private readonly UTF8Encoding encoder = new(true);

    public FileHandler(string file_path) {
        filePath = file_path;
        string[] split = file_path.Split(".", 2);

        fileName = split[0];
        fileName = split[1];

        //int i = 0;
        //while (File.Exists(filePath)) {
        //    filePath = $"{fileName}{i}.{fileType}";
        //    i++;
        //}

        File.Create(filePath).Close();
    }

    public void Write(string value, bool append = true) {
        using FileStream fs = File.OpenWrite(filePath);
        byte[] data = encoder.GetBytes(value);

        if (!append) {
            fs.Seek(0, SeekOrigin.Begin);
        } else {
            fs.Seek(0, SeekOrigin.End);
        }

        fs.Write(data, 0, data.Length);
        fs.Close();
    }
}
