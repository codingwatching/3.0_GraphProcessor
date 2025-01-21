﻿#if UNITY_EDITOR
using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Moyo.GraphProcessor.Editors
{
    public class StickyNoteView : UnityEditor.Experimental.GraphView.StickyNote, IGraphElementView<StickyNoteProcessor>
    {
        private TextField titleField;
        private TextField contentsField;

        public StickyNoteProcessor ViewModel { get; private set; }
        public IGraphElementProcessor V => ViewModel;

        public BaseGraphView Owner { get; private set; }

        public StickyNoteView()
        {
            var contents = this.Q<Label>("contents", (string)null);
            this.titleField = this.Q<TextField>("title-field", (string)null);
            this.contentsField = contents.Q<TextField>("contents-field", (string)null);
        }

        public void SetUp(StickyNoteProcessor note, BaseGraphView graphView)
        {
            this.ViewModel = note;
            this.Owner = graphView;
            // 初始化
            base.SetPosition(new Rect(ViewModel.Position.ToVector2(), ViewModel.Size.ToVector2()));
            this.title = note.Title;
            this.contents = note.Content;
        }

        public void OnCreate()
        {
            ViewModel.RegisterValueChanged<InternalVector2Int>(nameof(StickyNote.position), OnPositionChanged);
            ViewModel.RegisterValueChanged<InternalVector2Int>(nameof(StickyNote.size), OnSizeChanged);
            ViewModel.RegisterValueChanged<string>(nameof(StickyNote.title), OnTitleChanged);
            ViewModel.RegisterValueChanged<string>(nameof(StickyNote.content), OnContentsChanged);

            this.RegisterCallback<StickyNoteChangeEvent>(OnChanged);
        }

        public void OnDestroy()
        {
            ViewModel.UnregisterValueChanged<InternalVector2Int>(nameof(StickyNote.position), OnPositionChanged);
            ViewModel.UnregisterValueChanged<InternalVector2Int>(nameof(StickyNote.size), OnSizeChanged);
            ViewModel.UnregisterValueChanged<string>(nameof(StickyNote.title), OnTitleChanged);
            ViewModel.UnregisterValueChanged<string>(nameof(StickyNote.content), OnContentsChanged);

            this.UnregisterCallback<StickyNoteChangeEvent>(OnChanged);
        }

        private void OnChanged(StickyNoteChangeEvent evt)
        {
            switch (evt.change)
            {
                case StickyNoteChange.Title:
                {
                    var oldTitle = ViewModel.Title;
                    var newTitle = this.title;
                    Owner.CommandDispatcher.Do(() => { ViewModel.Title = newTitle; }, () => { ViewModel.Title = oldTitle; });
                    break;
                }
                case StickyNoteChange.Contents:
                {
                    var oldContent = ViewModel.Content;
                    var newContent = this.contents;
                    Owner.CommandDispatcher.Do(() => { ViewModel.Content = newContent; }, () => { ViewModel.Content = oldContent; });
                    break;
                }
                case StickyNoteChange.Theme:
                    break;
                case StickyNoteChange.FontSize:
                    break;
                case StickyNoteChange.Position:
                {
                    var oldPosition = ViewModel.Position;
                    var oldSize = ViewModel.Size;
                    this.schedule.Execute(() =>
                    {
                        var newPosition = GetPosition().position.ToInternalVector2Int();
                        var newSize = GetPosition().size.ToInternalVector2Int();
                        Owner.CommandDispatcher.Do(() =>
                        {
                            ViewModel.Position = newPosition;
                            ViewModel.Size = newSize;
                        }, () =>
                        {
                            ViewModel.Position = oldPosition;
                            ViewModel.Size = oldSize;
                        });
                    }).ExecuteLater(20);
                    
                    break;
                }
            }
        }

        void OnPositionChanged(ViewModel.ValueChangedArg<InternalVector2Int> arg)
        {
            base.SetPosition(new Rect(arg.newValue.ToVector2(), GetPosition().size));
            Owner.SetDirty();
        }

        void OnSizeChanged(ViewModel.ValueChangedArg<InternalVector2Int> arg)
        {
            base.SetPosition(new Rect(GetPosition().position, arg.newValue.ToVector2()));
            Owner.SetDirty();
        }

        private void OnContentsChanged(ViewModel.ValueChangedArg<string> arg)
        {
            this.contents = arg.newValue;
            Owner.SetDirty();
        }

        private void OnTitleChanged(ViewModel.ValueChangedArg<string> arg)
        {
            this.title = arg.newValue;
            Owner.SetDirty();
        }
    }
}
#endif