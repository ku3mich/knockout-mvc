﻿using System;
using System.ComponentModel.DataAnnotations;

namespace PerpetuumSoft.Knockout.Utilities
{
    public static class EnumExt
    {
        public static string GetEnumMemberDisplay(this ValueType enumValue)
        {
            var type = enumValue.GetType();
            var fld = type.GetField(enumValue.ToString());
            var da = (DisplayAttribute[])(fld.GetCustomAttributes(typeof(DisplayAttribute), false));

            return da.Length > 0 ? da[0].Name : enumValue.ToString();
        }
    }
}
