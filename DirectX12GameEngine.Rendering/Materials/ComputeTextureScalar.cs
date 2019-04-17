﻿using DirectX12GameEngine.Core;
using DirectX12GameEngine.Graphics;
using DirectX12GameEngine.Rendering.Core;
using DirectX12GameEngine.Shaders;
using DirectX12GameEngine.Shaders.Numerics;

namespace DirectX12GameEngine.Rendering.Materials
{
    [StaticResource]
    public class ComputeTextureScalar : IComputeScalar
    {
        private Buffer? colorChannelBuffer;

        public ComputeTextureScalar()
        {
        }

        public ComputeTextureScalar(Texture texture, ColorChannel channel = ColorChannel.R)
        {
            Texture = texture;
            Channel = channel;
        }

        public Texture? Texture { get; set; }

        public void Visit(MaterialGeneratorContext context)
        {
            if (Texture != null)
            {
                context.Textures.Add(Texture);
            }

            colorChannelBuffer ??= Buffer.Constant.New(context.GraphicsDevice, Channel).DisposeBy(context.GraphicsDevice);
            context.ConstantBuffers.Add(colorChannelBuffer);
        }

        #region Shader

#nullable disable
        [ShaderResource] public Texture2DResource ScalarTexture;
#nullable enable

        [ConstantBufferResource] public ColorChannel Channel { get; set; } = ColorChannel.R;

        [ShaderMethod]
        public float Compute()
        {
            Vector4 color = ScalarTexture.Sample(Texturing.Sampler, Texturing.TexCoord);
            return color[(int)Channel];
        }

        #endregion
    }
}
