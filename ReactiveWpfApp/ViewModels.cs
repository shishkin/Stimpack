using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using Stimpack;

namespace ReactiveWpfApp
{
    public class MainWindowViewModel
    {
        private readonly ApplicationModel model;
        public ObservableCollection<TagViewModel> Tags { get; private set; }
        public ObservableCollection<NoteViewModel> Notes { get; private set; }
        public Active<NoteViewModel> CurrentNote { get; set; }

        public MainWindowViewModel(ApplicationModel model)
        {
            this.model = model;
            Notes = model.Notes.Transform(x => new NoteViewModel(x));
            CurrentNote = new Active<NoteViewModel>(Notes.FirstOrDefault());

            Tags = new ObservableCollection<TagViewModel>();
            model.Notes.ObserveNoteTagged().Subscribe(OnNoteTagged);
        }

        private void OnNoteTagged(string tag)
        {
            if (!Tags.Any(x => x.Tag == tag))
            {
                Tags.Add(new TagViewModel(tag, model.Notes));
            }
        }

        public void CreateNewNote()
        {
            model.AddNote(new Note());
        }
    }

    public class NoteViewModel
    {
        public Note Note { get; private set; }

        public NoteViewModel(Note note)
        {
            Note = note;
        }

        public void AddNewTag(string tag)
        {
            Note.AddTag(tag);
        }
    }

    public class TagViewModel
    {
        private readonly ObservableCollection<Note> notes;

        public string Tag { get; private set; }
        public Active<int> Count { get; private set; }

        public TagViewModel(string tag, ObservableCollection<Note> notes)
        {
            Tag = tag;
            this.notes = notes;
            Count = new Active<int>(CountTaggedNotes());
            notes.ObserveNoteTagged().Subscribe(OnNoteTagged);
        }

        private int CountTaggedNotes()
        {
            return notes.SelectMany(x => x.Tags)
                .Count(x => x == Tag);
        }

        private void OnNoteTagged(string tag)
        {
            Count.Value = CountTaggedNotes();
        }
    }

    public static class ViewModelExtensions
    {
        public static IObservable<string> ObserveNoteTagged(this ObservableCollection<Note> notes)
        {
            var newNotes = new Subject<Note>();
            var newTags = new Subject<string>();
            newNotes
                .SelectMany(x => x.Tags.ObserveCollectionChangedArgs())
                .SelectMany(x => x.NewItems.Cast<string>())
                .Subscribe(newTags);
            notes.ObserveCollectionChangedArgs()
                .SelectMany(x => x.NewItems.Cast<Note>())
                .Subscribe(newNotes);

            return newTags;
        }
    }
}
