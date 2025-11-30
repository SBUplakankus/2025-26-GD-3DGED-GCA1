using GDEngine.Core.Components;
using GDEngine.Core.Entities;
using Microsoft.Xna.Framework;

namespace GDGame.Scripts.Player
{
    /// <summary>
    /// Controls the player camera movement.
    /// Created in <see cref="PlayerController"/>.
    /// </summary>
    public class PlayerCamera
    {
        #region Fields
        private Camera _camera;
        private float _cameraFOV = 0.9f;
        private readonly Vector3 _startPos = new(0, 5, 0);
        private const int FAR_PLANE_LIMIT = 1000;
        #endregion

        #region Constructors
        public PlayerCamera(GameObject parent, float aspectRatio)
        {
            _camera = parent.AddComponent<Camera>();
            _camera.FarPlane = FAR_PLANE_LIMIT;
            _camera.AspectRatio = aspectRatio;
            _camera.FieldOfView = _cameraFOV;

            parent.AddComponent<MouseYawPitchController>();
        }
        #endregion

        #region Accessors
        public Camera Cam => _camera;
        #endregion

        #region Methods

        #endregion

    }
}
