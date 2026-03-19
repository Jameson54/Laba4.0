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
    private string _content;

    public void SetContent(string newContent)
    {
      _content = newContent;
    }

    public string GetContent()
    {
      return _content;
    }

    public TextFileMemento SaveState()
    {
      return new TextFileMemento(_content);
    }

    public void RestoreState(TextFileMemento memento)
    {
      _content = memento.Content;
    }
  }

  public class TextEditorHistory
  {
    private Stack<TextFileMemento> _history;
    private TextFileOriginator _originator;

    public TextEditorHistory(TextFileOriginator originator)
    {
      this._history = new Stack<TextFileMemento>();
      this._originator = originator;
    }

    public void Backup()
    {
      _history.Push(_originator.SaveState());
    }

    public void Undo()
    {
      if (_history.Count > 0)
      {
        TextFileMemento memento = _history.Pop();
        _originator.RestoreState(memento);
      }
    }

    public void Clear()
    {
      _history.Clear();
    }

    public bool CanUndo()
    {
      return _history.Count > 0;
    }
  }
}