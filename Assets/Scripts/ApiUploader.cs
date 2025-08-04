using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts
{
    public class ApiUploader : MonoBehaviour
    {
        public void UploadStatsToServer()
        {
            StartCoroutine(SendStats());
        }

        private IEnumerator SendStats()
        {
            string json = JsonUtility.ToJson(new Wrapper { data = SimulationStats.Instance.history }, true);

            using UnityWebRequest request = new UnityWebRequest("http://localhost:5000/api/simulation/upload", "POST");
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
                Debug.Log("Stats uploaded succesfully");
            else
                Debug.Log("Stats couldn't uploaded succesfully");
        }
    }
}
