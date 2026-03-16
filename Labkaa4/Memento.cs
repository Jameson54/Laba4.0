using System;
using System.Collections.Generic;

namespace TextEditorApp
{
  public class TextFileMemento
  {
    public string Content { get; private set; }
    public DateTime Timestamp { get; private set; }

    public TextFileMemento(string content)
    {
      Content = content;
      Timestamp = DateTime.Now;
    }
  }

  public class TextFileOriginator
  {
    private string content;

    public void SetContent(string newContent)
    {
      content = newContent;
    }

    public string GetContent()
    {
      return content;
    }

    public TextFileMemento SaveState()
    {
      return new TextFileMemento(content);
    }

    public void RestoreState(TextFileMemento memento)
    {
      content = memento.Content;
    }
  }

  public class TextEditorHistory
  {
    private Stack<TextFileMemento> history;
    private TextFileOriginator originator;

    public TextEditorHistory(TextFileOriginator originator)
    {
      this.history = new Stack<TextFileMemento>();
      this.originator = originator;
    }

    public void Backup()
    {
      history.Push(originator.SaveState());
    }

    public void Undo()
    {
      if (history.Count > 0)
      {
        TextFileMemento memento = history.Pop();
        originator.RestoreState(memento);
      }
    }

    public void Clear()
    {
      history.Clear();
    }

    public bool CanUndo()
    {
      return history.Count > 0;
    }
  }
}