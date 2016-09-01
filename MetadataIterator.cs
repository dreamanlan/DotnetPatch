using System;
using System.Collections.Generic;
using System.Text;

namespace DotnetPatch
{
    public interface IMetadataIterator
    {
        uint RecordSize
        {
            get;
        }
        CorTokenType TokenType
        {
            get;
        }
        bool IsSorted
        {
            get;
        }
        uint RecordNumber
        {
            get;
        }
        uint Position
        {
            get;
        }
        bool IsFirst
        {
            get;
        }
        bool IsEnd
        {
            get;
        }
        void MoveToFirst();
        bool MoveToNext();
        void MoveToEnd();
        bool MoveTo(uint ix);
    }
    public interface INamedMetadataIterator : IMetadataIterator
    {
        uint Name
        {
            get;
            set;
        }
    }
    public interface INamesigedMetadataIterator : INamedMetadataIterator
    {
        uint Signature
        {
            get;
        }
    }
    public interface INamespacedMetadataIterator : INamedMetadataIterator
    {
        uint NameSpace
        {
            get;
            set;
        }
    }
    public abstract class MetadataIterator : IMetadataIterator
    {
        #region IMetadataIterator ≥…‘±

        public abstract uint RecordSize
        {
            get;
        }

        public CorTokenType TokenType
        {
            get
            {
                return tokenType;
            }
        }

        public bool IsSorted
        {
            get
            {
                int ix = (int)TokenType;
                return Tables.SortedOfTable[ix];
            }
        }

        public uint RecordNumber
        {
            get
            {
                int ix = (int)TokenType;
                return Tables.NumberOfTableRow[ix];
            }
        }
        public uint Index
        {
            get { return index; }
        }
        public uint Position
        {
            get { return pos + index * RecordSize; }
        }

        public bool IsFirst
        {
            get
            {
                return index == 0;
            }
        }
        public bool IsEnd
        {
            get
            {
                return index == RecordNumber;
            }
        }

        public void MoveToFirst()
        {
            index = 0;
        }

        public bool MoveToNext()
        {
            if (Index < RecordNumber) {
                index++;
            }
            if (Index < RecordNumber)
                return true;
            else
                return false;
        }

        public void MoveToEnd()
        {
            index = RecordNumber;
        }

        public bool MoveTo(uint ix)
        {
            if (ix >= RecordNumber)
                return false;
            else {
                index = ix;
                return true;
            }
        }

        #endregion

        public static IMetadataIterator Create(MetadataTables _tables, byte[] _data, uint _pos, CorTokenType _tokenType)
        {
            switch (_tokenType) {
                case CorTokenType.mdtAssembly:
                    return new AssemblyIterator(_tables, _data, _pos);
                case CorTokenType.mdtAssemblyOS:
                    return new AssemblyOSIterator(_tables, _data, _pos);
                case CorTokenType.mdtAssemblyProcessor:
                    return new AssemblyProcessorIterator(_tables, _data, _pos);
                case CorTokenType.mdtAssemblyRef:
                    return new AssemblyRefIterator(_tables, _data, _pos);
                case CorTokenType.mdtAssemblyRefOS:
                    return new AssemblyRefOSIterator(_tables, _data, _pos);
                case CorTokenType.mdtAssemblyRefProcessor:
                    return new AssemblyRefProcessorIterator(_tables, _data, _pos);
                case CorTokenType.mdtClassLayout:
                    return new ClassLayoutIterator(_tables, _data, _pos);
                case CorTokenType.mdtConstant:
                    return new ConstantIterator(_tables, _data, _pos);
                case CorTokenType.mdtCustomAttribute:
                    return new CustomAttributeIterator(_tables, _data, _pos);
                case CorTokenType.mdtDeclSecurity:
                    return new DeclSecurityIterator(_tables, _data, _pos);
                case CorTokenType.mdtEvent:
                    return new EventIterator(_tables, _data, _pos);
                case CorTokenType.mdtEventMap:
                    return new EventMapIterator(_tables, _data, _pos);
                case CorTokenType.mdtExportedType:
                    return new ExportedTypeIterator(_tables, _data, _pos);
                case CorTokenType.mdtFieldDef:
                    return new FieldIterator(_tables, _data, _pos);
                case CorTokenType.mdtFieldLayout:
                    return new FieldLayoutIterator(_tables, _data, _pos);
                case CorTokenType.mdtFieldMarshal:
                    return new FieldMarshalIterator(_tables, _data, _pos);
                case CorTokenType.mdtFieldRVA:
                    return new FieldRVAIterator(_tables, _data, _pos);
                case CorTokenType.mdtFile:
                    return new FileIterator(_tables, _data, _pos);
                case CorTokenType.mdtGenericParam:
                    return new GenericParamIterator(_tables, _data, _pos);
                case CorTokenType.mdtGenericParamConstraint:
                    return new GenericParamConstraintIterator(_tables, _data, _pos);
                case CorTokenType.mdtImplMap:
                    return new ImplMapIterator(_tables, _data, _pos);
                case CorTokenType.mdtInterfaceImpl:
                    return new InterfaceImplIterator(_tables, _data, _pos);
                case CorTokenType.mdtManifestResource:
                    return new ManifestResourceIterator(_tables, _data, _pos);
                case CorTokenType.mdtMemberRef:
                    return new MemberRefIterator(_tables, _data, _pos);
                case CorTokenType.mdtMethodDef:
                    return new MethodDefIterator(_tables, _data, _pos);
                case CorTokenType.mdtMethodImpl:
                    return new MethodImplIterator(_tables, _data, _pos);
                case CorTokenType.mdtMethodSemantics:
                    return new MethodSemanticsIterator(_tables, _data, _pos);
                case CorTokenType.mdtMethodSpec:
                    return new MethodSpecIterator(_tables, _data, _pos);
                case CorTokenType.mdtModule:
                    return new ModuleIterator(_tables, _data, _pos);
                case CorTokenType.mdtModuleRef:
                    return new ModuleRefIterator(_tables, _data, _pos);
                case CorTokenType.mdtNestedClass:
                    return new NestedClassIterator(_tables, _data, _pos);
                case CorTokenType.mdtParamDef:
                    return new ParamIterator(_tables, _data, _pos);
                case CorTokenType.mdtProperty:
                    return new PropertyIterator(_tables, _data, _pos);
                case CorTokenType.mdtPropertyMap:
                    return new PropertyMapIterator(_tables, _data, _pos);
                case CorTokenType.mdtSignature:
                    return new StandAloneSigIterator(_tables, _data, _pos);
                case CorTokenType.mdtTypeDef:
                    return new TypeDefIterator(_tables, _data, _pos);
                case CorTokenType.mdtTypeRef:
                    return new TypeRefIterator(_tables, _data, _pos);
                case CorTokenType.mdtTypeSpec:
                    return new TypeSpecIterator(_tables, _data, _pos);
                default:
                    return new NullIterator(_tables, _data, _pos);
            }
        }

        protected MetadataIterator(MetadataTables _tables, byte[] _data, uint _pos, CorTokenType _tokenType)
        {
            tables = _tables;
            data = _data;
            pos = _pos;
            tokenType = _tokenType;
        }

        protected MetadataTables Tables
        {
            get
            {
                return tables;
            }
        }
        protected byte[] Data
        {
            get
            {
                return data;
            }
        }

        private MetadataTables tables = null;
        private byte[] data = null;

        private uint pos = 0;
        private uint index = 0;
        private CorTokenType tokenType = 0;
    }
    public class NullIterator : MetadataIterator
    {
        public override uint RecordSize
        {
            get { return 0; }
        }

        public NullIterator(MetadataTables _tables, byte[] _data, uint _pos)
            : base(_tables, _data, _pos, CorTokenType.mdtMaxTokenType)
        { }
    }
    public class AssemblyIterator : MetadataIterator, INamedMetadataIterator
    {
        public override uint RecordSize
        {
            get
            {
                uint size = NameOffset + Tables.StringIndexSize * 2;
                return size;
            }
        }
        public uint Name
        {
            get
            {
                if (Tables.StringIndexSize == 2)
                    return (uint)MetadataTables.ReadUShort(Data, Position + NameOffset);
                else
                    return MetadataTables.ReadUInt(Data, Position + NameOffset);
            }
            set
            {
                if (Tables.StringIndexSize == 2)
                    MetadataTables.WriteUShort(Data, Position + NameOffset, (ushort)value);
                else
                    MetadataTables.WriteUInt(Data, Position + NameOffset, value);
            }
        }
        public AssemblyIterator(MetadataTables _tables, byte[] _data, uint _pos)
            : base(_tables, _data, _pos, CorTokenType.mdtAssembly)
        { }

        private uint NameOffset
        {
            get
            {
                uint offset = 16 + Tables.BlobIndexSize;
                return offset;
            }
        }
    }
    public class AssemblyOSIterator : MetadataIterator
    {
        public override uint RecordSize
        {
            get { return 12; }
        }

        public AssemblyOSIterator(MetadataTables _tables, byte[] _data, uint _pos)
            : base(_tables, _data, _pos, CorTokenType.mdtAssemblyOS)
        { }
    }
    public class AssemblyProcessorIterator : MetadataIterator
    {
        public override uint RecordSize
        {
            get { return 4; }
        }

        public AssemblyProcessorIterator(MetadataTables _tables, byte[] _data, uint _pos)
            : base(_tables, _data, _pos, CorTokenType.mdtAssemblyProcessor)
        { }
    }
    public class AssemblyRefIterator : MetadataIterator, INamedMetadataIterator
    {
        public override uint RecordSize
        {
            get
            {
                uint size = NameOffset + Tables.BlobIndexSize + Tables.StringIndexSize * 2;
                return size;
            }
        }
        public uint Name
        {
            get
            {
                if (Tables.StringIndexSize == 2)
                    return (uint)MetadataTables.ReadUShort(Data, Position + NameOffset);
                else
                    return MetadataTables.ReadUInt(Data, Position + NameOffset);
            }
            set
            {
                if (Tables.StringIndexSize == 2)
                    MetadataTables.WriteUShort(Data, Position + NameOffset, (ushort)value);
                else
                    MetadataTables.WriteUInt(Data, Position + NameOffset, value);
            }
        }
        public AssemblyRefIterator(MetadataTables _tables, byte[] _data, uint _pos)
            : base(_tables, _data, _pos, CorTokenType.mdtAssemblyRef)
        { }

        private uint NameOffset
        {
            get
            {
                uint offset = 12 + Tables.BlobIndexSize;
                return offset;
            }
        }
    }
    public class AssemblyRefOSIterator : MetadataIterator
    {
        public override uint RecordSize
        {
            get { return 12 + Tables.GetTableIndexSize(CorTokenType.mdtAssemblyRef); }
        }

        public AssemblyRefOSIterator(MetadataTables _tables, byte[] _data, uint _pos)
            : base(_tables, _data, _pos, CorTokenType.mdtAssemblyRefOS)
        { }
    }
    public class AssemblyRefProcessorIterator : MetadataIterator
    {
        public override uint RecordSize
        {
            get { return 4 + Tables.GetTableIndexSize(CorTokenType.mdtAssemblyRef); }
        }

        public AssemblyRefProcessorIterator(MetadataTables _tables, byte[] _data, uint _pos)
            : base(_tables, _data, _pos, CorTokenType.mdtAssemblyRefProcessor)
        { }
    }
    public class ClassLayoutIterator : MetadataIterator
    {
        public override uint RecordSize
        {
            get { return 6 + Tables.GetTableIndexSize(CorTokenType.mdtTypeDef); }
        }

        public ClassLayoutIterator(MetadataTables _tables, byte[] _data, uint _pos)
            : base(_tables, _data, _pos, CorTokenType.mdtClassLayout)
        { }
    }
    public class ConstantIterator : MetadataIterator
    {
        public override uint RecordSize
        {
            get { return 2 + Tables.GetCodedIndexSize(CodedIndexType.HasConstant) + Tables.BlobIndexSize; }
        }

        public ConstantIterator(MetadataTables _tables, byte[] _data, uint _pos)
            : base(_tables, _data, _pos, CorTokenType.mdtConstant)
        { }
    }
    public class CustomAttributeIterator : MetadataIterator
    {
        public override uint RecordSize
        {
            get { return Tables.GetCodedIndexSize(CodedIndexType.HasCustomAttribute) + Tables.GetCodedIndexSize(CodedIndexType.CustomAttributeType) + Tables.BlobIndexSize; }
        }

        public CustomAttributeIterator(MetadataTables _tables, byte[] _data, uint _pos)
            : base(_tables, _data, _pos, CorTokenType.mdtCustomAttribute)
        { }
    }
    public class DeclSecurityIterator : MetadataIterator
    {
        public override uint RecordSize
        {
            get { return 2 + Tables.GetCodedIndexSize(CodedIndexType.HasDeclSecurity) + Tables.BlobIndexSize; }
        }

        public DeclSecurityIterator(MetadataTables _tables, byte[] _data, uint _pos)
            : base(_tables, _data, _pos, CorTokenType.mdtDeclSecurity)
        { }
    }
    public class EventMapIterator : MetadataIterator
    {
        public override uint RecordSize
        {
            get { return Tables.GetTableIndexSize(CorTokenType.mdtTypeDef) + Tables.GetTableIndexSize(CorTokenType.mdtEvent); }
        }

        public EventMapIterator(MetadataTables _tables, byte[] _data, uint _pos)
            : base(_tables, _data, _pos, CorTokenType.mdtEventMap)
        { }
    }
    public class EventIterator : MetadataIterator, INamedMetadataIterator
    {
        public override uint RecordSize
        {
            get
            {
                uint size = 2 + Tables.StringIndexSize + Tables.GetCodedIndexSize(CodedIndexType.TypeDefOrRef);
                return size;
            }
        }
        public uint Name
        {
            get
            {
                if (Tables.StringIndexSize == 2)
                    return (uint)MetadataTables.ReadUShort(Data, Position + NameOffset);
                else
                    return MetadataTables.ReadUInt(Data, Position + NameOffset);
            }
            set
            {
                if (Tables.StringIndexSize == 2)
                    MetadataTables.WriteUShort(Data, Position + NameOffset, (ushort)value);
                else
                    MetadataTables.WriteUInt(Data, Position + NameOffset, value);
            }
        }
        public EventIterator(MetadataTables _tables, byte[] _data, uint _pos)
            : base(_tables, _data, _pos, CorTokenType.mdtEvent)
        { }

        private uint NameOffset
        {
            get
            {
                uint offset = 2;
                return offset;
            }
        }
    }
    public class ExportedTypeIterator : MetadataIterator, INamespacedMetadataIterator
    {
        public override uint RecordSize
        {
            get
            {
                uint size = NameSpaceOffset + Tables.StringIndexSize + Tables.GetCodedIndexSize(CodedIndexType.Implementation);
                return size;
            }
        }
        public uint TypeDefId
        {
            get
            {
                return MetadataTables.ReadUInt(Data, Position + 4);
            }
        }
        public uint Name
        {
            get
            {
                if (Tables.StringIndexSize == 2)
                    return (uint)MetadataTables.ReadUShort(Data, Position + NameOffset);
                else
                    return MetadataTables.ReadUInt(Data, Position + NameOffset);
            }
            set
            {
                if (Tables.StringIndexSize == 2)
                    MetadataTables.WriteUShort(Data, Position + NameOffset, (ushort)value);
                else
                    MetadataTables.WriteUInt(Data, Position + NameOffset, value);
            }
        }
        public uint NameSpace
        {
            get
            {
                if (Tables.StringIndexSize == 2)
                    return (uint)MetadataTables.ReadUShort(Data, Position + NameSpaceOffset);
                else
                    return MetadataTables.ReadUInt(Data, Position + NameSpaceOffset);
            }
            set
            {
                if (Tables.StringIndexSize == 2)
                    MetadataTables.WriteUShort(Data, Position + NameSpaceOffset, (ushort)value);
                else
                    MetadataTables.WriteUInt(Data, Position + NameSpaceOffset, value);
            }
        }
        public ExportedTypeIterator(MetadataTables _tables, byte[] _data, uint _pos)
            : base(_tables, _data, _pos, CorTokenType.mdtExportedType)
        { }

        private uint NameOffset
        {
            get
            {
                uint offset = 8;
                return offset;
            }
        }
        private uint NameSpaceOffset
        {
            get
            {
                uint offset = 8 + Tables.StringIndexSize;
                return offset;
            }
        }
    }
    public class FieldIterator : MetadataIterator, INamesigedMetadataIterator
    {
        public override uint RecordSize
        {
            get
            {
                uint size = SignatureOffset + Tables.BlobIndexSize;
                return size;
            }
        }
        public uint Name
        {
            get
            {
                if (Tables.StringIndexSize == 2)
                    return (uint)MetadataTables.ReadUShort(Data, Position + NameOffset);
                else
                    return MetadataTables.ReadUInt(Data, Position + NameOffset);
            }
            set
            {
                if (Tables.StringIndexSize == 2)
                    MetadataTables.WriteUShort(Data, Position + NameOffset, (ushort)value);
                else
                    MetadataTables.WriteUInt(Data, Position + NameOffset, value);
            }
        }
        public uint Signature
        {
            get
            {
                if (Tables.BlobIndexSize == 2)
                    return (uint)MetadataTables.ReadUShort(Data, Position + SignatureOffset);
                else
                    return MetadataTables.ReadUInt(Data, Position + SignatureOffset);
            }
        }
        public FieldIterator(MetadataTables _tables, byte[] _data, uint _pos)
            : base(_tables, _data, _pos, CorTokenType.mdtFieldDef)
        { }

        private uint NameOffset
        {
            get
            {
                uint offset = 2;
                return offset;
            }
        }
        private uint SignatureOffset
        {
            get
            {
                uint offset = 2 + Tables.StringIndexSize;
                return offset;
            }
        }
    }
    public class FieldLayoutIterator : MetadataIterator
    {
        public override uint RecordSize
        {
            get { return 4 + Tables.GetTableIndexSize(CorTokenType.mdtFieldDef); }
        }

        public FieldLayoutIterator(MetadataTables _tables, byte[] _data, uint _pos)
            : base(_tables, _data, _pos, CorTokenType.mdtFieldLayout)
        { }
    }
    public class FieldMarshalIterator : MetadataIterator
    {
        public override uint RecordSize
        {
            get { return Tables.GetCodedIndexSize(CodedIndexType.HasFieldMarshall) + Tables.BlobIndexSize; }
        }

        public FieldMarshalIterator(MetadataTables _tables, byte[] _data, uint _pos)
            : base(_tables, _data, _pos, CorTokenType.mdtFieldMarshal)
        { }
    }
    public class FieldRVAIterator : MetadataIterator
    {
        public override uint RecordSize
        {
            get { return 4 + Tables.GetTableIndexSize(CorTokenType.mdtFieldDef); }
        }

        public FieldRVAIterator(MetadataTables _tables, byte[] _data, uint _pos)
            : base(_tables, _data, _pos, CorTokenType.mdtFieldRVA)
        { }
    }
    public class FileIterator : MetadataIterator, INamedMetadataIterator
    {
        public override uint RecordSize
        {
            get
            {
                uint size = 4 + Tables.StringIndexSize + Tables.BlobIndexSize;
                return size;
            }
        }
        public uint Name
        {
            get
            {
                if (Tables.StringIndexSize == 2)
                    return (uint)MetadataTables.ReadUShort(Data, Position + NameOffset);
                else
                    return MetadataTables.ReadUInt(Data, Position + NameOffset);
            }
            set
            {
                if (Tables.StringIndexSize == 2)
                    MetadataTables.WriteUShort(Data, Position + NameOffset, (ushort)value);
                else
                    MetadataTables.WriteUInt(Data, Position + NameOffset, value);
            }
        }
        public FileIterator(MetadataTables _tables, byte[] _data, uint _pos)
            : base(_tables, _data, _pos, CorTokenType.mdtFile)
        { }

        private uint NameOffset
        {
            get
            {
                uint offset = 4;
                return offset;
            }
        }
    }
    public class GenericParamIterator : MetadataIterator, INamedMetadataIterator
    {
        public override uint RecordSize
        {
            get
            {
                uint size = NameOffset + Tables.StringIndexSize;
                return size;
            }
        }
        public uint Number
        {
            get
            {
                return (uint)MetadataTables.ReadUShort(Data, Position + 0);
            }
        }
        public uint Name
        {
            get
            {
                if (Tables.StringIndexSize == 2)
                    return (uint)MetadataTables.ReadUShort(Data, Position + NameOffset);
                else
                    return MetadataTables.ReadUInt(Data, Position + NameOffset);
            }
            set
            {
                if (Tables.StringIndexSize == 2)
                    MetadataTables.WriteUShort(Data, Position + NameOffset, (ushort)value);
                else
                    MetadataTables.WriteUInt(Data, Position + NameOffset, value);
            }
        }
        public GenericParamIterator(MetadataTables _tables, byte[] _data, uint _pos)
            : base(_tables, _data, _pos, CorTokenType.mdtGenericParam)
        { }

        private uint NameOffset
        {
            get
            {
                uint offset = 4 + Tables.GetCodedIndexSize(CodedIndexType.TypeOrMethodDef);
                return offset;
            }
        }
    }
    public class GenericParamConstraintIterator : MetadataIterator
    {
        public override uint RecordSize
        {
            get { return Tables.GetTableIndexSize(CorTokenType.mdtGenericParam) + Tables.GetCodedIndexSize(CodedIndexType.TypeDefOrRef); }
        }

        public GenericParamConstraintIterator(MetadataTables _tables, byte[] _data, uint _pos)
            : base(_tables, _data, _pos, CorTokenType.mdtGenericParamConstraint)
        { }
    }
    public class ImplMapIterator : MetadataIterator, INamedMetadataIterator
    {
        public override uint RecordSize
        {
            get
            {
                uint size = NameOffset + Tables.StringIndexSize + Tables.GetTableIndexSize(CorTokenType.mdtModuleRef);
                return size;
            }
        }
        public uint Name
        {
            get
            {
                if (Tables.StringIndexSize == 2)
                    return (uint)MetadataTables.ReadUShort(Data, Position + NameOffset);
                else
                    return MetadataTables.ReadUInt(Data, Position + NameOffset);
            }
            set
            {
                if (Tables.StringIndexSize == 2)
                    MetadataTables.WriteUShort(Data, Position + NameOffset, (ushort)value);
                else
                    MetadataTables.WriteUInt(Data, Position + NameOffset, value);
            }
        }
        public ImplMapIterator(MetadataTables _tables, byte[] _data, uint _pos)
            : base(_tables, _data, _pos, CorTokenType.mdtImplMap)
        { }

        private uint NameOffset
        {
            get
            {
                uint offset = 2 + Tables.GetCodedIndexSize(CodedIndexType.MemberForwarded);
                return offset;
            }
        }
    }
    public class InterfaceImplIterator : MetadataIterator
    {
        public override uint RecordSize
        {
            get { return Tables.GetTableIndexSize(CorTokenType.mdtTypeDef) + Tables.GetCodedIndexSize(CodedIndexType.TypeDefOrRef); }
        }

        public InterfaceImplIterator(MetadataTables _tables, byte[] _data, uint _pos)
            : base(_tables, _data, _pos, CorTokenType.mdtInterfaceImpl)
        { }
    }
    public class ManifestResourceIterator : MetadataIterator, INamedMetadataIterator
    {
        public override uint RecordSize
        {
            get
            {
                uint size = 8 + Tables.StringIndexSize + Tables.GetCodedIndexSize(CodedIndexType.Implementation);
                return size;
            }
        }
        public uint Name
        {
            get
            {
                if (Tables.StringIndexSize == 2)
                    return (uint)MetadataTables.ReadUShort(Data, Position + NameOffset);
                else
                    return MetadataTables.ReadUInt(Data, Position + NameOffset);
            }
            set
            {
                if (Tables.StringIndexSize == 2)
                    MetadataTables.WriteUShort(Data, Position + NameOffset, (ushort)value);
                else
                    MetadataTables.WriteUInt(Data, Position + NameOffset, value);
            }
        }
        public ManifestResourceIterator(MetadataTables _tables, byte[] _data, uint _pos)
            : base(_tables, _data, _pos, CorTokenType.mdtManifestResource)
        { }

        private uint NameOffset
        {
            get
            {
                uint offset = 8;
                return offset;
            }
        }
    }
    public class MemberRefIterator : MetadataIterator, INamesigedMetadataIterator
    {
        public override uint RecordSize
        {
            get
            {
                uint size = SignatureOffset + Tables.BlobIndexSize;
                return size;
            }
        }
        public uint Class
        {
            get
            {
                if (Tables.GetCodedIndexSize(CodedIndexType.MemberRefParent) == 2)
                    return (uint)MetadataTables.ReadUShort(Data, Position + 0);
                else
                    return MetadataTables.ReadUInt(Data, Position + 0);
            }
        }
        public uint Name
        {
            get
            {
                if (Tables.StringIndexSize == 2)
                    return (uint)MetadataTables.ReadUShort(Data, Position + NameOffset);
                else
                    return MetadataTables.ReadUInt(Data, Position + NameOffset);
            }
            set
            {
                if (Tables.StringIndexSize == 2)
                    MetadataTables.WriteUShort(Data, Position + NameOffset, (ushort)value);
                else
                    MetadataTables.WriteUInt(Data, Position + NameOffset, value);
            }
        }
        public uint Signature
        {
            get
            {
                if (Tables.BlobIndexSize == 2)
                    return (uint)MetadataTables.ReadUShort(Data, Position + SignatureOffset);
                else
                    return MetadataTables.ReadUInt(Data, Position + SignatureOffset);
            }
        }
        public MemberRefIterator(MetadataTables _tables, byte[] _data, uint _pos)
            : base(_tables, _data, _pos, CorTokenType.mdtMemberRef)
        { }

        private uint NameOffset
        {
            get
            {
                uint offset = Tables.GetCodedIndexSize(CodedIndexType.MemberRefParent);
                return offset;
            }
        }
        private uint SignatureOffset
        {
            get
            {
                uint offset = NameOffset + Tables.StringIndexSize;
                return offset;
            }
        }
    }
    public class MethodDefIterator : MetadataIterator, INamesigedMetadataIterator
    {
        public override uint RecordSize
        {
            get
            {
                uint size = SignatureOffset + Tables.BlobIndexSize + Tables.GetTableIndexSize(CorTokenType.mdtParamDef);
                return size;
            }
        }
        public uint RVA
        {
            get
            {
                return MetadataTables.ReadUInt(Data, Position + 0);
            }
            set
            {
                MetadataTables.WriteUInt(Data, Position + 0, value);
            }
        }
        public uint Name
        {
            get
            {
                if (Tables.StringIndexSize == 2)
                    return (uint)MetadataTables.ReadUShort(Data, Position + NameOffset);
                else
                    return MetadataTables.ReadUInt(Data, Position + NameOffset);
            }
            set
            {
                if (Tables.StringIndexSize == 2)
                    MetadataTables.WriteUShort(Data, Position + NameOffset, (ushort)value);
                else
                    MetadataTables.WriteUInt(Data, Position + NameOffset, value);
            }
        }
        public uint Signature
        {
            get
            {
                if (Tables.BlobIndexSize == 2)
                    return (uint)MetadataTables.ReadUShort(Data, Position + SignatureOffset);
                else
                    return MetadataTables.ReadUInt(Data, Position + SignatureOffset);
            }
        }
        public MethodDefIterator(MetadataTables _tables, byte[] _data, uint _pos)
            : base(_tables, _data, _pos, CorTokenType.mdtMethodDef)
        { }

        private uint NameOffset
        {
            get
            {
                uint offset = 8;
                return offset;
            }
        }
        private uint SignatureOffset
        {
            get
            {
                uint offset = 8 + Tables.StringIndexSize;
                return offset;
            }
        }
    }
    public class MethodImplIterator : MetadataIterator
    {
        public override uint RecordSize
        {
            get
            {
                return Tables.GetTableIndexSize(CorTokenType.mdtTypeDef) + Tables.GetCodedIndexSize(CodedIndexType.MethodDefOrRef) * 2;
            }
        }

        public MethodImplIterator(MetadataTables _tables, byte[] _data, uint _pos)
            : base(_tables, _data, _pos, CorTokenType.mdtMethodImpl)
        { }
    }
    public class MethodSemanticsIterator : MetadataIterator
    {
        public override uint RecordSize
        {
            get
            {
                return 2 + Tables.GetTableIndexSize(CorTokenType.mdtMethodDef) + Tables.GetCodedIndexSize(CodedIndexType.HasSemantics);
            }
        }

        public MethodSemanticsIterator(MetadataTables _tables, byte[] _data, uint _pos)
            : base(_tables, _data, _pos, CorTokenType.mdtMethodSemantics)
        { }
    }
    public class MethodSpecIterator : MetadataIterator
    {
        public override uint RecordSize
        {
            get
            {
                return Tables.GetCodedIndexSize(CodedIndexType.MethodDefOrRef) + Tables.BlobIndexSize;
            }
        }

        public MethodSpecIterator(MetadataTables _tables, byte[] _data, uint _pos)
            : base(_tables, _data, _pos, CorTokenType.mdtMethodSpec)
        { }
    }
    public class ModuleIterator : MetadataIterator, INamedMetadataIterator
    {
        public override uint RecordSize
        {
            get
            {
                uint size = 2 + Tables.StringIndexSize + Tables.GuidIndexSize * 3;
                return size;
            }
        }
        public uint Name
        {
            get
            {
                if (Tables.StringIndexSize == 2)
                    return (uint)MetadataTables.ReadUShort(Data, Position + NameOffset);
                else
                    return MetadataTables.ReadUInt(Data, Position + NameOffset);
            }
            set
            {
                if (Tables.StringIndexSize == 2)
                    MetadataTables.WriteUShort(Data, Position + NameOffset, (ushort)value);
                else
                    MetadataTables.WriteUInt(Data, Position + NameOffset, value);
            }
        }
        public ModuleIterator(MetadataTables _tables, byte[] _data, uint _pos)
            : base(_tables, _data, _pos, CorTokenType.mdtModule)
        { }

        private uint NameOffset
        {
            get
            {
                uint offset = 2;
                return offset;
            }
        }
    }
    public class ModuleRefIterator : MetadataIterator, INamedMetadataIterator
    {
        public override uint RecordSize
        {
            get
            {
                uint size = Tables.StringIndexSize;
                return size;
            }
        }
        public uint Name
        {
            get
            {
                if (Tables.StringIndexSize == 2)
                    return (uint)MetadataTables.ReadUShort(Data, Position + NameOffset);
                else
                    return MetadataTables.ReadUInt(Data, Position + NameOffset);
            }
            set
            {
                if (Tables.StringIndexSize == 2)
                    MetadataTables.WriteUShort(Data, Position + NameOffset, (ushort)value);
                else
                    MetadataTables.WriteUInt(Data, Position + NameOffset, value);
            }
        }
        public ModuleRefIterator(MetadataTables _tables, byte[] _data, uint _pos)
            : base(_tables, _data, _pos, CorTokenType.mdtModuleRef)
        { }

        private uint NameOffset
        {
            get
            {
                uint offset = 0;
                return offset;
            }
        }
    }
    public class NestedClassIterator : MetadataIterator
    {
        public override uint RecordSize
        {
            get
            {
                return Tables.GetTableIndexSize(CorTokenType.mdtTypeDef) * 2;
            }
        }

        public NestedClassIterator(MetadataTables _tables, byte[] _data, uint _pos)
            : base(_tables, _data, _pos, CorTokenType.mdtNestedClass)
        { }
    }
    public class ParamIterator : MetadataIterator, INamedMetadataIterator
    {
        public override uint RecordSize
        {
            get
            {
                uint size = 4 + Tables.StringIndexSize;
                return size;
            }
        }
        public uint Sequence
        {
            get
            {
                return (uint)MetadataTables.ReadUShort(Data, Position + 2);
            }
        }
        public uint Name
        {
            get
            {
                if (Tables.StringIndexSize == 2)
                    return (uint)MetadataTables.ReadUShort(Data, Position + NameOffset);
                else
                    return MetadataTables.ReadUInt(Data, Position + NameOffset);
            }
            set
            {
                if (Tables.StringIndexSize == 2)
                    MetadataTables.WriteUShort(Data, Position + NameOffset, (ushort)value);
                else
                    MetadataTables.WriteUInt(Data, Position + NameOffset, value);
            }
        }
        public ParamIterator(MetadataTables _tables, byte[] _data, uint _pos)
            : base(_tables, _data, _pos, CorTokenType.mdtParamDef)
        { }

        private uint NameOffset
        {
            get
            {
                uint offset = 4;
                return offset;
            }
        }
    }
    public class PropertyIterator : MetadataIterator, INamedMetadataIterator
    {
        public override uint RecordSize
        {
            get
            {
                uint size = 2 + Tables.StringIndexSize + Tables.BlobIndexSize;
                return size;
            }
        }
        public uint Name
        {
            get
            {
                if (Tables.StringIndexSize == 2)
                    return (uint)MetadataTables.ReadUShort(Data, Position + NameOffset);
                else
                    return MetadataTables.ReadUInt(Data, Position + NameOffset);
            }
            set
            {
                if (Tables.StringIndexSize == 2)
                    MetadataTables.WriteUShort(Data, Position + NameOffset, (ushort)value);
                else
                    MetadataTables.WriteUInt(Data, Position + NameOffset, value);
            }
        }
        public PropertyIterator(MetadataTables _tables, byte[] _data, uint _pos)
            : base(_tables, _data, _pos, CorTokenType.mdtProperty)
        { }

        private uint NameOffset
        {
            get
            {
                uint offset = 2;
                return offset;
            }
        }
    }
    public class PropertyMapIterator : MetadataIterator
    {
        public override uint RecordSize
        {
            get
            {
                return Tables.GetTableIndexSize(CorTokenType.mdtTypeDef) + Tables.GetTableIndexSize(CorTokenType.mdtProperty);
            }
        }

        public PropertyMapIterator(MetadataTables _tables, byte[] _data, uint _pos)
            : base(_tables, _data, _pos, CorTokenType.mdtPropertyMap)
        { }
    }
    public class StandAloneSigIterator : MetadataIterator
    {
        public override uint RecordSize
        {
            get
            {
                return Tables.BlobIndexSize;
            }
        }

        public StandAloneSigIterator(MetadataTables _tables, byte[] _data, uint _pos)
            : base(_tables, _data, _pos, CorTokenType.mdtSignature)
        { }
    }
    public class TypeDefIterator : MetadataIterator, INamespacedMetadataIterator
    {
        public override uint RecordSize
        {
            get
            {
                uint size = MethodListOffset + Tables.GetTableIndexSize(CorTokenType.mdtMethodDef);
                return size;
            }
        }
        public uint Name
        {
            get
            {
                if (Tables.StringIndexSize == 2)
                    return (uint)MetadataTables.ReadUShort(Data, Position + NameOffset);
                else
                    return MetadataTables.ReadUInt(Data, Position + NameOffset);
            }
            set
            {
                if (Tables.StringIndexSize == 2)
                    MetadataTables.WriteUShort(Data, Position + NameOffset, (ushort)value);
                else
                    MetadataTables.WriteUInt(Data, Position + NameOffset, value);
            }
        }
        public uint NameSpace
        {
            get
            {
                if (Tables.StringIndexSize == 2)
                    return (uint)MetadataTables.ReadUShort(Data, Position + NameSpaceOffset);
                else
                    return MetadataTables.ReadUInt(Data, Position + NameSpaceOffset);
            }
            set
            {
                if (Tables.StringIndexSize == 2)
                    MetadataTables.WriteUShort(Data, Position + NameSpaceOffset, (ushort)value);
                else
                    MetadataTables.WriteUInt(Data, Position + NameSpaceOffset, value);
            }
        }
        public uint FieldList
        {
            get
            {
                if (Tables.GetTableIndexSize(CorTokenType.mdtFieldDef) == 2)
                    return (uint)MetadataTables.ReadUShort(Data, Position + FieldListOffset);
                else
                    return MetadataTables.ReadUInt(Data, Position + FieldListOffset);
            }
        }
        public uint MethodList
        {
            get
            {
                if (Tables.GetTableIndexSize(CorTokenType.mdtMethodDef) == 2)
                    return (uint)MetadataTables.ReadUShort(Data, Position + MethodListOffset);
                else
                    return MetadataTables.ReadUInt(Data, Position + MethodListOffset);
            }
        }
        public TypeDefIterator(MetadataTables _tables, byte[] _data, uint _pos)
            : base(_tables, _data, _pos, CorTokenType.mdtTypeDef)
        { }

        private uint NameOffset
        {
            get
            {
                uint offset = 4;
                return offset;
            }
        }
        private uint NameSpaceOffset
        {
            get
            {
                uint offset = 4 + Tables.StringIndexSize;
                return offset;
            }
        }
        private uint FieldListOffset
        {
            get
            {
                uint offset = NameSpaceOffset + Tables.StringIndexSize + Tables.GetCodedIndexSize(CodedIndexType.TypeDefOrRef);
                return offset;
            }
        }
        private uint MethodListOffset
        {
            get
            {
                uint offset = FieldListOffset + Tables.GetTableIndexSize(CorTokenType.mdtFieldDef);
                return offset;
            }
        }
    }
    public class TypeRefIterator : MetadataIterator, INamespacedMetadataIterator
    {
        public override uint RecordSize
        {
            get
            {
                uint size = NameSpaceOffset + Tables.StringIndexSize;
                return size;
            }
        }
        public uint Name
        {
            get
            {
                if (Tables.StringIndexSize == 2)
                    return (uint)MetadataTables.ReadUShort(Data, Position + NameOffset);
                else
                    return MetadataTables.ReadUInt(Data, Position + NameOffset);
            }
            set
            {
                if (Tables.StringIndexSize == 2)
                    MetadataTables.WriteUShort(Data, Position + NameOffset, (ushort)value);
                else
                    MetadataTables.WriteUInt(Data, Position + NameOffset, value);
            }
        }
        public uint NameSpace
        {
            get
            {
                if (Tables.StringIndexSize == 2)
                    return (uint)MetadataTables.ReadUShort(Data, Position + NameSpaceOffset);
                else
                    return MetadataTables.ReadUInt(Data, Position + NameSpaceOffset);
            }
            set
            {
                if (Tables.StringIndexSize == 2)
                    MetadataTables.WriteUShort(Data, Position + NameSpaceOffset, (ushort)value);
                else
                    MetadataTables.WriteUInt(Data, Position + NameSpaceOffset, value);
            }
        }
        public TypeRefIterator(MetadataTables _tables, byte[] _data, uint _pos)
            : base(_tables, _data, _pos, CorTokenType.mdtTypeRef)
        { }

        private uint NameOffset
        {
            get
            {
                uint offset = Tables.GetCodedIndexSize(CodedIndexType.ResolutionScope);
                return offset;
            }
        }
        private uint NameSpaceOffset
        {
            get
            {
                uint offset = NameOffset + Tables.StringIndexSize;
                return offset;
            }
        }
    }
    public class TypeSpecIterator : MetadataIterator
    {
        public override uint RecordSize
        {
            get
            {
                return Tables.BlobIndexSize;
            }
        }

        public TypeSpecIterator(MetadataTables _tables, byte[] _data, uint _pos)
            : base(_tables, _data, _pos, CorTokenType.mdtTypeSpec)
        { }
    }
}
