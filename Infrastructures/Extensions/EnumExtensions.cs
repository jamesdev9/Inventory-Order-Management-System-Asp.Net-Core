using System.ComponentModel;

namespace Indotalent.Infrastructures.Extensions
{
    public static class EnumExtensions
    {
        public static string ValueToString(this Enum enumValue)
        {
            var enumType = enumValue.GetType();
            var enumMember = Enum.GetName(enumType, enumValue);
            if (enumMember == null)
            {
                return string.Empty;
            }
            var fieldInfo = enumType.GetField(enumMember);
            if (fieldInfo == null)
            {
                return string.Empty;
            }
            var descriptionAttribute = Attribute.GetCustomAttribute(fieldInfo, typeof(DescriptionAttribute)) as DescriptionAttribute;
            return descriptionAttribute?.Description ?? enumMember ?? string.Empty;
        }
    }
}
