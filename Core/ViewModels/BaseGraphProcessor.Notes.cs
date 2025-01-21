#region 注 释

/***
 *
 *  Title:
 *
 *  Description:
 *
 *  Date:
 *  Version:
 *  Writer: 半只龙虾人
 *  Github: https://github.com/haloman9527
 *  Blog: https://www.haloman.net/
 *
 */

#endregion

using System;
using System.Collections.Generic;

namespace Moyo.GraphProcessor
{
    public partial class BaseGraphProcessor
    {
        #region Fields

        private Dictionary<int, StickyNoteProcessor> notes;

        public event Action<StickyNoteProcessor> OnNoteAdded;
        public event Action<StickyNoteProcessor> OnNoteRemoved;

        #endregion

        #region Properties

        public IReadOnlyDictionary<int, StickyNoteProcessor> Notes => notes;

        #endregion

        private void InitNotes()
        {
            this.notes = new Dictionary<int, StickyNoteProcessor>(System.Math.Min(Model.connections.Count, 4));
            foreach (var note in model.notes)
            {
                notes.Add(note.id, (StickyNoteProcessor)ViewModelFactory.CreateViewModel(note));
            }
        }

        #region API

        public void AddNote(string title, string content, InternalVector2Int position)
        {
            var note = new StickyNote();
            note.id = NewID();
            note.position = position;
            note.title = title;
            note.content = content;
            var noteVm = ViewModelFactory.CreateViewModel(note) as StickyNoteProcessor;

            AddNote(noteVm);
        }

        public void AddNote(StickyNoteProcessor note)
        {
            notes.Add(note.ID, note);
            Model.notes.Add(note.Model);
            OnNoteAdded?.Invoke(note);
        }

        public void RemoveNote(int id)
        {
            if (!notes.TryGetValue(id, out var note))
                return;
            notes.Remove(note.ID);
            Model.notes.Remove(note.Model);
            OnNoteRemoved?.Invoke(note);
        }

        #endregion
    }
}