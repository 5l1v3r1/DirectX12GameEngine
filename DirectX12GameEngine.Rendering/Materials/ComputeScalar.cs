﻿using DirectX12GameEngine.Core;
using DirectX12GameEngine.Graphics;
using DirectX12GameEngine.Shaders;

namespace DirectX12GameEngine.Rendering.Materials
{
    [ShaderContract]
    [ConstantBufferView]
    public class ComputeScalar : IComputeScalar
    {
        private float scalarValue;
        private GraphicsBuffer<float>? valueBuffer;

        public ComputeScalar()
        {
        }

        public ComputeScalar(float value)
        {
            Value = value;
        }

        public void Visit(MaterialGeneratorContext context)
        {
            valueBuffer ??= GraphicsBuffer.Constant.New(context.GraphicsDevice, Value).DisposeBy(context.GraphicsDevice);
            context.ConstantBufferViews.Add(valueBuffer);
        }

        #region Shader

        [ShaderMember]
        public float Value
        {
            get => scalarValue;
            set
            {
                scalarValue = value;
                valueBuffer?.SetData(scalarValue);
            }
        }

        [ShaderMember]
        public float Compute()
        {
            return Value;
        }

        #endregion
    }
}
