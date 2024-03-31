
using UnityEngine;

public class SetSkin : MonoBehaviour
{
    [SerializeField] private MeshRenderer _mesh;


    public void Set(Material material) {

            _mesh.material = material;
        
    }
}
