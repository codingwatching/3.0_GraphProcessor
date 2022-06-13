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
using CZToolKit.Core;
using CZToolKit.Core.Editors;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace CZToolKit.GraphProcessor.Editors
{
    public partial class BaseGraphView
    {
        List<Port> compatiblePorts = new List<Port>();

        protected virtual void OnInitialized() { }

        protected virtual void OnBindingProperties() { }

        protected virtual void OnUnbindingProperties() { }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction("Create Group", delegate
            {
                var group = new Group("New Group");
                group.nodes.AddRange(selection.Where(select => select is BaseNodeView).Select(select => (select as BaseNodeView).Model.GUID));
                CommandDispacter.Do(new AddGroupCommand(Model, group));
            }, (DropdownMenuAction a) => canDeleteSelection && selection.Find(s => s is BaseNodeView) != null ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Hidden);

            base.BuildContextualMenu(evt);

            evt.menu.MenuItems().RemoveAll(item =>
            {
                if (item is DropdownMenuSeparator)
                {
                    return true;
                }
                if (!(item is DropdownMenuAction actionItem))
                {
                    return false;
                }
                switch (actionItem.name)
                {
                    case "Cut":
                    case "Copy":
                    case "Paste":
                    case "Duplicate":
                        return true;
                    default:
                        return false;
                }
            });

            if (evt.target is GraphView || evt.target is Node || evt.target is Group || evt.target is Edge)
            {
                evt.menu.AppendAction("Delete", delegate
                {
                    DeleteSelectionCallback(AskUser.DontAskUser);
                }, (DropdownMenuAction a) => canDeleteSelection ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Hidden);
            }
        }

        public override List<Port> GetCompatiblePorts(Port startPortView, NodeAdapter nodeAdapter)
        {
            BasePortView portView = startPortView as BasePortView;

            compatiblePorts.Clear();
            switch (portView.Model.direction)
            {
                case BasePort.Direction.Input:
                    {
                        ports.ForEach(_portView =>
                        {
                            var fromPortView = _portView as BasePortView;
                            if (IsCompatible(fromPortView, portView, nodeAdapter))
                                compatiblePorts.Add(_portView);
                        });
                    }
                    break;
                case BasePort.Direction.Output:
                    {
                        ports.ForEach(_portView =>
                        {
                            var toPortView = _portView as BasePortView;
                            if (IsCompatible(portView, toPortView, nodeAdapter))
                                compatiblePorts.Add(_portView);
                        });
                    }
                    break;
            }
            return compatiblePorts;
        }

        protected virtual IEnumerable<Type> GetNodeTypes()
        {
            foreach (var type in Util_Reflection.GetChildTypes<BaseNode>())
            {
                if (type.IsAbstract) continue;
                yield return type;
            }
        }

        protected virtual BaseNodeView NewNodeView(BaseNode node)
        {
            var type = GraphProcessorEditorUtil.GetNodeViewType(node.GetType());
            if (type == null)
                return new BaseNodeView();
            return Activator.CreateInstance(type) as BaseNodeView;
        }

        protected virtual BaseConnectionView NewConnectionView(BaseConnection connection)
        {
            return new BaseConnectionView();
        }

        protected virtual void UpdateInspector()
        {
            foreach (var element in selection)
            {
                switch (element)
                {
                    case BaseNodeView nodeView:
                        ObjectEditor.DrawObjectInInspector("Node", nodeView, GraphAsset);
                        return;
                    case BaseConnectionView edgeView:
                        ObjectEditor.DrawObjectInInspector("Connection", edgeView, GraphAsset);
                        return;
                    default:
                        break;
                }
            }

            if (Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<IGraphAssetOwner>() != null)
                return;
            ObjectEditor.DrawObjectInInspector("Graph", this, GraphAsset);
        }

        protected virtual bool IsCompatible(BasePortView fromPortView, BasePortView toPortView, NodeAdapter nodeAdapter)
        {
            if (toPortView.direction == fromPortView.direction)
                return false;
            // 类型兼容查询
            if (!toPortView.Model.Type.IsAssignableFrom(fromPortView.Model.Type) && !fromPortView.Model.Type.IsAssignableFrom(toPortView.Model.Type))
                return false;
            return true;
        }
    }
}
#endif