using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.IO;

namespace DotnetPatch
{
    /// <summary>
    /// 用于为反混淆提供PE文件读写的类，主要提供创建新的元数据段、调整文件对齐为段对齐、元数据的定位、名称元数据的读写。
    /// </summary>
    /// <remarks>
    /// 需要反混淆的：
    /// Field,MethodDef,
    /// Property,Event,Param,GenericParam,MemberRef,
    /// TypeDef,TypeRef,ExportedType
    /// 不知道如何反混淆的：
    /// ManifestResource
    /// 不需要反混淆但有名称的：
    /// Assembly,AssemblyRef,Module,ModuleRef,File,ImplMap
    /// </remarks>
    public class ClrFile
    {
        public byte[] Buffer
        {
            get { return buffer; }
        }
        public MetadataTables Metadata
        {
            get { return metadata; }
        }

        public void Load(string file)
        {
            FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            try {
                buffer = br.ReadBytes((int)fs.Length);
                dosHeader = ReadDosHeader(buffer, 0);
                ntHeader = ReadNTHeader(buffer, dosHeader.lfanew);
                sectionAlignment = ntHeader.NTOptionalHeader.SectionAlignment;
                uint pos = dosHeader.lfanew + IMAGE_NT_HEADERS.StructSize;
                uint va = ntHeader.NTOptionalHeader.DataDirectory.runtimeHeader.VirtualAddress;
                sectHeaders = new IMAGE_SECTION_HEADER[ntHeader.NTFileHeader.NumberOfSections];
                for (uint i = 0; i < ntHeader.NTFileHeader.NumberOfSections; i++) {
                    IMAGE_SECTION_HEADER sectHeader = ReadSectionHeader(buffer, pos);
                    pos += IMAGE_SECTION_HEADER.StructSize;

                    sectHeaders[i] = sectHeader;
                }
                newSectHeaderPos = pos;
                pos = GetFileOffset(va);
                corHeader = ReadCor20Header(buffer, pos);
                uint metadataRVA = corHeader.MetaData.VirtualAddress;
                uint metadataSize = corHeader.MetaData.Size;
                pos = GetFileOffset(metadataRVA);
                metadata = ReadMetadataTables(buffer, pos, metadataSize);
            } finally {
                br.Close();
                fs.Close();
                fs.Dispose();
            }
        }
        public uint GetFileOffset(uint va)
        {
            int n = sectHeaders.Length;
            uint sva = 0;
            uint addr = 0;
            for (uint i = 0; i < n; i++) {
                IMAGE_SECTION_HEADER sectHeader = sectHeaders[i];
                if (sectHeader.VirtualAddress <= va) {
                    if (sva <= sectHeader.VirtualAddress) {
                        sva = sectHeader.VirtualAddress;
                        addr = sectHeader.PointerToRawData;
                    }
                }
            }
            return va - sva + addr;
        }
        public void Save(string file)
        {
            FileStream fs = new FileStream(file, FileMode.Create, FileAccess.Write);
            BinaryWriter bw = new BinaryWriter(fs);
            try {
                bw.Write(buffer);
            } finally {
                bw.Close();
                fs.Close();
                fs.Dispose();
            }
        }

        public IMAGE_SECTION_HEADER PrepareNewSectionHeader(string name, uint sectSize, bool forMetadata)
        {
            uint curRVA = Alignment.Calc(newSectHeaderPos + IMAGE_SECTION_HEADER.StructSize, sectionAlignment);
            ntHeader.NTFileHeader.NumberOfSections++;
            ntHeader.NTOptionalHeader.FileAlignment = sectionAlignment;//必须让文件对齐与段对齐一样，这样才能保证跨段偏移对文件与对内存一致
            ntHeader.NTOptionalHeader.SizeOfHeaders = curRVA;

            newSectHeaders = new IMAGE_SECTION_HEADER[sectHeaders.Length];
            for (uint i = 0; i < newSectHeaders.Length; i++) {
                newSectHeaders[i] = sectHeaders[i];

                newSectHeaders[i].VirtualAddress = curRVA;
                newSectHeaders[i].SizeOfRawData = Alignment.Calc(newSectHeaders[i].SizeOfRawData, sectionAlignment);
                newSectHeaders[i].PointerToRawData = curRVA;

                curRVA = Alignment.Calc(curRVA + newSectHeaders[i].SizeOfRawData, sectionAlignment);
            }
            //添加新段
            IMAGE_SECTION_HEADER header = new IMAGE_SECTION_HEADER();
            header.Name = name;
            header.VirtualSize = sectSize;
            header.VirtualAddress = curRVA;
            header.SizeOfRawData = Alignment.Calc(sectSize, sectionAlignment);
            header.PointerToRawData = curRVA;
            header.PointerToRelocations = 0;
            header.PointerToLinenumbers = 0;
            header.NumberOfRelocations = 0;
            header.NumberOfLinenumbers = 0;
            header.Characteristics = 0x60000020;

            ntHeader.NTOptionalHeader.SizeOfImage = Alignment.Calc(header.VirtualAddress + header.SizeOfRawData, sectionAlignment);

            if (forMetadata) {
                corHeader.MetaData.VirtualAddress = header.VirtualAddress;
                corHeader.MetaData.Size = sectSize;
                uint va = ntHeader.NTOptionalHeader.DataDirectory.runtimeHeader.VirtualAddress;
                uint pos = GetFileOffset(va);
                WriteCor20Header(buffer, pos, corHeader);//改写原映像数据中的Cor20头，这些修改在Save时会随着所属段一起拷贝到新映像数据中
            }
            return header;
        }
        public void Save(string file, IMAGE_SECTION_HEADER header, byte[] newSection)
        {
            FileStream fs = new FileStream(file, FileMode.Create, FileAccess.Write);
            BinaryWriter bw = new BinaryWriter(fs);
            try {
                uint size = header.PointerToRawData + header.SizeOfRawData;
                byte[] buf = new byte[size];
                buf.Initialize();

                WriteDosHeader(buf, 0, dosHeader);
                uint pos = dosHeader.lfanew;
                WriteNTHeader(buf, pos, ntHeader);
                pos = dosHeader.lfanew + IMAGE_NT_HEADERS.StructSize;
                for (uint i = 0; i < newSectHeaders.Length; i++) {
                    WriteSectionHeader(buf, pos, newSectHeaders[i]);
                    pos += IMAGE_SECTION_HEADER.StructSize;
                }
                WriteSectionHeader(buf, newSectHeaderPos, header);

                for (uint i = 0; i < newSectHeaders.Length; i++) {
                    Array.Copy(buffer, sectHeaders[i].PointerToRawData, buf, newSectHeaders[i].PointerToRawData, sectHeaders[i].SizeOfRawData);
                }
                Array.Copy(newSection, 0, buf, header.PointerToRawData, newSection.Length);

                bw.Write(buf);
            } finally {
                bw.Close();
                fs.Close();
                fs.Dispose();
            }
        }

        private static unsafe IMAGE_DOS_HEADER ReadDosHeader(byte[] buffer, uint pos)
        {
            IMAGE_DOS_HEADER dosHeader = new IMAGE_DOS_HEADER();
            fixed (byte* p = buffer) {
                IMAGE_DOS_HEADER* ptr = (IMAGE_DOS_HEADER*)(p + pos);
                dosHeader = *ptr;
            }
            return dosHeader;
        }
        private static unsafe IMAGE_NT_HEADERS ReadNTHeader(byte[] buffer, uint pos)
        {
            IMAGE_NT_HEADERS dosHeader = new IMAGE_NT_HEADERS();
            fixed (byte* p = buffer) {
                IMAGE_NT_HEADERS* ptr = (IMAGE_NT_HEADERS*)(p + pos);
                dosHeader = *ptr;
            }
            return dosHeader;
        }
        private static unsafe IMAGE_SECTION_HEADER ReadSectionHeader(byte[] buffer, uint pos)
        {
            IMAGE_SECTION_HEADER dosHeader = new IMAGE_SECTION_HEADER();
            fixed (byte* p = buffer) {
                IMAGE_SECTION_HEADER* ptr = (IMAGE_SECTION_HEADER*)(p + pos);
                dosHeader = *ptr;
            }
            return dosHeader;
        }
        private static unsafe IMAGE_COR20_HEADER ReadCor20Header(byte[] buffer, uint pos)
        {
            IMAGE_COR20_HEADER dosHeader = new IMAGE_COR20_HEADER();
            fixed (byte* p = buffer) {
                IMAGE_COR20_HEADER* ptr = (IMAGE_COR20_HEADER*)(p + pos);
                dosHeader = *ptr;
            }
            return dosHeader;
        }

        private static unsafe void WriteDosHeader(byte[] buffer, uint pos, IMAGE_DOS_HEADER header)
        {
            fixed (byte* p = buffer) {
                IMAGE_DOS_HEADER* ptr = (IMAGE_DOS_HEADER*)(p + pos);
                *ptr = header;
            }
        }
        private static unsafe void WriteNTHeader(byte[] buffer, uint pos, IMAGE_NT_HEADERS header)
        {
            fixed (byte* p = buffer) {
                IMAGE_NT_HEADERS* ptr = (IMAGE_NT_HEADERS*)(p + pos);
                *ptr = header;
            }
        }
        private static unsafe void WriteSectionHeader(byte[] buffer, uint pos, IMAGE_SECTION_HEADER header)
        {
            fixed (byte* p = buffer) {
                IMAGE_SECTION_HEADER* ptr = (IMAGE_SECTION_HEADER*)(p + pos);
                *ptr = header;
            }
        }
        private static unsafe void WriteCor20Header(byte[] buffer, uint pos, IMAGE_COR20_HEADER header)
        {
            fixed (byte* p = buffer) {
                IMAGE_COR20_HEADER* ptr = (IMAGE_COR20_HEADER*)(p + pos);
                *ptr = header;
            }
        }

        private static MetadataTables ReadMetadataTables(byte[] buffer, uint pos, uint size)
        {
            MetadataTables table = new MetadataTables(buffer, pos, size);
            return table;
        }

        private byte[] buffer = null;
        private uint sectionAlignment = 0;
        private uint newSectHeaderPos = 0;

        IMAGE_DOS_HEADER dosHeader;
        IMAGE_NT_HEADERS ntHeader;
        private IMAGE_SECTION_HEADER[] sectHeaders = null;
        private IMAGE_COR20_HEADER corHeader;
        private MetadataTables metadata = null;

        IMAGE_SECTION_HEADER[] newSectHeaders = null;
    }
    public static class Alignment
    {
        public static uint Calc(uint val, uint align)
        {
            if (val % align == 0) {
                return val;
            } else {
                return val + align - val % align;
            }
        }
        public static int Calc(int val, int align)
        {
            if (val % align == 0) {
                return val;
            } else {
                return val + align - val % align;
            }
        }
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct IMAGE_NT_HEADERS
    {
        public uint Signature;
        public IMAGE_FILE_HEADER NTFileHeader;
        public IMAGE_OPTIONAL_HEADER32 NTOptionalHeader;

        public static unsafe uint StructSize
        {
            get
            {
                return (uint)sizeof(IMAGE_NT_HEADERS);
            }
        }
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct IMAGE_FILE_HEADER
    {
        public ushort Machine;
        public ushort NumberOfSections;
        public uint TimeDateStamp;
        public uint PointerToSymbolTable;
        public uint NumberOfSymbols;
        public ushort SizeOfOptionalHeader;
        public ushort Characteristics;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct IMAGE_DATA_DIRECTORY
    {
        public uint VirtualAddress;
        public uint Size;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct IMAGE_DATA_DIRECTORIES
    {
        public IMAGE_DATA_DIRECTORY exportTable;
        public IMAGE_DATA_DIRECTORY importTable;
        public IMAGE_DATA_DIRECTORY resourceTable;
        public IMAGE_DATA_DIRECTORY exceptionTable;
        public IMAGE_DATA_DIRECTORY certificateTable;
        public IMAGE_DATA_DIRECTORY baseRelocationTable;
        public IMAGE_DATA_DIRECTORY debug;
        public IMAGE_DATA_DIRECTORY copyright;
        public IMAGE_DATA_DIRECTORY globalPtr;
        public IMAGE_DATA_DIRECTORY tlsTable;
        public IMAGE_DATA_DIRECTORY loadConfigTable;
        public IMAGE_DATA_DIRECTORY boundImport;
        public IMAGE_DATA_DIRECTORY iat;
        public IMAGE_DATA_DIRECTORY delayImportDescriptor;
        public IMAGE_DATA_DIRECTORY runtimeHeader;
        public IMAGE_DATA_DIRECTORY reserved;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct IMAGE_OPTIONAL_HEADER32
    {
        public ushort Magic;
        public byte MajorLinkerVersion;
        public byte MinorLinkerVersion;
        public uint SizeOfCode;
        public uint SizeOfInitializedData;
        public uint SizeOfUninitializedData;
        public uint AddressOfEntryPoint;
        public uint BaseOfCode;
        public uint BaseOfData;
        public uint ImageBase;
        public uint SectionAlignment;
        public uint FileAlignment;
        public ushort MajorOperatingSystemVersion;
        public ushort MinorOperatingSystemVersion;
        public ushort MajorImageVersion;
        public ushort MinorImageVersion;
        public ushort MajorSubsystemVersion;
        public ushort MinorSubsystemVersion;
        public uint Win32VersionValue;
        public uint SizeOfImage;
        public uint SizeOfHeaders;
        public uint CheckSum;
        public ushort Subsystem;
        public ushort DllCharacteristics;
        public uint SizeOfStackReserve;
        public uint SizeOfStackCommit;
        public uint SizeOfHeapReserve;
        public uint SizeOfHeapCommit;
        public uint LoaderFlags;
        public uint NumberOfRvaAndSizes;
        public IMAGE_DATA_DIRECTORIES DataDirectory;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct IMAGE_DOS_HEADER_eres
    {
        public ushort eres1;
        public ushort eres2;
        public ushort eres3;
        public ushort eres4;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct IMAGE_DOS_HEADER_eres2
    {
        public ushort eres1;
        public ushort eres2;
        public ushort eres3;
        public ushort eres4;
        public ushort eres5;
        public ushort eres6;
        public ushort eres7;
        public ushort eres8;
        public ushort eres9;
        public ushort eres10;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct IMAGE_DOS_HEADER
    {
        [Description("Magic number")]
        public ushort magic;
        [Description("Bytes on last page of file")]
        public ushort cblp;
        [Description("Pages in file")]
        public ushort cp;
        [Description("Relocations")]
        public ushort crlc;
        [Description("Size of header in paragraphs")]
        public ushort cparhdr;
        [Description("Minimum extra paragraphs needed")]
        public ushort minalloc;
        [Description("Maximum extra paragraphs needed")]
        public ushort maxalloc;
        [Description("Initial (relative) SS value")]
        public ushort ss;
        [Description("Initial SP value")]
        public ushort sp;
        [Description("Checksum")]
        public ushort csum;
        [Description("Initial IP value")]
        public ushort ip;
        [Description("Initial (relative) CS value")]
        public ushort cs;
        [Description("File address of relocation table")]
        public ushort lfarlc;
        [Description("Overlay number")]
        public ushort ovno;
        [Description("Reserved words")]
        public IMAGE_DOS_HEADER_eres res;
        [Description("OEM identifier (for e_oeminfo)")]
        public ushort oemid;
        [Description("OEM information; e_oemid specific")]
        public ushort oeminfo;
        [Description("Reserved words")]
        public IMAGE_DOS_HEADER_eres2 res2;
        [Description("File address of new exe header")]
        public uint lfanew;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct IMAGE_COR20_HEADER
    {
        public uint Size;
        public ushort MajorRuntimeVersion;
        public ushort MinorRuntimeVersion;
        public IMAGE_DATA_DIRECTORY MetaData;
        public uint Flags;
        public uint EntryPointToken;
        public IMAGE_DATA_DIRECTORY Resources;
        public IMAGE_DATA_DIRECTORY StrongNameSignature;
        public IMAGE_DATA_DIRECTORY CodeManagerTable;
        public IMAGE_DATA_DIRECTORY VTableFixups;
        public IMAGE_DATA_DIRECTORY ExportAddressTableJumps;
        public IMAGE_DATA_DIRECTORY ManagedNativeHeader;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct IMAGE_SECTION_HEADER
    {
        public UInt64 NameField;
        public uint VirtualSize;
        public uint VirtualAddress;
        public uint SizeOfRawData;
        public uint PointerToRawData;
        public uint PointerToRelocations;
        public uint PointerToLinenumbers;
        public ushort NumberOfRelocations;
        public ushort NumberOfLinenumbers;
        public uint Characteristics;

        public unsafe string Name
        {
            get
            {
                fixed (UInt64* p = &NameField) {
                    byte[] data = new byte[8];
                    fixed (byte* p2 = data) {
                        UInt64* ptr = (UInt64*)p2;
                        *ptr = *p;
                    }
                    return System.Text.Encoding.ASCII.GetString(data);
                }
            }
            set
            {
                string s = value;
                if (s.Length > 8)
                    s = s.Substring(0, 8);
                byte[] data0 = System.Text.Encoding.ASCII.GetBytes(s);
                byte[] data = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };
                for (int i = 0; i < data0.Length; i++) {
                    data[i] = data0[i];
                }
                fixed (UInt64* p = &NameField) {
                    fixed (byte* p2 = data) {
                        UInt64* ptr = (UInt64*)p2;
                        *p = *ptr;
                    }
                }
            }
        }

        public static unsafe uint StructSize
        {
            get
            {
                return (uint)sizeof(IMAGE_SECTION_HEADER);
            }
        }
    }
}
