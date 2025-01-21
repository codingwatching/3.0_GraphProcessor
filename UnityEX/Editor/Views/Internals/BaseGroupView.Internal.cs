﻿#region 注 释

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

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using GroupView = UnityEditor.Experimental.GraphView.Group;

namespace Moyo.GraphProcessor.Editors
{
    public partial class BaseGroupView : GroupView, IGraphElementView<GroupProcessor>
    {
        bool WithoutNotify { get; set; }
        public TextField TitleField { get; private set; }
        public ColorField BackgroudColorField { get; private set; }
        public Label TitleLabel { get; private set; }
        public GroupProcessor ViewModel { get; protected set; }
        public IGraphElementProcessor V => ViewModel;
        public BaseGraphView Owner { get; private set; }


        public BaseGroupView()
        {
            this.styleSheets.Add(GraphProcessorEditorStyles.BaseGroupViewStyle);

            TitleLabel = headerContainer.Q<Label>();
            TitleField = headerContainer.Q<TextField>();

            BackgroudColorField = new ColorField();
            BackgroudColorField.name = "backgroundColorField";
            headerContainer.Add(BackgroudColorField);

            TitleField.RegisterCallback<FocusInEvent>(evt => { Input.imeCompositionMode = IMECompositionMode.On; });
            TitleField.RegisterCallback<FocusOutEvent>(evt => { Input.imeCompositionMode = IMECompositionMode.Auto; });
        }

        public void SetUp(GroupProcessor group, BaseGraphView graphView)
        {
            this.ViewModel = group;
            this.Owner = graphView;
            this.title = ViewModel.GroupName;
            this.style.backgroundColor = ViewModel.BackgroundColor.ToColor();
            this.style.unityBackgroundImageTintColor = ViewModel.BackgroundColor.ToColor();
            this.BackgroudColorField.SetValueWithoutNotify(ViewModel.BackgroundColor.ToColor());
            base.SetPosition(new Rect(ViewModel.Position.ToVector2(), GetPosition().size));
            WithoutNotify = true;
            base.AddElements(ViewModel.Nodes.Where(nodeID => Owner.NodeViews.ContainsKey(nodeID)).Select(nodeID => Owner.NodeViews[nodeID]).ToArray());
            WithoutNotify = false;
            this.AddManipulator(new ContextualMenuManipulator(BuildContextualMenu));
            BackgroudColorField.RegisterValueChangedCallback(OnGroupColorChanged);
        }

        public void OnCreate()
        {
            ViewModel.PropertyChanged += OnViewModelChanged;
            ViewModel.onNodeAdded += OnNodesAdded;
            ViewModel.onNodeRemoved += OnNodesRemoved;
        }

        public void OnDestroy()
        {
            ViewModel.PropertyChanged -= OnViewModelChanged;
            ViewModel.onNodeAdded -= OnNodesAdded;
            ViewModel.onNodeRemoved -= OnNodesRemoved;
        }

        #region Callbacks

        private void OnViewModelChanged(object sender, PropertyChangedEventArgs e)
        {
            var group = sender as GroupProcessor;
            switch (e.PropertyName)
            {
                case nameof(Group.position):
                {
                    base.SetPosition(new Rect(group.Position.ToVector2(), GetPosition().size));
                    break;
                }
                case nameof(Group.groupName):
                {
                    if (string.IsNullOrEmpty(group.GroupName))
                        return;
                    this.title = ViewModel.GroupName;
                    Owner.SetDirty();
                    break;
                }
                case nameof(Group.backgroundColor):
                {
                    this.BackgroudColorField.SetValueWithoutNotify(group.BackgroundColor.ToColor());
                    this.style.backgroundColor = group.BackgroundColor.ToColor();
                    this.style.unityBackgroundImageTintColor = group.BackgroundColor.ToColor();
                    Owner.SetDirty();
                    break;
                }
            }
        }

        private void OnNodesAdded(BaseNodeProcessor[] nodes)
        {
            if (WithoutNotify)
            {
                return;
            }
            
            var tmp = WithoutNotify;
            try
            {
                WithoutNotify = false;
                base.AddElements(nodes.Select(node => Owner.NodeViews[node.ID]));
            }
            finally
            {
                WithoutNotify = tmp;
            }
        }

        private void OnNodesRemoved(BaseNodeProcessor[] nodes)
        {
            if (WithoutNotify)
            {
                return;
            }
            
            var tmp = WithoutNotify;
            try
            {
                WithoutNotify = false;
                base.RemoveElementsWithoutNotification(nodes.Select(node => Owner.NodeViews[node.ID]));
            }
            finally
            {
                WithoutNotify = tmp;
            }
        }

        #endregion

        protected override void OnGroupRenamed(string oldName, string newName)
        {
            Owner.CommandDispatcher.Do(new RenameGroupCommand(ViewModel, newName));
        }

        private void OnGroupColorChanged(ChangeEvent<Color> evt)
        {
            ViewModel.BackgroundColor = evt.newValue.ToInternalColor();
        }

        public override bool AcceptsElement(GraphElement element, ref string reasonWhyNotAccepted)
        {
            if (!base.AcceptsElement(element, ref reasonWhyNotAccepted))
                return false;
            switch (element)
            {
                case BaseNodeView:
                    return true;
                case StickyNoteView:
                    return true;
            }

            return false;
        }

        protected override void OnElementsAdded(IEnumerable<GraphElement> elements)
        {
            if (WithoutNotify)
                return;

            var tmp = WithoutNotify;
            WithoutNotify = true;
            var nodes = elements.Where(item => item is BaseNodeView).Select(item => (item as BaseNodeView).ViewModel).ToArray();
            Owner.CommandDispatcher.Do(new AddToGroupCommand(Owner.ViewModel, this.ViewModel, nodes));
            
            Owner.SetDirty();
            WithoutNotify = tmp;
        }

        protected override void OnElementsRemoved(IEnumerable<GraphElement> elements)
        {
            if (WithoutNotify)
                return;
            var tmp = WithoutNotify;
            WithoutNotify = true;
            var nodes = elements.Where(item => item is BaseNodeView).Select(item => (item as BaseNodeView).ViewModel).ToArray();
            Owner.CommandDispatcher.Do(new RemoveFromGroupCommand(Owner.ViewModel, this.ViewModel, nodes));

            Owner.SetDirty();
            WithoutNotify = tmp;
        }

        public override void OnSelected()
        {
            base.OnSelected();
            this.BringToFront();
        }
    }
}
#endif