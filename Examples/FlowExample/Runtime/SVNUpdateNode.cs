﻿using System.Collections.Generic;
using Atom.GraphProcessor;
using Atom;
using Sirenix.OdinInspector;

[NodeMenu("SVN Update")]
public class SVNUpdateNode : FlowNode
{
    [FolderPath]
    [LabelText("目标文件夹")]
    public List<string> folders = new List<string>();
}

[ViewModel(typeof(SVNUpdateNode))]
public class SvnUpdateNodeProcessor : FlowNodeProcessor
{
    public SvnUpdateNodeProcessor(SVNUpdateNode model) : base(model)
    {
        
    }

    protected override void Execute()
    {
        FlowNext();
    }
}
