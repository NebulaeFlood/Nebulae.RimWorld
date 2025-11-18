using Nebulae.RimWorld.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using static Nebulae.RimWorld.UI.Core.Emit.Emission;

namespace Nebulae.RimWorld.UI.Core.Emit
{
    /// <summary>
    /// 保存 <see cref="Emission"/> 的链表
    /// </summary>
    public sealed class EmissionList : RoughLinkedListBase<Emission>
    {
        /// <summary>
        /// 初始化 <see cref="EmissionList"/> 的新实例
        /// </summary>
        public EmissionList() { }


        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Methods

        /// <summary>
        /// 移除所有保存的 <see cref="Emission"/>
        /// </summary>
        public void Clear()
        {
            head = null;
            tail = null;

            count = 0;
        }

        /// <summary>
        /// 将所有保存的 <see cref="Emission"/> 发送到 IL 指令流中
        /// </summary>
        /// <param name="il">IL 指令流生成器</param>
        public void Emit(ILGenerator il)
        {
            if (il is null)
            {
                throw new ArgumentNullException(nameof(il));
            }

            var node = head;

            while (node != null)
            {
                node.Item.Emit(il);
                node = node.Next;
            }
        }

        /// <summary>
        /// 创建 IL 指令发送记录
        /// </summary>
        /// <param name="opCode">发送的指令</param>
        public void Emit(OpCode opCode)
        {
            InsertLast(new EmitWithOpCode(opCode));
            count++;
        }

        /// <summary>
        /// 创建 IL 指令发送记录
        /// </summary>
        /// <param name="opCode">发送的指令</param>
        /// <param name="local">指令参数</param>
        public void Emit(OpCode opCode, LocalBuilder local)
        {
            InsertLast(new EmitWithLocal(opCode, local));
            count++;
        }

        /// <summary>
        /// 创建 IL 指令发送记录
        /// </summary>
        /// <param name="opCode">发送的指令</param>
        /// <param name="type">指令参数</param>
        public void Emit(OpCode opCode, Type type)
        {
            InsertLast(new EmitWithType(opCode, type));
            count++;
        }

        /// <summary>
        /// 创建 IL 指令发送记录
        /// </summary>
        /// <param name="opCode">发送的指令</param>
        /// <param name="field">指令参数</param>
        public void Emit(OpCode opCode, FieldInfo field)
        {
            InsertLast(new EmitWithField(opCode, field));
            count++;
        }

        /// <summary>
        /// 创建 IL 指令发送记录
        /// </summary>
        /// <param name="opCode">发送的指令</param>
        /// <param name="method">指令参数</param>
        public void Emit(OpCode opCode, MethodInfo method)
        {
            InsertLast(new EmitWithMethod(opCode, method));
            count++;
        }

        /// <summary>
        /// 创建 IL 指令发送记录
        /// </summary>
        /// <param name="opCode">发送的指令</param>
        /// <param name="constructor">指令参数</param>
        public void Emit(OpCode opCode, ConstructorInfo constructor)
        {
            InsertLast(new EmitWithConstructor(opCode, constructor));
            count++;
        }

        /// <summary>
        /// 创建 IL 指令发送记录
        /// </summary>
        /// <param name="opCode">发送的指令</param>
        /// <param name="label">指令参数</param>
        public void Emit(OpCode opCode, Label label)
        {
            InsertLast(new EmitWithLabel(opCode, label));
            count++;
        }

        /// <summary>
        /// 创建 IL 指令发送记录
        /// </summary>
        /// <param name="opCode">发送的指令</param>
        /// <param name="labels">指令参数</param>
        public void Emit(OpCode opCode, Label[] labels)
        {
            InsertLast(new EmitWithLabels(opCode, labels));
            count++;
        }

        /// <summary>
        /// 创建 IL 指令发送记录
        /// </summary>
        /// <param name="opCode">发送的指令</param>
        /// <param name="arg">指令参数</param>
        public void Emit(OpCode opCode, byte arg)
        {
            InsertLast(new EmitWithByte(opCode, arg));
            count++;
        }

        /// <summary>
        /// 创建 IL 指令发送记录
        /// </summary>
        /// <param name="opCode">发送的指令</param>
        /// <param name="arg">指令参数</param>
        public void Emit(OpCode opCode, sbyte arg)
        {
            InsertLast(new EmitWithSByte(opCode, arg));
            count++;
        }

        /// <summary>
        /// 创建 IL 指令发送记录
        /// </summary>
        /// <param name="opCode">发送的指令</param>
        /// <param name="arg">指令参数</param>
        public void Emit(OpCode opCode, short arg)
        {
            InsertLast(new EmitWithInt16(opCode, arg));
            count++;
        }

        /// <summary>
        /// 创建 IL 指令发送记录
        /// </summary>
        /// <param name="opCode">发送的指令</param>
        /// <param name="arg">指令参数</param>
        public void Emit(OpCode opCode, int arg)
        {
            InsertLast(new EmitWithInt32(opCode, arg));
            count++;
        }

        /// <summary>
        /// 创建 IL 指令发送记录
        /// </summary>
        /// <param name="opCode">发送的指令</param>
        /// <param name="arg">指令参数</param>
        public void Emit(OpCode opCode, long arg)
        {
            InsertLast(new EmitWithInt64(opCode, arg));
            count++;
        }

        /// <summary>
        /// 创建 IL 指令发送记录
        /// </summary>
        /// <param name="opCode">发送的指令</param>
        /// <param name="arg">指令参数</param>
        public void Emit(OpCode opCode, float arg)
        {
            InsertLast(new EmitWithSingle(opCode, arg));
            count++;
        }

        /// <summary>
        /// 创建 IL 指令发送记录
        /// </summary>
        /// <param name="opCode">发送的指令</param>
        /// <param name="arg">指令参数</param>
        public void Emit(OpCode opCode, double arg)
        {
            InsertLast(new EmitWithDouble(opCode, arg));
            count++;
        }

        /// <summary>
        /// 创建 IL 指令发送记录
        /// </summary>
        /// <param name="opCode">发送的指令</param>
        /// <param name="arg">指令参数</param>
        public void Emit(OpCode opCode, string arg)
        {
            InsertLast(new EmitWithString(opCode, arg));
            count++;
        }

        /// <summary>
        /// 创建 IL 指令发送记录
        /// </summary>
        /// <param name="opCode">发送的指令</param>
        /// <param name="signature">指令参数</param>
        public void Emit(OpCode opCode, SignatureHelper signature)
        {
            InsertLast(new EmitWithSignature(opCode, signature));
            count++;
        }

        /// <summary>
        /// 移除最后保存的 <see cref="Emission"/>
        /// </summary>
        /// <returns>移除的 <see cref="Emission"/>。</returns>
        public Emission Recall()
        {
            if (count < 1)
            {
                return null;
            }

            var emission = tail.Item;

            PickUp(tail);
            count--;

            return emission;
        }

        /// <summary>
        /// 保存一个 <see cref="Emission"/>
        /// </summary>
        /// <param name="emission">要保存的 <see cref="Emission"/></param>
        public void Save(Emission emission)
        {
            if (emission is null)
            {
                throw new ArgumentNullException(nameof(emission));
            }

            InsertLast(emission);
            count++;
        }

        /// <summary>
        /// 保存多个 <see cref="Emission"/>
        /// </summary>
        /// <param name="emissions">要保存的 <see cref="Emission"/></param>
        /// <remarks><paramref name="emissions"/> 中的 <see langword="null"/> 将被忽略。</remarks>
        public void Save(IEnumerable<Emission> emissions)
        {
            if (emissions is null)
            {
                throw new ArgumentNullException(nameof(emissions));
            }

            foreach (var emission in emissions)
            {
                if (emission is null)
                {
                    continue;
                }

                InsertLast(emission);
                count++;
            }
        }

        #endregion
    }
}
