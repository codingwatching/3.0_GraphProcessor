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
 *  Github: https://github.com/HalfLobsterMan
 *  Blog: https://www.crosshair.top/
 *
 */

#endregion

#if UNITY_EDITOR
using CZToolKit;
using CZToolKit.GraphProcessor;
using CZToolKit.GraphProcessor.Editors;
using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class FlowGraphView : BaseGraphView
{
    public FlowGraphView(BaseGraphVM graph, BaseGraphWindow window, CommandDispatcher commandDispatcher) : base(graph, window, commandDispatcher)
    {
    }

    protected override void BuildNodeMenu(NodeMenuWindow nodeMenu)
    {
        foreach (var pair in GraphProcessorUtil.NodeStaticInfos)
        {
            if (!typeof(FlowNode).IsAssignableFrom(pair.Key))
                continue;
            var nodeStaticInfo = pair.Value;
            if (nodeStaticInfo.hidden)
                continue;
            
            var path = nodeStaticInfo.path;
            var menu = nodeStaticInfo.menu;
            nodeMenu.entries.Add(new NodeMenuWindow.NodeEntry(path, menu, pair.Key));
        }
    }
    
    protected override BaseConnectionView NewConnectionView(BaseConnectionVM connection)
    {
        return new SampleConnectionView();
    }
}
#endif