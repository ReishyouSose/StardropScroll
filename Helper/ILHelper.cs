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

        /// <summary>
        /// <see cref="Type.GetMethod(string)"/> 注意默认只寻找公有方法
        /// <br/><see cref="AccessTools.Method(Type, string, Type[], Type[])"/> 推荐用这个
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public static CodeInstruction Call(MethodInfo method) => new(OpCodes.Call, method);

        /// <summary>
        /// <see cref="AccessTools.Method(Type, string, Type[], Type[])"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="methodName"></param>
        /// <returns></returns>
        public static CodeInstruction Call<T>(string methodName, Type[] parameters = null, Type[] generics = null)
            => new(OpCodes.Call, AccessTools.Method(typeof(T), methodName, parameters, generics));
        public static bool MatchMethod(this CodeInstruction code, string methodName)
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
