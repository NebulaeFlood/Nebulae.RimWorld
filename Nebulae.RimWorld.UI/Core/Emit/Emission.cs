using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Nebulae.RimWorld.UI.Core.Emit
{
    /// <summary>
    /// 将 IL 指令发送到指令流的记录
    /// </summary>
    public abstract partial class Emission
    {
        /// <summary>
        /// 发送的 <see cref="System.Reflection.Emit.OpCode"/>
        /// </summary>
        public readonly OpCode OpCode;


        private Emission(OpCode opCode)
        {
            OpCode = opCode;
        }


        //------------------------------------------------------
        //
        //  Public Static Mehtods
        //
        //------------------------------------------------------

        #region Public Static Mehtods

        /// <summary>
        /// 创建 IL 指令发送记录
        /// </summary>
        /// <param name="opCode">发送的指令</param>
        /// <returns>记录发送操作的 <see cref="Emission"/>。</returns>
        public static Emission Emit(OpCode opCode)
        {
            return new EmitWithOpCode(opCode);
        }

        /// <summary>
        /// 创建 IL 指令发送记录
        /// </summary>
        /// <param name="opCode">发送的指令</param>
        /// <param name="local">指令参数</param>
        /// <returns>记录发送操作的 <see cref="Emission"/>。</returns>
        public static Emission Emit(OpCode opCode, LocalBuilder local)
        {
            return new EmitWithLocal(opCode, local);
        }

        /// <summary>
        /// 创建 IL 指令发送记录
        /// </summary>
        /// <param name="opCode">发送的指令</param>
        /// <param name="type">指令参数</param>
        /// <returns>记录发送操作的 <see cref="Emission"/>。</returns>
        public static Emission Emit(OpCode opCode, Type type)
        {
            return new EmitWithType(opCode, type);
        }

        /// <summary>
        /// 创建 IL 指令发送记录
        /// </summary>
        /// <param name="opCode">发送的指令</param>
        /// <param name="field">指令参数</param>
        /// <returns>记录发送操作的 <see cref="Emission"/>。</returns>
        public static Emission Emit(OpCode opCode, FieldInfo field)
        {
            return new EmitWithField(opCode, field);
        }

        /// <summary>
        /// 创建 IL 指令发送记录
        /// </summary>
        /// <param name="opCode">发送的指令</param>
        /// <param name="method">指令参数</param>
        /// <returns>记录发送操作的 <see cref="Emission"/>。</returns>
        public static Emission Emit(OpCode opCode, MethodInfo method)
        {
            return new EmitWithMethod(opCode, method);
        }

        /// <summary>
        /// 创建 IL 指令发送记录
        /// </summary>
        /// <param name="opCode">发送的指令</param>
        /// <param name="constructor">指令参数</param>
        /// <returns>记录发送操作的 <see cref="Emission"/>。</returns>
        public static Emission Emit(OpCode opCode, ConstructorInfo constructor)
        {
            return new EmitWithConstructor(opCode, constructor);
        }

        /// <summary>
        /// 创建 IL 指令发送记录
        /// </summary>
        /// <param name="opCode">发送的指令</param>
        /// <param name="label">指令参数</param>
        /// <returns>记录发送操作的 <see cref="Emission"/>。</returns>
        public static Emission Emit(OpCode opCode, Label label)
        {
            return new EmitWithLabel(opCode, label);
        }

        /// <summary>
        /// 创建 IL 指令发送记录
        /// </summary>
        /// <param name="opCode">发送的指令</param>
        /// <param name="labels">指令参数</param>
        /// <returns>记录发送操作的 <see cref="Emission"/>。</returns>
        public static Emission Emit(OpCode opCode, Label[] labels)
        {
            return new EmitWithLabels(opCode, labels);
        }

        /// <summary>
        /// 创建 IL 指令发送记录
        /// </summary>
        /// <param name="opCode">发送的指令</param>
        /// <param name="arg">指令参数</param>
        /// <returns>记录发送操作的 <see cref="Emission"/>。</returns>
        public static Emission Emit(OpCode opCode, byte arg)
        {
            return new EmitWithByte(opCode, arg);
        }

        /// <summary>
        /// 创建 IL 指令发送记录
        /// </summary>
        /// <param name="opCode">发送的指令</param>
        /// <param name="arg">指令参数</param>
        /// <returns>记录发送操作的 <see cref="Emission"/>。</returns>
        public static Emission Emit(OpCode opCode, sbyte arg)
        {
            return new EmitWithSByte(opCode, arg);
        }

        /// <summary>
        /// 创建 IL 指令发送记录
        /// </summary>
        /// <param name="opCode">发送的指令</param>
        /// <param name="arg">指令参数</param>
        /// <returns>记录发送操作的 <see cref="Emission"/>。</returns>
        public static Emission Emit(OpCode opCode, short arg)
        {
            return new EmitWithInt16(opCode, arg);
        }

        /// <summary>
        /// 创建 IL 指令发送记录
        /// </summary>
        /// <param name="opCode">发送的指令</param>
        /// <param name="arg">指令参数</param>
        /// <returns>记录发送操作的 <see cref="Emission"/>。</returns>
        public static Emission Emit(OpCode opCode, int arg)
        {
            return new EmitWithInt32(opCode, arg);
        }

        /// <summary>
        /// 创建 IL 指令发送记录
        /// </summary>
        /// <param name="opCode">发送的指令</param>
        /// <param name="arg">指令参数</param>
        /// <returns>记录发送操作的 <see cref="Emission"/>。</returns>
        public static Emission Emit(OpCode opCode, long arg)
        {
            return new EmitWithInt64(opCode, arg);
        }

        /// <summary>
        /// 创建 IL 指令发送记录
        /// </summary>
        /// <param name="opCode">发送的指令</param>
        /// <param name="arg">指令参数</param>
        /// <returns>记录发送操作的 <see cref="Emission"/>。</returns>
        public static Emission Emit(OpCode opCode, float arg)
        {
            return new EmitWithSingle(opCode, arg);
        }

        /// <summary>
        /// 创建 IL 指令发送记录
        /// </summary>
        /// <param name="opCode">发送的指令</param>
        /// <param name="arg">指令参数</param>
        /// <returns>记录发送操作的 <see cref="Emission"/>。</returns>
        public static Emission Emit(OpCode opCode, double arg)
        {
            return new EmitWithDouble(opCode, arg);
        }

        /// <summary>
        /// 创建 IL 指令发送记录
        /// </summary>
        /// <param name="opCode">发送的指令</param>
        /// <param name="arg">指令参数</param>
        /// <returns>记录发送操作的 <see cref="Emission"/>。</returns>
        public static Emission Emit(OpCode opCode, string arg)
        {
            return new EmitWithString(opCode, arg);
        }

        /// <summary>
        /// 创建 IL 指令发送记录
        /// </summary>
        /// <param name="opCode">发送的指令</param>
        /// <param name="signature">指令参数</param>
        /// <returns>记录发送操作的 <see cref="Emission"/>。</returns>
        public static Emission Emit(OpCode opCode, SignatureHelper signature)
        {
            return new EmitWithSignature(opCode, signature);
        }

        #endregion


        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Methods

        /// <summary>
        /// 将保存的 IL 指令发送到指令流
        /// </summary>
        /// <param name="generator">IL 指令流生成器</param>
        public abstract void Emit(ILGenerator generator);

        /// <summary>
        /// 判断指定对象是否等于当前对象
        /// </summary>
        /// <param name="obj">要比较的对象</param>
        /// <returns>若指定的对象等于当前对象，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public override abstract bool Equals(object obj);

        /// <summary>
        /// 获取当前对象的哈希代码
        /// </summary>
        /// <returns>当前对象的哈希代码。</returns>
        public override abstract int GetHashCode();

        /// <summary>
        /// 获取表示当前对象的字符串
        /// </summary>
        /// <returns>表示当前对象的字符串。</returns>
        public override abstract string ToString();

        #endregion
    }
}
