using Amazon.DynamoDBv2.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Dynamo.ORM.Constants
{
    internal class AttributeValueConverter
    {
        internal static IDictionary<Type, Func<object, AttributeValue>> ConvertToAttributeValue = new Dictionary<Type, Func<object, AttributeValue>>
        {
            { typeof(bool), (object @object) => new AttributeValue { BOOL = bool.Parse($"{@object}")} },
            { typeof(bool?), (object @object) =>
                {
                    var attributeValue = new AttributeValue();
                    if (@object != null)
                        attributeValue.BOOL = bool.Parse($"{@object}");
                    else
                        attributeValue.NULL = true;
                    return attributeValue;
                }
            },
            { typeof(byte), (object @object) => new AttributeValue { B = new MemoryStream(new byte[] { (byte)@object }) } },
            { typeof(byte?), (object @object) =>
                {
                    var attributeValue = new AttributeValue();
                    if (@object != null)
                        attributeValue.B = new MemoryStream(new byte[] { (byte)@object });
                    else
                        attributeValue.NULL = true;
                    return attributeValue;
                }
            },
            { typeof(byte[]), (object @object) =>
                {
                    var attributeValue = new AttributeValue();
                    if (@object != null && ((byte[])@object).Length > 0)
                        attributeValue.B = new MemoryStream((byte[])@object);
                    else
                        attributeValue.NULL = true;
                    return attributeValue;
                }
            },
            { typeof(char), (object @object) => new AttributeValue($"{@object}")},
            { typeof(char?), (object @object) =>
                {
                    var attributeValue = new AttributeValue();
                    if (@object != null)
                        attributeValue.S = $"{@object}";
                    else
                        attributeValue.NULL = true;
                    return attributeValue;
                }
            },
            { typeof(DateTime), (object @object) => new AttributeValue { S = ((DateTime)@object).ToString(Formats.DateFormat) } },
            { typeof(DateTime?), (object @object) =>
                {
                    var attributeValue = new AttributeValue();
                    if (@object != null)
                        attributeValue.S = ((DateTime)@object).ToString(Formats.DateFormat);
                    else
                        attributeValue.NULL = true;
                    return attributeValue;
                }
            },
            { typeof(decimal), (object @object) => new AttributeValue{ N = $"{@object}" } },
            { typeof(decimal?), (object @object) =>
                {
                    var attributeValue = new AttributeValue();
                    if (@object != null)
                        attributeValue.N = $"{@object}";
                    else
                        attributeValue.NULL = true;
                    return attributeValue;
                }
            },
            { typeof(double), (object @object) => new AttributeValue{ N = $"{@object}" } },
            { typeof(double?), (object @object) =>
                {
                    var attributeValue = new AttributeValue();
                    if (@object != null)
                        attributeValue.N = $"{@object}";
                    else
                        attributeValue.NULL = true;
                    return attributeValue;
                }
            },
            { typeof(float), (object @object) => new AttributeValue{ N = $"{@object}" } },
            { typeof(float?), (object @object) =>
                {
                    var attributeValue = new AttributeValue();
                    if (@object != null)
                        attributeValue.N = $"{@object}";
                    else
                        attributeValue.NULL = true;
                    return attributeValue;
                }
            },
            { typeof(short), (object @object) => new AttributeValue{ N = $"{@object}" } },
            { typeof(short?), (object @object) =>
                {
                    var attributeValue = new AttributeValue();
                    if (@object != null)
                        attributeValue.N = $"{@object}";
                    else
                        attributeValue.NULL = true;
                    return attributeValue;
                }
            },
            { typeof(int), (object @object) => new AttributeValue { N = $"{@object}" } },
            { typeof(int?), (object @object) =>
                {
                    var attributeValue = new AttributeValue();
                    if (@object != null)
                        attributeValue.N = $"{@object}";
                    else
                        attributeValue.NULL = true;
                    return attributeValue;
                }
            },
            { typeof(long), (object @object) => new AttributeValue { N = $"{@object}" } },
            { typeof(long?), (object @object) =>
                {
                    var attributeValue = new AttributeValue();
                    if (@object != null)
                        attributeValue.N = $"{@object}";
                    else
                        attributeValue.NULL = true;
                    return attributeValue;
                }
            },
            { typeof(string), (object @object) => new AttributeValue($"{@object}") },
            { typeof(ushort), (object @object) => new AttributeValue{ N = $"{@object}" } },
            { typeof(ushort?), (object @object) =>
                {
                    var attributeValue = new AttributeValue();
                    if (@object != null)
                        attributeValue.N = $"{@object}";
                    else
                        attributeValue.NULL = true;
                    return attributeValue;
                }
            },
            { typeof(uint), (object @object) => new AttributeValue { N = $"{@object}" } },
            { typeof(uint?), (object @object) =>
                {
                    var attributeValue = new AttributeValue();
                    if (@object != null)
                        attributeValue.N = $"{@object}";
                    else
                        attributeValue.NULL = true;
                    return attributeValue;
                }
            },
            { typeof(ulong), (object @object) => new AttributeValue { N = $"{@object}" } },
            { typeof(ulong?), (object @object) =>
                {
                    var attributeValue = new AttributeValue();
                    if (@object != null)
                        attributeValue.N = $"{@object}";
                    else
                        attributeValue.NULL = true;
                    return attributeValue;
                }
            },
        };

        internal static IDictionary<Type, Func<AttributeValue, object>> ConvertToValue = new Dictionary<Type, Func<AttributeValue, object>>
        {
            { typeof(bool), (AttributeValue attributeValue) => attributeValue.BOOL },
            { typeof(bool?), (AttributeValue attributeValue) =>
                {
                    if (!attributeValue.NULL)
                        return attributeValue.BOOL;
                    return null;
                }
            },
            { typeof(byte), (AttributeValue attributeValue) => attributeValue.B.ToArray()[0] },
            { typeof(byte?), (AttributeValue attributeValue) =>
                {
                    if (!attributeValue.NULL)
                        return (byte?)attributeValue.B.ToArray()[0];
                    return null;
                }
            },
            { typeof(byte[]), (AttributeValue attributeValue) =>
                {
                    if (!attributeValue.NULL)
                        return attributeValue.B.ToArray();
                    return null;
                }
            },
            { typeof(char), (AttributeValue attributeValue) => attributeValue.S[0] },
            { typeof(char?), (AttributeValue attributeValue) =>
                {
                    if (!attributeValue.NULL)
                        return (char?)attributeValue.S[0];
                    return null;
                }
            },
            { typeof(DateTime), (AttributeValue attributeValue) => DateTime.ParseExact(attributeValue.S, Formats.DateFormat, CultureInfo.InvariantCulture)},
            { typeof(DateTime?), (AttributeValue attributeValue) =>
                {
                    if (!attributeValue.NULL)
                        return DateTime.ParseExact(attributeValue.S, Formats.DateFormat, CultureInfo.InvariantCulture);
                    return null;
                }
            },
            { typeof(decimal), (AttributeValue attributeValue) => decimal.Parse(attributeValue.N) },
            { typeof(decimal?), (AttributeValue attributeValue) =>
                {
                    if (!attributeValue.NULL)
                        return decimal.Parse(attributeValue.N);
                    return null;
                }
            },
            { typeof(double), (AttributeValue attributeValue) => double.Parse(attributeValue.N) },
            { typeof(double?), (AttributeValue attributeValue) =>
                {
                    if (!attributeValue.NULL)
                        return double.Parse(attributeValue.N);
                    return null;
                }
            },
            { typeof(float), (AttributeValue attributeValue) => float.Parse(attributeValue.N) },
            { typeof(float?), (AttributeValue attributeValue) =>
                {
                    if (!attributeValue.NULL)
                        return float.Parse(attributeValue.N);
                    return null;
                }
            },
            { typeof(short), (AttributeValue attributeValue) => short.Parse(attributeValue.N) },
            { typeof(short?), (AttributeValue attributeValue) =>
                {
                    if (!attributeValue.NULL)
                        return short.Parse(attributeValue.N);
                    return null;
                }
            },
            { typeof(int), (AttributeValue attributeValue) => int.Parse(attributeValue.N) },
            { typeof(int?), (AttributeValue attributeValue) =>
                {
                    if (!attributeValue.NULL)
                        return (int?)int.Parse(attributeValue.N);
                    return null;
                }
            },
            { typeof(long), (AttributeValue attributeValue) => long.Parse(attributeValue.N) },
            { typeof(long?), (AttributeValue attributeValue) =>
                {
                    if (!attributeValue.NULL)
                        return (long?)long.Parse(attributeValue.N);
                    return null;
                }
            },
            { typeof(string), (AttributeValue attributeValue) => attributeValue.S },
            { typeof(ushort), (AttributeValue attributeValue) => ushort.Parse(attributeValue.N) },
            { typeof(ushort?), (AttributeValue attributeValue) =>
                {
                    if (!attributeValue.NULL)
                        return ushort.Parse(attributeValue.N);
                    return null;
                }
            },
            { typeof(uint), (AttributeValue attributeValue) => uint.Parse(attributeValue.N) },
            { typeof(uint?), (AttributeValue attributeValue) =>
                {
                    if (!attributeValue.NULL)
                        return (uint?)uint.Parse(attributeValue.N);
                    return null;
                }
            },
            { typeof(ulong), (AttributeValue attributeValue) => ulong.Parse(attributeValue.N) },
            { typeof(ulong?), (AttributeValue attributeValue) =>
                {
                    if (!attributeValue.NULL)
                        return (ulong?)ulong.Parse(attributeValue.N);
                    return null;
                }
            },
        };
    }
}
