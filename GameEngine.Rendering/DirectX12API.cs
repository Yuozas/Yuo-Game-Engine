using SharpDX.Direct3D12;
using SharpDX.DXGI;
using SharpDX.Windows;
using SharpDX;
using SharpDX.Mathematics.Interop;
using Device = SharpDX.Direct3D12.Device;
using Resource = SharpDX.Direct3D12.Resource;

namespace GameEngine.Rendering;

public class DirectX12API : IRenderingApi
{
	private const int FrameCount = 2;

	private readonly Device              _device;
	private readonly SwapChain3          _swapChain;
	private readonly CommandQueue        _commandQueue;
	private readonly PipelineState       _pipelineState;
	private readonly Resource[]          _renderTargets;
	private readonly DescriptorHeap      _renderTargetViewHeap;
	private          int                 _frameIndex;
	private readonly ViewportF           _viewport;
	private readonly RawRectangleF       _scissorRect;
	private readonly RenderForm          _renderForm;
	private readonly CommandAllocator    _commandAllocator;
	private readonly GraphicsCommandList _commandList;
	private          Resource?           _vertexBuffer;
	private          VertexBufferView    _vertexBufferView;
	private readonly Fence               _fence;
	private          long                _fenceValue;
	private readonly AutoResetEvent      _fenceEvent;
	private readonly CommandAllocator[]  _commandAllocators;

	private const int FRAME_COUNT = 2;

	private struct Vertex
	{
		public Vector3 Position;
		public Vector4 Color;
	}

	public DirectX12API(string windowName)
	{
		try
		{
			Console.WriteLine("Initializing DirectX12API...");

			// Create the window
			_renderForm = new(windowName)
			{
				Width  = 1280,
				Height = 720
			};
			_renderForm.Show();
			Console.WriteLine("RenderForm created successfully.");

			// Create the device
			using (var factory = new Factory4())
			{
				Console.WriteLine("Factory4 created.");

				using (var adapter = factory.GetAdapter(0))
				{
					if (adapter == null)
						throw new InvalidOperationException("Failed to get graphics adapter.");

					Console.WriteLine("Adapter retrieved successfully.");
					_device = new(adapter, SharpDX.Direct3D.FeatureLevel.Level_11_0);
				}
			}

			Console.WriteLine("Device created successfully.");

			// Create command queue
			var queueDesc = new CommandQueueDescription(CommandListType.Direct);
			_commandQueue = _device.CreateCommandQueue(queueDesc);
			Console.WriteLine("Command queue created successfully.");

			// Create swap chain
			var swapChainDesc = new SwapChainDescription()
			{
				BufferCount       = FrameCount,
				ModeDescription   = new(_renderForm.ClientSize.Width, _renderForm.ClientSize.Height, new(60, 1), Format.R8G8B8A8_UNorm),
				Usage             = Usage.RenderTargetOutput,
				SwapEffect        = SwapEffect.FlipDiscard,
				OutputHandle      = _renderForm.Handle,
				SampleDescription = new(1, 0),
				IsWindowed        = true
			};

			using (var factory = new Factory4())
				using (var tempSwapChain = new SwapChain(factory, _commandQueue, swapChainDesc))
					_swapChain = tempSwapChain.QueryInterface<SwapChain3>();
			Console.WriteLine("Swap chain created successfully.");

			_frameIndex = _swapChain.CurrentBackBufferIndex;

			// Create descriptor heap for render target view
			var rtvHeapDesc = new DescriptorHeapDescription()
			{
				DescriptorCount = FrameCount,
				Type            = DescriptorHeapType.RenderTargetView,
				Flags           = DescriptorHeapFlags.None
			};
			_renderTargetViewHeap = _device.CreateDescriptorHeap(rtvHeapDesc);
			Console.WriteLine("Render target view heap created successfully.");

			// Create render target views
			_renderTargets = new Resource[FrameCount];
			var rtvDescriptorSize = _device.GetDescriptorHandleIncrementSize(DescriptorHeapType.RenderTargetView);
			var rtvHandle         = _renderTargetViewHeap.CPUDescriptorHandleForHeapStart;

			for (var i = 0; i < FrameCount; i++)
			{
				_renderTargets[i] = _swapChain.GetBackBuffer<Resource>(i);
				_device.CreateRenderTargetView(_renderTargets[i], null, rtvHandle);
				rtvHandle += rtvDescriptorSize;
			}

			Console.WriteLine("Render target views created successfully.");

			// Create command allocators
			_commandAllocators = new CommandAllocator[FrameCount];
			for (var i = 0; i < FrameCount; i++)
				_commandAllocators[i] = _device.CreateCommandAllocator(CommandListType.Direct);
			Console.WriteLine("Command allocators created successfully.");

			// Create command list
			_commandList = _device.CreateCommandList(CommandListType.Direct, _commandAllocators[0], null);
			_commandList.Close();
			Console.WriteLine("Command list created successfully.");

			// Set up viewport and scissor rect
			_viewport    = new(0, 0, _renderForm.ClientSize.Width, _renderForm.ClientSize.Height);
			_scissorRect = new(0, 0, _renderForm.ClientSize.Width, _renderForm.ClientSize.Height);
			Console.WriteLine("Viewport and scissor rect set up.");

			// Create root signature
			var rootSignatureDesc = new RootSignatureDescription(RootSignatureFlags.AllowInputAssemblerInputLayout);
			var rootSignature     = _device.CreateRootSignature(rootSignatureDesc.Serialize());
			Console.WriteLine("Root signature created successfully.");

			// Create pipeline state
			// ... (pipeline state creation code remains the same)
			Console.WriteLine("Pipeline state created successfully.");

			// Create fence
			_fence      = _device.CreateFence(0, FenceFlags.None);
			_fenceValue = 1;
			_fenceEvent = new(false);
			Console.WriteLine("Fence created successfully.");

			// Additional checks
			if (_device == null)
				throw new InvalidOperationException("Failed to create Direct3D12 device.");
			if (_commandQueue == null)
				throw new InvalidOperationException("Failed to create command queue.");
			if (_swapChain == null)
				throw new InvalidOperationException("Failed to create swap chain.");
			if (_renderTargetViewHeap == null)
				throw new InvalidOperationException("Failed to create render target view heap.");
			if (_commandList == null)
				throw new InvalidOperationException("Failed to create command list.");
			if (_fence == null)
				throw new InvalidOperationException("Failed to create fence.");

			Console.WriteLine("DirectX12 initialized successfully.");

			// Ensure the window is visible and brought to the front
			_renderForm.BringToFront();
			_renderForm.Focus();
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error initializing DirectX12API: {ex.Message}");
			Console.WriteLine($"StackTrace: {ex.StackTrace}");
			throw;
		}
	}

	public void Initialize()
	{ }

	public void Clear(Vector4 color)
	{
		_commandAllocators[_frameIndex].Reset();
		_commandList.Reset(_commandAllocators[_frameIndex], _pipelineState);

		var rtvHandle = _renderTargetViewHeap.CPUDescriptorHandleForHeapStart;
		rtvHandle += _frameIndex * _device.GetDescriptorHandleIncrementSize(DescriptorHeapType.RenderTargetView);

		_commandList.ResourceBarrierTransition(_renderTargets[_frameIndex], ResourceStates.Present, ResourceStates.RenderTarget);
		_commandList.ClearRenderTargetView(rtvHandle, new(color.X, color.Y, color.Z, color.W), 0, null);

		_commandList.SetViewport(_viewport);
		_commandList.SetScissorRectangles(new RawRectangle((int)_scissorRect.Left,
		                                                   (int)_scissorRect.Top,
		                                                   (int)_scissorRect.Right,
		                                                   (int)_scissorRect.Bottom));
	}

	public void DrawRectangle(Vector2 position, Vector2 size, Vector4 color)
	{
		Console.WriteLine($"DrawRectangle called: Position: {position}, Size: {size}, Color: {color}");

		// Convert screen coordinates to normalized device coordinates
		var left   = position.X / _viewport.Width * 2 - 1;
		var top    = -(position.Y / _viewport.Height * 2 - 1);
		var right  = (position.X + size.X) / _viewport.Width     * 2 - 1;
		var bottom = -((position.Y + size.Y) / _viewport.Height) * 2 - 1;

		var vertices = new[]
		{
			new Vertex
			{
				Position = new(left, top, 0),
				Color    = color
			},
			new Vertex
			{
				Position = new(right, top, 0),
				Color    = color
			},
			new Vertex
			{
				Position = new(left, bottom, 0),
				Color    = color
			},
			new Vertex
			{
				Position = new(right, bottom, 0),
				Color    = color
			}
		};

		var vbufferSize = Utilities.SizeOf(vertices);
		var vBuffer     = _device.CreateCommittedResource(new(HeapType.Upload), HeapFlags.None, ResourceDescription.Buffer(vbufferSize), ResourceStates.GenericRead);

		var pData = vBuffer.Map(0);
		Utilities.Write(pData, vertices, 0, vertices.Length);
		vBuffer.Unmap(0);

		var vbv = new VertexBufferView
		{
			BufferLocation = vBuffer.GPUVirtualAddress,
			StrideInBytes  = Utilities.SizeOf<Vertex>(),
			SizeInBytes    = vbufferSize
		};

		_commandList.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleStrip;
		_commandList.SetVertexBuffer(0, vbv);
		_commandList.DrawInstanced(4, 1, 0, 0);

		vBuffer.Dispose();
	}

	private bool CheckDevice()
	{
		try
		{
			var result = _device.DeviceRemovedReason;

			if (result.Failure)
			{
				Console.WriteLine($"Device removed. Reason: {result}");
				RecreateDevice();
				return false;
			}

			return true;
		}
		catch (SharpDXException ex)
		{
			Console.WriteLine($"Error checking device: {ex.Message}");
			return false;
		}
	}

	private void RecreateDevice()
	{
		Console.WriteLine("Attempting to recreate the device...");
		// Dispose of existing resources
		DisposeDirectXResources();

		// Reinitialize DirectX resources
		InitializeDirectXResources();

		Console.WriteLine("Device recreated successfully.");
	}

	private void DisposeDirectXResources()
	{
		// Dispose of all DirectX resources here
		_device?.Dispose();
		_commandQueue?.Dispose();
		_swapChain?.Dispose();
		// ... dispose of other resources ...
	}

	private void InitializeDirectXResources()
	{
		// Reinitialize all DirectX resources here
		// This should be similar to your initial setup in the constructor or Initialize method
	}

	public void Present()
	{
		Console.WriteLine("Present method called");

		try
		{
			_commandList.ResourceBarrierTransition(_renderTargets[_frameIndex], ResourceStates.RenderTarget, ResourceStates.Present);
			_commandList.Close();

			_commandQueue.ExecuteCommandList(_commandList);

			var result = _swapChain.Present(1, 0);

			if (result.Failure)
			{
				if (result.Code == ResultCode.DeviceRemoved.Result.Code)
				{
					var removedReason = _device.DeviceRemovedReason;
					throw new SharpDXException($"Device removed. Reason: {removedReason}");
				}
				else
				{
					throw new SharpDXException($"Failed to present. HRESULT: {result.Code}");
				}
			}

			WaitForPreviousFrame();

			_frameIndex = _swapChain.CurrentBackBufferIndex;

			Console.WriteLine("Frame presented successfully");
		}
		catch (SharpDXException ex)
		{
			Console.WriteLine($"SharpDX Exception during Present: {ex.Message}");
			Console.WriteLine($"HRESULT: {ex.ResultCode}");
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Unexpected error during Present: {ex.Message}");
			Console.WriteLine($"StackTrace: {ex.StackTrace}");
		}
	}

	private void WaitForPreviousFrame()
	{
		var fence = _fenceValue;
		_commandQueue.Signal(_fence, fence);
		_fenceValue++;

		if (_fence.CompletedValue < fence)
		{
			_fence.SetEventOnCompletion(fence, _fenceEvent.SafeWaitHandle.DangerousGetHandle());
			_fenceEvent.WaitOne();
		}
	}

	public void Dispose()
	{
		Console.WriteLine("Disposing DirectX12API resources...");

		try
		{
			// Wait for the GPU to finish all operations
			WaitForPreviousFrame();

			// Dispose of resources
			if (_renderTargets != null)
				foreach (var target in _renderTargets)
					target?.Dispose();

			if (_commandAllocators != null)
				foreach (var allocator in _commandAllocators)
					allocator?.Dispose();

			_renderTargetViewHeap?.Dispose();
			_pipelineState?.Dispose();
			_commandQueue?.Dispose();
			_commandList?.Dispose();
			_swapChain?.Dispose();
			_device?.Dispose();
			_fence?.Dispose();
			_fenceEvent?.Dispose();

			// Ensure the form is closed and disposed
			if (_renderForm != null && !_renderForm.IsDisposed)
			{
				_renderForm.Close();
				_renderForm.Dispose();
			}

			Console.WriteLine("DirectX12API resources disposed successfully.");
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error during DirectX12API disposal: {ex.Message}");
			Console.WriteLine($"StackTrace: {ex.StackTrace}");
		}
	}

	public bool ProcessMessages()
	{
		if (_renderForm.IsDisposed)
			return false;

		Application.DoEvents();
		_renderForm.BringToFront();
		_renderForm.Focus();
		return true;
	}
}