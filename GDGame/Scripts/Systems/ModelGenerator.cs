using GDEngine.Core.Collections;
using GDEngine.Core.Entities;
using GDEngine.Core.Factories;
using GDEngine.Core.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Scripts.Systems
{
    /// <summary>
    /// Creates Models off of given data
    /// </summary>
    public class ModelGenerator
    {
        #region Fields
        private ContentDictionary<Texture2D> _textures;
        private ContentDictionary<Model> _models;
        private Material _material;
        private GraphicsDeviceManager _graphics;
        #endregion

        #region Constructors
        public ModelGenerator(ContentDictionary<Texture2D> tex, ContentDictionary<Model> models,
            Material mat, GraphicsDeviceManager g) 
        { 
            _textures = tex;
            _models = models;
            _material = mat;
            _graphics = g;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Generate a model and add it to the scene
        /// </summary>
        /// <param name="position">Spawn Position</param>
        /// <param name="eulerRotationDegrees">Spawn Rotation</param>
        /// <param name="scale">Spawn Scale</param>
        /// <param name="textureName">Name of the Texture</param>
        /// <param name="modelName">Name of the Model</param>
        /// <param name="objectName">Name of the Game Object</param>
        /// <returns></returns>
        public GameObject GenerateModel( Vector3 position,
            Vector3 eulerRotationDegrees, Vector3 scale,
            string textureName, string modelName, string objectName)
        {
            GameObject gameObject = null;

            gameObject = new GameObject(objectName);
            gameObject.Transform.TranslateTo(position);
            gameObject.Transform.RotateEulerBy(eulerRotationDegrees * MathHelper.Pi / 180f);
            gameObject.Transform.ScaleTo(scale);

            var model = _models.Get(modelName);
            var texture = _textures.Get(textureName);
            var meshFilter = MeshFilterFactory.CreateFromModel(model, _graphics.GraphicsDevice, 0, 0);
            gameObject.AddComponent(meshFilter);

            var meshRenderer = gameObject.AddComponent<MeshRenderer>();

            meshRenderer.Material = _material;
            meshRenderer.Overrides.MainTexture = texture;

            return gameObject;
        }
        #endregion
    }
}
