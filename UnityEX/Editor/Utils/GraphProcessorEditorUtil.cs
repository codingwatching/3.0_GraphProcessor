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

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using Atom;
using UnityEditor;

namespace Atom.GraphProcessor.Editors
{
    public static class GraphProcessorEditorUtil
    {
        private static Dictionary<Type, Type> s_ViewTypesCache;

        static GraphProcessorEditorUtil()
        {
            Init();
        }

        private static void Init()
        {
            s_ViewTypesCache = new Dictionary<Type, Type>();
            foreach (var type in TypeCache.GetTypesWithAttribute<CustomViewAttribute>())
            {
                if (type.IsAbstract) continue;
                foreach (var attribute in type.GetCustomAttributes(false))
                {
                    if (!(attribute is CustomViewAttribute customViewAttribute))
                        continue;
                    s_ViewTypesCache[customViewAttribute.targetType] = type;
                }
            }
        }

        public static Type GetViewType(Type targetType)
        {
            var viewType = (Type)null;
            while (viewType == null)
            {
                s_ViewTypesCache.TryGetValue(targetType, out viewType);
                if (targetType.BaseType == null)
                    break;
                targetType = targetType.BaseType;
            }

            return viewType;
        }

        public static BaseGraph Clone(this BaseGraph graph)
        {
            var cloneGraph = Activator.CreateInstance(graph.GetType()) as BaseGraph;
            EditorUtility.CopySerializedManagedFieldsOnly(graph, cloneGraph);
            return cloneGraph;
        }
    }
}
#endif