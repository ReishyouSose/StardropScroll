using HarmonyLib;
using System.Reflection;
using System.Reflection.Emit;

namespace StardropScroll.Helper
{
    public static class ILHelper
    {
        /// <summary>
        /// 对实例方法用！压入this
        /// </summary>
        /// <returns></returns>
        public static CodeInstruction Instance() => new(OpCodes.Ldarg_0);
        public static CodeInstruction Call(Type type, string methodName, Type[] parameters = null, Type[] generics = null)
            => new(OpCodes.Call, AccessTools.Method(type, methodName, parameters, generics));
        public static bool Contains(this CodeInstruction code, string methodName)
            => code.operand.ToString().Contains(methodName);
        public static CodeInstruction Int(int value)
        {
            if (0 <= value && value <= 8)
            {
                return new(value switch
                {
                    0 => OpCodes.Ldc_I4_0,
                    1 => OpCodes.Ldc_I4_1,
                    2 => OpCodes.Ldc_I4_2,
                    3 => OpCodes.Ldc_I4_3,
                    4 => OpCodes.Ldc_I4_4,
                    5 => OpCodes.Ldc_I4_5,
                    6 => OpCodes.Ldc_I4_6,
                    7 => OpCodes.Ldc_I4_7,
                    8 => OpCodes.Ldc_I4_8,
                    _ => throw new Exception("Are u sure?")
                });
            }
            return new(OpCodes.Ldc_I4, value);
        }
    }
}
