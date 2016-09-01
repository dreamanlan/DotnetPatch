using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace DotnetPatch
{
    public class MetadataTables
    {
        public uint MetadataPosition
        {
            get { return metadataPosition; }
        }

        public uint MetadataSize
        {
            get { return metadataSize; }
        }

        public MetadataRoot Root
        {
            get { return root; }
        }

        public StreamHeader MetadataHeader
        {
            get { return metadataHeader; }
        }

        public StreamHeader StringsHeader
        {
            get { return stringsHeader; }
        }

        public StreamHeader UserStringsHeader
        {
            get { return userStringsHeader; }
        }

        public StreamHeader GuidHeader
        {
            get { return guidHeader; }
        }

        public StreamHeader BlobHeader
        {
            get { return blobHeader; }
        }

        public MetadataStream MetadataStreamRoot
        {
            get { return metadataStreamRoot; }
        }

        public IMetadataIterator[] Tables
        {
            get { return tables; }
        }

        public uint BlobIndexSize
        {
            get { return blobIndexSize; }
        }

        public uint GuidIndexSize
        {
            get { return guidIndexSize; }
        }

        public uint StringIndexSize
        {
            get { return stringIndexSize; }
        }

        public uint GetTableIndexSize(CorTokenType id)
        {
            uint v = (0x01 << 16);
            int index = (int)id;
            if (NumberOfTableRow[index] >= v) {
                return 4;
            } else {
                return 2;
            }
        }
        public uint GetCodedIndexSize(CodedIndexType type)
        {
            return CodedIndexSize.GetSize(type, NumberOfTableRow);
        }

        public MetadataTables(byte[] buffer, uint pos, uint size)
        {
            metadataPosition = pos;
            metadataSize = size;

            ReadMetadataRoot(buffer, pos);
            pos += metadataHeader.Offset;

            sortedOfTable = new bool[64];
            numberOfTableRow = new uint[64];
            tables = new IMetadataIterator[64];
            metadataStreamRoot = ReadMetadataStream(buffer, pos);
            pos += MetadataStream.StructSize;
            if (((HeapSizeType)MetadataStreamRoot.HeapSizes & HeapSizeType.Blob) == HeapSizeType.Blob) {
                blobIndexSize = 4;
            } else {
                blobIndexSize = 2;
            }
            if (((HeapSizeType)MetadataStreamRoot.HeapSizes & HeapSizeType.GUID) == HeapSizeType.GUID) {
                guidIndexSize = 4;
            } else {
                guidIndexSize = 2;
            }
            if (((HeapSizeType)MetadataStreamRoot.HeapSizes & HeapSizeType.String) == HeapSizeType.String) {
                stringIndexSize = 4;
            } else {
                stringIndexSize = 2;
            }
            for (int i = 0; i < 64; i++) {
                UInt64 bit = (((UInt64)0x01) << i);
                if ((MetadataStreamRoot.Valid & bit) == bit) {
                    numberOfTableRow[i] = ReadUInt(buffer, pos);
                    pos += 4;
                } else {
                    numberOfTableRow[i] = 0;
                }
                if ((MetadataStreamRoot.Sorted & bit) == bit) {
                    sortedOfTable[i] = true;
                } else {
                    sortedOfTable[i] = false;
                }
            }
            for (int i = 0; i < 64; i++) {
                CorTokenType type = (CorTokenType)i;
                tables[i] = MetadataIterator.Create(this, buffer, pos, type);
                tables[i].MoveToEnd();
                pos = tables[i].Position;
            }
        }

        internal bool[] SortedOfTable
        {
            get { return sortedOfTable; }
        }

        internal uint[] NumberOfTableRow
        {
            get { return numberOfTableRow; }
        }

        internal static unsafe uint ReadUInt(byte[] buffer, uint pos)
        {
            uint v = 0;
            fixed (byte* p = buffer) {
                uint* ptr = (uint*)(p + pos);
                v = *ptr;
            }
            return v;
        }
        internal static unsafe ushort ReadUShort(byte[] buffer, uint pos)
        {
            ushort v = 0;
            fixed (byte* p = buffer) {
                ushort* ptr = (ushort*)(p + pos);
                v = *ptr;
            }
            return v;
        }

        internal static unsafe void WriteUInt(byte[] buffer, uint pos, uint val)
        {
            fixed (byte* p = buffer) {
                uint* ptr = (uint*)(p + pos);
                *ptr = val;
            }
        }
        internal static unsafe void WriteUShort(byte[] buffer, uint pos, ushort val)
        {
            fixed (byte* p = buffer) {
                ushort* ptr = (ushort*)(p + pos);
                *ptr = val;
            }
        }

        private unsafe void ReadMetadataRoot(byte[] buffer, uint pos)
        {
            root = new MetadataRoot(buffer, pos);
            pos += root.StructSize;
            ushort streamNum = ReadUShort(buffer, pos);
            pos += sizeof(ushort);
            for (uint i = 0; i < streamNum; i++) {
                StreamHeader header = new StreamHeader(buffer, pos);
                pos += header.StructSize;

                if (header.Name == "#~") {
                    metadataHeader = header;
                } else if (header.Name == "#Strings") {
                    stringsHeader = header;
                } else if (header.Name == "#US") {
                    userStringsHeader = header;
                } else if (header.Name == "#GUID") {
                    guidHeader = header;
                } else if (header.Name == "#Blob") {
                    blobHeader = header;
                }
            }
        }
        private static unsafe MetadataStream ReadMetadataStream(byte[] buffer, uint pos)
        {
            MetadataStream meta = new MetadataStream();
            fixed (byte* p = buffer) {
                MetadataStream* ptr = (MetadataStream*)(p + pos);
                meta = *ptr;
            }
            return meta;
        }

        private MetadataRoot root = null;
        private StreamHeader metadataHeader;
        private StreamHeader stringsHeader;
        private StreamHeader userStringsHeader;
        private StreamHeader guidHeader;
        private StreamHeader blobHeader;

        private MetadataStream metadataStreamRoot;
        private IMetadataIterator[] tables = null;

        private bool[] sortedOfTable;
        private uint[] numberOfTableRow;

        private uint blobIndexSize;
        private uint guidIndexSize;
        private uint stringIndexSize;

        private uint metadataPosition = 0;
        private uint metadataSize = 0;
    }

    public class MetadataRoot
    {
        public uint StructSize
        {
            get { return structSize; }
        }

        public uint Signature
        {
            get { return signature; }
        }

        public ushort MajorVersion
        {
            get { return majorVersion; }
        }

        public ushort MinorVersion
        {
            get { return minorVersion; }
        }

        public string Version
        {
            get { return version; }
        }

        public MetadataRoot(byte[] data, uint _pos)
        {
            uint pos = _pos;
            signature = MetadataTables.ReadUInt(data, pos);
            pos += sizeof(uint);
            majorVersion = MetadataTables.ReadUShort(data, pos);
            pos += sizeof(ushort);
            minorVersion = MetadataTables.ReadUShort(data, pos);
            pos += sizeof(ushort);
            pos += 4;//Reserved;
            uint len = MetadataTables.ReadUInt(data, pos);
            pos += sizeof(uint);
            version = System.Text.Encoding.UTF8.GetString(data, (int)pos, (int)len);
            len = Alignment.Calc(len, 4);
            pos += len;
            pos += 2;//Flags
            structSize = pos - _pos;
        }

        private uint structSize = 0;
        private uint signature;//0x424A5342 = "BSJB"
        private ushort majorVersion;
        private ushort minorVersion;
        private string version;
    }
    public class StreamHeader
    {
        public uint Position
        {
            get { return position; }
        }

        public uint StructSize
        {
            get { return structSize; }
        }

        public uint Offset
        {
            get
            {
                return offset;
            }
            set
            {
                offset = value;
                MetadataTables.WriteUInt(data, position + OffsetOffset, value);
            }
        }

        public uint Size
        {
            get
            {
                return size;
            }
            set
            {
                size = value;
                MetadataTables.WriteUInt(data, position + SizeOffset, value);
            }
        }

        public string Name
        {
            get { return name; }
        }

        public StreamHeader(byte[] _data, uint _pos)
        {
            data = _data;
            position = _pos;

            uint pos = _pos;
            offset = MetadataTables.ReadUInt(data, pos);
            pos += sizeof(uint);
            size = MetadataTables.ReadUInt(data, pos);
            pos += sizeof(uint);
            int nlen = 0;
            for (; data[pos + nlen] > 0; nlen++) ;
            name = System.Text.Encoding.ASCII.GetString(data, (int)pos, nlen);
            nlen = Alignment.Calc(nlen + 1, 4);
            pos += (uint)nlen;
            structSize = pos - position;
        }

        private uint OffsetOffset
        {
            get
            {
                return 0;
            }
        }

        private uint SizeOffset
        {
            get
            {
                return 4;
            }
        }

        private byte[] data = null;
        private uint position = 0;
        private uint structSize = 0;
        private uint offset;
        private uint size;
        private string name;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct MetadataStream
    {
        public uint Reserved1;
        public byte MajorVersion;
        public byte MinorVersion;
        public byte HeapSizes;
        public byte Reserved2;
        public UInt64 Valid;
        public UInt64 Sorted;

        public static unsafe uint StructSize
        {
            get
            {
                return (uint)sizeof(MetadataStream);
            }
        }
    }
    public enum HeapSizeType : byte
    {
        None = 0,
        String = 0x01,
        GUID = 0x02,
        Blob = 0x04,
        String_GUID = 0x03,
        String_Blob = 0x05,
        GUID_Blob = 0x06,
        ALL = 0x07
    }
    public enum CorTokenType
    {
        mdtModule = 0x00,
        mdtTypeRef = 0x01,
        mdtTypeDef = 0x02,
        mdtUnused03 = 0x03,
        mdtFieldDef = 0x04,
        mdtUnused05 = 0x05,
        mdtMethodDef = 0x06,
        mdtUnused07 = 0x07,
        mdtParamDef = 0x08,
        mdtInterfaceImpl = 0x09,
        mdtMemberRef = 0x0a,
        mdtConstant = 0x0b,
        mdtCustomAttribute = 0x0c,
        mdtFieldMarshal = 0x0d,
        mdtDeclSecurity = 0x0e,
        mdtClassLayout = 0x0f,
        mdtFieldLayout = 0x10,
        mdtSignature = 0x11,
        mdtEventMap = 0x12,
        mdtUnused13 = 0x13,
        mdtEvent = 0x14,
        mdtPropertyMap = 0x15,
        mdtUnused16 = 0x16,
        mdtProperty = 0x17,
        mdtMethodSemantics = 0x18,
        mdtMethodImpl = 0x19,
        mdtModuleRef = 0x1a,
        mdtTypeSpec = 0x1b,
        mdtImplMap = 0x1c,
        mdtFieldRVA = 0x1d,
        mdtUnused1E = 0x1e,
        mdtUnused1F = 0x1f,
        mdtAssembly = 0x20,
        mdtAssemblyProcessor = 0x21,
        mdtAssemblyOS = 0x22,
        mdtAssemblyRef = 0x23,
        mdtAssemblyRefProcessor = 0x24,
        mdtAssemblyRefOS = 0x25,
        mdtFile = 0x26,
        mdtExportedType = 0x27,
        mdtManifestResource = 0x28,
        mdtNestedClass = 0x29,
        mdtGenericParam = 0x2a,
        mdtMethodSpec = 0x2b,
        mdtGenericParamConstraint = 0x2c,
        mdtUnused2D = 0x2d,
        mdtUnused2E = 0x2e,
        mdtUnused2F = 0x2f,
        mdtUnused30 = 0x30,
        mdtUnused31 = 0x31,
        mdtUnused32 = 0x32,
        mdtUnused33 = 0x33,
        mdtUnused34 = 0x34,
        mdtUnused35 = 0x35,
        mdtUnused36 = 0x36,
        mdtUnused37 = 0x37,
        mdtUnused38 = 0x38,
        mdtUnused39 = 0x39,
        mdtUnused3A = 0x3a,
        mdtUnused3B = 0x3b,
        mdtUnused3C = 0x3c,
        mdtUnused3D = 0x3d,
        mdtUnused3E = 0x3e,
        mdtUnused3F = 0x3f,
        mdtMaxTokenType = 0x3f
    }
    public enum CodedIndexType
    {
        TypeDefOrRef = 0,
        HasConstant = 1,
        HasCustomAttribute = 2,
        HasFieldMarshall = 3,
        HasDeclSecurity = 4,
        MemberRefParent = 5,
        HasSemantics = 6,
        MethodDefOrRef = 7,
        MemberForwarded = 8,
        Implementation = 9,
        CustomAttributeType = 10,
        ResolutionScope = 11,
        TypeOrMethodDef = 12
    }
    internal static class CodedIndexSize
    {
        static CodedIndexSize()
        {
            codedIndexTypes[CodedIndexType.TypeDefOrRef] = new CorTokenType[]
                {
                    CorTokenType.mdtTypeDef,
                    CorTokenType.mdtTypeRef,
                    CorTokenType.mdtTypeSpec
                };
            codedIndexTypes[CodedIndexType.HasConstant] = new CorTokenType[]
                {
                    CorTokenType.mdtFieldDef,
                    CorTokenType.mdtParamDef,
                    CorTokenType.mdtProperty
                };
            codedIndexTypes[CodedIndexType.HasCustomAttribute] = new CorTokenType[]
                {
                    CorTokenType.mdtMethodDef,
                    CorTokenType.mdtFieldDef,
                    CorTokenType.mdtTypeRef,
                    CorTokenType.mdtTypeDef,
                    CorTokenType.mdtParamDef,
                    CorTokenType.mdtInterfaceImpl,
                    CorTokenType.mdtMemberRef,
                    CorTokenType.mdtModule,
                    CorTokenType.mdtDeclSecurity,
                    CorTokenType.mdtProperty,
                    CorTokenType.mdtEvent,
                    CorTokenType.mdtSignature,
                    CorTokenType.mdtModuleRef,
                    CorTokenType.mdtTypeSpec,
                    CorTokenType.mdtAssembly,
                    CorTokenType.mdtAssemblyRef,
                    CorTokenType.mdtFile,
                    CorTokenType.mdtExportedType,
                    CorTokenType.mdtManifestResource,
                    CorTokenType.mdtGenericParam,
                    CorTokenType.mdtGenericParamConstraint,
                    CorTokenType.mdtMethodSpec
                };
            codedIndexTypes[CodedIndexType.HasFieldMarshall] = new CorTokenType[]
                {
                    CorTokenType.mdtFieldDef,
                    CorTokenType.mdtParamDef
                };
            codedIndexTypes[CodedIndexType.HasDeclSecurity] = new CorTokenType[]
                {
                    CorTokenType.mdtTypeDef,
                    CorTokenType.mdtMethodDef,
                    CorTokenType.mdtAssembly
                };
            codedIndexTypes[CodedIndexType.MemberRefParent] = new CorTokenType[]
                {
                    CorTokenType.mdtTypeDef,
                    CorTokenType.mdtTypeRef,
                    CorTokenType.mdtModuleRef,
                    CorTokenType.mdtMethodDef,
                    CorTokenType.mdtTypeSpec
                };
            codedIndexTypes[CodedIndexType.HasSemantics] = new CorTokenType[]
                {
                    CorTokenType.mdtEvent,
                    CorTokenType.mdtProperty
                };
            codedIndexTypes[CodedIndexType.MethodDefOrRef] = new CorTokenType[]
                {
                    CorTokenType.mdtMethodDef,
                    CorTokenType.mdtMemberRef
                };
            codedIndexTypes[CodedIndexType.MemberForwarded] = new CorTokenType[]
                {
                    CorTokenType.mdtFieldDef,
                    CorTokenType.mdtMethodDef
                };
            codedIndexTypes[CodedIndexType.Implementation] = new CorTokenType[]
                {
                    CorTokenType.mdtFile,
                    CorTokenType.mdtAssemblyRef,
                    CorTokenType.mdtExportedType
                };
            codedIndexTypes[CodedIndexType.CustomAttributeType] = new CorTokenType[]
                {
                    CorTokenType.mdtMaxTokenType,
                    CorTokenType.mdtMaxTokenType,
                    CorTokenType.mdtMethodDef,
                    CorTokenType.mdtMemberRef,
                    CorTokenType.mdtMaxTokenType
                };
            codedIndexTypes[CodedIndexType.ResolutionScope] = new CorTokenType[]
                {
                    CorTokenType.mdtModule,
                    CorTokenType.mdtModuleRef,
                    CorTokenType.mdtAssemblyRef,
                    CorTokenType.mdtTypeRef
                };
            codedIndexTypes[CodedIndexType.TypeOrMethodDef] = new CorTokenType[]
                {
                    CorTokenType.mdtTypeDef,
                    CorTokenType.mdtMethodDef
                };

            codedIndexBits[CodedIndexType.TypeDefOrRef] = 2;
            codedIndexBits[CodedIndexType.HasConstant] = 2;
            codedIndexBits[CodedIndexType.HasCustomAttribute] = 5;
            codedIndexBits[CodedIndexType.HasFieldMarshall] = 1;
            codedIndexBits[CodedIndexType.HasDeclSecurity] = 2;
            codedIndexBits[CodedIndexType.MemberRefParent] = 3;
            codedIndexBits[CodedIndexType.HasSemantics] = 1;
            codedIndexBits[CodedIndexType.MethodDefOrRef] = 1;
            codedIndexBits[CodedIndexType.MemberForwarded] = 1;
            codedIndexBits[CodedIndexType.Implementation] = 2;
            codedIndexBits[CodedIndexType.CustomAttributeType] = 3;
            codedIndexBits[CodedIndexType.ResolutionScope] = 2;
            codedIndexBits[CodedIndexType.TypeOrMethodDef] = 1;
        }
        public static uint GetSize(CodedIndexType type, uint[] numberOfTableRow)
        {
            if (codedIndexTypes.ContainsKey(type) && codedIndexBits.ContainsKey(type)) {
                CorTokenType[] tokens = codedIndexTypes[type];
                uint v = 0x01;
                int bits = codedIndexBits[type];
                v <<= 16 - bits;
                uint size = 0;
                foreach (CorTokenType token in tokens) {
                    int ix = (int)token;
                    uint tsize = numberOfTableRow[ix];
                    if (tsize > size)
                        size = tsize;
                }
                if (size >= v)
                    return 4;
                else
                    return 2;
            }
            return 0;
        }
        private static Dictionary<CodedIndexType, CorTokenType[]> codedIndexTypes = new Dictionary<CodedIndexType, CorTokenType[]>();
        private static Dictionary<CodedIndexType, int> codedIndexBits = new Dictionary<CodedIndexType, int>();
    }
    public enum MethodType : byte
    {
        TinyFormat = 0x2,
        FatFormat = 0x3,
        MoreSects = 0x8,
        InitLocals = 0x10
    }
    public enum MethodSectType : byte
    {
        EHTable = 0x1,
        ILTable = 0x2,
        FatFormat = 0x40,
        MoreSects = 0x80
    }
    public enum EHClauseType : ushort
    {
        Exception = 0x0000,
        Filter = 0x0001,
        Finally = 0x0002,
        Fault = 0x0004
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TinyMethodHeader
    {
        public byte Flags
        {
            get
            {
                return (byte)(data & 0x03);
            }
            set
            {
                data |= 0x03;
                data &= (byte)(value & 0x03);
            }
        }
        public byte Size
        {
            get
            {
                return (byte)((data & 0xFC) >> 2);
            }
            set
            {
                byte v = (byte)(value << 2);
                data |= 0xFC;
                data &= (byte)(v & 0xFC);
            }
        }
        public static unsafe uint StructSize
        {
            get
            {
                return (uint)sizeof(TinyMethodHeader);
            }
        }
        private byte data;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct FatMethodHeader
    {
        public ushort Flags
        {
            get
            {
                return (ushort)(FlagsAndSize & 0x0FFF);
            }
            set
            {
                FlagsAndSize |= 0x0FFF;
                FlagsAndSize &= (ushort)(value & 0x0FFF);
            }
        }
        public byte Size
        {
            get
            {
                return (byte)((FlagsAndSize & 0xF000) >> 12);
            }
            set
            {
                ushort v = (ushort)(value << 12);
                FlagsAndSize |= 0xF000;
                FlagsAndSize &= (ushort)(v & 0xF000);
            }
        }
        private ushort FlagsAndSize;
        public ushort MaxStack;
        public uint CodeSize;
        public uint LocalVarSigTok;

        public static unsafe uint StructSize
        {
            get
            {
                return (uint)sizeof(FatMethodHeader);
            }
        }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct ExceptionHeader
    {
        public bool IsFat
        {
            get
            {
                return (Kind & (byte)MethodSectType.FatFormat) == (byte)MethodSectType.FatFormat;
            }
        }
        public byte Kind;
        public byte SmallDataSize
        {
            get
            {
                return data1;
            }
            set
            {
                data1 = value;
                data2 = 0;
                data3 = 0;
            }
        }
        public uint FatDataSize
        {
            get
            {
                return (uint)((data3 << 16) + (data2 << 8) + data1);
            }
            set
            {
                data3 = (byte)((value & 0x00FF0000) >> 16);
                data2 = (byte)((value & 0x0000FF00) >> 8);
                data1 = (byte)(value & 0x0000FF);
            }
        }
        private byte data1;
        private byte data2;
        private byte data3;

        public static unsafe uint StructSize
        {
            get
            {
                return (uint)sizeof(ExceptionHeader);
            }
        }
    }
    public class EHClause
    {
        public uint Position
        {
            get { return position; }
        }
        public uint Size
        {
            get
            {
                return (uint)(isFat ? 24 : 12);
            }
        }
        public uint Flags
        {
            get
            {
                if (isFat)
                    return MetadataTables.ReadUInt(data, position + 0);
                else
                    return (uint)MetadataTables.ReadUShort(data, position + 0);
            }
            set
            {
                if (isFat)
                    MetadataTables.WriteUInt(data, position + 0, value);
                else
                    MetadataTables.WriteUShort(data, position + 0, (ushort)value);
            }
        }
        public uint TryOffset
        {
            get
            {
                if (isFat)
                    return MetadataTables.ReadUInt(data, position + 4);
                else
                    return (uint)MetadataTables.ReadUShort(data, position + 2);
            }
            set
            {
                if (isFat)
                    MetadataTables.WriteUInt(data, position + 4, value);
                else
                    MetadataTables.WriteUShort(data, position + 2, (ushort)value);
            }
        }
        public uint TryLength
        {
            get
            {
                if (isFat)
                    return MetadataTables.ReadUInt(data, position + 8);
                else
                    return (uint)data[position + 4];
            }
            set
            {
                if (isFat)
                    MetadataTables.WriteUInt(data, position + 8, value);
                else
                    data[position + 4] = (byte)value;
            }
        }
        public uint HandlerOffset
        {
            get
            {
                if (isFat)
                    return MetadataTables.ReadUInt(data, position + 12);
                else
                    return (uint)MetadataTables.ReadUShort(data, position + 5);
            }
            set
            {
                if (isFat)
                    MetadataTables.WriteUInt(data, position + 12, value);
                else
                    MetadataTables.WriteUShort(data, position + 5, (ushort)value);
            }
        }
        public uint HandlerLength
        {
            get
            {
                if (isFat)
                    return MetadataTables.ReadUInt(data, position + 16);
                else
                    return (uint)data[position + 7];
            }
            set
            {
                if (isFat)
                    MetadataTables.WriteUInt(data, position + 16, value);
                else
                    data[position + 7] = (byte)value;
            }
        }
        public uint ClassToken
        {
            get
            {
                if (isFat)
                    return MetadataTables.ReadUInt(data, position + 20);
                else
                    return MetadataTables.ReadUInt(data, position + 8);
            }
            set
            {
                if (isFat)
                    MetadataTables.WriteUInt(data, position + 20, value);
                else
                    MetadataTables.WriteUInt(data, position + 8, value);
            }
        }
        public uint FilterOffset
        {
            get
            {
                if (isFat)
                    return MetadataTables.ReadUInt(data, position + 20);
                else
                    return MetadataTables.ReadUInt(data, position + 8);
            }
            set
            {
                if (isFat)
                    MetadataTables.WriteUInt(data, position + 20, value);
                else
                    MetadataTables.WriteUInt(data, position + 8, value);
            }
        }

        public EHClause(byte[] buffer, uint pos, bool fat)
        {
            data = buffer;
            position = pos;

            isFat = fat;
        }

        private byte[] data = null;
        private uint position = 0;
        private bool isFat = false;
    }
    public class ClrMethod
    {
        public bool IsFat
        {
            get
            {
                return isFat;
            }
        }
        public uint Position
        {
            get { return position; }
        }

        public TinyMethodHeader TinyMethodHeader
        {
            get { return tinyMethodHeader; }
        }
        public FatMethodHeader FatmethodHeader
        {
            get { return fatMethodHeader; }
        }
        public bool HaveExceptionData
        {
            get
            {
                if (!IsFat)
                    return false;
                return (fatMethodHeader.Flags & (ushort)MethodType.MoreSects) == (ushort)MethodType.MoreSects;
            }
        }
        public ExceptionHeader ExceptionHeader
        {
            get { return exceptionHeader; }
        }
        public EHClause[] Clauses
        {
            get { return clauses; }
        }

        public ClrMethod(byte[] buffer, uint pos)
        {
            data = buffer;
            position = pos;

            byte b = buffer[pos];
            if (!IsFatFlags(b)) {
                isFat = false;

                tinyMethodHeader = ReadTinyMethodHeader(buffer, pos);
            } else {
                isFat = true;

                fatMethodHeader = ReadFatMethodHeader(buffer, pos);
                pos += FatMethodHeader.StructSize;
                pos += fatMethodHeader.CodeSize;
                pos = Alignment.Calc(pos, 4);
                if (HaveExceptionData) {
                    exceptionHeader = ReadExceptionHeader(buffer, pos);
                    pos += ExceptionHeader.StructSize;

                    if (exceptionHeader.IsFat) {
                        uint size = exceptionHeader.FatDataSize;
                        uint n = (size - 4) / 24;

                        clauses = new EHClause[n];
                        for (int i = 0; i < n; i++) {
                            Clauses[i] = new EHClause(buffer, pos, true);
                            pos += Clauses[i].Size;
                        }
                    } else {
                        uint size = exceptionHeader.SmallDataSize;
                        uint n = (size - 4) / 12;

                        clauses = new EHClause[n];
                        for (int i = 0; i < n; i++) {
                            Clauses[i] = new EHClause(buffer, pos, false);
                            pos += Clauses[i].Size;
                        }
                    }
                }
            }
        }
        private static bool IsFatFlags(byte firstByte)
        {
            if ((firstByte & (byte)MethodType.FatFormat) == (byte)MethodType.FatFormat)
                return true;
            else
                return false;
        }

        private static unsafe TinyMethodHeader ReadTinyMethodHeader(byte[] buffer, uint pos)
        {
            TinyMethodHeader methodHeader = new TinyMethodHeader();
            fixed (byte* p = buffer) {
                TinyMethodHeader* ptr = (TinyMethodHeader*)(p + pos);
                methodHeader = *ptr;
            }
            return methodHeader;
        }
        private static unsafe FatMethodHeader ReadFatMethodHeader(byte[] buffer, uint pos)
        {
            FatMethodHeader methodHeader = new FatMethodHeader();
            fixed (byte* p = buffer) {
                FatMethodHeader* ptr = (FatMethodHeader*)(p + pos);
                methodHeader = *ptr;
            }
            return methodHeader;
        }
        private static unsafe ExceptionHeader ReadExceptionHeader(byte[] buffer, uint pos)
        {
            ExceptionHeader exceptionHeader = new ExceptionHeader();
            fixed (byte* p = buffer) {
                ExceptionHeader* ptr = (ExceptionHeader*)(p + pos);
                exceptionHeader = *ptr;
            }
            return exceptionHeader;
        }

        private static unsafe void WriteTinyMethodHeader(byte[] buffer, uint pos, TinyMethodHeader header)
        {
            fixed (byte* p = buffer) {
                TinyMethodHeader* ptr = (TinyMethodHeader*)(p + pos);
                *ptr = header;
            }
        }
        private static unsafe void WriteFatMethodHeader(byte[] buffer, uint pos, FatMethodHeader header)
        {
            fixed (byte* p = buffer) {
                FatMethodHeader* ptr = (FatMethodHeader*)(p + pos);
                *ptr = header;
            }
        }
        private static unsafe void WriteExceptionHeader(byte[] buffer, uint pos, ExceptionHeader header)
        {
            fixed (byte* p = buffer) {
                ExceptionHeader* ptr = (ExceptionHeader*)(p + pos);
                *ptr = header;
            }
        }

        private bool isFat = false;
        private TinyMethodHeader tinyMethodHeader;
        private FatMethodHeader fatMethodHeader;
        private ExceptionHeader exceptionHeader;
        private EHClause[] clauses = null;

        private byte[] data = null;
        private uint position = 0;
    }
}
