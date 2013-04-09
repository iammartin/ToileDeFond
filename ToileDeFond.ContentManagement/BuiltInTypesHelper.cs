using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToileDeFond.Utilities;

namespace ToileDeFond.ContentManagement
{
    //public static class BuiltInTypesHelper
    //{
    //    public static readonly Type[] BuiltInTypes = new[] { typeof(bool), typeof(byte), typeof(char), typeof(DateTime), typeof(decimal), typeof(double), typeof(Int16), typeof(Int32), typeof(Int64),
    //    typeof(sbyte), typeof(Single), typeof(string), typeof(UInt16), typeof(UInt32), typeof(UInt64), typeof(CultureInfo), typeof(Guid), typeof(Content)};


    //    public static bool IsBuiltIn(this Type type)
    //    {
    //        if (type.IsGenericList())
    //        {
    //            var genericArguments = type.GetGenericArguments();

    //            if (genericArguments.Length != 1)
    //            {
    //                return false;
    //            }

    //            type = genericArguments.ElementAt(0);
    //        }

    //        if (type.IsNullable())
    //        {
    //            var nc = new NullableConverter(type);
    //            type = nc.UnderlyingType;
    //        }

    //        return BuiltInTypes.Contains(type);

    //        //if (type == typeof(Guid) || type == typeof(List<Guid>))
    //        //    return true;


    //        ////TODO: Tout supporter et serializer au pire

    //        //if (type == typeof(CultureInfo))
    //        //    return true;

    //        //var result = true;

    //        //switch (Type.GetTypeCode(type))
    //        //{
    //        //    case TypeCode.Boolean:
    //        //    case TypeCode.Byte:
    //        //    case TypeCode.Char:
    //        //    case TypeCode.DateTime:
    //        //    case TypeCode.Decimal:
    //        //    case TypeCode.Double:
    //        //    case TypeCode.Int16:
    //        //    case TypeCode.Int32:
    //        //    case TypeCode.Int64:
    //        //    case TypeCode.SByte:
    //        //    case TypeCode.Single:
    //        //    case TypeCode.String:
    //        //    case TypeCode.UInt16:
    //        //    case TypeCode.UInt32:
    //        //    case TypeCode.UInt64:
    //        //        break;
    //        //    default:
    //        //        result = false;
    //        //        break;
    //        //}

    //        //return result;
    //    }
    //}
}
