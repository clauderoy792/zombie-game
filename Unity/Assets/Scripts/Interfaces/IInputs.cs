using UnityEngine;

public interface IInputs
{
	/// <summary>
	/// Gets mouse position in world space
	/// </summary>
	/// <returns>The world position.</returns>
	Vector2 GetWorldPosition();

	/// <summary>
	/// Gets mouse position on viewport (from 0 - 1)
	/// </summary>
	/// <returns>The viewport position.</returns>
	Vector2 GetViewportPosition();

	/// <summary>
	/// Gets mouse position on screen in pixels.
	/// </summary>
	/// <returns>The screen position.</returns>
	Vector2 GetScreenPosition();

	float GetZoomValue();
	bool IsTouchDown();
	bool IsTouch();
	bool IsTouchUp();
	bool IsTouchMoved();
	bool IsZooming();
}