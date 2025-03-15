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

namespace Atom.GraphProcessor
{
    public interface IGetPortValue
    {
        object GetValue(string portName);
    }

    public interface IGetPortValue<T>
    {
        T GetValue(string portName);
    }

    public interface ISetPortValue
    {
        void GetValue(string portName, object value);
    }

    public interface ISetPortValue<T>
    {
        void GetValue(string portName, T value);
    }
}
