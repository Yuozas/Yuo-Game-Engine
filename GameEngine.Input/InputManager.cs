using System.Runtime.InteropServices;

namespace GameEngine.Input;

public enum Keys
{
	Up   = 0x26,
	Down = 0x28,
	Left = 0x25,

	Right = 0x27
	// TODO: Add more keys as needed
}

public class InputManager
{
	[DllImport("user32.dll")]
	private static extern short GetAsyncKeyState(int vKey);

	public static InputState GetInput()
	{
		var state = new InputState();

		foreach (Keys key in Enum.GetValues(typeof(Keys)))
			if (IsKeyDown(key))
				state.SetKeyPressed(key);

		return state;
	}

	private static bool IsKeyDown(Keys key) => (GetAsyncKeyState((int)key) & 0x8000) != 0;
}

public class InputState
{
	private readonly HashSet<Keys> _pressedKeys = new();

	public void SetKeyPressed(Keys key) => _pressedKeys.Add(key);

	public bool IsKeyPressed(Keys key) => _pressedKeys.Contains(key);
}