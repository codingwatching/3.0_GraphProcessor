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
using UnityEngine;

namespace CZToolKit.GraphProcessor
{
    [Serializable]
    public partial class BaseConnection
    {
        [SerializeField] string from;
        [SerializeField] string fromSlotName;
        [SerializeField] string to;
        [SerializeField] string toSlotName;
    }
}