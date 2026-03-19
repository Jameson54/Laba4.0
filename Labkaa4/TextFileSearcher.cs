using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace TextEditorApp
{
  public class TextFileSearcher
  {
    private List<string> searchResults;
    private Dictionary<string, List<string>> fileIndex;

    public TextFileSearcher()
    {
      searchResults = new List<string>();
      fileIndex = new Dictionary<string, List<string>>();
    }

    public List<string> SearchByKeyword(string directoryPath, string keyword, bool recursive = true)
    {
      searchResults.Clear();

      try
      {
        SearchOption searchOption = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
        string[] files = Directory.GetFiles(directoryPath, "*.txt", searchOption);

        foreach (string file in files)
        {
          if (FileContainsKeyword(file, keyword))
          {
            searchResults.Add(file);
          }
        }

        return searchResults;
      }
      catch (Exception ex)
      {
        throw new Exception($"Ошибка при поиске файлов: {ex.Message}");
      }
    }

    public List<string> SearchByKeywords(string directoryPath, List<string> keywords, bool matchAll = true)
    {
      searchResults.Clear();
      bool matches;
      string[] files;

      matches = matchAll;
      try
      {
        files = Directory.GetFiles(directoryPath, "*.txt", SearchOption.AllDirectories);

        foreach (string file in files)
        {
             KeywordsMatchAll(file, keywords);
             KeywordsMatchAny(file, keywords);

          if (matches)
          {
            searchResults.Add(file);
          }
        }
      
        
        return searchResults;
      }
      catch (Exception ex)
      {
        throw new Exception($"Ошибка при поиске файлов: {ex.Message}");
      }
    }

    private bool FileContainsKeyword(string filePath, string keyword)
    {
      string content;
      try
      {
        content = File.ReadAllText(filePath);
        return content.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0;
      }
      catch
      {
        return false;
      }
    }

    private bool KeywordsMatchAll(string filePath, List<string> keywords)
    {
      string content;
      try
      {
        content = File.ReadAllText(filePath);
        return keywords.All(keyword =>
            content.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0);
      }
      catch
      {
        return false;
      }
    }

    private bool KeywordsMatchAny(string filePath, List<string> keywords)
    {
      string content;
      try
      {
        content = File.ReadAllText(filePath);
        return keywords.Any(keyword =>
            content.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0);
      }
      catch
      {
        return false;
      }
    }
    public void CreateIndex(string directoryPath, List<string> keywords)
    {
      List<string> foundKeywords;
      string content;
      fileIndex.Clear();

      try
      {
        string[] files = Directory.GetFiles(directoryPath, "*.txt", SearchOption.AllDirectories);

        foreach (string file in files)
        {
          foundKeywords = new List<string>();
          content = File.ReadAllText(file);

          foreach (string keyword in keywords)
          {
            if (content.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
            {
              foundKeywords.Add(keyword);
            }
          }

          if (foundKeywords.Any())
          {
            fileIndex[file] = foundKeywords;
          }
        }
      }
      catch (Exception ex)
      {
        throw new Exception($"Ошибка при создании индекса: {ex.Message}");
      }
    }

    public Dictionary<string, List<string>> GetFileIndex()
    {
      return fileIndex;
    }
  }
}