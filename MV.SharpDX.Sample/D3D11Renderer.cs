/*

MIT License

Media Vault Library DotNet 

Copyright (C) 2020 Jakub Gluszkiewicz (kubabrt@gmail.com)

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

*/

using System;
using System.Runtime.InteropServices;

using SharpDX.DXGI;
using SharpDX.Direct3D11;
using SharpDX.Direct3D;
using SharpDX.D3DCompiler;

using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;
using DXGIDevice = SharpDX.DXGI.Device1;
using Matrix = SharpDX.Matrix;
using ViewPort = SharpDX.Viewport;
using Color = SharpDX.Color;

namespace MV.SharpDX.Sample
{
    public class D3D11Renderer
    {
        private Factory _dxgiFactory = null;
        private Adapter _dxiAdapter = null;
        private Device _d3d11Device = null;
        private DXGIDevice _dxgiDevice = null;
        private VertexShader _vertexShader = null;
        private PixelShader _pixelShader = null;
        private Buffer _vertexShaderConstans = null;
        private SamplerState _samplerState = null;
        private RasterizerState _rasterizerState = null;
        private InputLayout _inputLayout = null;
        private SwapChain _swapChain = null;
        private RenderTargetView _mainRenderTargerView = null;
        private IntPtr _vertexShaderConstansData;
        private Texture2D _sharedTexture = null;
        private ShaderResourceView _shaderResource = null;
        private IntPtr _sharedHandle = IntPtr.Zero;

        [StructLayout(LayoutKind.Sequential)]
        public struct VertexShaderConstants
        {
            public Matrix model;
            public Matrix projectionAndView;
        }

        public D3D11Renderer()
        {

        }

        public void CloseD3D11()
        {


            CloseSharedResource();

            if (_mainRenderTargerView != null)
            {
                _mainRenderTargerView.Dispose();
                _mainRenderTargerView = null;

            }

            if (_vertexShaderConstansData != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(_vertexShaderConstansData);
                _vertexShaderConstansData = IntPtr.Zero;
            }


            if (_swapChain != null)
            {
                _swapChain.Dispose();
                _swapChain = null;
            }

            if (_dxgiFactory != null)
            {
                _dxgiFactory.Dispose();
                _dxgiFactory = null;

            }

            if (_dxiAdapter != null)
            {
                _dxiAdapter.Dispose();
                _dxiAdapter = null;
            }

            if (_d3d11Device != null)
            {
                _d3d11Device.Dispose();
                _d3d11Device = null;
            }

            if (_dxgiDevice != null)
            {
                _dxgiDevice.Dispose();
                _dxgiDevice = null;
            }

            if (_vertexShader != null)
            {
                _vertexShader.Dispose();
                _vertexShader = null;
            }

            if (_pixelShader != null)
            {
                _pixelShader.Dispose();
                _pixelShader = null;
            }

            if (_vertexShaderConstans != null)
            {
                _vertexShaderConstans.Dispose();
                _vertexShaderConstans = null;
            }

            if (_samplerState != null)
            {
                _samplerState.Dispose();
                _samplerState = null;
            }

            if (_rasterizerState != null)
            {
                _rasterizerState.Dispose();
                _rasterizerState = null;
            }

            if (_inputLayout != null)
            {
                _inputLayout.Dispose();
                _inputLayout = null;
            }
        }

        public void CloseSharedResource()
        {
            _sharedHandle = IntPtr.Zero;

            if (_shaderResource != null)
            {
                _shaderResource.Dispose();
                _shaderResource = null;
            }

            if (_sharedTexture != null)
            {
                _sharedTexture.Dispose();
                _sharedTexture = null;
            }
        }

        public void InitializeD3D11(IntPtr wndHandle, int width, int height)
        {
            CloseD3D11();

            _dxgiFactory = new Factory1();

            _dxiAdapter = _dxgiFactory.Adapters[0];

            _d3d11Device = new Device(_dxiAdapter, DeviceCreationFlags.BgraSupport, FeatureLevel.Level_11_0);

            _dxgiDevice = _d3d11Device.QueryInterface<DXGIDevice>();

            _dxgiDevice.MaximumFrameLatency = 1;

            // Compile Vertex and Pixel shaders
            var vertexShaderByteCode = ShaderBytecode.CompileFromFile("VSShader.fx", "main", "vs_4_0", ShaderFlags.None, EffectFlags.None);
            _vertexShader = new VertexShader(_d3d11Device, vertexShaderByteCode);

            var pixelShaderByteCode = ShaderBytecode.CompileFromFile("PSShader.fx", "main", "ps_4_0", ShaderFlags.None, EffectFlags.None);
            _pixelShader = new PixelShader(_d3d11Device, pixelShaderByteCode);

            InputElement[] inputElements = new InputElement[3];

            inputElements[0] = new InputElement("POSITION", 0, Format.R32G32B32_Float, 0, 0, InputClassification.PerVertexData, 0);
            inputElements[1] = new InputElement("TEXCOORD", 0, Format.R32G32_Float, 12, 0, InputClassification.PerVertexData, 0);
            inputElements[2] = new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 20, 0, InputClassification.PerVertexData, 0);

            _inputLayout = new InputLayout(_d3d11Device, vertexShaderByteCode, inputElements);

            BufferDescription vertexShaderDesc = new BufferDescription(Matrix.SizeInBytes * 2, BindFlags.ConstantBuffer, ResourceUsage.Default);

            _vertexShaderConstans = new Buffer(_d3d11Device, vertexShaderDesc);

            SamplerStateDescription samplerStateDescription = new SamplerStateDescription();

            samplerStateDescription.Filter = Filter.MinMagMipLinear;
            samplerStateDescription.AddressU = TextureAddressMode.Clamp;
            samplerStateDescription.AddressV = TextureAddressMode.Clamp;
            samplerStateDescription.AddressW = TextureAddressMode.Clamp;
            samplerStateDescription.MipLodBias = 0.0f;
            samplerStateDescription.MaximumAnisotropy = 1;
            samplerStateDescription.ComparisonFunction = Comparison.Always;
            samplerStateDescription.MinimumLod = 0.0f;
            samplerStateDescription.MaximumLod = float.MaxValue;

            _samplerState = new SamplerState(_d3d11Device, samplerStateDescription);

            RasterizerStateDescription rasterizerStateDescription = new RasterizerStateDescription();

            rasterizerStateDescription.IsAntialiasedLineEnabled = false;
            rasterizerStateDescription.CullMode = CullMode.None;
            rasterizerStateDescription.DepthBias = 0;
            rasterizerStateDescription.DepthBiasClamp = 0.0f;
            rasterizerStateDescription.IsDepthClipEnabled = true;
            rasterizerStateDescription.FillMode = FillMode.Solid;
            rasterizerStateDescription.IsFrontCounterClockwise = false;
            rasterizerStateDescription.IsMultisampleEnabled = false;
            rasterizerStateDescription.IsScissorEnabled = false;
            rasterizerStateDescription.SlopeScaledDepthBias = 0.0f;

            _rasterizerState = new RasterizerState(_d3d11Device, rasterizerStateDescription);

            _d3d11Device.ImmediateContext.InputAssembler.InputLayout = _inputLayout;
            _d3d11Device.ImmediateContext.VertexShader.SetShader(_vertexShader, null, 0);
            _d3d11Device.ImmediateContext.VertexShader.SetConstantBuffers(0, 1, _vertexShaderConstans);

            SwapChainDescription swapChainDescription = new SwapChainDescription();

            swapChainDescription.ModeDescription.Width = width;
            swapChainDescription.ModeDescription.Height = height;
            swapChainDescription.ModeDescription.Format = Format.B8G8R8A8_UNorm;
            swapChainDescription.ModeDescription.RefreshRate.Numerator = 1;

            //pretty ugly
            //its better to autodetect screen refresh rate
            swapChainDescription.ModeDescription.RefreshRate.Denominator = 60;

            swapChainDescription.SampleDescription.Count = 1;
            swapChainDescription.SampleDescription.Quality = 0;
            swapChainDescription.Usage = Usage.RenderTargetOutput;
            swapChainDescription.BufferCount = 2;
            swapChainDescription.ModeDescription.Scaling = DisplayModeScaling.Unspecified;
            swapChainDescription.SwapEffect = SwapEffect.FlipSequential;
            swapChainDescription.Flags = 0;
            swapChainDescription.IsWindowed = true;
            swapChainDescription.OutputHandle = wndHandle;

            _swapChain = new SwapChain(_dxgiFactory, _d3d11Device, swapChainDescription);

            _dxgiFactory.MakeWindowAssociation(wndHandle, WindowAssociationFlags.IgnoreAll);

            Texture2D backBuffer = _swapChain.GetBackBuffer<Texture2D>(0);

            _mainRenderTargerView = new RenderTargetView(_d3d11Device, backBuffer);

            backBuffer.Dispose();
            backBuffer = null;

            Matrix projection = Matrix.Identity;

            Matrix view = new Matrix();

            /* Update the view matrix */
            view[0, 0] = 2.0f / (float)width;
            view[0, 1] = 0.0f;
            view[0, 2] = 0.0f;
            view[0, 3] = 0.0f;
            view[1, 0] = 0.0f;
            view[1, 1] = -2.0f / (float)height;
            view[1, 2] = 0.0f;
            view[1, 3] = 0.0f;
            view[2, 0] = 0.0f;
            view[2, 1] = 0.0f;
            view[2, 2] = 1.0f;
            view[2, 3] = 0.0f;
            view[3, 0] = -1.0f;
            view[3, 1] = 1.0f;
            view[3, 2] = 0.0f;
            view[3, 3] = 1.0f;

            VertexShaderConstants vertexShaderConstansData = new VertexShaderConstants();

            vertexShaderConstansData.projectionAndView = Matrix.Multiply(view, projection);
            vertexShaderConstansData.model = Matrix.Identity;

            _vertexShaderConstansData =  Marshal.AllocHGlobal(Marshal.SizeOf(vertexShaderConstansData));

            Marshal.StructureToPtr(vertexShaderConstansData, _vertexShaderConstansData, false);

            _d3d11Device.ImmediateContext.UpdateSubresource(ref vertexShaderConstansData, _vertexShaderConstans);
            

            ViewPort viewPort = new ViewPort();

            viewPort.X = 0;
            viewPort.Y = 0;
            viewPort.Width = width;
            viewPort.Height = height;
            viewPort.MinDepth = 0.0f;
            viewPort.MaxDepth = 1.0f;

            _d3d11Device.ImmediateContext.Rasterizer.SetViewport(viewPort);


            float minu, maxu, minv, maxv;
            
            minu = 0.0f;
            maxu = 1.0f;
            minv = 0.0f;
            maxv = 1.0f;

            
            // Instantiate Vertex buiffer from vertex data
            var vertices = Buffer.Create(_d3d11Device, BindFlags.VertexBuffer, new[]
                                  {
                                      //ul
                                      0.0f, 0.0f, 0.0f, minu, minv, 1.0f, 1.0f, 1.0f, 1.0f,
                                      //dl
                                      0.0f, (float)height, 0.0f, minu, maxv, 1.0f, 1.0f, 1.0f, 1.0f,
                                      //ur
                                      (float) width, 0.0f, 0.0f, maxu, minv, 1.0f, 1.0f, 1.0f, 1.0f,
                                      //dr
                                      (float) width, (float)height, 0.0f, maxu, maxv,1.0f, 1.0f, 1.0f, 1.0f

                                  });

            _d3d11Device.ImmediateContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(vertices, 36, 0));
            _d3d11Device.ImmediateContext.Rasterizer.State = _rasterizerState;
            _d3d11Device.ImmediateContext.PixelShader.SetShader(_pixelShader, null, 0);
            _d3d11Device.ImmediateContext.PixelShader.SetSamplers(0, 1, _samplerState);
            _d3d11Device.ImmediateContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleStrip;

        }

        public void OpenSharedResource(IntPtr sharedHandle)
        {
            CloseSharedResource();

            if (sharedHandle == IntPtr.Zero)
                return;

            if (_d3d11Device == null)
                return;

            _sharedHandle = sharedHandle;

            _sharedTexture = _d3d11Device.OpenSharedResource<Texture2D>(sharedHandle);

            if (_sharedTexture == null)
                return;

            //check if we have shared texture
            if (!_sharedTexture.Description.BindFlags.HasFlag(BindFlags.ShaderResource))
                return;

            ShaderResourceViewDescription shaderResourceViewDescription = new ShaderResourceViewDescription();

            shaderResourceViewDescription.Format = _sharedTexture.Description.Format;
            shaderResourceViewDescription.Dimension = ShaderResourceViewDimension.Texture2D;
            shaderResourceViewDescription.Texture2D.MostDetailedMip = 0;
            shaderResourceViewDescription.Texture2D.MipLevels = 1;

            _shaderResource = new ShaderResourceView(_d3d11Device, _sharedTexture, shaderResourceViewDescription);

            
            if (_shaderResource == null)
                return;
            
            _d3d11Device.ImmediateContext.PixelShader.SetShaderResources(0, 1, _shaderResource);

        }

        public IntPtr GetSharedHandle()
        {
            return _sharedHandle;
        }

        public void Clear()
        {
            if (_d3d11Device == null || _mainRenderTargerView == null)
                return;

            _d3d11Device.ImmediateContext.OutputMerger.SetRenderTargets(_mainRenderTargerView);
            
            _d3d11Device.ImmediateContext.ClearRenderTargetView(_mainRenderTargerView, Color.CornflowerBlue);
        }

        public void Render()
        {
            if (_shaderResource == null)
                  return;

            if (_d3d11Device == null)
                return;

            _d3d11Device.ImmediateContext.Draw(4, 0);
            
        }

        public void Present()
        {
            if (_swapChain == null)
                return;

            _swapChain.Present(1, 0);
        }
    }
}
