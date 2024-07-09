using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DotnetPatch;
using ScriptableFramework;
using DotnetStoryScript;
using DotnetStoryScript.DslExpression;

namespace DotnetStoryScript.DslExpression
{
    //---------------------------------------------------------------------------------------------------------------
    internal class BeginCommand : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            string info = operands[0].AsString;
            ScriptProcessor.Begin(info);
            return 0;
        }
    }
    internal class EndCommand : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            string info = operands[0].AsString;
            ScriptProcessor.End(info);
            return 0;
        }
    }
    internal class BeginFileCommand : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            string file = operands[0].AsString;
            string info = operands[1].AsString;
            ScriptProcessor.BeginFile(file, info);
            return 0;
        }
    }
    internal class EndFileCommand : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            ScriptProcessor.EndFile();
            return 0;
        }
    }
    internal class BeginReplaceCommand : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            string file = operands[0].AsString;
            ScriptProcessor.BeginReplace(file);
            return 0;
        }
    }
    internal class ReplaceCommand : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            string file = operands[0].AsString;
            string srcClass = operands[1].AsString;
            string targetClass = operands[2].AsString;
            ScriptProcessor.Replace(file, srcClass, targetClass);
            return 0;
        }
    }
    internal class ReplaceAssemblyRefNameCommand : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            string file = operands[0].AsString;
            string srcName = operands[1].AsString;
            string targetName = operands[2].AsString;
            ScriptProcessor.ReplaceAssemblyRefName(file, srcName, targetName);
            return 0;
        }
    }
    internal class RedirectAssemblyCommand : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            string file = operands[0].AsString;
            string targetAssem = operands[1].AsString;
            List<string> srcAssems = new List<string>();
            for (int i = 2; i < operands.Count; ++i) {
                var str = operands[i].AsString;
                if (null != str) {
                    srcAssems.Add(str);
                }
                else {
                    var strList = operands[i].As<IList>();
                    if (null != strList) {
                        foreach (var strObj in strList) {
                            var tempStr = strObj as string;
                            if (null != tempStr)
                                srcAssems.Add(tempStr);
                        }
                    }
                }
            }
            ScriptProcessor.RedirectAssembly(file, targetAssem, srcAssems);
            return 0;
        }
    }
    internal class EndReplaceCommand : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            string file = operands[0].AsString;
            ScriptProcessor.EndReplace(file);
            return 0;
        }
    }
    internal class BeginExtendCommand : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            string file = operands[0].AsString;
            uint newSize = CastTo<uint>(operands[1]);
            ScriptProcessor.BeginExtend(file, newSize);
            return 0;
        }
    }
    internal class ExtendCommand : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            string file = operands[0].AsString;
            string className = operands[1].AsString;
            string methodName = operands[2].AsString;
            uint insertSize = CastTo<uint>(operands[3]);
            ScriptProcessor.Extend(file, className, methodName, insertSize);
            return 0;
        }
    }
    internal class EndExtendCommand : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            string file = operands[0].AsString;
            ScriptProcessor.EndExtend(file);
            return 0;
        }
    }
    internal class BeginModifyCommand : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            string file = operands[0].AsString;
            ScriptProcessor.BeginModify(file);
            return 0;
        }
    }
    internal class EndModifyCommand : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            string file = operands[0].AsString;
            ScriptProcessor.EndModify(file);
            return 0;
        }
    }
    //---------------------------------------------------------------------------------------------------------------
    internal class WriteLoadArgCommand : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            string file = operands[0].AsString;
            string className = operands[1].AsString;
            string methodName = operands[2].AsString;
            uint pos = CastTo<uint>(operands[3]);
            int index = CastTo<int>(operands[4]);
            ScriptProcessor.WriteLoadArg(file, className, methodName, pos, index);
            return 0;
        }
    }
    internal class WriteLoadLocalCommand : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            string file = operands[0].AsString;
            string className = operands[1].AsString;
            string methodName = operands[2].AsString;
            uint pos = CastTo<uint>(operands[3]);
            int index = CastTo<int>(operands[4]);
            ScriptProcessor.WriteLoadLocal(file, className, methodName, pos, index);
            return 0;
        }
    }
    internal class WriteLoadFieldCommand : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            string file = operands[0].AsString;
            string className = operands[1].AsString;
            string methodName = operands[2].AsString;
            uint pos = CastTo<uint>(operands[3]);
            string cn = operands[4].AsString;
            string fn = operands[5].AsString;
            ScriptProcessor.WriteLoadField(file, className, methodName, pos, cn, fn);
            return 0;
        }
    }
    internal class WriteLoadStaticFieldCommand : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            string file = operands[0].AsString;
            string className = operands[1].AsString;
            string methodName = operands[2].AsString;
            uint pos = CastTo<uint>(operands[3]);
            string cn = operands[4].AsString;
            string fn = operands[5].AsString;
            ScriptProcessor.WriteLoadStaticField(file, className, methodName, pos, cn, fn);
            return 0;
        }
    }
    internal class WriteCallCommand : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            string file = operands[0].AsString;
            string className = operands[1].AsString;
            string methodName = operands[2].AsString;
            uint pos = CastTo<uint>(operands[3]);
            string cn = operands[4].AsString;
            string mn = operands[5].AsString;
            ScriptProcessor.WriteCall(file, className, methodName, pos, cn, mn);
            return 0;
        }
    }
    internal class WriteCallVirtCommand : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            string file = operands[0].AsString;
            string className = operands[1].AsString;
            string methodName = operands[2].AsString;
            uint pos = CastTo<uint>(operands[3]);
            string cn = operands[4].AsString;
            string mn = operands[5].AsString;
            ScriptProcessor.WriteCallVirt(file, className, methodName, pos, cn, mn);
            return 0;
        }
    }
    internal class WriteNopsCommand : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            string file = operands[0].AsString;
            string className = operands[1].AsString;
            string methodName = operands[2].AsString;
            uint pos = CastTo<uint>(operands[3]);
            int size = CastTo<int>(operands[4]);
            ScriptProcessor.WriteNops(file, className, methodName, pos, size);
            return 0;
        }
    }
    internal class GetFileListCommand : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            return BoxedValue.FromObject(ScriptProcessor.GetFileList());
        }
    }
    //---------------------------------------------------------------------------------------------------------------
    internal class LogCommand : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count > 0) {
                string format = operands[0].AsString;
                List<object> vargs = new List<object>();
                for(int i = 1; i < operands.Count; ++i) {
                    var opd = operands[i].GetObject();
                    vargs.Add(opd);
                }
                ClrFileModifier.ResultTexts.Add(string.Format(format, vargs.ToArray()));
            }
            return 0;
        }
    }
    //---------------------------------------------------------------------------------------------------------------
}
