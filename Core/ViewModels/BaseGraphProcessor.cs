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

using CZToolKit.Blackboard;
using System;
using System.Linq;
using System.Collections.Generic;

namespace CZToolKit.GraphProcessor
{
    [ViewModel(typeof(BaseGraph))]
    public partial class BaseGraphProcessor : ViewModel
    {
        #region Properties

        public BaseGraph Model { get; }

        public Type ModelType { get; }

        public InternalVector2Int Pan
        {
            get => GetPropertyValue<InternalVector2Int>(nameof(BaseGraph.pan));
            set => SetPropertyValue(nameof(BaseGraph.pan), value);
        }

        public float Zoom
        {
            get => GetPropertyValue<float>(nameof(BaseGraph.zoom));
            set => SetPropertyValue(nameof(BaseGraph.zoom), value);
        }

        public Events<string> Events { get; }

        public BlackboardProcessor<string> Blackboard { get; }

        #endregion

        public BaseGraphProcessor(BaseGraph model)
        {
            Model = model;
            ModelType = model.GetType();
            Model.pan = Model.pan == default ? InternalVector2Int.zero : Model.pan;
            Model.zoom = Model.zoom == 0 ? 1 : Model.zoom;
            Model.notes = Model.notes == null ? new Dictionary<int, StickyNote>() : Model.notes;

            this.RegisterProperty(nameof(BaseGraph.pan), () => ref Model.pan);
            this.RegisterProperty(nameof(BaseGraph.zoom), () => ref Model.zoom);

            this.Events = new Events<string>();
            this.Blackboard = new BlackboardProcessor<string>(new Blackboard<string>(), Events);

            BeginInitNodes();
            BeginInitConnections();
            EndInitConnections();
            EndInitNodes();
            InitGroups();
            InitNotes();
        }

        #region API

        public int NewID()
        {
            var id = 0;
            do
            {
                id++;
            } while (nodes.ContainsKey(id) || notes.ContainsKey(id) || id == 0);

            return id;
        }

        #endregion
    }
}