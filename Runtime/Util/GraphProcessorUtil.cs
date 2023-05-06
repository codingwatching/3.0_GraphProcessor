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
using System;
using System.Collections.Generic;
using CZToolKit.Common;

namespace CZToolKit.GraphProcessor
{
    public class NodeStaticInfo
    {
        public string path;
        public string[] menu;
        public bool hidden;
        public string title;
        public string tooltip;
        public bool customTitleColor;
        public InternalColor titleColor;
    }
    
    public static class GraphProcessorUtil
    {
        public readonly static Dictionary<Type, NodeStaticInfo> NodeStaticInfos = new Dictionary<Type, NodeStaticInfo>();

        static GraphProcessorUtil()
        {
            foreach (var t in Util_TypeCache.GetTypesDerivedFrom<BaseNode>())
            {
                if (t.IsAbstract)
                    continue;
                
                var nodeStaticInfo = new NodeStaticInfo();
                nodeStaticInfo.title = t.Name;
                nodeStaticInfo.tooltip = string.Empty;
                nodeStaticInfo.titleColor = default;
                NodeStaticInfos.Add(t, nodeStaticInfo);
                
                if (Util_Attribute.TryGetTypeAttribute(t, true, out NodeMenuAttribute nodeMenu))
                {
                    if (!string.IsNullOrEmpty(nodeMenu.path))
                    {
                        nodeStaticInfo.path = nodeMenu.path;
                        nodeStaticInfo.menu = nodeMenu.path.Split('/');
                        nodeStaticInfo.title = nodeStaticInfo.menu[nodeStaticInfo.menu.Length - 1];
                    }
                    else
                    {
                        nodeStaticInfo.path = t.Name;
                        nodeStaticInfo.menu = new string[] { t.Name };
                        nodeStaticInfo.title = t.Name;
                    }
                    nodeStaticInfo.hidden = nodeMenu.hidden;
                }
                
                if (Util_Attribute.TryGetTypeAttribute(t, true, out NodeTitleAttribute titleAttr))
                {
                    if (!string.IsNullOrEmpty(titleAttr.title))
                        nodeStaticInfo.title = titleAttr.title;
                }
                
                if (Util_Attribute.TryGetTypeAttribute(t, true, out NodeTooltipAttribute tooltipAttr))
                {
                    nodeStaticInfo.tooltip = tooltipAttr.Tooltip;
                }
                
                if (Util_Attribute.TryGetTypeAttribute(t, true, out NodeTitleColorAttribute titleColorAttr))
                {
                    nodeStaticInfo.customTitleColor = true;
                    nodeStaticInfo.titleColor = titleColorAttr.color;
                }
            }
        }
    }
}