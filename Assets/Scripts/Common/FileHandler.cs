using System.IO;
using System.Text;

/// <summary>
/// Provides a simple write-only interface to a file.
/// </summary>
public class FileHandler {
    public readonly string FilePath;
    public readonly string FileName;

    private readonly UTF8Encoding _encoder = new(true);

    public FileHandler(string filePath) {
        FilePath = filePath;
        string[] split = filePath.Split(".", 2);

        FileName = split[0];
        FileName = split[1];

        File.Create(FilePath).Close();
    }

    /// <summary>
    /// Writes some text to the file being handled.
    /// </summary>
    /// <param name="value">The string to write.</param>
    /// <param name="append">Whether text should be appended, or the file overwritten with new data.</param>
    public void Write(string value, bool append = true) {
        using FileStream fs = File.OpenWrite(FilePath);
        byte[] data = _encoder.GetBytes(value);

        fs.Seek(0, !append ? SeekOrigin.Begin : SeekOrigin.End);

        fs.Write(data, 0, data.Length);
        fs.Close();
    }

    public string[] ReadLines() {
        using FileStream fs = new(FilePath, FileMode.Open, FileAccess.Read);
        byte[] bytes = new byte[fs.Length];
        
        int remaining = (int)fs.Length;
        int read = 0;

        while (remaining > 0) {
            int n = fs.Read(bytes, read, remaining);
            
            read += n;
            remaining -= n;
        }

        string file = _encoder.GetString(bytes);

        return file.Split("\n");
    }
}