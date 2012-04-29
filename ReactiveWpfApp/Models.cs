using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Stimpack;

namespace ReactiveWpfApp
{
    public class ApplicationModel
    {
        public ObservableCollection<Note> Notes { get; private set; }

        public ApplicationModel()
        {
            Notes = new ObservableCollection<Note>();
        }

        public void AddNote(Note note)
        {
            Notes.Add(note);
        }
    }

    public class Note
    {
        public Active<string> Title { get; private set; }
        public Active<string> Text { get; private set; }
        public ObservableCollection<string> Tags { get; private set; }

        public Note()
        {
            Title = new Active<string>("Note Title");
            Text = new Active<string>("");
            Tags = new ObservableCollection<string>();
        }

        public void AddTag(string tag)
        {
            if (!Tags.Contains(tag))
            {
                Tags.Add(tag);
            }
        }
    }
}
