﻿using System;
using System.IO;
using System.Threading.Tasks;
using DirectX12GameEngine.Serialization;
using DirectX12GameEngine.Graphics;
using Microsoft.Extensions.DependencyInjection;

namespace DirectX12GameEngine.Assets
{
    public class TextureAsset : AssetWithSource
    {
        public async override Task<object> CreateAssetAsync(IServiceProvider services)
        {
            IContentManager contentManager = services.GetRequiredService<IContentManager>();
            GraphicsDevice device = services.GetRequiredService<GraphicsDevice>();

            using Stream stream = await contentManager.FileProvider.OpenStreamAsync(Source, FileMode.Open, FileAccess.Read);
            return await Texture.LoadAsync(device, stream);
        }
    }
}
