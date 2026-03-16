using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace TextEditorApp
{
  [Serializable]
  public class TextFile
  {
    public string FilePath { get; set; }
    public string Content { get; set; }
    public DateTime LastModified { get; set; }
    public bool IsModified { get; set; }

    public TextFile()
    {
      FilePath = string.Empty;
      Content = string.Empty;
      LastModified = DateTime.Now;
      IsModified = false;
    }

    public TextFile(string path)
    {
      FilePath = path;
      LoadFromFile();
    }

    public void LoadFromFile()
    {
      try
      {
        if (File.Exists(FilePath))
        {
          Content = File.ReadAllText(FilePath, Encoding.UTF8);
          LastModified = File.GetLastWriteTime(FilePath);
          IsModified = false;
        }
        else
        {
          Content = string.Empty;
          IsModified = true;
        }
      }
      catch (Exception ex)
      {
        throw new Exception($"Ошибка при загрузке файла: {ex.Message}");
      }
    }

    public void SaveToFile()
    {
      try
      {
        File.WriteAllText(FilePath, Content, Encoding.UTF8);
        LastModified = DateTime.Now;
        IsModified = false;
      }
      catch (Exception ex)
      {
        throw new Exception($"Ошибка при сохранении файла: {ex.Message}");
      }
    }

    public void BinarySerialize(string outputPath)
    {
      try
      {
        using (FileStream stream = new FileStream(outputPath, FileMode.Create))
        {
          BinaryFormatter formatter = new BinaryFormatter();
          formatter.Serialize(stream, this);
        }
      }
      catch (Exception ex)
      {
        throw new Exception($"Ошибка при бинарной сериализации: {ex.Message}");
      }
    }

    public static TextFile BinaryDeserialize(string inputPath)
    {
      try
      {
        using (FileStream stream = new FileStream(inputPath, FileMode.Open))
        {
          BinaryFormatter formatter = new BinaryFormatter();
          return (TextFile)formatter.Deserialize(stream);
        }
      }
      catch (Exception ex)
      {
        throw new Exception($"Ошибка при бинарной десериализации: {ex.Message}");
      }
    }

    public void XmlSerialize(string outputPath)
    {
      try
      {
        XmlSerializer serializer = new XmlSerializer(typeof(TextFile));
        using (StreamWriter writer = new StreamWriter(outputPath))
        {
          serializer.Serialize(writer, this);
        }
      }
      catch (Exception ex)
      {
        throw new Exception($"Ошибка при XML сериализации: {ex.Message}");
      }
    }

    public static TextFile XmlDeserialize(string inputPath)
    {
      try
      {
        XmlSerializer serializer = new XmlSerializer(typeof(TextFile));
        using (StreamReader reader = new StreamReader(inputPath))
        {
          return (TextFile)serializer.Deserialize(reader);
        }
      }
      catch (Exception ex)
      {
        throw new Exception($"Ошибка при XML десериализации: {ex.Message}");
      }
    }

    public string GetFileName()
    {
      return Path.GetFileName(FilePath);
    }
  }
}