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
using Atom.GraphProcessor;
using UnityEngine;

public class SampleGraphTest : GraphAssetOwner<SampleGraphAsset, SampleGraphProcessor>
{
    private void Update()
    {
        foreach (var node in T_Graph.Nodes.Values)
        {
            if (node is LogNodeProcessor debugNode)
            {
                debugNode.DebugInput();
            }
        }
    }
}
