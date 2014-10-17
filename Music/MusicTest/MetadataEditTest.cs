using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WMFSDKWrapper;

namespace MusicTest
{
    /// <summary>
    /// Summary description for MetadataEditTest
    /// </summary>
    [TestClass]
    public class MetadataEditTest
    {
        public MetadataEditTest()
        {
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        void PrintListHeader()
        {
            Console.WriteLine("*");
            Console.WriteLine("* Idx  Name                   Stream Language Type  Value");
            Console.WriteLine("* ---  ----                   ------ -------- ----  -----");
        }

        void PrintAttribute(ushort wIndex,
                             ushort wStream,
                             string pwszName,
                             WMT_ATTR_DATATYPE AttribDataType,
                             ushort wLangID,
                             byte[] pbValue,
                             uint dwValueLen)
        {
            string pwszValue = String.Empty;

            //
            // Make the data type string
            //
            string pwszType = "Unknown";
            string[] pTypes = { "DWORD", "STRING", "BINARY", "BOOL", "QWORD", "WORD", "GUID" };

            if (pTypes.Length > Convert.ToInt32(AttribDataType))
            {
                pwszType = pTypes[Convert.ToInt32(AttribDataType)];
            }

            //
            // The attribute value.
            //
            switch (AttribDataType)
            {
                // String
                case WMFSDKWrapper.WMT_ATTR_DATATYPE.WMT_TYPE_STRING:

                    if (0 == dwValueLen)
                    {
                        pwszValue = "***** NULL *****";
                    }
                    else
                    {
                        if ((0xFE == Convert.ToInt16(pbValue[0])) &&
                             (0xFF == Convert.ToInt16(pbValue[1])))
                        {
                            pwszValue = "\"UTF-16LE BOM+\"";

                            if (4 <= dwValueLen)
                            {
                                for (int i = 0; i < pbValue.Length - 2; i += 2)
                                {
                                    pwszValue += Convert.ToString(BitConverter.ToChar(pbValue, i));
                                }
                            }

                            pwszValue = pwszValue + "\"";
                        }
                        else if ((0xFF == Convert.ToInt16(pbValue[0])) &&
                                  (0xFE == Convert.ToInt16(pbValue[1])))
                        {
                            pwszValue = "\"UTF-16BE BOM+\"";
                            if (4 <= dwValueLen)
                            {
                                for (int i = 0; i < pbValue.Length - 2; i += 2)
                                {
                                    pwszValue += Convert.ToString(BitConverter.ToChar(pbValue, i));
                                }
                            }

                            pwszValue = pwszValue + "\"";
                        }
                        else
                        {
                            pwszValue = "\"";
                            if (2 <= dwValueLen)
                            {
                                for (int i = 0; i < pbValue.Length - 2; i += 2)
                                {
                                    pwszValue += Convert.ToString(BitConverter.ToChar(pbValue, i));
                                }
                            }

                            pwszValue += "\"";
                        }
                    }
                    break;

                // Binary
                case WMFSDKWrapper.WMT_ATTR_DATATYPE.WMT_TYPE_BINARY:

                    pwszValue = "[" + dwValueLen.ToString() + " bytes]";
                    break;

                // Boolean
                case WMFSDKWrapper.WMT_ATTR_DATATYPE.WMT_TYPE_BOOL:

                    if (BitConverter.ToBoolean(pbValue, 0))
                    {
                        pwszValue = "True";
                    }
                    else
                    {
                        pwszValue = "False";
                    }
                    break;

                // DWORD
                case WMFSDKWrapper.WMT_ATTR_DATATYPE.WMT_TYPE_DWORD:

                    uint dwValue = BitConverter.ToUInt32(pbValue, 0);
                    pwszValue = dwValue.ToString();
                    break;

                // QWORD
                case WMFSDKWrapper.WMT_ATTR_DATATYPE.WMT_TYPE_QWORD:

                    ulong qwValue = BitConverter.ToUInt64(pbValue, 0);
                    pwszValue = qwValue.ToString();
                    break;

                // WORD
                case WMFSDKWrapper.WMT_ATTR_DATATYPE.WMT_TYPE_WORD:

                    uint wValue = BitConverter.ToUInt16(pbValue, 0);
                    pwszValue = wValue.ToString();
                    break;

                // GUID
                case WMFSDKWrapper.WMT_ATTR_DATATYPE.WMT_TYPE_GUID:

                    pwszValue = BitConverter.ToString(pbValue, 0, pbValue.Length);
                    break;

                default:

                    break;
            }

            //
            // Dump the string to the screen.
            //  
            Console.WriteLine("* {0, 3}  {1, -25} {2, 3}  {3, 3}  {4, 7}  {5}",
                wIndex, pwszName, wStream, wLangID, pwszType, pwszValue);
        }

        [TestMethod]
        public void GetMetadata()
        {
            try
            {
                String filename = @"E:\Music\Album Leaf\Vermillion .WMA";
                ushort wStreamNum = 0;

                IWMMetadataEditor MetadataEditor;
                IWMHeaderInfo3 HeaderInfo3;
                ushort wAttributeCount;

                WMFSDKFunctions.WMCreateEditor(out MetadataEditor);

                MetadataEditor.Open(filename);

                HeaderInfo3 = (IWMHeaderInfo3)MetadataEditor;

                HeaderInfo3.GetAttributeCount(wStreamNum, out wAttributeCount);

                PrintListHeader();

                for (ushort wAttribIndex = 0; wAttribIndex < wAttributeCount; wAttribIndex++)
                {
                    WMT_ATTR_DATATYPE wAttribType;
                    string pwszAttribName = null;
                    byte[] pbAttribValue = null;
                    ushort wAttribNameLen = 0;
                    ushort wAttribValueLen = 0;

                    HeaderInfo3.GetAttributeByIndex(wAttribIndex,
                                                     ref wStreamNum,
                                                     pwszAttribName,
                                                     ref wAttribNameLen,
                                                     out wAttribType,
                                                     pbAttribValue,
                                                     ref wAttribValueLen);

                    pbAttribValue = new byte[wAttribValueLen];
                    pwszAttribName = new String((char)0, wAttribNameLen);

                    HeaderInfo3.GetAttributeByIndex(wAttribIndex,
                                                     ref wStreamNum,
                                                     pwszAttribName,
                                                     ref wAttribNameLen,
                                                     out wAttribType,
                                                     pbAttribValue,
                                                     ref wAttribValueLen);

                    PrintAttribute(wAttribIndex, wStreamNum, pwszAttribName, wAttribType, 0, pbAttribValue, wAttribValueLen);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }
    }
}
