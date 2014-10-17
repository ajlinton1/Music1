// Documentation Reference:
//
// * Interface identifiers taken from the following URL:
//   http://msdn.com/library/en-us/wmform/htm/interfaceidentifiers.asp
//
// * IWMHeaderInfo3 API declaration taken from the following URL:
//   http://msdn.com/library/en-us/wmform/htm/iwmheaderinfo3interface.asp
//
// * IWMMetadataEditor2 API declaration taken from the following URL:
//   http://msdn.com/library/en-us/wmform/htm/iwmmetadataeditorinterface.asp
//
// * WMCreateEditor API call taken from the following URL:
//   http://msdn.com/library/en-us/wmform/htm/wmcreateeditor.asp
//

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.Samples.MediaCatalog
{

	// IWMHeaderInfo3
	[
		ComImport,
		Guid("15cc68e3-27cc-4ecd-b222-3f5d02d80bd5"),
		InterfaceType(ComInterfaceType.InterfaceIsIUnknown)
	]
	public interface IWMHeaderInfo3
	{
		// HRESULT GetAttributeCount(WORD wStreamNum, WORD* pcAttributes);
		void GetAttributeCount([In]	ushort wStreamNum, [Out] out ushort pcAttributes);


		// HRESULT GetAttributeByIndex(WORD wIndex, WORD* pwStreamNum, 
		//    WCHAR* pwszName, WORD* pcchNameLen, WMT_ATTR_DATATYPE* pType, 
		//    BYTE* pValue, WORD* pcbLength);
		void GetAttributeByIndex( 
			[In] ushort wIndex,
			[Out, In] ref ushort pwStreamNum,
			[Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pwszName,
			[Out, In] ref ushort pcchNameLen,
			[Out] out WMT_ATTR_DATATYPE pType,
			[Out, MarshalAs(UnmanagedType.LPArray)] byte[] pValue,
			[Out, In] ref ushort pcbLength );


		// HRESULT GetAttributeByName(WORD* pwStreamNum, LPCWSTR pszName, 
		//    WMT_ATTR_DATATYPE* pType, BYTE* pValue, WORD* pcbLength);
		void GetAttributeByName(
			[Out, In] ref ushort pwStreamNum,
			[In, MarshalAs(UnmanagedType.LPWStr)] string pszName,
			[Out] out WMT_ATTR_DATATYPE pType,
			[Out, MarshalAs(UnmanagedType.LPArray)] byte[] pValue,
			[Out, In] ref ushort pcbLength );

	}


	// IWMMetadataEditor2
	[
		ComImport,
		Guid("203cffe3-2e18-4fdf-b59d-6e71530534cf"),
		InterfaceType(ComInterfaceType.InterfaceIsIUnknown)
	]
	public interface IWMMetadataEditor2
	{
		// HRESULT Open(const WCHAR* pwszFilename);
		void Open([In,MarshalAs(UnmanagedType.LPWStr)] string pwszFilename);

		// HRESULT Flush();
		void Flush();

		// HRESULT Close();
		void Close();
	}


	public class WindowsMediaWrapper
	{

		// Hide default constructor
		private WindowsMediaWrapper()
		{
		}

		// HRESULT WMCreateEditor(IWMMetadataEditor** ppEditor);
		[DllImport("WMVCore.dll", EntryPoint="WMCreateEditor", PreserveSig=false, 
			 CharSet=CharSet.Unicode, ExactSpelling=true, SetLastError=true)]
		public static extern void CreateEditor(
			[Out, MarshalAs(UnmanagedType.Interface)] out IWMMetadataEditor2 ppEditor);
	}

	// Just the ones we handle
	public enum ComError
	{
		ASF_E_NOTFOUND		  = -1072887824,
		NS_E_FILE_OPEN_FAILED = -1072889827
	}

	public enum WMT_ATTR_DATATYPE
	{
		WMT_TYPE_DWORD   = 0,
		WMT_TYPE_STRING  = 1,
		WMT_TYPE_BINARY  = 2,
		WMT_TYPE_BOOL    = 3,
		WMT_TYPE_QWORD   = 4,
		WMT_TYPE_WORD    = 5,
		WMT_TYPE_GUID    = 6,
	}
}
