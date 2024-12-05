using UnityEngine;
using UnityEngine.SceneManagement;
#pragma warning disable IDE0051 

namespace CleaningCompany.Monos
{
    internal class BodyInShip : MonoBehaviour
    {
        private GameObject gO;
        private GrabbableObject grabbableBody;
        private Scene currentScene;
        private Scene shipScene;

        void Start() 
        {
            gO = gameObject;
            grabbableBody = gO.GetComponent<GrabbableObject>();
        }

        void Update() 
        {
            if (grabbableBody.isInShipRoom && currentScene.name != "SampleSceneRelay")
            {
                currentScene = gO.scene;
                shipScene = SceneManager.GetSceneByName("SampleSceneRelay");
                gO.transform.parent = null;
                SceneManager.MoveGameObjectToScene(gO, shipScene);
                GameObject propsParent = GameObject.Find("/Environment/HangarShip");
                gO.transform.SetParent(propsParent.transform);
            }
        }
    }
}