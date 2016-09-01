using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using Calculator;

namespace DotnetPatch
{
    public class MethodBodyModifier
    {
        public MethodBodyModifier(string s, string path)
        {
            m_OriginalFile = s;
            m_OutputPath = path;

            string fn = Path.GetFileName(m_OriginalFile);
            File.Copy(m_OriginalFile, Path.Combine(m_OutputPath, fn), true);
        }

        public static List<string> ErrorTxts
        {
            get { return s_ErrorTxts; }
        }

        public void BeginExtend(uint newSize)
        {
            Load();
            m_Methods = new byte[newSize];
            m_Pos = 0;
            m_Header = m_ClrFile.PrepareNewSectionHeader(".ilcode", newSize, false);
            m_Methods.Initialize();
            m_Find = false;
        }

        public void Extend(string fullName, string methodName, uint insertSize)
        {
            string classNs = string.Empty;
            string className = fullName;
            int srcIx = fullName.LastIndexOf('.');
            if (srcIx > 0) {
                classNs = fullName.Substring(0, srcIx);
                className = fullName.Substring(srcIx + 1);
            }

            int srcStart, srcEnd;
            bool srcValid = GetTypeMethods(classNs, className, out srcStart, out srcEnd);
            if (!srcValid) {
                s_ErrorTxts.Add(string.Format("Can't find src {0}->{1}", className, srcValid));
                return;
            }
            MethodDefIterator it = m_ClrFile.Metadata.Tables[(int)CorTokenType.mdtMethodDef] as MethodDefIterator;
            for (uint ix = (uint)srcStart; ix < (uint)srcEnd; ++ix) {
                it.MoveTo(ix - 1);
                string name = GetString(m_ClrFile.Buffer, m_StringStart, it.Name);
                if (name == methodName) {
                    uint offset = m_ClrFile.GetFileOffset(it.RVA);
                    byte ilheader = m_ClrFile.Buffer[offset];
                    if ((ilheader & 0x03) == 0x02) {
                        uint size = (uint)(ilheader >> 2);
                        if (size + insertSize < 64) {
                            m_Methods[m_Pos] = (byte)(((size + insertSize) << 2) | 0x02);
                            Array.Copy(m_ClrFile.Buffer, offset + 1, m_Methods, m_Pos + 1 + insertSize, size);
                            it.RVA = m_Header.VirtualAddress + m_Pos;
                        } else {
                            MetadataTables.WriteUInt(m_Methods, m_Pos, 0x7fff3003);
                            MetadataTables.WriteUInt(m_Methods, m_Pos + 4, size + insertSize);
                            MetadataTables.WriteUInt(m_Methods, m_Pos + 8, 0);
                            Array.Copy(m_ClrFile.Buffer, offset + 1, m_Methods, m_Pos + 12 + insertSize, size);
                            it.RVA = m_Header.VirtualAddress + m_Pos;
                        }
                    } else if ((ilheader & 0x03) == 0x03) {
                        uint one = MetadataTables.ReadUInt(m_ClrFile.Buffer, offset);
                        uint two = MetadataTables.ReadUInt(m_ClrFile.Buffer, offset + 4);
                        uint three = MetadataTables.ReadUInt(m_ClrFile.Buffer, offset + 8);
                        MetadataTables.WriteUInt(m_Methods, m_Pos, one);
                        MetadataTables.WriteUInt(m_Methods, m_Pos + 4, two + insertSize);
                        MetadataTables.WriteUInt(m_Methods, m_Pos + 8, three);
                        Array.Copy(m_ClrFile.Buffer, offset + 12, m_Methods, m_Pos + 12 + insertSize, two);
                        it.RVA = m_Header.VirtualAddress + m_Pos;
                    }
                    m_Find = true;
                    break;
                }
            }
        }

        public void EndExtend()
        {
            if (m_Find) {
                SaveWithNewSection();
            }
        }

        public void BeginModify()
        {
            Load();
        }

        public void ModifyWithData(string fullName, string methodName, uint pos, byte[] data)
        {
            string classNs = string.Empty;
            string className = fullName;
            int srcIx = fullName.LastIndexOf('.');
            if (srcIx > 0) {
                classNs = fullName.Substring(0, srcIx);
                className = fullName.Substring(srcIx + 1);
            }

            Modify(classNs, className, methodName, (uint offset) => {
                Array.Copy(data, 0, m_ClrFile.Buffer, offset + pos, data.Length);
            });
        }
        public void ModifyWithField(string fullName, string methodName, uint pos, byte op, string fullCn, string fn)
        {
            string classNs = string.Empty;
            string className = fullName;
            int srcIx = fullName.LastIndexOf('.');
            if (srcIx > 0) {
                classNs = fullName.Substring(0, srcIx);
                className = fullName.Substring(srcIx + 1);
            }
            string ns = string.Empty;
            string cn = fullCn;
            int targetIx = fullCn.LastIndexOf('.');
            if (targetIx > 0) {
                ns = fullCn.Substring(0, targetIx);
                cn = fullCn.Substring(targetIx + 1);
            }

            Modify(classNs, className, methodName, (uint offset) => {
                int srcStart, srcEnd;
                bool srcValid = GetTypeFields(ns, cn, out srcStart, out srcEnd);
                if (!srcValid) {
                    s_ErrorTxts.Add(string.Format("Can't find src {0}->{1}", cn, srcValid));
                    return;
                }
                FieldIterator it = m_ClrFile.Metadata.Tables[(int)CorTokenType.mdtFieldDef] as FieldIterator;
                for (uint ix = (uint)srcStart; ix < (uint)srcEnd; ++ix) {
                    it.MoveTo(ix - 1);
                    string name = GetString(m_ClrFile.Buffer, m_StringStart, it.Name);
                    if (name == fn) {
                        m_ClrFile.Buffer[offset + pos] = op;
                        m_ClrFile.Buffer[offset + pos + 1] = (byte)(ix & 0xff);
                        m_ClrFile.Buffer[offset + pos + 2] = (byte)((ix >> 8) & 0xff);
                        m_ClrFile.Buffer[offset + pos + 3] = (byte)((ix >> 16) & 0xff);
                        m_ClrFile.Buffer[offset + pos + 4] = (byte)0x04;
                        break;
                    }
                }
            });
        }

        public void ModifyWithCallMethod(string fullName, string methodName, uint pos, byte op, string fullCn, string mn)
        {
            string classNs = string.Empty;
            string className = fullName;
            int srcIx = fullName.LastIndexOf('.');
            if (srcIx > 0) {
                classNs = fullName.Substring(0, srcIx);
                className = fullName.Substring(srcIx + 1);
            }
            string ns = string.Empty;
            string cn = fullCn;
            int targetIx = fullCn.LastIndexOf('.');
            if (targetIx > 0) {
                ns = fullCn.Substring(0, targetIx);
                cn = fullCn.Substring(targetIx + 1);
            }

            Modify(classNs, className, methodName, (uint offset) => {
                int srcStart, srcEnd;
                bool srcValid = GetTypeMethods(ns, cn, out srcStart, out srcEnd);
                if (!srcValid) {
                    s_ErrorTxts.Add(string.Format("Can't find src {0}->{1}", cn, srcValid));
                    return;
                }
                MethodDefIterator it = m_ClrFile.Metadata.Tables[(int)CorTokenType.mdtMethodDef] as MethodDefIterator;
                for (uint ix = (uint)srcStart; ix < (uint)srcEnd; ++ix) {
                    it.MoveTo(ix - 1);
                    string name = GetString(m_ClrFile.Buffer, m_StringStart, it.Name);
                    if (name == mn) {
                        m_ClrFile.Buffer[offset + pos] = op;
                        m_ClrFile.Buffer[offset + pos + 1] = (byte)(ix & 0xff);
                        m_ClrFile.Buffer[offset + pos + 2] = (byte)((ix >> 8) & 0xff);
                        m_ClrFile.Buffer[offset + pos + 3] = (byte)((ix >> 16) & 0xff);
                        m_ClrFile.Buffer[offset + pos + 4] = (byte)0x06;
                        break;
                    }
                }
            });
        }
        
        public void EndModify()
        {
            Save();
        }

        public void BeginReplace()
        {
            Load();
        }

        public void Replace(string src, string target)
        {
            string srcNs = string.Empty;
            string srcName = src;
            int srcIx = src.LastIndexOf('.');
            if (srcIx > 0) {
                srcNs = src.Substring(0, srcIx);
                srcName = src.Substring(srcIx + 1);
            }
            string targetNs = string.Empty;
            string targetName = target;
            int targetIx = target.LastIndexOf('.');
            if (targetIx > 0) {
                targetNs = target.Substring(0, targetIx);
                targetName = target.Substring(targetIx + 1);
            }

            int srcStart, srcEnd;
            int targetStart, targetEnd;
            bool srcValid = GetTypeMethods(srcNs, srcName, out srcStart, out srcEnd);
            bool targetValid = GetTypeMethods(targetNs, targetName, out targetStart, out targetEnd);
            if (!srcValid || !targetValid) {
                s_ErrorTxts.Add(string.Format("Can't find src {0}->{1} or target {2}->{3}", srcName, srcValid, targetName, targetValid));
                return;
            }
            MethodDefIterator it = m_ClrFile.Metadata.Tables[(int)CorTokenType.mdtMethodDef] as MethodDefIterator;
            for (uint ix = (uint)srcStart; ix < (uint)srcEnd; ++ix) {
                it.MoveTo(ix - 1);
                if (MoveToMatchedMethod(it, targetStart, targetEnd)) {
                    uint rva = it.RVA;
                    it.MoveTo(ix - 1);
                    it.RVA = rva;
                }
            }
        }

        public void EndReplace()
        {
            Save();
        }

        private void Load()
        {
            string fn = Path.GetFileName(m_OriginalFile);
            string tmp = Path.Combine(m_OutputPath, fn);

            m_ClrFile = new ClrFile();
            m_ClrFile.Load(tmp);
            uint mpos = m_ClrFile.Metadata.MetadataPosition;
            uint stringOffset = m_ClrFile.Metadata.StringsHeader.Offset;
            uint blobOffset = m_ClrFile.Metadata.BlobHeader.Offset;

            m_StringStart = mpos + stringOffset;
            m_BlobStart = mpos + blobOffset;
        }
        private void Save()
        {
            string fn = Path.GetFileName(m_OriginalFile);
            m_ClrFile.Save(Path.Combine(m_OutputPath, fn));
        }

        private void SaveWithNewSection()
        {
            string fn = Path.GetFileName(m_OriginalFile);
            m_ClrFile.Save(Path.Combine(m_OutputPath, fn), m_Header, m_Methods);
        }

        private void Modify(string nsName, string className, string methodName, Action<uint> callback)
        {
            int srcStart, srcEnd;
            bool srcValid = GetTypeMethods(nsName, className, out srcStart, out srcEnd);
            if (!srcValid) {
                s_ErrorTxts.Add(string.Format("Can't find src {0}->{1}", className, srcValid));
                return;
            }
            MethodDefIterator it = m_ClrFile.Metadata.Tables[(int)CorTokenType.mdtMethodDef] as MethodDefIterator;
            for (uint ix = (uint)srcStart; ix < (uint)srcEnd; ++ix) {
                it.MoveTo(ix - 1);
                string name = GetString(m_ClrFile.Buffer, m_StringStart, it.Name);
                if (name == methodName) {
                    uint offset = m_ClrFile.GetFileOffset(it.RVA);
                    byte ilheader = m_ClrFile.Buffer[offset];
                    if ((ilheader & 0x03) == 0x02) {
                        uint size = (uint)(ilheader >> 2);
                        uint headerSize = 1;

                        callback(offset + headerSize);
                    } else if ((ilheader & 0x03) == 0x03) {
                        uint one = MetadataTables.ReadUInt(m_ClrFile.Buffer, offset);
                        uint two = MetadataTables.ReadUInt(m_ClrFile.Buffer, offset + 4);
                        uint three = MetadataTables.ReadUInt(m_ClrFile.Buffer, offset + 8);

                        uint headerSize = ((one & 0x0000f000) >> 12) * 4;
                        
                        callback(offset + headerSize);
                    }
                }
            }
        }

        private bool MoveToMatchedMethod(MethodDefIterator it, int start, int end)
        {
            bool ret = false;
            string name = GetString(m_ClrFile.Buffer, m_StringStart, it.Name);
            string signature = GetBlobString(m_ClrFile.Buffer, m_BlobStart, it.Signature);
            if (name == ".ctor" || name == ".cctor")
                return false;
            for (uint ix = (uint)start; ix < (uint)end; ++ix) {
                it.MoveTo(ix - 1);
                string name2 = GetString(m_ClrFile.Buffer, m_StringStart, it.Name);
                string signature2 = GetBlobString(m_ClrFile.Buffer, m_BlobStart, it.Signature);
                if (0 == name.CompareTo(name2) && 0 == signature.CompareTo(signature2)) {
                    ret = true;
                    break;
                }
            }
            return ret;
        }

        private bool GetTypeMethods(string ns, string name, out int start, out int end)
        {
            start = end = -1;
            bool ret = false;
            MethodDefIterator methodIt = m_ClrFile.Metadata.Tables[(int)CorTokenType.mdtMethodDef] as MethodDefIterator;
            TypeDefIterator it = m_ClrFile.Metadata.Tables[(int)CorTokenType.mdtTypeDef] as TypeDefIterator;
            it.MoveToFirst();
            for (; !it.IsEnd; it.MoveToNext()) {
                string nsName = GetString(m_ClrFile.Buffer, m_StringStart, it.NameSpace);
                string typeName = GetString(m_ClrFile.Buffer, m_StringStart, it.Name);                
                if (0 == typeName.CompareTo(name) && (string.IsNullOrEmpty(ns) || 0==nsName.CompareTo(ns))) {
                    start = (int)it.MethodList;
                    if (it.MoveToNext()) {
                        end = (int)it.MethodList;
                    } else {
                        end = (int)methodIt.RecordNumber;
                    }
                    ret = true;
                    break;
                }
            }
            return ret;
        }
        private bool GetTypeFields(string ns, string name, out int start, out int end)
        {
            start = end = -1;
            bool ret = false;
            FieldIterator fieldIt = m_ClrFile.Metadata.Tables[(int)CorTokenType.mdtFieldDef] as FieldIterator;
            TypeDefIterator it = m_ClrFile.Metadata.Tables[(int)CorTokenType.mdtTypeDef] as TypeDefIterator;
            it.MoveToFirst();
            for (; !it.IsEnd; it.MoveToNext()) {
                string nsName = GetString(m_ClrFile.Buffer, m_StringStart, it.NameSpace);
                string typeName = GetString(m_ClrFile.Buffer, m_StringStart, it.Name);
                if (0 == typeName.CompareTo(name) && (string.IsNullOrEmpty(ns) || 0 == nsName.CompareTo(ns))) {
                    start = (int)it.FieldList;
                    if (it.MoveToNext()) {
                        end = (int)it.FieldList;
                    } else {
                        end = (int)fieldIt.RecordNumber;
                    }
                    ret = true;
                    break;
                }
            }
            return ret;
        }

        private static string GetString(byte[] buffer, uint start, uint offset)
        {
            int pos = (int)(start + offset);
            if (pos < 0 || pos > buffer.Length)
                return null;
            int nlen = 0;
            for (; buffer[pos + nlen] > 0; nlen++) ;
            if (nlen <= 0)
                return null;
            return System.Text.Encoding.UTF8.GetString(buffer, (int)pos, nlen);
        }
        private static string GetBlobString(byte[] buffer, uint start, uint offset)
        {
            int pos = (int)(start + offset);
            if (pos < 0 || pos > buffer.Length)
                return "";
            byte b = buffer[pos];
            int len = 0;
            if ((b & 0x80) == 0) {
                len = (b & 0x7F);
                pos++;
            } else if ((b & 0x40) == 0) {
                ushort v1 = b;
                ushort v2 = buffer[pos + 1];
                len = ((v1 & 0x3F) << 8) + v2;
                pos += 2;
            } else if ((b & 0x20) == 0) {
                uint v1 = b;
                uint v2 = buffer[pos + 1];
                uint v3 = buffer[pos + 2];
                uint v4 = buffer[pos + 3];
                len = (int)(((v1 & 0x1F) << 24) + (v2 << 16) + (v3 << 8) + v4);
                pos += 4;
            } else
                return "";
            string s = "";
            for (int i = 0; i < len; i++) {
                b = buffer[pos + i];
                s += b.ToString("X2");
            }
            return s;
        }

        private string m_OriginalFile = null;
        private string m_OutputPath = null;
        private ClrFile m_ClrFile = null;
        private uint m_StringStart = 0;
        private uint m_BlobStart = 0;

        private byte[] m_Methods = null;
        private uint m_Pos = 0;
        private IMAGE_SECTION_HEADER m_Header;
        bool m_Find = false;

        private static List<String> s_ErrorTxts = new List<string>();
    }
    public static class ScriptProcessor
    {
        public static void Init()
        {
            s_Calculator.Init();

            //◊¢≤·Gm√¸¡Ó
            s_Calculator.Register("begin", new ExpressionFactoryHelper<BeginCommand>());
            s_Calculator.Register("end", new ExpressionFactoryHelper<EndCommand>());
            s_Calculator.Register("beginfile", new ExpressionFactoryHelper<BeginFileCommand>());
            s_Calculator.Register("endfile", new ExpressionFactoryHelper<EndFileCommand>());
            s_Calculator.Register("beginreplace", new ExpressionFactoryHelper<BeginReplaceCommand>());
            s_Calculator.Register("replace", new ExpressionFactoryHelper<ReplaceCommand>());
            s_Calculator.Register("endreplace", new ExpressionFactoryHelper<EndReplaceCommand>());
            s_Calculator.Register("beginextend", new ExpressionFactoryHelper<BeginExtendCommand>());
            s_Calculator.Register("extend", new ExpressionFactoryHelper<ExtendCommand>());
            s_Calculator.Register("endextend", new ExpressionFactoryHelper<EndExtendCommand>());
            s_Calculator.Register("beginmodify", new ExpressionFactoryHelper<BeginModifyCommand>());
            s_Calculator.Register("endmodify", new ExpressionFactoryHelper<EndModifyCommand>());
            s_Calculator.Register("writeloadarg", new ExpressionFactoryHelper<WriteLoadArgCommand>());
            s_Calculator.Register("writeloadlocal", new ExpressionFactoryHelper<WriteLoadLocalCommand>());
            s_Calculator.Register("writeloadfield", new ExpressionFactoryHelper<WriteLoadFieldCommand>());
            s_Calculator.Register("writeloadstaticfield", new ExpressionFactoryHelper<WriteLoadStaticFieldCommand>());
            s_Calculator.Register("writecall", new ExpressionFactoryHelper<WriteCallCommand>());
            s_Calculator.Register("writecallvirt", new ExpressionFactoryHelper<WriteCallVirtCommand>());
            s_Calculator.Register("writenops", new ExpressionFactoryHelper<WriteNopsCommand>());
            s_Calculator.Register("getfilelist", new ExpressionFactoryHelper<GetFileListCommand>());
            s_Calculator.Register("log", new ExpressionFactoryHelper<LogCommand>());
        }
        public static void Start(IList<string> files, string outputPath, string scpFile)
        {
            s_FileList.Clear();
            s_FileList.AddRange(files);
            s_OutputPath = outputPath;
            
            s_Calculator.Load(scpFile);
            s_Calculator.Calc("main");
        }
        public static IList<string> GetFileList()
        {
            return s_FileList;
        }
        public static void Begin(string info)
        {
            s_CurNum = 0;
            s_TotalNum = s_FileList.Count;
            Program.MainForm.ProgressBar.Value = 0;
            Program.MainForm.StatusBar.Text = info;

            MethodBodyModifier.ErrorTxts.Clear();
            s_MethodBodyModifiers.Clear();
        }
        public static void End(string info)
        {
            Program.MainForm.StatusBar.Text = info;
            Program.MainForm.ResultCtrl.Text = string.Join("\r\n", MethodBodyModifier.ErrorTxts.ToArray());
            MethodBodyModifier.ErrorTxts.Clear();
        }
        public static void BeginFile(string file, string info)
        {
            Program.MainForm.StatusBar.Text = info;

            if (!s_MethodBodyModifiers.ContainsKey(file)) {
                MethodBodyModifier methodBodyModifier = new MethodBodyModifier(file, s_OutputPath);
                s_MethodBodyModifiers[file] = methodBodyModifier;
            }
        }
        public static void EndFile()
        {
            s_CurNum++;
            Program.MainForm.ProgressBar.Value = s_CurNum * 100 / s_TotalNum;
        }
        public static void BeginReplace(string file)
        {
            s_MethodBodyModifiers[file].BeginReplace();
        }
        public static void EndReplace(string file)
        {
            s_MethodBodyModifiers[file].EndReplace();
        }
        public static void Replace(string file, string src, string target)
        {
            s_MethodBodyModifiers[file].Replace(src, target);
        }
        public static void BeginExtend(string file, uint newSize)
        {
            s_MethodBodyModifiers[file].BeginExtend(newSize);
        }
        public static void Extend(string file, string fullClassName, string methodName, uint insertSize)
        {
            s_MethodBodyModifiers[file].Extend(fullClassName, methodName, insertSize);
        }
        public static void EndExtend(string file)
        {
            s_MethodBodyModifiers[file].EndExtend();
        }
        public static void BeginModify(string file)
        {
            s_MethodBodyModifiers[file].BeginModify();
        }
        public static void EndModify(string file)
        {
            s_MethodBodyModifiers[file].EndModify();
        }
        public static void WriteLoadArg(string file, string fullClassName, string methodName, uint pos, int index)
        {
            if (index >= 0 && index <= 3) {
                s_MethodBodyModifiers[file].ModifyWithData(fullClassName, methodName, pos, new byte[] { (byte)(0x02 + index) });
            } else if (index <= 0xff) {
                s_MethodBodyModifiers[file].ModifyWithData(fullClassName, methodName, pos, new byte[] { 0x0E, (byte)index });
            } else if (index <= 0xffff) {
                s_MethodBodyModifiers[file].ModifyWithData(fullClassName, methodName, pos, new byte[] { 0xFE, 0x09, (byte)(index & 0xff), (byte)((index >> 8) & 0xff) });
            }
        }
        public static void WriteLoadLocal(string file, string fullClassName, string methodName, uint pos, int index)
        {
            if (index >= 0 && index <= 3) {
                s_MethodBodyModifiers[file].ModifyWithData(fullClassName, methodName, pos, new byte[] { (byte)(0x06 + index) });
            } else if (index <= 0xff) {
                s_MethodBodyModifiers[file].ModifyWithData(fullClassName, methodName, pos, new byte[] { 0x11, (byte)index });
            } else if (index <= 0xffff) {
                s_MethodBodyModifiers[file].ModifyWithData(fullClassName, methodName, pos, new byte[] { 0xFE, 0x0C, (byte)(index & 0xff), (byte)((index >> 8) & 0xff) });
            }
        }
        public static void WriteLoadField(string file, string fullClassName, string methodName, uint pos, string cn, string fn)
        {
            s_MethodBodyModifiers[file].ModifyWithField(fullClassName, methodName, pos, 0x7B, cn, fn);
        }
        public static void WriteLoadStaticField(string file, string fullClassName, string methodName, uint pos, string cn, string fn)
        {
            s_MethodBodyModifiers[file].ModifyWithField(fullClassName, methodName, pos, 0x7E, cn, fn);
        }
        public static void WriteCall(string file, string fullClassName, string methodName, uint pos, string cn, string fn)
        {
            s_MethodBodyModifiers[file].ModifyWithCallMethod(fullClassName, methodName, pos, 0x28, cn, fn);
        }
        public static void WriteCallVirt(string file, string fullClassName, string methodName, uint pos, string cn, string fn)
        {
            s_MethodBodyModifiers[file].ModifyWithCallMethod(fullClassName, methodName, pos, 0x6F, cn, fn);
        }
        public static void WriteNops(string file, string fullClassName, string methodName, uint pos, int size)
        {
            byte[] nops = new byte[size];
            for (int ix = 0; ix < size; ++ix) {
                nops[ix] = 0;
            }
            s_MethodBodyModifiers[file].ModifyWithData(fullClassName, methodName, pos, nops);
        }

        private static DslCalculator s_Calculator = new DslCalculator();
        private static Dictionary<string, MethodBodyModifier> s_MethodBodyModifiers = new Dictionary<string, MethodBodyModifier>();
        private static List<string> s_FileList = new List<string>();
        private static string s_OutputPath = string.Empty;
        private static string s_Class = string.Empty;
        private static string s_Method = string.Empty;
        private static int s_CurNum = 0;
        private static int s_TotalNum = 0;
    }
}
