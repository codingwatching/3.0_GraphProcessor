#region 注 释
/***
 *
 *  Title:
 *  
 *  Description:
 *  
 *  Date:
 *  Version:
 *  Writer: 
 *
 */
#endregion
using UnityObject = UnityEngine.Object;

namespace CZToolKit.GraphProcessor
{
    public interface IGraphFromAsset
    {
        UnityObject Asset { get; }

        void Enable(UnityObject _from);
    }
}