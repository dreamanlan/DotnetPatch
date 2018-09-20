using System;
using System.Collections.Generic;
using System.IO;
using DotnetPatch;

namespace Calculator
{
    //---------------------------------------------------------------------------------------------------------------
    internal class BeginCommand : SimpleExpressionBase
    {
        protected override object OnCalc(IList<object> operands)
        {
            string info = operands[0] as string;
            ScriptProcessor.Begin(info);
            return 0;
        }
    }
    internal class EndCommand : SimpleExpressionBase
    {
        protected override object OnCalc(IList<object> operands)
        {
            string info = operands[0] as string;
            ScriptProcessor.End(info);
            return 0;
        }
    }
    internal class BeginFileCommand : SimpleExpressionBase
    {
        protected override object OnCalc(IList<object> operands)
        {
            string file = operands[0] as string;
            string info = operands[1] as string;
            ScriptProcessor.BeginFile(file, info);
            return 0;
        }
    }
    internal class EndFileCommand : SimpleExpressionBase
    {
        protected override object OnCalc(IList<object> operands)
        {
            ScriptProcessor.EndFile();
            return 0;
        }
    }
    internal class BeginReplaceCommand : SimpleExpressionBase
    {
        protected override object OnCalc(IList<object> operands)
        {
            string file = operands[0] as string;
            ScriptProcessor.BeginReplace(file);
            return 0;
        }
    }
    internal class ReplaceCommand : SimpleExpressionBase
    {
        protected override object OnCalc(IList<object> operands)
        {
            string file = operands[0] as string;
            string srcClass = operands[1] as string;
            string targetClass = operands[2] as string;
            ScriptProcessor.Replace(file, srcClass, targetClass);
            return 0;
        }
    }
    internal class EndReplaceCommand : SimpleExpressionBase
    {
        protected override object OnCalc(IList<object> operands)
        {
            string file = operands[0] as string;
            ScriptProcessor.EndReplace(file);
            return 0;
        }
    }
    internal class BeginExtendCommand : SimpleExpressionBase
    {
        protected override object OnCalc(IList<object> operands)
        {
            string file = operands[0] as string;
            uint newSize = CastTo<uint>(operands[1]);
            ScriptProcessor.BeginExtend(file, newSize);
            return 0;
        }
    }
    internal class ExtendCommand : SimpleExpressionBase
    {
        protected override object OnCalc(IList<object> operands)
        {
            string file = operands[0] as string;
            string className = operands[1] as string;
            string methodName = operands[2] as string;
            uint insertSize = CastTo<uint>(operands[3]);
            ScriptProcessor.Extend(file, className, methodName, insertSize);
            return 0;
        }
    }
    internal class EndExtendCommand : SimpleExpressionBase
    {
        protected override object OnCalc(IList<object> operands)
        {
            string file = operands[0] as string;
            ScriptProcessor.EndExtend(file);
            return 0;
        }
    }
    internal class BeginModifyCommand : SimpleExpressionBase
    {
        protected override object OnCalc(IList<object> operands)
        {
            string file = operands[0] as string;
            ScriptProcessor.BeginModify(file);
            return 0;
        }
    }
    internal class EndModifyCommand : SimpleExpressionBase
    {
        protected override object OnCalc(IList<object> operands)
        {
            string file = operands[0] as string;
            ScriptProcessor.EndModify(file);
            return 0;
        }
    }
    //---------------------------------------------------------------------------------------------------------------
    internal class WriteLoadArgCommand : SimpleExpressionBase
    {
        protected override object OnCalc(IList<object> operands)
        {
            string file = operands[0] as string;
            string className = operands[1] as string;
            string methodName = operands[2] as string;
            uint pos = CastTo<uint>(operands[3]);
            int index = CastTo<int>(operands[4]);
            ScriptProcessor.WriteLoadArg(file, className, methodName, pos, index);
            return 0;
        }
    }
    internal class WriteLoadLocalCommand : SimpleExpressionBase
    {
        protected override object OnCalc(IList<object> operands)
        {
            string file = operands[0] as string;
            string className = operands[1] as string;
            string methodName = operands[2] as string;
            uint pos = CastTo<uint>(operands[3]);
            int index = CastTo<int>(operands[4]);
            ScriptProcessor.WriteLoadLocal(file, className, methodName, pos, index);
            return 0;
        }
    }
    internal class WriteLoadFieldCommand : SimpleExpressionBase
    {
        protected override object OnCalc(IList<object> operands)
        {
            string file = operands[0] as string;
            string className = operands[1] as string;
            string methodName = operands[2] as string;
            uint pos = CastTo<uint>(operands[3]);
            string cn = operands[4] as string;
            string fn = operands[5] as string;
            ScriptProcessor.WriteLoadField(file, className, methodName, pos, cn, fn);
            return 0;
        }
    }
    internal class WriteLoadStaticFieldCommand : SimpleExpressionBase
    {
        protected override object OnCalc(IList<object> operands)
        {
            string file = operands[0] as string;
            string className = operands[1] as string;
            string methodName = operands[2] as string;
            uint pos = CastTo<uint>(operands[3]);
            string cn = operands[4] as string;
            string fn = operands[5] as string;
            ScriptProcessor.WriteLoadStaticField(file, className, methodName, pos, cn, fn);
            return 0;
        }
    }
    internal class WriteCallCommand : SimpleExpressionBase
    {
        protected override object OnCalc(IList<object> operands)
        {
            string file = operands[0] as string;
            string className = operands[1] as string;
            string methodName = operands[2] as string;
            uint pos = CastTo<uint>(operands[3]);
            string cn = operands[4] as string;
            string mn = operands[5] as string;
            ScriptProcessor.WriteCall(file, className, methodName, pos, cn, mn);
            return 0;
        }
    }
    internal class WriteCallVirtCommand : SimpleExpressionBase
    {
        protected override object OnCalc(IList<object> operands)
        {
            string file = operands[0] as string;
            string className = operands[1] as string;
            string methodName = operands[2] as string;
            uint pos = CastTo<uint>(operands[3]);
            string cn = operands[4] as string;
            string mn = operands[5] as string;
            ScriptProcessor.WriteCallVirt(file, className, methodName, pos, cn, mn);
            return 0;
        }
    }
    internal class WriteNopsCommand : SimpleExpressionBase
    {
        protected override object OnCalc(IList<object> operands)
        {
            string file = operands[0] as string;
            string className = operands[1] as string;
            string methodName = operands[2] as string;
            uint pos = CastTo<uint>(operands[3]);
            int size = CastTo<int>(operands[4]);
            ScriptProcessor.WriteNops(file, className, methodName, pos, size);
            return 0;
        }
    }
    internal class GetFileListCommand : SimpleExpressionBase
    {
        protected override object OnCalc(IList<object> operands)
        {
            return ScriptProcessor.GetFileList();
        }
    }
    //---------------------------------------------------------------------------------------------------------------
    internal class LogCommand : SimpleExpressionBase
    {
        protected override object OnCalc(IList<object> operands)
        {
            if (operands.Count > 0) {
                string format = operands[0] as string;
                List<object> vargs = new List<object>();
                vargs.AddRange(operands);
                vargs.RemoveAt(0);
                MethodBodyModifier.ErrorTxts.Add(string.Format(format, vargs.ToArray()));
            }
            return 0;
        }
    }
    //---------------------------------------------------------------------------------------------------------------
}
