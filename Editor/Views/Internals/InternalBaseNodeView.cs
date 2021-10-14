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
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

using NodeView = UnityEditor.Experimental.GraphView.Node;

namespace CZToolKit.GraphProcessor.Editors
{
    public abstract class InternalBaseNodeView : NodeView, IBindableView<BaseNode>
    {
        #region 字段
        Label titleLabel;
        public readonly VisualElement nodeBorder;
        public readonly VisualElement topPortContainer;
        public readonly VisualElement bottomPortContainer;
        public readonly VisualElement controlsContainer;
        public readonly VisualElement contentsHorizontalDivider;
        public readonly VisualElement portsVerticalDivider;
        public readonly VisualElement controlsHorizontalDivider;
        public readonly Dictionary<string, InternalBasePortView> portViews = new Dictionary<string, InternalBasePortView>();
        [NonSerialized]
        List<IconBadge> badges = new List<IconBadge>();
        #endregion

        #region 属性
        public Label TitleLabel
        {
            get
            {
                if (titleLabel == null)
                    titleLabel = titleContainer.Q<Label>("title-label");
                return titleLabel;
            }
        }
        public InternalBaseGraphView Owner { get; private set; }
        public BaseNode Model { get; protected set; }
        #endregion

        public InternalBaseNodeView()
        {
            styleSheets.Add(GraphProcessorStyles.BaseNodeViewStyle);
            styleSheets.Add(GraphProcessorStyles.PortViewTypesStyle);

            nodeBorder = this.Q(name: "node-border");

            contentsHorizontalDivider = contentContainer.Q(name: "divider", className: "horizontal");
            contentsHorizontalDivider.AddToClassList("contents-horizontal-divider");
            contentsHorizontalDivider.style.backgroundColor = Color.green;

            portsVerticalDivider = topContainer.Q(name: "divider", className: "vertical");
            portsVerticalDivider.AddToClassList("ports-vertical-divider");
            portsVerticalDivider.style.backgroundColor = Color.red;

            controlsContainer = new VisualElement { name = "controls" };
            controlsContainer.AddToClassList("node-controls");
            controlsContainer.style.backgroundColor = new Color(0.2f, 0.2f, 0.2f, 1);
            mainContainer.Add(controlsContainer);

            controlsHorizontalDivider = new VisualElement() { name = "divider" };
            controlsHorizontalDivider.style.height = 1;
            controlsHorizontalDivider.style.backgroundColor = new Color(0.2f, 0.2f, 0.2f, 1);
            controlsHorizontalDivider.StretchToParentWidth();
            controlsHorizontalDivider.AddToClassList("horizontal");
            controlsHorizontalDivider.AddToClassList("controls-horizontal-divider");
            controlsHorizontalDivider.style.backgroundColor = Color.blue;
            controlsContainer.Add(controlsHorizontalDivider);

            topPortContainer = new VisualElement { name = "top-port-container" };
            topPortContainer.style.justifyContent = Justify.Center;
            topPortContainer.style.alignItems = Align.Center;
            topPortContainer.style.flexDirection = FlexDirection.Row;
            Insert(0, topPortContainer);

            bottomPortContainer = new VisualElement { name = "bottom-port-container" };
            bottomPortContainer.style.justifyContent = Justify.Center;
            bottomPortContainer.style.alignItems = Align.Center;
            bottomPortContainer.style.flexDirection = FlexDirection.Row;
            Add(bottomPortContainer);

            TitleLabel.style.flexWrap = Wrap.Wrap;
        }

        #region Initialization
        public void SetUp(BaseNode node, InternalBaseGraphView graphView)
        {
            Model = node;
            Owner = graphView;

            // 初始化
            title = Model.Title;
            tooltip = Model.Tooltip;
            base.SetPosition(new Rect(Model.Position, GetPosition().size));
            titleContainer.style.backgroundColor = Model.TitleColor;
            TitleLabel.style.color = Model.TitleColor.GetLuminance() > 0.5f && Model.TitleColor.a > 0.5f ? Color.black : Color.white * 0.9f;

            foreach (var slot in Model.GetSlots())
            {
                InternalBasePortView portView = NewPortView(slot);
                portView.SetUp(slot, Owner);
                if (portView.orientation == Orientation.Horizontal)
                {
                    if (portView.direction == Direction.Input)
                        inputContainer.Add(portView);
                    else
                        outputContainer.Add(portView);
                }
                else
                {
                    if (portView.direction == Direction.Input)
                        topPortContainer.Add(portView);
                    else
                        bottomPortContainer.Add(portView);
                }
                portViews[slot.name] = portView;
            }

            // 绑定
            BindingProperties();
            RefreshPorts();
        }

        protected virtual void BindingProperties()
        {
            Model.BindingProperty<string>(BaseNode.TITLE_NAME, OnTitleChanged);
            Model.BindingProperty<Color>(BaseNode.TITLE_COLOR_NAME, OnTitleColorChanged);
            Model.BindingProperty<string>(BaseNode.TOOLTIP_NAME, OnTooltipChanged);
            Model.BindingProperty<Vector2>(BaseNode.POSITION_NAME, OnPositionChanged);
        }

        public virtual void UnBindingProperties()
        {
            Model.UnBindingProperty<string>(BaseNode.TITLE_NAME, OnTitleChanged);
            Model.UnBindingProperty<Color>(BaseNode.TITLE_COLOR_NAME, OnTitleColorChanged);
            Model.UnBindingProperty<string>(BaseNode.TOOLTIP_NAME, OnTooltipChanged);
            Model.UnBindingProperty<Vector2>(BaseNode.POSITION_NAME, OnPositionChanged);
        }
        #endregion

        #region 数据监听回调
        void OnTitleChanged(string title)
        {
            base.title = title;
        }
        void OnTooltipChanged(string tooltip)
        {
            base.tooltip = tooltip;
        }
        void OnPositionChanged(Vector2 position)
        {
            base.SetPosition(new Rect(position, GetPosition().size));
            Owner.SetDirty();
        }
        void OnTitleColorChanged(Color color)
        {
            titleContainer.style.backgroundColor = color;
            TitleLabel.style.color = color.GetLuminance() > 0.5f && color.a > 0.5f ? Color.black : Color.white * 0.9f;
        }
        #endregion

        #region 方法
        public void HighlightOn()
        {
            nodeBorder.AddToClassList("highlight");
        }

        public void HighlightOff()
        {
            nodeBorder.RemoveFromClassList("highlight");
        }

        public void SetDeletable(bool deletable)
        {
            if (deletable)
                capabilities |= Capabilities.Deletable;
            else
                capabilities &= ~Capabilities.Deletable;
        }

        public void SetMovable(bool movable)
        {
            if (movable)
                capabilities |= Capabilities.Movable;
            else
                capabilities &= ~Capabilities.Movable;
        }

        public void SetSelectable(bool selectable)
        {
            if (selectable)
                capabilities |= Capabilities.Selectable;
            else
                capabilities &= ~Capabilities.Selectable;
        }

        public void AddBadge(IconBadge badge)
        {
            Add(badge);
            badges.Add(badge);
            badge.AttachTo(topContainer, SpriteAlignment.RightCenter);
        }


        public void RemoveBadge(Func<IconBadge, bool> callback)
        {
            badges.RemoveAll(b =>
            {
                if (callback(b))
                {
                    b.Detach();
                    b.RemoveFromHierarchy();
                    return true;
                }
                return false;
            });
        }
        #endregion

        #region 抽象方法

        public abstract void Initialized();

        public abstract InternalBasePortView NewPortView(BaseSlot slot);

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            throw new NotImplementedException("你必须实现该方法才能正常使用");
        }
        #endregion
    }
}