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
    }
}
