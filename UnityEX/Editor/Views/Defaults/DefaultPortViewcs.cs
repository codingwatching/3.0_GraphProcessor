﻿
#if UNITY_EDITOR
using System;
using UnityEditor.Experimental.GraphView;

namespace Atom.GraphProcessor.Editors
{
    [CustomView(typeof(BasePort))]
    public class DefaultPortView : BasePortView
    {
        protected DefaultPortView(Orientation orientation, Direction direction, Capacity capacity, Type type, IEdgeConnectorListener connectorListener) : base(orientation, direction, capacity, type, connectorListener)
        {
        }

        public DefaultPortView(PortProcessor port, IEdgeConnectorListener connectorListener) : base(port, connectorListener)
        {
        }
    }
}
#endif