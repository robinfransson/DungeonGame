using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace DungeonGame.Extensions;

public class CameraWrapper
{
    public static CameraWrapper Empty { get; } = new (null!);
    private readonly OrthographicCamera _camera;

    public CameraWrapper(OrthographicCamera camera)
    {
        _camera = camera;
    }

    public Matrix GetViewMatrix()
    {
        return _camera.GetViewMatrix();
    }

    public void Move(Vector2 direction)
    {
        _camera.Move(direction);
    }

    public void ZoomIn(float delta)
    {
        _camera.ZoomIn(delta);
    }
    
    public void ZoomOut(float delta)
    {
        _camera.ZoomOut(delta);
    }
    
    public void SetZoom(float delta)
    {
        _camera.Zoom = delta;
    }

    public void Rotate(float delta)
    {
        _camera.Rotate(delta);
    }

    public void LookAt(Vector2 position)
    {
        _camera.LookAt(position);
    }
}