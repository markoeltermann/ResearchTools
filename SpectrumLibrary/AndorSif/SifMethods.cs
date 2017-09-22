using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SpectrumLibrary.AndorSif
{
    /*
        AT_EXP_MOD AT_U32 AT_EXP_CONV ATSIF_SetFileAccessMode(ATSIF_ReadMode _mode);

    AT_EXP_MOD AT_U32 AT_EXP_CONV ATSIF_ReadFromFile(AT_C * _sz_filename);

    AT_EXP_MOD AT_U32 AT_EXP_CONV ATSIF_CloseFile();

    AT_EXP_MOD AT_U32 AT_EXP_CONV ATSIF_ReadFromByteArray(AT_U8 * _buffer, AT_U32 _ui_bufferSize);

    AT_EXP_MOD AT_U32 AT_EXP_CONV ATSIF_IsLoaded(AT_32 * _i_loaded);

    AT_EXP_MOD AT_U32 AT_EXP_CONV ATSIF_IsDataSourcePresent(ATSIF_DataSource _source, AT_32 *_i_present);

    AT_EXP_MOD AT_U32 AT_EXP_CONV ATSIF_GetStructureVersion(ATSIF_StructureElement _element, AT_U32 * _ui_versionHigh, AT_U32 * _ui_versionLow);

    AT_EXP_MOD AT_U32 AT_EXP_CONV ATSIF_GetFrameSize(ATSIF_DataSource _source, AT_U32 * _ui_size);

    AT_EXP_MOD AT_U32 AT_EXP_CONV ATSIF_GetNumberFrames(ATSIF_DataSource _source, AT_U32 * _ui_images);

    AT_EXP_MOD AT_U32 AT_EXP_CONV ATSIF_GetNumberSubImages(ATSIF_DataSource _source, AT_U32 * _ui_subimages);

    AT_EXP_MOD AT_U32 AT_EXP_CONV ATSIF_GetSubImageInfo(ATSIF_DataSource _source, AT_U32 _ui_index,
                                                         AT_U32 * _ui_left, AT_U32 * _ui_bottom,
                                                         AT_U32 * _ui_right, AT_U32 * _ui_top,
                                                         AT_U32 * _ui_hBin, AT_U32 * _ui_vBin);

    AT_EXP_MOD AT_U32 AT_EXP_CONV ATSIF_GetAllFrames(ATSIF_DataSource _source, float * _pf_data, AT_U32 _ui_bufferSize);

    AT_EXP_MOD AT_U32 AT_EXP_CONV ATSIF_GetFrame(ATSIF_DataSource _source, AT_U32 _ui_index, float * _pf_data, AT_U32 _ui_bufferSize);

    AT_EXP_MOD AT_U32 AT_EXP_CONV ATSIF_GetPropertyValue(ATSIF_DataSource _source,
                                                         const AT_C * _sz_propertyName,
                                                         AT_C * _sz_propertyValue,
                                                         AT_U32 _ui_bufferSize);

    AT_EXP_MOD AT_U32 AT_EXP_CONV ATSIF_GetPropertyType(ATSIF_DataSource _source,
                                                        const AT_C * _sz_propertyName,
                                                        ATSIF_PropertyType * _propertyType);

    AT_EXP_MOD AT_U32 AT_EXP_CONV ATSIF_GetDataStartBytePosition(ATSIF_DataSource _source,
                                                                 AT_32 * _i_startPosition);

    AT_EXP_MOD AT_U32 AT_EXP_CONV ATSIF_GetPixelCalibration(ATSIF_DataSource _source, ATSIF_CalibrationAxis _axis,
                                                            AT_32 _i_pixel, double * _d_calibValue);
        */
    public class SifMethods
    {

        private const string AndorDllName = "ATSIFIO.dll";

        [DllImport(AndorDllName, EntryPoint = "ATSIF_ReadFromFile")]
        public static extern uint ReadFromFile([MarshalAs(UnmanagedType.LPStr)]string _sz_filename);

        [DllImport(AndorDllName, EntryPoint = "ATSIF_CloseFile")]
        public static extern uint CloseFile();

        //AT_EXP_MOD AT_U32 AT_EXP_CONV ATSIF_IsLoaded(AT_32 * _i_loaded);

        [DllImport(AndorDllName, EntryPoint = "ATSIF_IsDataSourcePresent")]
        public static extern uint IsDataSourcePresent(SifDataSource _source, out int _i_present);

        //AT_EXP_MOD AT_U32 AT_EXP_CONV ATSIF_GetStructureVersion(ATSIF_StructureElement _element, AT_U32 * _ui_versionHigh, AT_U32 * _ui_versionLow);

        [DllImport(AndorDllName, EntryPoint = "ATSIF_GetFrameSize")]
        public static extern uint GetFrameSize(SifDataSource _source, out uint _ui_size);

        [DllImport(AndorDllName, EntryPoint = "ATSIF_GetNumberFrames")]
        public static extern uint GetNumberFrames(SifDataSource _source, out uint _ui_images);

        [DllImport(AndorDllName, EntryPoint = "ATSIF_GetAllFrames")]
        public static extern uint GetAllFrames(SifDataSource _source, [In]float[] _pf_data, uint _ui_bufferSize);

        [DllImport(AndorDllName, EntryPoint = "ATSIF_GetFrame")]
        public static extern uint GetFrame(SifDataSource _source, uint _ui_index, [In]float[] _pf_data, uint _ui_bufferSize);

        [DllImport(AndorDllName, EntryPoint = "ATSIF_GetPixelCalibration")]
        public static extern uint GetPixelCalibration(SifDataSource _source, SifCalibrationAxis _axis, int _i_pixel, out double _d_calibValue);
    }
}
