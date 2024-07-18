extern "C" void RegisterStaticallyLinkedModulesGranular()
{
	void RegisterModule_SharedInternals();
	RegisterModule_SharedInternals();

	void RegisterModule_Core();
	RegisterModule_Core();

	void RegisterModule_Audio();
	RegisterModule_Audio();

	void RegisterModule_InputLegacy();
	RegisterModule_InputLegacy();

	void RegisterModule_IMGUI();
	RegisterModule_IMGUI();

	void RegisterModule_JSONSerialize();
	RegisterModule_JSONSerialize();

	void RegisterModule_Physics();
	RegisterModule_Physics();

	void RegisterModule_RuntimeInitializeOnLoadManagerInitializer();
	RegisterModule_RuntimeInitializeOnLoadManagerInitializer();

	void RegisterModule_TextRendering();
	RegisterModule_TextRendering();

	void RegisterModule_TextCoreFontEngine();
	RegisterModule_TextCoreFontEngine();

	void RegisterModule_TextCoreTextEngine();
	RegisterModule_TextCoreTextEngine();

	void RegisterModule_WebGL();
	RegisterModule_WebGL();

}

template <typename T> void RegisterUnityClass(const char*);
template <typename T> void RegisterStrippedType(int, const char*, const char*);

void InvokeRegisterStaticallyLinkedModuleClasses()
{
	// Do nothing (we're in stripping mode)
}

class AudioBehaviour; template <> void RegisterUnityClass<AudioBehaviour>(const char*);
class AudioClip; template <> void RegisterUnityClass<AudioClip>(const char*);
class AudioListener; template <> void RegisterUnityClass<AudioListener>(const char*);
class AudioManager; template <> void RegisterUnityClass<AudioManager>(const char*);
class AudioSource; template <> void RegisterUnityClass<AudioSource>(const char*);
class SampleClip; template <> void RegisterUnityClass<SampleClip>(const char*);
class Behaviour; template <> void RegisterUnityClass<Behaviour>(const char*);
class BuildSettings; template <> void RegisterUnityClass<BuildSettings>(const char*);
class Camera; template <> void RegisterUnityClass<Camera>(const char*);
namespace Unity { class Component; } template <> void RegisterUnityClass<Unity::Component>(const char*);
class ComputeShader; template <> void RegisterUnityClass<ComputeShader>(const char*);
class Cubemap; template <> void RegisterUnityClass<Cubemap>(const char*);
class CubemapArray; template <> void RegisterUnityClass<CubemapArray>(const char*);
class DelayedCallManager; template <> void RegisterUnityClass<DelayedCallManager>(const char*);
class EditorExtension; template <> void RegisterUnityClass<EditorExtension>(const char*);
class FlareLayer; template <> void RegisterUnityClass<FlareLayer>(const char*);
class GameManager; template <> void RegisterUnityClass<GameManager>(const char*);
class GameObject; template <> void RegisterUnityClass<GameObject>(const char*);
class GlobalGameManager; template <> void RegisterUnityClass<GlobalGameManager>(const char*);
class GraphicsSettings; template <> void RegisterUnityClass<GraphicsSettings>(const char*);
class InputManager; template <> void RegisterUnityClass<InputManager>(const char*);
class LevelGameManager; template <> void RegisterUnityClass<LevelGameManager>(const char*);
class Light; template <> void RegisterUnityClass<Light>(const char*);
class LightProbes; template <> void RegisterUnityClass<LightProbes>(const char*);
class LightingSettings; template <> void RegisterUnityClass<LightingSettings>(const char*);
class LightmapSettings; template <> void RegisterUnityClass<LightmapSettings>(const char*);
class LowerResBlitTexture; template <> void RegisterUnityClass<LowerResBlitTexture>(const char*);
class Material; template <> void RegisterUnityClass<Material>(const char*);
class Mesh; template <> void RegisterUnityClass<Mesh>(const char*);
class MeshFilter; template <> void RegisterUnityClass<MeshFilter>(const char*);
class MeshRenderer; template <> void RegisterUnityClass<MeshRenderer>(const char*);
class MonoBehaviour; template <> void RegisterUnityClass<MonoBehaviour>(const char*);
class MonoManager; template <> void RegisterUnityClass<MonoManager>(const char*);
class MonoScript; template <> void RegisterUnityClass<MonoScript>(const char*);
class NamedObject; template <> void RegisterUnityClass<NamedObject>(const char*);
class Object; template <> void RegisterUnityClass<Object>(const char*);
class PlayerSettings; template <> void RegisterUnityClass<PlayerSettings>(const char*);
class PreloadData; template <> void RegisterUnityClass<PreloadData>(const char*);
class QualitySettings; template <> void RegisterUnityClass<QualitySettings>(const char*);
namespace UI { class RectTransform; } template <> void RegisterUnityClass<UI::RectTransform>(const char*);
class ReflectionProbe; template <> void RegisterUnityClass<ReflectionProbe>(const char*);
class RenderSettings; template <> void RegisterUnityClass<RenderSettings>(const char*);
class RenderTexture; template <> void RegisterUnityClass<RenderTexture>(const char*);
class Renderer; template <> void RegisterUnityClass<Renderer>(const char*);
class ResourceManager; template <> void RegisterUnityClass<ResourceManager>(const char*);
class RuntimeInitializeOnLoadManager; template <> void RegisterUnityClass<RuntimeInitializeOnLoadManager>(const char*);
class Shader; template <> void RegisterUnityClass<Shader>(const char*);
class ShaderNameRegistry; template <> void RegisterUnityClass<ShaderNameRegistry>(const char*);
class Sprite; template <> void RegisterUnityClass<Sprite>(const char*);
class SpriteRenderer; template <> void RegisterUnityClass<SpriteRenderer>(const char*);
class TagManager; template <> void RegisterUnityClass<TagManager>(const char*);
class TextAsset; template <> void RegisterUnityClass<TextAsset>(const char*);
class Texture; template <> void RegisterUnityClass<Texture>(const char*);
class Texture2D; template <> void RegisterUnityClass<Texture2D>(const char*);
class Texture2DArray; template <> void RegisterUnityClass<Texture2DArray>(const char*);
class Texture3D; template <> void RegisterUnityClass<Texture3D>(const char*);
class TimeManager; template <> void RegisterUnityClass<TimeManager>(const char*);
class TrailRenderer; template <> void RegisterUnityClass<TrailRenderer>(const char*);
class Transform; template <> void RegisterUnityClass<Transform>(const char*);
class BoxCollider; template <> void RegisterUnityClass<BoxCollider>(const char*);
class Collider; template <> void RegisterUnityClass<Collider>(const char*);
class MeshCollider; template <> void RegisterUnityClass<MeshCollider>(const char*);
class PhysicMaterial; template <> void RegisterUnityClass<PhysicMaterial>(const char*);
class PhysicsManager; template <> void RegisterUnityClass<PhysicsManager>(const char*);
class Rigidbody; template <> void RegisterUnityClass<Rigidbody>(const char*);
class SphereCollider; template <> void RegisterUnityClass<SphereCollider>(const char*);
namespace TextRendering { class Font; } template <> void RegisterUnityClass<TextRendering::Font>(const char*);
namespace TextRenderingPrivate { class TextMesh; } template <> void RegisterUnityClass<TextRenderingPrivate::TextMesh>(const char*);

void RegisterAllClasses()
{
void RegisterBuiltinTypes();
RegisterBuiltinTypes();
	//Total: 68 non stripped classes
	//0. AudioBehaviour
	RegisterUnityClass<AudioBehaviour>("Audio");
	//1. AudioClip
	RegisterUnityClass<AudioClip>("Audio");
	//2. AudioListener
	RegisterUnityClass<AudioListener>("Audio");
	//3. AudioManager
	RegisterUnityClass<AudioManager>("Audio");
	//4. AudioSource
	RegisterUnityClass<AudioSource>("Audio");
	//5. SampleClip
	RegisterUnityClass<SampleClip>("Audio");
	//6. Behaviour
	RegisterUnityClass<Behaviour>("Core");
	//7. BuildSettings
	RegisterUnityClass<BuildSettings>("Core");
	//8. Camera
	RegisterUnityClass<Camera>("Core");
	//9. Component
	RegisterUnityClass<Unity::Component>("Core");
	//10. ComputeShader
	RegisterUnityClass<ComputeShader>("Core");
	//11. Cubemap
	RegisterUnityClass<Cubemap>("Core");
	//12. CubemapArray
	RegisterUnityClass<CubemapArray>("Core");
	//13. DelayedCallManager
	RegisterUnityClass<DelayedCallManager>("Core");
	//14. EditorExtension
	RegisterUnityClass<EditorExtension>("Core");
	//15. FlareLayer
	RegisterUnityClass<FlareLayer>("Core");
	//16. GameManager
	RegisterUnityClass<GameManager>("Core");
	//17. GameObject
	RegisterUnityClass<GameObject>("Core");
	//18. GlobalGameManager
	RegisterUnityClass<GlobalGameManager>("Core");
	//19. GraphicsSettings
	RegisterUnityClass<GraphicsSettings>("Core");
	//20. InputManager
	RegisterUnityClass<InputManager>("Core");
	//21. LevelGameManager
	RegisterUnityClass<LevelGameManager>("Core");
	//22. Light
	RegisterUnityClass<Light>("Core");
	//23. LightProbes
	RegisterUnityClass<LightProbes>("Core");
	//24. LightingSettings
	RegisterUnityClass<LightingSettings>("Core");
	//25. LightmapSettings
	RegisterUnityClass<LightmapSettings>("Core");
	//26. LowerResBlitTexture
	RegisterUnityClass<LowerResBlitTexture>("Core");
	//27. Material
	RegisterUnityClass<Material>("Core");
	//28. Mesh
	RegisterUnityClass<Mesh>("Core");
	//29. MeshFilter
	RegisterUnityClass<MeshFilter>("Core");
	//30. MeshRenderer
	RegisterUnityClass<MeshRenderer>("Core");
	//31. MonoBehaviour
	RegisterUnityClass<MonoBehaviour>("Core");
	//32. MonoManager
	RegisterUnityClass<MonoManager>("Core");
	//33. MonoScript
	RegisterUnityClass<MonoScript>("Core");
	//34. NamedObject
	RegisterUnityClass<NamedObject>("Core");
	//35. Object
	//Skipping Object
	//36. PlayerSettings
	RegisterUnityClass<PlayerSettings>("Core");
	//37. PreloadData
	RegisterUnityClass<PreloadData>("Core");
	//38. QualitySettings
	RegisterUnityClass<QualitySettings>("Core");
	//39. RectTransform
	RegisterUnityClass<UI::RectTransform>("Core");
	//40. ReflectionProbe
	RegisterUnityClass<ReflectionProbe>("Core");
	//41. RenderSettings
	RegisterUnityClass<RenderSettings>("Core");
	//42. RenderTexture
	RegisterUnityClass<RenderTexture>("Core");
	//43. Renderer
	RegisterUnityClass<Renderer>("Core");
	//44. ResourceManager
	RegisterUnityClass<ResourceManager>("Core");
	//45. RuntimeInitializeOnLoadManager
	RegisterUnityClass<RuntimeInitializeOnLoadManager>("Core");
	//46. Shader
	RegisterUnityClass<Shader>("Core");
	//47. ShaderNameRegistry
	RegisterUnityClass<ShaderNameRegistry>("Core");
	//48. Sprite
	RegisterUnityClass<Sprite>("Core");
	//49. SpriteRenderer
	RegisterUnityClass<SpriteRenderer>("Core");
	//50. TagManager
	RegisterUnityClass<TagManager>("Core");
	//51. TextAsset
	RegisterUnityClass<TextAsset>("Core");
	//52. Texture
	RegisterUnityClass<Texture>("Core");
	//53. Texture2D
	RegisterUnityClass<Texture2D>("Core");
	//54. Texture2DArray
	RegisterUnityClass<Texture2DArray>("Core");
	//55. Texture3D
	RegisterUnityClass<Texture3D>("Core");
	//56. TimeManager
	RegisterUnityClass<TimeManager>("Core");
	//57. TrailRenderer
	RegisterUnityClass<TrailRenderer>("Core");
	//58. Transform
	RegisterUnityClass<Transform>("Core");
	//59. BoxCollider
	RegisterUnityClass<BoxCollider>("Physics");
	//60. Collider
	RegisterUnityClass<Collider>("Physics");
	//61. MeshCollider
	RegisterUnityClass<MeshCollider>("Physics");
	//62. PhysicMaterial
	RegisterUnityClass<PhysicMaterial>("Physics");
	//63. PhysicsManager
	RegisterUnityClass<PhysicsManager>("Physics");
	//64. Rigidbody
	RegisterUnityClass<Rigidbody>("Physics");
	//65. SphereCollider
	RegisterUnityClass<SphereCollider>("Physics");
	//66. Font
	RegisterUnityClass<TextRendering::Font>("TextRendering");
	//67. TextMesh
	RegisterUnityClass<TextRenderingPrivate::TextMesh>("TextRendering");

}
